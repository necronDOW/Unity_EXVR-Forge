using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Collider))]
public class BendTool : AnvilTool
{

    protected override void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        base.Start();
    }

    protected override void Freeze(GameObject o)
    {
        base.Freeze(o);

   
    }

    protected override void Unfreeze(GameObject o)
    {
        base.Unfreeze(o);

 
    }
    public void UnfreezePublic()
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.tag == tooltag)
    //    {
    //        parent.GetComponent<BoxCollider>().enabled = false;

    //        if (!other.GetComponent<Throwable>().attached)
    //        {
    //            GetComponent<Collider>().enabled = false;
    //            attachedRb = other.GetComponent<Rigidbody>();
    //            attachedRb.constraints = RigidbodyConstraints.FreezeAll;
    //        }
    //        else
    //        {
    //            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    //        }
    //    }
    //}

    //public void ReEnableBendTool()
    //{
    //    parent.GetComponent<BoxCollider>().enabled = true;
    //    GetComponent<Collider>().enabled = true;
    //}

    //on collide and side button press

    //spawn bend prefab


}
