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
    void Start()
    {
        Cursor.visible = true;
        Screen.lockCursor = false;
        Time.timeScale = 1; 

        playerListPanel.SetActive(true);
        playerListEntries = new List<string>();                     

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isTeamSuccess_;
            p.CustomProperties.TryGetValue(ZombieGame.PLAYER_SUCCESSFUL, out isTeamSuccess_);
            if (!(bool) isTeamSuccess_)
            {
                isTeamSuccess = false;
            }
            if (!playerListEntries.Contains(p.NickName))
            {                    
                entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(PlayerListContent.transform);
                entry.transform.localScale = Vector3.one;
                object killCount;
                p.CustomProperties.TryGetValue(ZombieGame.PLAYER_ZOMBIE_KILL, out killCount);

                entry.GetComponent<PlayerListEntryInGame>().Initialize(p.NickName, (string) killCount);
                playerListEntries.Add(p.NickName);
            }
        }

        if (isTeamSuccess)
        {
            teamSituation.text = "You succesfully complete the map. You can continue to new level and new challenges!";
        }
        else 
        {
            teamSituation.text = "You fail :( You must play the scene until you success. You can do it!";
        }
    }

    public void PassToDockScene()
    {
        Debug.Log("dock--");
        SceneManager.LoadScene("Dock Thing");
    }

    public void PassToIndustryScene()
    {
        Debug.Log("industry--");
        SceneManager.LoadScene("Tunnel");  
    }

}
