using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ArayuzKontrol : MonoBehaviour
{
    public Text bulletText;
    public Text healthPackText;

 
    bool isLocked;
    bool oyunDurdu;
    GameObject player;
    void Start()
    {
        SetCursorLock(false);
        player = GameObject.Find("SWAT");
    }

    // Update is called once per frame
    void Update()
    {
        bulletText.text = player.GetComponent<AtesSistemi>().GetSarjor().ToString()+"/"+ player.GetComponent<AtesSistemi>().GetCephane().ToString();
        healthPackText.text = player.GetComponent<KarakterKontrol>().GetHealthPack().ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible=true;
            Cursor.lockState=CursorLockMode.None;
        }
    }
    public void OyunuDevamEttir()
    {
        //sahteMenu.SetActive(false);
        Time.timeScale = 1;
        oyunDurdu = false;
    }
    public void OyunuDurdur()
    {
        //sahteMenu.SetActive(true);
        Time.timeScale = 0;
        oyunDurdu = true;
    }

    void SetCursorLock(bool isLocked){
        this.isLocked=isLocked;
        Cursor.visible=false;
        Debug.Log(Cursor.visible);
        Cursor.lockState=CursorLockMode.Locked;
    }
}
