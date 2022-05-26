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

        public GameObject topPanel;
        
        public GameObject playerListPanel;
        public GameObject PlayerListContent;

        public GameObject PlayerListEntryPrefab;

        public GameObject[] mazeList;
        private List <List<Vector3> > mazePositionsList;
        private Vector3 ppos;
        private int PlayerCount;
        private int zombieCount = 50;
        private int healthPackCount = 5;
        private int pistolAmmoCount = 5;
        private int finishedCount = 0;
        public GameObject healthPack;        
        public GameObject pistolAmmo;        
        private List<string> playerListEntries;
        GameObject entry;
        public bool succesfullyFinishScene = false; 
        public String currentSceneName; 

        public void Awake()
        {
            Instance = this;
            currentSceneName = SceneManager.GetActiveScene().name;
            Debug.Log(currentSceneName);
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
            playerListEntries = new List<string>(); 
        }
        void Start() 
        {
            
            zombieList = new List<String>();
            zombieList.Add(this.yakuZombiePrefab.name);
            zombieList.Add(this.warZombiePrefab.name);
            //zombieList.Add(this.copZombiePrefab.name);
            PlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            List<Vector3> positionList = new List<Vector3>();

            // in case we started this demo with the wrong scene being active, simply load the menu scene
			if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("Launcher");
				return;
			}

            


            if (currentSceneName == "Dock Thing") 
            {
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

                PhotonNetwork.CurrentRoom.CustomProperties["CurrentSceneName"] = currentSceneName;
                maze.GetComponent<DockRecursive>().map = map;

                maze.GetComponent<DockRecursive>().DrawMap();
                
                Hashtable props = new Hashtable
                {
                    {ZombieGame.PLAYER_LOADED_LEVEL, true},
                    //{ZombieGame.PLAYER_ZOMBIE_KILL, "0"},
                    {ZombieGame.PLAYER_LIVES, true},
                    {ZombieGame.PLAYER_SUCCESSFUL, false},
                    {ZombieGame.PLAYER_CURRENT_MAP, currentSceneName}
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                positionList = GetMazeMapDock();

                if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
                    Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
                } else {
                    if (KarakterKontrol.LocalPlayerInstance==null)
                    {
                        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        
                        Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];
                        InstantiateDockPlayer();
                        StartScene(positionList);
                    }
                    else{
                        Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                    }
                }
            }
            else if (currentSceneName == "Tunnel")
            {
                int count = 0;
                map = new byte[10,10];
                for (int z = 0; z < 10; z++) 
                {
                    for (int x = 0; x < 10; x++)
                    {
                        map[x, z] = (byte) PhotonNetwork.CurrentRoom.CustomProperties["map1_"+count.ToString()];
                        count++;
                    }
                }
                Debug.Log("mapppp" + map[5,1] + " " + map[5,2] + " " + map[5,3] + " " + map[5,4] + " " + map[5,5] + " " + map[5,6] + " " + map[5,7] + " " + map[5,8]);

                PhotonNetwork.CurrentRoom.CustomProperties["CurrentSceneName"] = currentSceneName;
                maze.GetComponent<TunnelRecursive>().map = map;

                maze.GetComponent<TunnelRecursive>().DrawMap();
                
                Hashtable props = new Hashtable
                {
                    {ZombieGame.PLAYER_LOADED_LEVEL, true},
                    //{ZombieGame.PLAYER_ZOMBIE_KILL, "0"},
                    {ZombieGame.PLAYER_LIVES, true},
                    {ZombieGame.PLAYER_SUCCESSFUL, false},
                    {ZombieGame.PLAYER_CURRENT_MAP, currentSceneName}
                };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                positionList = GetMazeMapTunnel();

                if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
                    Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
                } else {
                    if (KarakterKontrol.LocalPlayerInstance==null)
                    {
                        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        
                        Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];
                        InstantiateTunnelPlayer();
                        StartScene(positionList);
                    }
                    else{
                        Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                    }
                }
                   
            } 
            GameObject[] pList;
            pList = GameObject.FindGameObjectsWithTag("Player");
            Debug.Log(pList.Length);
            mazePositionsList = new List<List<Vector3>>();
            for (int i=0; i < pList.Length; i++){
                playerList.Add(pList[i]);
                isPlayerFinishList.Add(false);
                //isPlayerInMazeList.Add(false);
            }
            topPanel.SetActive(false);
            playerListPanel.SetActive(false);

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (!playerListEntries.Contains(p.NickName))
                {                    
                entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(PlayerListContent.transform);
                entry.transform.localScale = Vector3.one;
                object killCount;
                p.CustomProperties.TryGetValue(ZombieGame.PLAYER_ZOMBIE_KILL, out killCount);
                
                entry.GetComponent<PlayerListEntryInGame>().Initialize(p.NickName, (string) killCount.ToString());
                playerListEntries.Add(p.NickName);
                }
            }
            
            
        }

        void Update() 
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                topPanel.SetActive(true);
                Cursor.visible = true;
                Screen.lockCursor = false;
            }
            else 
            {
                Debug.Log("continue");
                topPanel.SetActive(false);
                Cursor.visible = false;
                Screen.lockCursor = true;
            }
            if (Input.GetKey(KeyCode.Tab))
            {
                playerListPanel.SetActive(true);
                //Cursor.visible = true;
                //Screen.lockCursor = false;
                //Time.timeScale = 1; 
            }
            else{
                playerListPanel.SetActive(false);
                //playerListEntries.Clear();
                //Destroy(entry);

               // PlayerListContent.SetActive(false);
            }

            CheckFinishedPlayers();            
        }


        private void InstantiateDockPlayer( ) 
        {
                Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];
                PhotonNetwork.Instantiate(this.playerPrefab.name, currentV, Quaternion.identity, 0);
        }

        private void InstantiateTunnelPlayer( ) 
        {
            
            Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];
            PhotonNetwork.Instantiate(this.playerPrefab.name, currentV, Quaternion.identity, 0);
            
        }

        private List<Vector3> GetMazeMapTunnel()
        {
            int depth = maze.GetComponent<TunnelRecursive>().depth;
            int width = maze.GetComponent<TunnelRecursive>().width;
		    int initialX = maze.GetComponent<TunnelRecursive>().initialX;
		    int initialY = maze.GetComponent<TunnelRecursive>().initialY + 1;
		    int initialZ = maze.GetComponent<TunnelRecursive>().initialZ;
		    int scale = maze.GetComponent<TunnelRecursive>().scale;

            for (int z = 0; z < depth; z++){
                for (int x = 0; x < width; x++)
                {
                    if (map[x, z] != 1)
                    {
			    		positionList.Add(new Vector3(initialX + scale * x  , initialY , initialZ + scale * z ));
			    	}
			    }
		    }
            return positionList;
        }

        private List<Vector3> GetMazeMapDock() 
        {
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
                        //Debug.Log("x , z " + x +"  " + z);
			    		positionList.Add(new Vector3(initialX + scale * x  , initialY , initialZ + scale * z  ));
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

        
        

        public void CheckFinishedPlayers() {
            for (int i = 0; i < playerList.Count; i++) {
                if (playerList[i] == null) 
                {
                    playerList.RemoveAt(i);
                }
                
                if (playerList[i].GetComponent<KarakterKontrol>().transform.position.x <= 10 && playerList[i].GetComponent<KarakterKontrol>().transform.position.x >= -15 &&
                playerList[i].GetComponent<KarakterKontrol>().transform.position.z <= 77 && playerList[i].GetComponent<KarakterKontrol>().transform.position.z >= 68) 
                {
                    Hashtable props = new Hashtable
                    {
                        {ZombieGame.PLAYER_FINISHED, true}
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                    
                }
                else 
                {
                    Hashtable props = new Hashtable
                    {
                        {ZombieGame.PLAYER_FINISHED, false}
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                }
            }
        }

    public void FinishScene() {

        //Hashtable roomProps = new Hashtable();
        if (succesfullyFinishScene) 
        {
            PhotonNetwork.CurrentRoom.CustomProperties.Add("TeamSuccess", true);
            //roomProps.Add(ZombieGame.PLAYER_SUCCESSFUL, true);
        } 
        else 
        {
            PhotonNetwork.CurrentRoom.CustomProperties.Add("TeamSuccess", false);
            //roomProps.Add(ZombieGame.PLAYER_SUCCESSFUL, false);
        }
        //PhotonNetwork.LocalPlayer.SetCustomProperties(roomProps);
        
        int pCount = 0;
        PhotonNetwork.CurrentRoom.CustomProperties.Add("TotalPlayerCount" , PhotonNetwork.PlayerList.Count());
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            PhotonNetwork.CurrentRoom.CustomProperties.Add("name"+pCount , p.NickName);

            //object killCount;
            //p.CustomProperties.TryGetValue(ZombieGame.PLAYER_ZOMBIE_KILL, out killCount);
            object killCount =  p.CustomProperties[ZombieGame.PLAYER_ZOMBIE_KILL];
            Debug.Log(killCount);
            
            PhotonNetwork.CurrentRoom.CustomProperties.Add("kill"+pCount , (string) killCount.ToString());
            
            
            pCount++;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }

        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject o in objects)
        {
            if (o.GetComponent<PhotonView>() != null && !o.tag.Equals("Player") && !o.tag.Equals("MainCamera") && !o.tag.Equals("Console_Text"))
            {
                PhotonNetwork.Destroy(o);
            }
        }
        
        PhotonNetwork.AutomaticallySyncScene = true;
        
        
        
        //PhotonNetwork.LoadLevel("FinishScene");
        SceneManager.LoadScene("FinishScene");
    }

        

        #endregion

        private IEnumerator SpawnDockZombie(List<Vector3> positionList)
        {
            for (int i = zombieCount; i > 0 ; i--)
            {
                yield return new WaitForSeconds(3f);
                
                Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];
                String zombieDesicion = zombieList[UnityEngine.Random.Range(0, zombieList.Count)];
                PhotonNetwork.Instantiate(zombieDesicion, currentV, Quaternion.identity, 0);
            }
        }

        public void SpawnDockHealthPack(List<Vector3> positionList)
        {
           for (int i = healthPackCount; i > 0 ;i--)
           {
               Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];                
            PhotonNetwork.Instantiate(this.healthPack.name, currentV, Quaternion.identity, 0);
           }
        }

        public void SpawnDockPistolAmmo(List<Vector3> positionList)
        {
           for (int i = pistolAmmoCount; i > 0 ;i--)
           {
               Vector3 currentV = positionList[UnityEngine.Random.Range(0, positionList.Count)];                
            PhotonNetwork.Instantiate(this.pistolAmmo.name, currentV, Quaternion.identity, 0);
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

        public override void OnJoinedRoom()
        {
            Debug.Log("joined room");
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

        private void StartScene(List<Vector3> positionList)
        {
            Debug.Log("StartGame!");
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnDockZombie(positionList));
                SpawnDockHealthPack(positionList);
                SpawnDockPistolAmmo(positionList);
            }
        }


        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(ZombieGame.PLAYER_LIVES))
            {
                CheckAnyPlayerDie();
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (changedProps.ContainsKey(ZombieGame.PLAYER_FINISHED))
            {
                if (CheckPlayersFinish())
                {
                    FinishScene();
                }
            }

            if (changedProps.ContainsKey(ZombieGame.PLAYER_ZOMBIE_KILL))
            {
                UpdatePlayerKillList();
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

        private bool CheckPlayersFinish()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerFinished;
                if (p.CustomProperties.TryGetValue(ZombieGame.PLAYER_FINISHED, out isPlayerFinished))
                {
                    if (!(bool) isPlayerFinished)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            succesfullyFinishScene = true;
            return true;
        }

        private void UpdatePlayerKillList()
        {
            Destroy(entry);
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(PlayerListContent.transform);
                entry.transform.localScale = Vector3.one;
                object killCount;
                p.CustomProperties.TryGetValue(ZombieGame.PLAYER_ZOMBIE_KILL, out killCount);
                entry.GetComponent<PlayerListEntryInGame>().Initialize(p.NickName, (string) killCount.ToString());
                playerListEntries.Add(p.NickName);
                
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

        private void CheckAnyPlayerDie()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerLives;
                if (p.CustomProperties.TryGetValue(ZombieGame.PLAYER_LIVES, out isPlayerLives))
                {
                    if (!(bool) isPlayerLives)
                    {
                        FinishScene();
                    }
                }
                else
                {
                    FinishScene();
                }
            }

        }

        private void OnCountdownTimerIsExpired()
        {
            //StartScene(positionList);
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
