using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshInfo))]
public class CuttableMesh : MonoBehaviour
{
    public int hitsToCut = 4;

    private int hits;
    private MeshInfo mInfo;
    private float minImpactDistance;
    private bool canCut = false;
    private Transform cuttingSrc;
    private float cuttingSrcExtents;

    private void Start()
    {
        mInfo = GetComponent<MeshInfo>();

        Collider c = GetComponent<Collider>();
        minImpactDistance = Mathf.Min(c.bounds.extents.x, Mathf.Min(c.bounds.extents.y, c.bounds.extents.z)) * 2.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canCut)
            return;

        if (other.gameObject != cuttingSrc.gameObject)
        {
            //if (Vector3.Distance(other.ClosestPointOnBounds(cuttingSrc.position), cuttingSrc.position) 
            //    < (minImpactDistance + cuttingSrcExtents + other.bounds.extents.magnitude) * 1.1f)
            //{
                CuttingTool.HitSound();
                if (hits++ >= hitsToCut)
                {
                    CuttingTool.HitSound();
                    PerformCut();
                    DisableCut();
                }
            //}
        }
    }

    public void PerformCut()
    {
        MeshCutter.MeshCut.Cut(gameObject, cuttingSrc.position, cuttingSrc.right, mInfo);
    }

    public void EnableCut(Transform cuttingSrc, Collider cuttingSrcCollider)
    {
        canCut = true;
        cuttingSrcExtents = cuttingSrcCollider.bounds.extents.magnitude;
        this.cuttingSrc = cuttingSrc;
        hits = 0;
    }

    public void DisableCut()
    {
        canCut = false;
        hits = 0;
    }
}
