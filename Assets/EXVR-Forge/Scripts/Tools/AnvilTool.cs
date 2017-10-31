using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class AnvilTool : MonoBehaviour
{
    public string lookupTag = "";
    public Collider[] colliders; // 0 = parent, 1 = this.

    protected virtual void Start()
    {
        colliders = transform.parent.GetComponentsInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == lookupTag)
        {
            colliders[0].enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == lookupTag)
        {
            if (IsAttached(other.gameObject))
                Unfreeze(other.gameObject);
            else Freeze(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == lookupTag)
        {
            colliders[0].enabled = true;
            Unfreeze(other.gameObject);
        }
    }

    protected virtual void Freeze(GameObject o)
    {
        o.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
    }

    protected virtual void Unfreeze(GameObject o)
    {
        o.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private bool IsAttached(GameObject other)
    {
        Throwable t = other.GetComponent<Throwable>();
        Network_InteractableObject nio = other.GetComponent<Network_InteractableObject>();

        return ((t && t.attached) || (nio && nio.isAttached));
        //    return true;
        //else return false;
    }
    
    public void SetAttachState(bool state)
    {
        colliders[0].enabled = !state;
        colliders[1].enabled = state;
    }

    public void ReEnableTool()
    {
        colliders[0].enabled = true;
        colliders[1].enabled = false;
    }
}
