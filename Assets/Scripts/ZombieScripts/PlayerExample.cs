using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerExample : MonoBehaviour {
    public AudioClip shootSound;
    public float soundIntensity = 5f;
    public float walkEnemyPerceptionRadius = 1f;
    public float sprintEnemyPerceptionRadius = 1.5f;
    public LayerMask zombieLayer;
    public Transform spherecastSpawn;
    private AudioSource audioSource;
    private KarakterKontrol fpsc;
    private SphereCollider sphereCollider;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fpsc = GetComponent<KarakterKontrol >();
        sphereCollider = GetComponent<SphereCollider>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        if (fpsc.GetPlayerStealthProfile() == 0)
        {
            sphereCollider.radius = walkEnemyPerceptionRadius;
        } else
        {
            sphereCollider.radius = sprintEnemyPerceptionRadius;
        }

        if ((fpsc.transform.position.x <= 72 && fpsc.transform.position.x >= 66) && (fpsc.transform.position.z <= -80 && fpsc.transform.position.z >= -82)) 
        {
            StartCoroutine(FinishGame());
        }
        
    }

    public void Fire()
    {
        audioSource.PlayOneShot(shootSound);
        Collider[] zombies = Physics.OverlapSphere(transform.position, soundIntensity, zombieLayer);
        for (int i = 0; i < zombies.Length; i++)
        {
            zombies[i].GetComponent<AIExample>().OnAware();
            Debug.Log("Fire()");
        } 
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zombi"))
        {
            other.GetComponent<AIExample>().OnAware();
             Debug.Log("OnTriggerEnter()");
        }
    }

    IEnumerator FinishGame()
    {
        yield return new WaitForSeconds(5f);
            Debug.Log("GAME OVER");
            SceneManager.LoadScene("FinishScene");
        
    }
}
