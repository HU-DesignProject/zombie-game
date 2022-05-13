using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class KarakterKontrol : MonoBehaviour, IPunObservable
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

    public AudioClip shootSound;
    public float soundIntensity = 5f;
    public float walkEnemyPerceptionRadius = 1f;
    public float sprintEnemyPerceptionRadius = 1.5f;
    public LayerMask zombieLayer;
    public Transform spherecastSpawn;
    private AudioSource audioSource;
    private SphereCollider sphereCollider;

    #region Public Fields
    [Tooltip("The current Health of our player")]
    public float playerHealth = 100f;
    public float bulletCount;
    public int zombieKillCount = 0;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    #endregion

    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    private GameObject playerUiPrefab;

    bool hayattaMi;
    AudioSource srcSound;
    public AudioClip painSound;

    PlayerHealth healthBar;
    
    [SerializeField]
    public PhotonView photonView;

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    public void Awake()
    {
        /*if (this.beams == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
        }
        else
        {
            this.beams.SetActive(false);
        }*/
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
        if (photonView.IsMine)
        {
            //playerUiPrefab = GameManager.FindObjectOfType<GameObject>();
            LocalPlayerInstance = gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sphereCollider = GetComponent<SphereCollider>();
        photonView = GetComponent<PhotonView>();

        anim = this.gameObject.GetComponent<Animator>();
        hayattaMi = true;
        isGrounded=true;
        damage=false;
        //view = GetComponent<PhotonView>();
        healthBar=GetComponent<PlayerHealth>();
        srcSound=this.gameObject.GetComponent<AudioSource>();

    
        // Create the UI
        if (this.playerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
        }
        #if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        #endif


        
       
    }

    void Update()
    {
        ySpeed+=Physics.gravity.y * Time.deltaTime;
        if (photonView.IsMine) {
            if (Input.GetMouseButtonDown(0))
            {
                Fire();
            }
            if (GetPlayerStealthProfile() == 0)
            {
                sphereCollider.radius = walkEnemyPerceptionRadius;
            } else
            {
                sphereCollider.radius = sprintEnemyPerceptionRadius;
            }

            if (playerHealth <= 0)
            {
                StartCoroutine(CharacterDie());
            }
            if (playerHealth >= 100)
            {
                playerHealth = 100;
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
    public float GetPlayerHealth()
    {
        return playerHealth;
    }
    public bool YasiyorMu()
    {
        return hayattaMi;
    }
    public IEnumerator HasarAl()
    {
        if (photonView.IsMine)
        {   srcSound.PlayOneShot(painSound);
            anim.SetBool("Damage",true);
            float damage=Random.Range(5, 10);
            playerHealth -= damage;
            healthBar.TakeDamage(damage);
            yield return new WaitForSeconds(5f);
            anim.SetBool("Damage",false);
        }

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
         if (!photonView.IsMine)
            {
                return;
            }
        if (other.gameObject.tag == "SaglikKutusu")
        {
            if (playerHealth < 100)
            {
                playerHealth += Random.Range(10, 25);

                //Destroy(other.gameObject);
                DestroyPlayerObject(other);
            }
        }
        if (other.gameObject.CompareTag("Zombi"))
        {
            other.GetComponent<ZombieAI>().OnAware();
        }
    }

    public void DestroyPlayerObject(Collider other) {
        if (!photonView.IsMine)
            {
                return;
            }
        PhotonNetwork.Destroy(other.gameObject);

    }

    public void DestroyPlayerObject() {
        if (!photonView.IsMine)
            {
                return;
            }
        //PhotonNetwork.Destroy(this.f);

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

        /// <summary>
        /// MonoBehaviour method called after a new level of index 'level' was loaded.
        /// We recreate the Player UI because it was destroy when we switched level.
        /// Also reposition the player if outside the current arena.
        /// </summary>
        /// <param name="level">Level index loaded</param>
        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }


    #if UNITY_5_4_OR_NEWER
	void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
	{
		this.CalledOnLevelWasLoaded(scene.buildIndex);
	}
	#endif

            #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                //stream.SendNext(this.IsFiring);
                stream.SendNext(this.playerHealth);
                stream.SendNext(this.bulletCount);
            }
            else
            {
                // Network player, receive data
                //this.IsFiring = (bool)stream.ReceiveNext();
                this.playerHealth = (float)stream.ReceiveNext();
                this.bulletCount = (float)stream.ReceiveNext();
            }
        }

        #endregion

      

    public void Fire()
    {
        audioSource.PlayOneShot(shootSound);
        Collider[] zombies = Physics.OverlapSphere(transform.position, soundIntensity, zombieLayer);
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].GetComponent<ZombieAI>().OnAware();
        } 
    }
    
    IEnumerator CharacterDie()
    {
        playerHealth = 0;
        hayattaMi = false;
        anim.SetBool("yasiyorMu", hayattaMi);
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Destroy(LocalPlayerInstance);
        //GetComponent<GameManager>().QuitApplication();
        
    }
}   
