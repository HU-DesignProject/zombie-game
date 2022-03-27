using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Launcher : MonoBehaviour
{
    string gameVersion = "1";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
}
