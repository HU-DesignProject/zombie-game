using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KarakterKontrol : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    private float karakterHiz;
    [SerializeField] 
    private bool m_IsWalking = false;

    [SerializeField] 
    private float jumpSpeed;

    [SerializeField] 
    private float jumpButtonGracePeriod;

    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    private bool isJumping;
    private bool isGrounded;

    public bool damage;

    private float ySpeed;

    private float saglik = 100;
    bool hayattaMi;

    AudioSource srcSound;
    public AudioClip painSound;

    PlayerHealth healthBar;

    

    PhotonView view;
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        hayattaMi = true;
        isGrounded=true;
        damage=false;
        view = GetComponent<PhotonView>();
        healthBar=GetComponent<PlayerHealth>();
        srcSound=this.gameObject.GetComponent<AudioSource>();

    }

    void Update()
    {
        ySpeed+=Physics.gravity.y * Time.deltaTime;
        if (view.IsMine) {
            if (saglik <= 0)
            {
                saglik = 0;
                hayattaMi = false;
                anim.SetBool("yasiyorMu", hayattaMi);
            }
            if (saglik >= 100)
            {
                saglik = 100;
            }

            if (hayattaMi == true)
            {
                Hareket();
                m_IsWalking = true;

                /*if(isGrounded==true){
                    lastGroundedTime=Time.time;
                }
                if(Input.GetButtonDown("Jump")){
                    jumpButtonPressedTime=Time.time;
                }
                if(Time.time-lastGroundedTime<=jumpButtonGracePeriod){
                    ySpeed=-0.5f;
                    anim.SetBool("IsGrounded",true);
                    isGrounded=true;
                    anim.SetBool("IsJumping",false);
                    isJumping=false;
                    anim.SetBool("IsFalling",false);
                
                    if(Time.time-jumpButtonPressedTime<=jumpButtonGracePeriod){
                        ySpeed=jumpSpeed;
                        anim.SetBool("IsJumping",true);
                        isJumping=true;
                        jumpButtonPressedTime=null;
                        lastGroundedTime=null;    
                    }
                }
                else{
                    anim.SetBool("IsGrounded",false);
                    isGrounded=false;
                    if((isJumping && ySpeed<0)|| ySpeed<-2){
                        anim.SetBool("IsFalling",true);

                    }
                    

                }*/
 
            }
        }
    }
    public float GetSaglik()
    {
        return saglik;
    }
    public bool YasiyorMu()
    {
        return hayattaMi;
    }
    public void HasarAl()
    {
        srcSound.PlayOneShot(painSound);
        anim.SetBool("Damage",true);
        float damage=Random.Range(5, 10);
        saglik -= damage;
        healthBar.TakeDamage(damage);
        anim.SetBool("Damage",false);
    }
    void Hareket()
    {
        float yatay = Input.GetAxis("Horizontal");
        float dikey = Input.GetAxis("Vertical");
        anim.SetFloat("Horizontal", yatay);
        anim.SetFloat("Vertical", dikey);
        this.gameObject.transform.Translate(yatay * karakterHiz*Time.deltaTime, 0, dikey * karakterHiz*Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "SaglikKutusu")
        {
            if (saglik < 100)
            {
                saglik += Random.Range(10, 25);

                Destroy(other.gameObject);
            }
        }
    }
     public int GetPlayerStealthProfile()
        {
            if (m_IsWalking)
            {
                return 0;
            } else
            {
                return 1;
            }
        }

}
