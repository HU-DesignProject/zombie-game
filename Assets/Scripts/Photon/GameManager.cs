using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

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

        #endregion

        #region Photon Callbacks

        [SerializeField]
        private GameObject topPanel;
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
            positionList = GetMazeMap();

            zombieList = new List<String>();
            zombieList.Add(this.yakuZombiePrefab.name);
            zombieList.Add(this.warZombiePrefab.name);
            zombieList.Add(this.copZombiePrefab.name);

            // in case we started this demo with the wrong scene being active, simply load the menu scene
			if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("Loading");

				return;
			}

            Hashtable props = new Hashtable
            {
                {ZombieGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);


            if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			} else {


				if (KarakterKontrol.LocalPlayerInstance==null)
				{
				    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

					// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    
					Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];

                    //PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(UnityEngine.Random.Range(0,5), 7, UnityEngine.Random.Range(-3, 0)), Quaternion.identity, 0);
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(currentV.x, currentV.y , currentV.z), Quaternion.identity, 0);
                    StartGame();
                 }else{

					Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
				}


			}

            topPanel.SetActive(false);
        }

        private List<Vector3> GetMazeMap() 
        {

            map = maze.GetComponent<TunnelRecursive>().SendMap();
		    int depth = maze.GetComponent<TunnelRecursive>().depth;
		    int width = maze.GetComponent<TunnelRecursive>().width;
		    int initialX = maze.GetComponent<TunnelRecursive>().initialX;
		    //int initialY = maze.GetComponent<TunnelRecursive>().initialY;
		    int initialY = 3;
		    int initialZ = maze.GetComponent<TunnelRecursive>().initialZ;
		    int scale = maze.GetComponent<TunnelRecursive>().scale;

            for (int z = 0; z < depth; z++){
                for (int x = 0; x < width; x++)
                {
                    if (map[x, z] != 1)
                    {
			    		positionList.Add(new Vector3(initialX + scale * x, initialY , initialZ + scale * z));
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


        private IEnumerator SpawnZombie()
        {
            while (true)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(ZombieGame.ASTEROIDS_MIN_SPAWN_TIME, ZombieGame.ASTEROIDS_MAX_SPAWN_TIME));

                //Vector3 currentV = new Vector3(UnityEngine.Random.Range(-5, 0) , 9, UnityEngine.Random.Range(16, 60));
				Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];

                String zombieDesicion = zombieList[UnityEngine.Random.Range(0, zombieList.Count)];
                PhotonNetwork.InstantiateRoomObject(zombieDesicion, currentV, Quaternion.identity, 0);
            }
        }


        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Tunnel");
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
                StartCoroutine(SpawnZombie());
            }
        }

        private void StartGame()
        {
            Debug.Log("StartGame!");

            // on rejoin, we have to figure out if the spaceship exists or not
            // if this is a rejoin (the ship is already network instantiated and will be setup via event) we don't need to call PN.Instantiate

            Vector3 currentV = new Vector3(UnityEngine.Random.Range(-5, 0) , 9, UnityEngine.Random.Range(16, 60));


            //PhotonNetwork.Instantiate(this.zombiePrefab.name, currentV, Quaternion.identity, 0);
            


            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnZombie());
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
            StartGame();
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
