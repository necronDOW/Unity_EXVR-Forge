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
    private bool isAttached;

    void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == tooltag)
        {
            if (!isAttached)
            {
                if (!other.GetComponent<Throwable>().attached)
                {
                    attachedRb = other.GetComponent<Rigidbody>();
                    attachedRb.constraints = RigidbodyConstraints.FreezeAll;
                    other.transform.position = transform.position;
                    other.transform.rotation = transform.rotation;
                    isAttached = true;
                }
                else
                {
                    other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    Highlight();
                }
            }
        }
    }

    public void AttachedCheck()
    {
        isAttached = false;
    }

    public void Highlight()
    {
        if(!isAttached)
        GetComponent<Renderer>().material = highlight;
    }
    public void UnHighlight()
    {
        GetComponent<Renderer>().material = normal;
    }
}

