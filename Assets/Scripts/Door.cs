using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Transform PlayerCamera;
    [Header("MaxDistance you can open or close the door.")]
    public float MaxDistance = 5;

    private bool opened = false;
    private Animator _animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Pressed();
            Debug.Log("You Press E");
        }
    }

    void Pressed()
    {
        RaycastHit doorhit;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out doorhit, MaxDistance))
        {
            if (doorhit.transform.tag == "Door")
            {
                _animator = doorhit.transform.GetComponentInParent<Animator>();

                opened = !opened;

                _animator.SetBool("Opened", !opened);
            }
        }
    }


    /*public Animator _animator;

    public GameObject OpenPanel = null;

    public bool _isInsideTrigger = false;

    public string OpenText = "Press 'E' to open";

    public string CloseText = "Press 'E' to close";

    private bool _isOpen = false; 

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _isInsideTrigger = true;
            OpenPanel.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _isInsideTrigger = false;
            _animator.SetBool("open", false);
            OpenPanel.SetActive(false);
        }
    }

    private bool IsOpenPanelActive
    {
        get
        {
            return OpenPanel.activeInHierarchy;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOpenPanelActive && _isInsideTrigger)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //toggle open state
                _isOpen = !_isOpen;

                //OpenPanel.SetActive(false);
                _animator.SetBool("open", true);
            }
        }
    }*/
}
