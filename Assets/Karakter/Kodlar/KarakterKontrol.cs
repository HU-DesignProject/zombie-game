using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarakterKontrol : MonoBehaviour
{
    Animator anim;
    [SerializeField]
    private float karakterHiz;

    private float saglik = 100;
    bool hayattaMi;
    void Start()
    {
        anim = this.GetComponent<Animator>();
        hayattaMi = true;
    }

    void Update()
    {
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
        saglik -= Random.Range(5, 10);
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
}
