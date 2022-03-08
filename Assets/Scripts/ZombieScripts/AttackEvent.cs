using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackEvent : MonoBehaviour {
    private PlayerExample pe;
    public void Start() 
    {
        pe = GetComponent<PlayerExample>();
    }
    public void DamageEvent()
    {
        pe.Fire();
    }
}