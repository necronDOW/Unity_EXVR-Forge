using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CuttingTool : MonoBehaviour {

    public string tooltag;
    public static bool rodAttached;
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
                rodAttached = true;
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

    public void ReEnableCutTool()
    {
        parent.GetComponent<BoxCollider>().enabled = true;
        GetComponent<Collider>().enabled = true;
        rodAttached = false;
        Cutter.cut = false;
    }

    //on collide and side button press

    //spawn bend prefab


}
