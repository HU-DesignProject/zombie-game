using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Pickup : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    
    [SerializeField]
    float pickupDistance=10f;


    AudioSource pickupAS;
    [Header("Audio")]
    public AudioClip pickupAmmoAC;
    public AudioClip pickupHealthAC;

    Camera mainCam;

    public Text pickupText;

    AtesSistemi pistolScript;
    PlayerHealth playerHealth;

    public LayerMask layer;

    KarakterKontrol hpCheck;

    public PhotonView photonView;

    private void Start() {
        pickupAS=GetComponent<AudioSource>();
        mainCam=Camera.main;
        pistolScript=GetComponent<AtesSistemi>();
        playerHealth=GetComponent<PlayerHealth>();
        hpCheck = this.gameObject.GetComponent<KarakterKontrol>();
        photonView = GetComponent<PhotonView>();

    }

    private void Update() {
        ray=mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        //PickupHealthPack();
        if(Physics.Raycast(ray,out hit, pickupDistance, layer)){
            
            if(hit.collider.gameObject.CompareTag("PistolAmmo")){
                pickupText.text="Pistol Ammunition (Pickup with E)";
                pickupText.enabled=true;
                PickupPistolAmmo();
            }
            else if(hit.collider.gameObject.CompareTag("HealthyPack")){
                pickupText.text="Healthy Pack (Pickup with E)";
                pickupText.enabled=true;
                PickupHealthPack();
            }
        }
        else{
            Debug.Log("OLMADI MI");
            pickupText.enabled=false;
        }

    }

    void PickupPistolAmmo(){
        if(Input.GetKeyDown(KeyCode.E)){
            Destroy(hit.transform.gameObject);
            pistolScript.SetAmmunition(pistolScript.GetBulletCapacity());
            pickupAS.PlayOneShot(pickupAmmoAC);
            pickupText.enabled=false;
        }
    }

    void PickupHealthPack(){
        if(Input.GetKeyDown(KeyCode.E)){
            Destroy(hit.transform.gameObject);
            hpCheck.AddHealthPack();
            pickupAS.PlayOneShot(pickupHealthAC);
            pickupText.enabled=false;
        }
    }



    

}
