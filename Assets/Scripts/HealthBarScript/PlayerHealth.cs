using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float lerpTimer;
    public float maxHealth=100f;
    public float chipSpeed=2f;
    public Image frontHealthBar;
    public Image backHealthBar;

    [Header ("Damage Screen")]
    public Color damageColor;
    public Image damageImage;
    float colorSmoothing=6f;
    bool isTakingDamage=false;

    GameObject targetPlayer;
    


    /*Color blue = new Color(255, 171, 3);
    Color orange = new Color(236,186,34);*/

    // Start is called before the first frame update
    void Start()
    {
        targetPlayer = GameObject.Find("SWAT");
        health=targetPlayer.GetComponent<KarakterKontrol>().GetSaglik();
        damageImage.enabled=false;


    }

    // Update is called once per frame
    void Update()
    {
        health=Mathf.Clamp(health,0,maxHealth);
        UpdateHealthUI();
        if(Input.GetKeyDown(KeyCode.N)){
            TakeDamage(Random.Range(5,10));
        }
        if(Input.GetKeyDown(KeyCode.M)){
            RestoreHealth(Random.Range(5,10));
        }

        if(isTakingDamage){
            damageImage.enabled=true;
            damageImage.color=damageColor;

        }
        else{
            damageImage.color=Color.Lerp(damageImage.color,Color.clear,colorSmoothing*Time.deltaTime*Time.deltaTime);

        }

        isTakingDamage=false;

    }


    public void UpdateHealthUI(){
        Debug.Log(health);
        float fillF=frontHealthBar.fillAmount;
        float fillB=backHealthBar.fillAmount;
        float hFraction=health/maxHealth;

        if(fillB>hFraction){
            frontHealthBar.fillAmount=hFraction;
            backHealthBar.color=Color.red;
            lerpTimer+=Time.deltaTime;
            float percentComplete =lerpTimer/chipSpeed;
            percentComplete=percentComplete*percentComplete;
            backHealthBar.fillAmount=Mathf.Lerp(fillB,hFraction,percentComplete);
        }

        if(fillF<hFraction){
            backHealthBar.color=Color.green;
            backHealthBar.fillAmount=hFraction;
            lerpTimer+=Time.deltaTime;
            float percentComplete =lerpTimer/chipSpeed;
            percentComplete=percentComplete*percentComplete;
            frontHealthBar.fillAmount=Mathf.Lerp(fillF,backHealthBar.fillAmount,percentComplete);
        }
    }

    public void TakeDamage(float damage){
        isTakingDamage=true;
        Debug.Log(damage);
        health-=damage;
        lerpTimer=0f;
    }

    public void RestoreHealth(float healAmount){
        health+=healAmount;
        lerpTimer=0f;
    }


}
