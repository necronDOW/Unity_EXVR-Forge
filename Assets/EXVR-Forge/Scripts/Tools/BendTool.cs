using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Collider))]
public class BendTool : AnvilTool
{
    public GameObject bendPrefab;
    private Network_BendTool nbt;

    protected override void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        nbt = GetComponentInParent<Network_BendTool>();

        base.Start();
    }

    protected override void Freeze(GameObject o)
    {
        base.Freeze(o);
        
        if (nbt)
            nbt.OnAttachToAnvil();
    }

    protected override void Unfreeze(GameObject o)
    {
        if (nbt)
            nbt.DestroyAllBendInstances();

        if (attachedRod) {
            attachedRod.GetComponent<DeformableBase>().UpdateMeshCollider(true);
        }

        base.Unfreeze(o);
    }

    public void UnfreezePublic()
    {
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }
}
