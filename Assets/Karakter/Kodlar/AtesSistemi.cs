using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AtesSistemi : MonoBehaviour
{
    Camera kamera;
    public LayerMask zombiKatman;
    KarakterKontrol hpKontrol;
    public ParticleSystem muzzleFlash;
    Animator anim;
    //public GameObject impactEffect;
    public float sarjor = 30;
    private float cephane = 120;
    private float sarjorKapasitesi = 30;
    public float range = 100f;

    AudioSource sesKaynagi;
    public AudioClip atesSes;
    public AudioClip reloadSes;
    void Start()
    {
            kamera = Camera.main;
            hpKontrol = this.gameObject.GetComponent<KarakterKontrol>();
            anim = this.gameObject.GetComponent<Animator>();
            sesKaynagi= this.gameObject.GetComponent<AudioSource>();
                
    }

    // Update is called once per frame
    void Update()
    {
        if (hpKontrol.YasiyorMu() == true && hpKontrol.photonView.IsMine)
        {
            if (Input.GetMouseButton(0))
            {
                if (sarjor > 0)
                {
                    anim.SetBool("atesEt", true);
                }
                if (sarjor <= 0)
                {
                    anim.SetBool("atesEt", false);
                }
                if (sarjor <= 0 && cephane > 0)
                {
                    anim.SetBool("sarjorDegistirme", true);
                }
            }
            else
            {
                anim.SetBool("atesEt", false);

            }
        }

    }
    public void SarjorDegistirmeSes()
    {

        sesKaynagi.PlayOneShot(reloadSes);
        sesKaynagi.volume = 0.6f;
    }
    public void SarjorDegistirme()
    {

        sesKaynagi.volume = 1f;
        cephane -= sarjorKapasitesi - sarjor;
        sarjor = sarjorKapasitesi;
        anim.SetBool("sarjorDegistirme", false);

    }
    public void AtesEtme()
    {
        if (sarjor > 0)
        {
            muzzleFlash.Play();
            sesKaynagi.PlayOneShot(atesSes);
            Ray ray = kamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(hpKontrol.transform.position, hpKontrol.transform.forward , out hit, range))
            {
                if (hit.collider.gameObject.CompareTag("Zombi"))
                {
                    hit.collider.gameObject.GetComponent<AIExample>().GetDamage(Random.Range(25, 50));
                    Debug.Log("vurdum");
                }
               // Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
            sarjor--;
        }
    }
    
    public float GetSarjor()
    {
        return sarjor;
    }
    public float GetCephane()
    {
        return cephane;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MermiKutusu")
        {
            cephane += Random.Range(1, 9);
            Destroy(other.gameObject);
        }
    }
}