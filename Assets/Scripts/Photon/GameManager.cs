using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


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

        #endregion

        #region Photon Callbacks

        [SerializeField]
        private GameObject topPanel;
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>

        void Start() 
        {
            Instance = this;

            // in case we started this demo with the wrong scene being active, simply load the menu scene
			if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("Loading");

				return;
			}

            if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			} else {


				if (KarakterKontrol.LocalPlayerInstance==null)
				{
				    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

					// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
					PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(UnityEngine.Random.Range(0,5), 7, UnityEngine.Random.Range(-3, 0)), Quaternion.identity, 0);
				}else{

					Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
				}


			}

            topPanel.SetActive(false);
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


        

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Dock Thing");
        }


        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                //LoadArena();
            }
        }

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
