using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieScript : MonoBehaviour
{
    Animator zombiAnim;
    public Transform aim;
    NavMeshAgent Agent;
    public float distance;

    // Start is called before the first frame update
    void Start()
    {
        zombiAnim = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        zombiAnim.SetFloat("speed", Agent.velocity.magnitude);
        distance = Vector3.Distance(transform.position, aim.position);
        Agent.destination = aim.position;
        if (distance <= 10)
        {
            Agent.enabled = true;
        } else
        {
            Agent.enabled = false;
        }

    }
}
