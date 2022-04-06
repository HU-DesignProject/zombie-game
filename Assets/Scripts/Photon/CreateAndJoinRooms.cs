using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;


public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    private const int maxPlayers = 2;

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(createInput.text))
        {
            return;
        }
        //RoomOptions roomOptions = new RoomOptions();
        //roomOptions.MaxPlayers = 4;
        //roomOptions.IsVisible = true;
        //roomOptions.IsOpen = true;
        //roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        //roomOptions.CustomRoomProperties.Add("playersAlive", 0);
        //roomOptions.CustomRoomProperties.Add("timesUp", false);
        //roomOptions.CustomRoomProperties.Add("levelIndx", 1);
        //roomOptions.CustomRoomProperties.Add("scoreList", new int[] { 0, 0, 0, 0, 0, 0, 0 });

        PhotonNetwork.CreateRoom(createInput.text);
        Debug.Log("Created Room");
    }
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
        Debug.Log("Joined Room");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("industry");
    }
}
