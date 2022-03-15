using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawner : MonoBehaviour {

    public GameObject theZombie; 
    public int zombieCount;
    public int zombieCountInMaze;
	public GameObject maze;
	public byte[,] map;
    public void Start()
    {
		//maze = GetComponent<PipeMaze>();
		maze = GameObject.Find("Maze");

		SpawnZombieInMaze();
        //StartCoroutine(SpawnZombie());
		Debug.Log("ZombieSpawner dayim ");
    }

	public async void SpawnZombieInMaze()
    {

		map = maze.GetComponent<PipeRecursive>().SendMap();
		int depth = maze.GetComponent<PipeRecursive>().depth;
		int width = maze.GetComponent<PipeRecursive>().width;
		
		for (int z = 0; z < depth; z++)
            for (int x = 0; x < width; x++)
            {
                if (map[x, z] != 1)
                {
					StartCoroutine(SpawnZombie(x, z));
				}
			}
		
    }
    IEnumerator SpawnZombie(int x, int z)
    {
		int initialX = maze.GetComponent<PipeRecursive>().initialX;
		int initialY = maze.GetComponent<PipeRecursive>().initialY;
		int initialZ = maze.GetComponent<PipeRecursive>().initialZ;
		int scale = maze.GetComponent<PipeRecursive>().scale;


		Instantiate(theZombie, new Vector3(initialX + x * scale, 0, initialZ + z * scale), Quaternion.identity);


		yield return new WaitForSeconds(5f);
        /*while (zombieCount < 10 )
        {
            Instantiate(theZombie, new Vector3(Random.Range(0,50), 0, Random.Range(0,50)), Quaternion.identity);
            yield return new WaitForSeconds(5f);
            zombieCount += 1;
        }*/
    }


/*
	public float respawnDuration = 5.0f;
	public List<GameObject> spawnPoints = new List<GameObject>();
    public int startHealth = 100;
	public float startMoveSpeed = 1f;
	public float startDamage = 15f;
	public int startEXP = 3;
	public int startFund = 5;
	public float upgradeDuration = 60f;	// Increase all enemy stats every 30 seconds

    private float upgradeTimer;
	[SerializeField]
	private int currentHealth;
	[SerializeField]
	private float currentMoveSpeed;
	[SerializeField]
	private float currentDamage;
	[SerializeField]
	private int currentEXP;
	[SerializeField]
	private int currentFund;
    private float spawnTimer;
    public Transform target;
    public GameObject zombie;
	private List<GameObject> enemies = new List<GameObject>();

	void Start() {
        currentHealth = startHealth;
		currentMoveSpeed = startMoveSpeed;
		currentDamage = startDamage;
		currentEXP = startEXP;
		currentFund = startFund;

        enemies.Add(zombie);

    }

    void Update() {
		if(spawnTimer < respawnDuration) {
			spawnTimer += Time.deltaTime;
		}
		else {
			SpawnEnemy();
		}

		if(upgradeTimer < upgradeDuration) {
			upgradeTimer += Time.deltaTime;
		}
		else {
			UpgradeEnemy();
		}
	}

    float GetDistanceFrom(Vector3 src, Vector3 dist) {
		return Vector3.Distance(src, dist);
	}

    void SpawnEnemy() {
		if(spawnTimer < respawnDuration) return;

		foreach(GameObject spawnPoint in spawnPoints) {
			GameObject zombie = enemies[0];
			zombie.GetComponent<AIExample>().fpsc = target;
			zombie.GetComponent<NavMeshAgent>().speed = currentMoveSpeed;
			zombie.GetComponent<AIExample>().health = currentHealth;

			// Boost rotating speed
			float rotateSpeed = 120f + currentMoveSpeed;
			rotateSpeed = Mathf.Max(rotateSpeed, 200f);	// Max 200f
			zombie.GetComponent<NavMeshAgent>().angularSpeed = rotateSpeed;

			// PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, spawnPoint.transform.rotation, 0);
			Instantiate(zombie, spawnPoint.transform.position, spawnPoint.transform.rotation);
		}
		
		spawnTimer = 0f;
	}

    void UpgradeEnemy() {
		print("ENEMY UPGRADED");

		currentHealth += 5;

		if(currentMoveSpeed < 4f) {
			currentMoveSpeed += 0.2f;
		}
		if(currentDamage < 51f) {
			currentDamage += 2f;
		}
		
		currentEXP++;
		currentFund++;

		upgradeTimer = 0;
	}*/

   /* public GameObject[] zombies;
    public GameObject zombie;
	private void Start() {
        zombies = new GameObject[5];

        for (int i = 0; i < zombies.Length; i++ )
        {
            zombies[i] = transform.GetChild(i).gameObject;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnZombie();
        }
    }

    private void SpawnZombie()
    {
        int zombieID = Random.Range(0, zombies.Length);
        Instantiate(zombie, zombies[zombieID].transform.position, zombies[zombieID].transform.rotation);
    }
*/
}