using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
  

public class InterfaceControl : MonoBehaviour
{
    public Text magazineText;
    public Text healthPackText;
    //public GameObject sahteMenu;

    bool oyunDurdu;
    GameObject player;
    void Start()
    {
        player = GameObject.Find("SWAT");
    }

    // Update is called once per frame
    void Update()
    {
        magazineText.text = player.GetComponent<RaycastShooting>().GetMagazine().ToString()+"/"+ player.GetComponent<RaycastShooting>().GetAmmunition().ToString();
        healthPackText.text = player.GetComponent<KarakterKontrol>().GetHealthPack().ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (oyunDurdu == true)
            {
                OyunuDevamEttir();
            }
            else if (oyunDurdu == false)
            {
                OyunuDurdur();
            }
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
}
