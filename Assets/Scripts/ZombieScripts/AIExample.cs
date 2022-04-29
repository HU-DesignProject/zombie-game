using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class AIExample : MonoBehaviour {

    public enum WanderType { Random, Waypoint};
    private GameObject fpsc;
    public GameObject[] playerList;

    public WanderType wanderType = WanderType.Random;
    public int health = 100;
    public float wanderSpeed = 4f;
    public float chaseSpeed = 9f;
    public float fov = 120f;
    public float viewDistanceIfInAngle = 10f;
    public float viewDistance = 5f;
    public float wanderRadius = 7f;
    public float loseThreshold = 10f; //time in seconds until we lose the player after we stop detecting
    //public Transform[] waypoints; //Array of waypoints is only used when waypoint wandering is selected

    private bool isAware = false;
    private bool isDetecting = false;
    private bool isAttacking = false;

    private Vector3 wanderPoint;
    private NavMeshAgent agent;
    private Renderer renderer;
    private int waypointIndex = 0;
    private Animator animator;
    AudioSource soundSource;
    public AudioClip attackSound;
    private PhotonView photonView;
    private bool isDestroyed;

    private float loseTimer = 0;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;

    public void Awake()
    {
       
    }
    public void Start()
    {

        photonView = GetComponent<PhotonView>();
        Debug.Log("zombie start");
        agent = GetComponent<NavMeshAgent>();
        
        playerList = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("playerList  ", playerList[0]);
        fpsc = playerList[0];

        //fpsc = GetComponent<GameObject>();
        renderer = GetComponent<Renderer>();
        animator = GetComponentInChildren<Animator>();
        soundSource = GetComponent<AudioSource>();
        wanderPoint = RandomWanderPoint();
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        if (photonView.IsMine)
        {
            foreach (Collider col in ragdollColliders)
        {
            if (!col.CompareTag("Zombi"))
            {
                col.enabled = false;
            }
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
        }
        

        
        
    }
    public void Update()
    {
        if (!photonView.IsMine)
            {
                return;
            }
        if (health <= 0)
        {
            StartCoroutine(Die());
            return;
        }
        if (isAware)
        {
            agent.SetDestination(fpsc.transform.position);
            animator.SetBool("Aware", true);
            agent.speed = chaseSpeed;
            if (!isDetecting) {
                loseTimer += Time.deltaTime;
                if (loseTimer >= loseThreshold) {
                    isAware = false;
                    loseTimer = 0;
                }
            }

            if(Vector3.Distance(agent.transform.position, fpsc.transform.position) < 2f)
            {
                animator.SetBool("Attack", true);
                DoDamage();
                SetDamageSound();
            } else {
                animator.SetBool("Attack", false);
            }
            //renderer.material.color = Color.red;
        } else
        {
            Wander();
            animator.SetBool("Aware", false);
            animator.SetBool("Attack", false);
            agent.speed = wanderSpeed;
            //renderer.material.color = Color.blue;
        }
        SearchForPlayer();
    }

    public void SearchForPlayer()
    {
        for (int i = 0; i < playerList.Length; i++) {

            if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(playerList[i].transform.position)) < fov / 2f)
        {
            if (Vector3.Distance(playerList[i].transform.position, transform.position) < viewDistanceIfInAngle)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, playerList[i].transform.position, out hit, -1))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        fpsc = playerList[i];
                        OnAware();
                    } else {
                        isDetecting = false;
                    }
                } else {
                    isDetecting = false;
                }
            } else {
                isDetecting = false;
            }
        } else {
            if (Vector3.Distance(playerList[i].transform.position, transform.position) < viewDistance / 2f)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position, playerList[i].transform.position, out hit, -1))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        fpsc = playerList[i];
                        OnAware();
                    } else {
                        isDetecting = false;
                    }
                } else {
                    isDetecting = false;
                }
            } else {
                isDetecting = false;
            }
        }

        }
        
    }

    public void OnCollisionEnter(Collision collision)
        {
            if (isDestroyed)
            {
                return;
            }

            //if (collision.gameObject.CompareTag("Bullet"))
            //{
            //    if (photonView.IsMine)
            //    {
            //        //Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            //        //bullet.Owner.AddScore(isLargeAsteroid ? 2 : 1);
////
            //        //DestroyAsteroidGlobally();
            //    }
            //    else
            //    {
            //        //DestroyAsteroidLocally();
            //    }
            //}
            //else if (collision.gameObject.CompareTag("Player"))
            //{
            //    if (photonView.IsMine)
            //    {
            //        collision.gameObject.GetComponent<PhotonView>().RPC("DestroySpaceship", RpcTarget.All);
//
            //        //DestroyAsteroidGlobally();
            //    }
            //}
        }

    public void OnAware()
    {   
        isAware = true;
        isDetecting = true;
        loseTimer = 0;
        
    }

    IEnumerator Die()
    {
        isDestroyed = true;
        agent.speed = 0;
        animator.enabled = false;

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = true;
        }

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Destroy(agent.gameObject);
        

        
    }

    public void Wander()
    {
        //if (wanderType == WanderType.Random)
        //{
            if (Vector3.Distance(transform.position, wanderPoint) < 2f)
            {
                wanderPoint = RandomWanderPoint();
            }
            else
            {
                agent.SetDestination(wanderPoint);
            }
        //}
        /*else
        {
            //Waypoint wandering
            if (waypoints.Length >= 2)
            {
                if (Vector3.Distance(waypoints[waypointIndex].position, transform.position) < 2f)
                {
                    if (waypointIndex == waypoints.Length - 1)
                    {
                        waypointIndex = 0;
                    }
                    else
                    {
                        waypointIndex++;
                    }
                }
                else
                {
                    agent.SetDestination(waypoints[waypointIndex].position);
                }
            } else
            {
                Debug.LogWarning("Please assign more than 1 waypoint to the AI: " + gameObject.name);
            }
        }*/
    }

     public void GetDamage(int damage) 
    {
        health -= damage;
    }

    public void SetDamageSound()
    {
        //soundSource.PlayOneShot(attackSound);
    }

    public void DoDamage()
    {
        Debug.Log("hasar aldi");
        fpsc.GetComponent<KarakterKontrol>().HasarAl();
    }

    public Vector3 RandomWanderPoint()
    {
        Vector3 randomPoint = (Random.insideUnitSphere * wanderRadius) + transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomPoint, out navHit, wanderRadius, -1);
        return new Vector3(navHit.position.x, transform.position.y, navHit.position.z);
    }
}
