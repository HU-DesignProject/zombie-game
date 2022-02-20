using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
  

public class ArayuzKontrol : MonoBehaviour
{
    public Text mermiText;
    public Text saglikText;
    public GameObject sahteMenu;

    bool oyunDurdu;
    GameObject oyuncu;
    void Start()
    {
        oyuncu = GameObject.Find("SWAT");
    }

    // Update is called once per frame
    void Update()
    {
        mermiText.text = oyuncu.GetComponent<AtesSistemi>().GetSarjor().ToString()+"/"+ oyuncu.GetComponent<AtesSistemi>().GetCephane().ToString();
        saglikText.text = "HP:" + oyuncu.GetComponent<KarakterKontrol>().GetSaglik();
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
