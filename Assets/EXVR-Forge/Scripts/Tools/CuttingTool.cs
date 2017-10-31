using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Collider))]
public class CuttingTool : AnvilTool
{
    public float cuttingDiameter;

    private CuttableMesh cutTarget;

    protected override void Start()
    {
        base.Start();

        if (cuttingDiameter == 0.0f)
            cuttingDiameter = Mathf.Min(Mathf.Min(colliders[0].bounds.extents.x, colliders[0].bounds.extents.y), colliders[0].bounds.extents.z) * 2.0f;
    }

    private void Update()
    {
        DrawDebug();
    }

    protected override void Freeze(GameObject o)
    {
        base.Freeze(o);

        if (this.cutTarget == null)
        {
            this.cutTarget = o.GetComponent<CuttableMesh>();
            this.cutTarget.EnableCut(transform, colliders[1]);
        }
    }

    protected override void Unfreeze(GameObject o)
    {
        base.Unfreeze(o);

        if (o.GetComponent<CuttableMesh>() == this.cutTarget)
        {
            this.cutTarget.DisableCut();
            this.cutTarget = null;
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
}
