using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Collider))]
public class CuttingTool : MonoBehaviour
{
    public static AudioSource hitSound;
    
    private Collider[] colliders; // 0 = parent, 1 = this.
    private CuttableMesh cutTarget;

    void Start()
    {
        colliders = transform.parent.GetComponentsInChildren<Collider>();
        hitSound = GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other)
    {
        CuttableMesh cutTarget = other.GetComponent<CuttableMesh>();

        if (cutTarget)
        {
            if (!this.cutTarget)
            {
                Throwable t = other.GetComponent<Throwable>();
                if (t && !t.attached)
                {
                    colliders[0].enabled = false;
                    this.cutTarget = cutTarget;
                    other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    cutTarget.EnableCut(transform, colliders[1]);
                }
            }
            else if (this.cutTarget == cutTarget)
            {
                Throwable t = other.GetComponent<Throwable>();

                if (t && t.attached)
                {
                    other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    cutTarget = null;
                }
            }
        }
    }

    public void ReEnableCutTool()
    {
        foreach (Collider c in colliders)
            c.enabled = true;

        if (cutTarget)
            cutTarget.DisableCut();
    }

    public static void HitSound()
    {
        //hitSound.Play();
    }
}
