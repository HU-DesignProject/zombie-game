using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class FinishSceneController : MonoBehaviour {

    public GameObject playerListPanel;
    private List<string> playerListEntries;
    public GameObject PlayerListEntryPrefab;
    private GameObject entry;
    public GameObject PlayerListContent;
    void Start()
    {
        Cursor.visible = true;
        Screen.lockCursor = false;
        Time.timeScale = 1; 

        playerListPanel.SetActive(true);
        playerListEntries = new List<string>(); 
        foreach (Player p in PhotonNetwork.PlayerList)
        {
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
