using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
    {

        #region Public Fields

		static public GameManager Instance;

		#endregion

		#region Private Fields

		private GameObject instance;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefab;
        public GameObject yakuZombiePrefab;
        public GameObject warZombiePrefab;
        public GameObject copZombiePrefab;

        public GameObject maze;
	    public byte[,] map;
        public List<Vector3> positionList;
        public List<String> zombieList;
        private List<GameObject> playerList = new List<GameObject>();
        private List<bool> isPlayerFinishList = new List<bool>();
        private int playersCompleteMaze = 0;
        #endregion

        #region Photon Callbacks

        [SerializeField]
        private GameObject topPanel;
        public GameObject[] mazeList;
        private List <List<Vector3> > mazePositionsList;
        private List<bool> isPlayerInMazeList = new List<bool>();
        private Vector3 ppos;
        private int PlayerCount;
        private int zombieCount = 0;
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>

        public void Awake()
        {
            Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;

        }
        void Start() 
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            zombieList = new List<String>();
            zombieList.Add(this.yakuZombiePrefab.name);
            zombieList.Add(this.warZombiePrefab.name);
            zombieList.Add(this.copZombiePrefab.name);
            PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;

            GameObject[] pList;
            pList = GameObject.FindGameObjectsWithTag("Player");
            mazePositionsList = new List<List<Vector3>>();
            for (int i=0; i < pList.Length; i++){
                playerList.Add(pList[i]);
                isPlayerFinishList.Add(false);
                isPlayerInMazeList.Add(false);
            }

            // in case we started this demo with the wrong scene being active, simply load the menu scene
			if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("Launcher");
				return;
			}

            int count = 0;
            map = new byte[15,15];
            for (int z = 0; z < 15; z++) 
            {
                for (int x = 0; x < 15; x++)
                {
                    map[x, z] = (byte) PhotonNetwork.CurrentRoom.CustomProperties["map"+count.ToString()];
                    count++;
                }
            }
            maze.GetComponent<DockRecursive>().map = map;

            maze.GetComponent<DockRecursive>().Start();

            maze.GetComponent<DockRecursive>().DrawMap(map);
            
            
            Hashtable props = new Hashtable
            {
                {ZombieGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);


            if (SceneManager.GetActiveScene().name == "Dock Thing") 
            {
                maze.GetComponent<DockRecursive>().Start();
                positionList = GetMazeMap();

                if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

                    Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
                } else {

                    if (KarakterKontrol.LocalPlayerInstance==null)
                    {
                        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        
                        Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];
                        InstantiateDockPlayer();
                        StartScene();
                    }else{

                        Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                    }
                }
            }
            else if (SceneManager.GetActiveScene().name == "Tunnel")
            {
                Debug.Log("tunnele girdi");

                mazeList = GameObject.FindGameObjectsWithTag("Maze");
                
                for (int i = 0; i < mazeList.Length; i++) 
                {
                    positionList = GetMazePositions(mazeList[i].GetComponent<TunnelMaze>());
                    mazePositionsList.Add(positionList);
                }

                Debug.Log("playerList.Count  "+ playerList.Count);

                InstantiatePlayer();
                StartScene();

                
            } 
            
            topPanel.SetActive(false);
        }

        private void InstantiateDockPlayer( ) 
        {
                Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];
                PhotonNetwork.Instantiate(this.playerPrefab.name, currentV, Quaternion.identity, 0);
                //isPlayerInMazeList[i] = true;
            
        }

        private void InstantiatePlayer( ) 
        {
            for (int i = 0; i < PlayerCount; i++) 
            {
                Vector3 currentV = mazePositionsList[i][UnityEngine.Random.Range(0, mazePositionsList[i].Count)];
                PhotonNetwork.Instantiate(this.playerPrefab.name, currentV, Quaternion.identity, 0);
                //isPlayerInMazeList[i] = true;
            }
        }

        private List<Vector3> GetMazePositions(TunnelMaze maze)
        {
            map = maze.GetMap();
            int depth = maze.depth;
            int width = maze.width;
		    int initialX = maze.initialX;
		    int initialY = maze.initialY + 1;
		    int initialZ = maze.initialZ;
		    int scale = maze.scale;

            for (int z = 0; z < depth; z++){
                for (int x = 0; x < width; x++)
                {
                    if (map[x, z] != 1)
                    {
			    		positionList.Add(new Vector3(initialX + scale * x  , initialY , initialZ + scale * z ));
                        
			    		//StartCoroutine(SpawnZombie(x, z));
			    	}
			    }
		    }
            return positionList;
        }

        private List<Vector3> GetMazeMap() 
        {

            map = maze.GetComponent<DockRecursive>().GetMap();
		    int depth = maze.GetComponent<DockRecursive>().depth;
		    int width = maze.GetComponent<DockRecursive>().width;
		    int initialX = maze.GetComponent<DockRecursive>().initialX;
		    //int initialY = maze.GetComponent<TunnelRecursive>().initialY;
		    int initialY = 8;
		    int initialZ = maze.GetComponent<DockRecursive>().initialZ;
		    int scale = maze.GetComponent<DockRecursive>().scale;

            for (int z = 0; z < depth; z++){
                for (int x = 0; x < width; x++)
                {
                    if (map[x, z] != 1)
                    {
                        Debug.Log("x , z " + x +"  " + z);
			    		positionList.Add(new Vector3(initialX + scale * x , initialY , initialZ + scale * z + 5 ));
			    		//StartCoroutine(SpawnZombie(x, z));
			    	}
			    }
		    }
		    return positionList;
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        void Update() 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                topPanel.SetActive(true);
                Cursor.visible = true;
                Screen.lockCursor = false;
                Time.timeScale = 1; 
            }
            
         
            CheckFinishedPlayers();

            if (isPlayerFinishList.Distinct().Count() == 1)
            {
                FinishScene();
            }
            
        }
        
        public void CheckFinishedPlayers() {

            for (int i = 0; i < playerList.Count; i++) {
                if (playerList[i] == null) 
                {
                    playerList.RemoveAt(i);
                }

                Debug.Log(playerList[i].transform.position);
                if (playerList[i].transform.position.x <= 30 && ((byte)playerList[i].transform.position.x) >= 20 &&
                playerList[i].transform.position.z <= 55 && playerList[i].transform.position.z >= 45) 
                {
                    Debug.Log("finitoya girdi");
                    isPlayerFinishList[i] = true;
                }
                else 
                {
                    isPlayerFinishList[i] = false;
                }
            }
        }

        public void FinishScene() {
            Debug.Log("playersCompleteMaze:  " + playerList.Count);
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("FinishScene");
        }

        public void continuePressed() 
        {
            Debug.Log("continue");
            topPanel.SetActive(false);
            Cursor.visible = false;
            Screen.lockCursor = true;
            Time.timeScale = 1; 
        }

        #endregion

        private IEnumerator SpawnDockZombie()
        {
            while (true)
            {
                if (zombieCount < 100) {
                    yield return new WaitForSeconds(2f);
                
                Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];

                String zombieDesicion = zombieList[UnityEngine.Random.Range(0, zombieList.Count)];
                PhotonNetwork.Instantiate(zombieDesicion, currentV, Quaternion.identity, 0);

                zombieCount++;
                }
                

            }
        }

        private IEnumerator SpawnZombie()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                
                List<Vector3>  randomMaze = mazePositionsList[UnityEngine.Random.Range(0, mazePositionsList.Count)];
                Vector3 currentV = randomMaze[UnityEngine.Random.Range(0, randomMaze.Count)];

                String zombieDesicion = zombieList[UnityEngine.Random.Range(0, zombieList.Count)];
                PhotonNetwork.Instantiate(zombieDesicion, currentV, Quaternion.identity, 0);

            }
        }


        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Dock Thing");
        }


        #region Photon Callbacks

        //public override void OnPlayerEnteredRoom(Player other)
        //{
        //    Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
//
//
        //    if (PhotonNetwork.IsMasterClient)
        //    {
        //        Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
//
//
        //        //LoadArena();
        //    }
        //}

        /// <summary>
		/// Called when a Photon Player got disconnected. We need to load a smaller scene.
		/// </summary>
		/// <param name="other">Other.</param>

        public override void OnJoinedRoom()
        {
            Debug.Log("joined roommm");
            Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;

            props.TryGetValue("isSync", out object sync);

            Debug.Log("syncccc" + sync.ToString());
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
                
                //DestroyPlayerObject(other);
                PhotonNetwork.DestroyPlayerObjects(other);
                //LoadArena();
            }
        }

        /// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			SceneManager.LoadScene("Launcher");
		}

        #endregion

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                //StartCoroutine(SpawnZombie());
            }
        }

        private void StartScene()
        {
            Debug.Log("StartGame!");

            // on rejoin, we have to figure out if the spaceship exists or not
            // if this is a rejoin (the ship is already network instantiated and will be setup via event) we don't need to call PN.Instantiate

            //Vector3 currentV = new Vector3(UnityEngine.Random.Range(-5, 0) , 9, UnityEngine.Random.Range(16, 60));


            //PhotonNetwork.Instantiate(this.zombiePrefab.name, currentV, Quaternion.identity, 0);
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnDockZombie());
            }
        }


        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(ZombieGame.PLAYER_LIVES))
            {
                //CheckEndOfGame();
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }


            // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
            int startTimestamp;
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(ZombieGame.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        CountdownTimer.SetStartTime();
                    }
                }
                else
                {
                    // not all players loaded yet. wait:
                    //Debug.Log("setting text waiting for players! ",this.InfoText);
                    //InfoText.text = "Waiting for other players...";
                }
            }
        
        }

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(ZombieGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool) playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        private void OnCountdownTimerIsExpired()
        {
            StartScene();
        }
        
        #region Public Methods

		public void LeaveRoom()
		{   
			PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

		#endregion

    }
