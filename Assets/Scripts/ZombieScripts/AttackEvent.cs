using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackEvent : MonoBehaviour {
    private KarakterKontrol pe;
    public void Start() 
    {
        pe = GetComponent<KarakterKontrol>();
    }
    public void DamageEvent()
    {
        pe.Fire();
    }
}