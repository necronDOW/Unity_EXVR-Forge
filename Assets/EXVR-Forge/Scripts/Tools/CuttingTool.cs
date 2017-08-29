using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Collider))]
public class CuttingTool : MonoBehaviour
{
    public static AudioSource hitSound;
    public float cuttingDiameter;

    private Collider[] colliders; // 0 = parent, 1 = this.
    private CuttableMesh cutTarget;

    void Start()
    {
        colliders = transform.parent.GetComponentsInChildren<Collider>();
        hitSound = GetComponent<AudioSource>();

        if (cuttingDiameter == 0.0f)
            cuttingDiameter = Mathf.Min(Mathf.Min(colliders[0].bounds.extents.x, colliders[0].bounds.extents.y), colliders[0].bounds.extents.z) * 2.0f;
    }

    private void Update()
    {
        DrawDebug();
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

    private void DrawDebug()
    {
        Vector3 topRight = transform.position + (transform.forward + transform.up).normalized * cuttingDiameter;
        Vector3 bottomRight = transform.position + (transform.forward + -transform.up).normalized * cuttingDiameter;
        Vector3 bottomLeft = transform.position + (-transform.forward + -transform.up).normalized * cuttingDiameter;
        Vector3 topLeft = transform.position + (-transform.forward + transform.up).normalized * cuttingDiameter;

        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);
        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomLeft, Color.red);
        Debug.DrawLine(topLeft, bottomRight, Color.red);
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
