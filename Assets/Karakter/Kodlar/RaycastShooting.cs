using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RaycastShooting : MonoBehaviour
{
    Camera cam;
    public LayerMask zombieLayer;
    KarakterKontrol hpCheck;
    public ParticleSystem muzzleFlash;
    Animator anim;

    //public GameObject impactEffect;
    private float magazine = 30;
    private float ammunition = 120;
    private float magazineCapacity = 30;
    public float range = 100f;

    AudioSource audioSrc;
    public AudioClip shootingAudio;
    public AudioClip reloadAudio;
    void Start()
    {
        cam = Camera.main;
        hpCheck = this.gameObject.GetComponent<KarakterKontrol>();
        anim = this.gameObject.GetComponent<Animator>();
        audioSrc= this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hpCheck.YasiyorMu() == true)
        {
            if (Input.GetMouseButton(0))
            {
                if (magazine > 0)
                {
                    anim.SetBool("Shooting", true);
                }
                if (magazine <= 0)
                {
                    anim.SetBool("Shooting", false);
                }
                if (magazine <= 0 && ammunition > 0)
                {
                    anim.SetBool("Reload", true);
                }
            }
            else
            {
                anim.SetBool("Shooting", false);

            }
        }

    }
    public void ReloadAudio()
    {

        audioSrc.PlayOneShot(reloadAudio);
        audioSrc.volume = 0.6f;
    }
    public void Reload()
    {
        audioSrc.volume = 1f;
        ammunition -= magazineCapacity - magazine;
        magazine = magazineCapacity;
        anim.SetBool("Reload", false);

    }
    public void Shooting()
    {
        if (magazine > 0)
        {
            muzzleFlash.Play();
            audioSrc.PlayOneShot(shootingAudio);
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(hpCheck.transform.position, hpCheck.transform.forward , out hit, range))
            {
                if (hit.collider.gameObject.CompareTag("Zombi"))
                {
                    hit.collider.gameObject.GetComponent<Zombi>().GetDamage(Random.Range(25, 50));
                    Debug.Log("Hit Enemy");
                }
               // Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            }
            magazine--;
        }


    }
    
    public float GetMagazine()
    {
        return magazine;
    }
    public float GetAmmunition()
    {
        return ammunition;
    }
    public void SetAmmunition(float addedAmmunition)
    {
        ammunition+=addedAmmunition;
    }
    public float GetMagazineCapacity()
    {
        return magazineCapacity;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "MermiKutusu")
        {
            ammunition += Random.Range(1, 9);
            Destroy(other.gameObject);
        }
    }
  

   

}
