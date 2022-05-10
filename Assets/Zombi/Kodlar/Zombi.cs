using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombi : MonoBehaviour
{
    public float zombiHP = 100;
    Animator zombiAnim;
    bool zombiOlu;
    public float kovalamaMesafesi;
    public float saldirmaMesafesi;
    float mesafe;
    NavMeshAgent zombiNavMesh;

    GameObject hedefOyuncu;

    AudioSource sesKaynagi;
    public AudioClip saldirmaSesi;
    void Start()
    {
        zombiAnim = this.GetComponent<Animator>();
        hedefOyuncu = GameObject.Find("SWAT");
        zombiNavMesh = this.GetComponent<NavMeshAgent>();
        sesKaynagi = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (zombiHP <= 0)
        {
            zombiOlu = true;
        }
        if (zombiOlu == true)
        {
            zombiAnim.SetBool("oldu", true);
                 zombiAnim.SetBool("yuruyor", false);
                    zombiAnim.SetBool("saldiriyor", false);
            StartCoroutine(YokOl());
        }
        else
        {
            mesafe = Vector3.Distance(this.transform.position, hedefOyuncu.transform.position);
            if (mesafe < kovalamaMesafesi)
            {
                this.transform.LookAt(hedefOyuncu.transform.position);
                if (mesafe <= saldirmaMesafesi)
                {
                    this.transform.LookAt(hedefOyuncu.transform.position);
                    zombiNavMesh.isStopped = true;
                    zombiAnim.SetBool("yuruyor", false);
                    zombiAnim.SetBool("saldiriyor", true);
                }
                else
                {
                    zombiNavMesh.SetDestination(hedefOyuncu.transform.position);
                    zombiNavMesh.isStopped = false;
                    zombiAnim.SetBool("saldiriyor", false);
                    zombiAnim.SetBool("yuruyor", true);
                }
            }
            else if (mesafe > kovalamaMesafesi)
            {
                zombiNavMesh.isStopped = true;
                zombiAnim.SetBool("yuruyor", false);
                zombiAnim.SetBool("saldiriyor", false);
            }
        }
    }
    public void HasarVerSes()
    {
        sesKaynagi.PlayOneShot(saldirmaSesi);
    }
    public void HasarVer()
    {
        StartCoroutine(hedefOyuncu.GetComponent<KarakterKontrol>().HasarAl());
    }
    IEnumerator YokOl()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }
    public void HasarAl()
    {
        zombiHP -= Random.Range(15, 25);
    }
}
