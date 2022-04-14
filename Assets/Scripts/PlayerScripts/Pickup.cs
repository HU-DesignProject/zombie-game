using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    RaycastShooting pistolScript;
    PlayerHealth playerHealth;

    public LayerMask layer;

    KarakterKontrol hpCheck;



    private void Start() {
        pickupAS=GetComponent<AudioSource>();
        mainCam=Camera.main;
        pistolScript=GetComponent<RaycastShooting>();
        playerHealth=GetComponent<PlayerHealth>();
        hpCheck = this.gameObject.GetComponent<KarakterKontrol>();

    }

    private void Update() {
        ray=mainCam.ViewportPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
         PickupHealthPack();
        if(Physics.Raycast(ray,out hit, pickupDistance, layer)){
            pickupText.text="Pistol Ammunition (Pickup with E)";
            pickupText.enabled=true;
            if(hit.collider.gameObject.CompareTag("PistolAmmunition")){
                PickupPistolAmmo();
            }
            else if(hit.collider.gameObject.CompareTag("HealthPack")){
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
            pistolScript.SetAmmunition(pistolScript.GetMagazineCapacity());
            pickupAS.PlayOneShot(pickupAmmoAC);
            pickupText.enabled=false;
        }
    }

    void PickupHealthPack(){
        if(Input.GetKeyDown(KeyCode.E)){
            //Destroy(hit.transform.gameObject);
            hpCheck.AddHealthPack();
            pickupAS.PlayOneShot(pickupHealthAC);
            pickupText.enabled=false;
        }
    }



    

}
