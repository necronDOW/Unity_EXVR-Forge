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
            bool attached = other.GetComponent<Throwable>().attached;
            Network_InteractableObject nio = other.GetComponent<Network_InteractableObject>();
            if (nio) attached = nio.isAttached;

            if (!attached)
            {
                attachedRb = other.GetComponent<Rigidbody>();
                attachedRb.constraints = RigidbodyConstraints.FreezeAll;
                other.transform.position = transform.position;
                other.transform.rotation = transform.rotation;// * other.transform.rotation;
                isAttached = true;

                // Needs to use the transform otherwise it doesn't work.
                // The first Collider it gets is not the AttachPoint. So we need to get the second one it returns ( a child ).
                other.transform.GetComponentsInChildren<BoxCollider>()[1].enabled = true;
            }
            else
            {
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                other.transform.GetComponentsInChildren<BoxCollider>()[1].enabled = false;
                Highlight();
            }
        }
    }

    public void AttachedCheck()
    {
        isAttached = false;
    }

    public void Highlight()
    {
        if (!isAttached)
            GetComponent<Renderer>().material = highlight;
    }

    public void UnHighlight()
    {
        GetComponent<Renderer>().material = normal;
    }
}

