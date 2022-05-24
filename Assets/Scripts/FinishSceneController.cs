using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;


public class FinishSceneController : MonoBehaviour {

    public GameObject playerListPanel;
    private List<string> playerListEntries;
    [SerializeField]
    public GameObject PlayerListEntryPrefab;

    private GameObject entry;
    [SerializeField]
    public GameObject PlayerListContent;
    public List<string> mapNameList = new List<string> { "Dock Thing", "Tunnel" };
    private bool isTeamSuccess = false;
    [SerializeField]
    public Text teamSituation;
    private string currentMap = "";
    private string nextMap = "";

    public GameObject tunnelRecursive;

    void Awake()
    {
        currentMap = (string) PhotonNetwork.CurrentRoom.CustomProperties["CurrentSceneName"];
        isTeamSuccess = (bool) PhotonNetwork.CurrentRoom.CustomProperties["TeamSuccess"];
    }
    void Start()
    {
        Cursor.visible = true;
        Screen.lockCursor = false;
        Time.timeScale = 1; 

        playerListPanel.SetActive(true);
        playerListEntries = new List<string>();              

        int pCount = 0;
        int totalPlayerCount = (int) PhotonNetwork.CurrentRoom.CustomProperties["TotalPlayerCount"];

        for (int i = 0; i < totalPlayerCount; i++)
        {
            entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(PlayerListContent.transform);
            entry.transform.localScale = Vector3.one;

            string name = (string) PhotonNetwork.CurrentRoom.CustomProperties["name" + i];
            string kill = (string) PhotonNetwork.CurrentRoom.CustomProperties["kill" + i];
            Debug.Log("name " + name);
            Debug.Log("kill " + kill);
            entry.GetComponent<PlayerListEntryInGame>().Initialize((string) name.ToString() , (string) kill.ToString());
        }
      
        if (isTeamSuccess)
        {
            teamSituation.text = "You succesfully complete the map. You can continue to new level and new challenges!";
            if (currentMap == "Dock Thing")
            {
                nextMap = "Tunnel";
            }
            else 
            {
                teamSituation.text = "You complete the game. Congratilations!";
            }
        }
        else 
        {
            teamSituation.text = "You fail :( You must play the scene until you success. You can do it!";
            nextMap = currentMap;
        }

        SendTunnelMazePositionsToProp();
    }

    public byte[,]  GetMazeFromTunnel()
    {
        byte[,] map = tunnelRecursive.GetComponent<TunnelRecursive>().StartTunnelMaze(1);
        Debug.Log("drawed maze");
        Debug.Log("mapppp" + map[5,1] + " " + map[5,2] + " " + map[5,3] + " " + map[5,4] + " " + map[5,5] + " " + map[5,6] + " " + map[5,7] + " " + map[5,8]);

        return map;
    }

    public void SendTunnelMazePositionsToProp()
    {
        Hashtable roomProps = new Hashtable
        {
            {"roomprop", "OLDIII"},
        };
        byte[,] map = GetMazeFromTunnel();
                Debug.Log("mapppp" + map[5,1] + " " + map[5,2] + " " + map[5,3] + " " + map[5,4] + " " + map[5,5] + " " + map[5,6] + " " + map[5,7] + " " + map[5,8]);

        int count = 0;
        for (int z = 0; z < tunnelRecursive.GetComponent<TunnelRecursive>().depth; z++)
        {
            for (int x = 0; x < tunnelRecursive.GetComponent<TunnelRecursive>().width; x++)
            {
                roomProps.Add("map1_"+count, map[x,z]);
                count++;
            }
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }

    public void PassToScene()
    {
        Debug.Log(nextMap);
        SceneManager.LoadScene(nextMap);
    }
}
