using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OyunKontrol : MonoBehaviour
{
    public List<Transform> saglikNoktalari;
    public List<Transform> mermiNoktalari;
    public Transform zombiSpawnSol;
    public Transform zombiSpawnSag;

    public GameObject zombi;
    public GameObject saglikKutu;
    public GameObject mermiKutu;

    int dalgaSayaci=0;
    bool zombiSpawnlandi=false;

    public List<GameObject> ZombiListesi;
    void Start()
    {
        ZombiCikar(dalgaSayaci);
    }

    void Update()
    {
        if (ZombiListesi.Count == 0)
        {
            if (zombiSpawnlandi == false)
            {
                ZombiCikar(dalgaSayaci);
            }
        }
    }
    void ZombiCikar(int dalgaKuvveti)
    {
        zombiSpawnlandi = true;
        for(int x = 0; x < 1 + (dalgaKuvveti * 3); x++)
        {
            GameObject temp = Instantiate(zombi);
            ZombiListesi.Add(temp);
            temp.transform.position = zombiSpawnSol.transform.position + new Vector3(2 * x, 0, -2 * x);
        }
        for (int x = 0; x < 1 + (dalgaKuvveti * 3); x++)
        {
            GameObject temp = Instantiate(zombi);
            ZombiListesi.Add(temp);
            temp.transform.position = zombiSpawnSag.transform.position + new Vector3(2 * x, 0, -2 * x);
        }
    }
}
