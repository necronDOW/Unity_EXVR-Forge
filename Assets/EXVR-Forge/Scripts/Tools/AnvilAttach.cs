using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]

public class AnvilAttach : MonoBehaviour {

    public string tooltag;
    public Material highlight;
    public Material normal;
    private Rigidbody attachedRb;

    void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == tooltag)
        {
            if (!other.GetComponent<Throwable>().attached)
            {
                attachedRb = other.GetComponent<Rigidbody>();
                attachedRb.constraints = RigidbodyConstraints.FreezeAll;
                other.transform.position = transform.position;
                other.transform.rotation = transform.rotation;
            }
            else
            {
                attachedRb.constraints = RigidbodyConstraints.None;
            }
        }
    } 

    public void Highlight()
    {
        GetComponent<Renderer>().material = highlight;
    }
    public void UnHighlight()
    {
        GetComponent<Renderer>().material = normal;
    }
}

