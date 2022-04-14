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

    public Text infoText;
    //public GameObject impactEffect;
    private float bullet = 30;
    private float ammunition = 120;
    private float bulletCapacity = 30;
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
                if (bullet > 0)
                {
                    anim.SetBool("Shooting", true);
                }
                if (bullet <= 0)
                {
                    anim.SetBool("Shooting", false);
                }
                if (bullet <= 0 && ammunition > 0)
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
        ammunition -= bulletCapacity - bullet;
        bullet = bulletCapacity;
        anim.SetBool("Reload", false);

    }
    public void Shooting()
    {
        if (bullet > 0)
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
            bullet--;
        }
        else{
            infoText.text="No more bullets.";
        }


    }
    
    public float GetBullet()
    {
        return bullet;
    }
    public float GetAmmunition()
    {
        return ammunition;
    }
    public void SetAmmunition(float addedAmmunition)
    {
        ammunition+=addedAmmunition;
    }
    public float GetBulletCapacity()
    {
        return bulletCapacity;
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
