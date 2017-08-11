using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class BendTool : MonoBehaviour {

    public string tooltag;

    private Rigidbody attachedRb;
    private GameObject parent;

    void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        parent = this.transform.parent.gameObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == tooltag)
        {
            parent.GetComponent<BoxCollider>().enabled = false;

            if (!other.GetComponent<Throwable>().attached)
            {
                GetComponent<Collider>().enabled = false;
                attachedRb = other.GetComponent<Rigidbody>();
                attachedRb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else
            {
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
    }

    public void ReEnableBendTool()
    {
        parent.GetComponent<BoxCollider>().enabled = true;
        GetComponent<Collider>().enabled = true;
    }

    //on collide and side button press

    //spawn bend prefab


}
