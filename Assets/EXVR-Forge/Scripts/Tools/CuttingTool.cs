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

    private void OnTriggerEnter(Collider other)
    {
        CuttableMesh cutTarget = other.GetComponent<CuttableMesh>();

        if (cutTarget) {
            this.cutTarget = cutTarget;
            colliders[0].enabled = false;

            Throwable t = other.GetComponent<Throwable>();
            if (true) {//(t && !t.attached) {
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                cutTarget.EnableCut(transform, colliders[1]);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        CuttableMesh cutTarget = other.GetComponent<CuttableMesh>();

        if (cutTarget && this.cutTarget == cutTarget)
        {
            Throwable t = other.GetComponent<Throwable>();
            if (t && t.attached)
            {
                other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                ReEnableCutTool();
                cutTarget = null;
            }
        }
    }

    public void ReEnableCutTool()
    {
        foreach (Collider c in colliders)
            c.enabled = true;

        cutTarget.DisableCut();
    }
}
