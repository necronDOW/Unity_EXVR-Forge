using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Collider))]
public class CuttingTool : MonoBehaviour
{
    public AudioSource hitSound;
    
    private Collider[] colliders; // 0 = parent, 1 = this.
    private CuttableMesh cutTarget;

    void Start()
    {
        colliders = transform.parent.GetComponentsInChildren<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        CuttableMesh cutTarget = other.GetComponent<CuttableMesh>();

        if (cutTarget) {
            colliders[0].enabled = false;

            if (true) {//!other.GetComponent<Throwable>().attached) {
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                cutTarget.cuttingTool = this;
                cutTarget.minImpactDistance += colliders[1].bounds.extents.magnitude;
            }
            else {
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
        }
    }

    public void ReEnableCutTool()
    {
        foreach (Collider c in colliders)
            c.enabled = true;

        if (cutTarget)
        {
            cutTarget.cuttingTool = null;
            cutTarget.minImpactDistance -= colliders[1].bounds.extents.magnitude;
        }
    }
}
