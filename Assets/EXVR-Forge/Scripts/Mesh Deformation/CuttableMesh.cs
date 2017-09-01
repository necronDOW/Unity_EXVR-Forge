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
    private CuttingTool cuttingSrc;
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
            if (Vector3.Distance(other.transform.position, cuttingSrc.transform.position) 
                < (minImpactDistance + cuttingSrcExtents + other.bounds.extents.magnitude) * 1.1f)
            {
                Debug.Log(hits);
                if (hits++ >= hitsToCut)
                {
                    PerformCut();
                    DisableCut();
                }
            }
        }
    }

    public virtual void PerformCut(bool allowNetworking = true)
    {
        Network_CuttableMesh ncm = GetComponent<Network_CuttableMesh>();

        if (ncm && allowNetworking)
            ncm.CmdOnCut();
        else MeshCutter.MeshCut.Cut(gameObject, cuttingSrc.transform.position, cuttingSrc.transform.right, cuttingSrc.cuttingDiameter);
    }

    public void EnableCut(Transform cuttingSrc, Collider cuttingSrcCollider)
    {
        canCut = true;
        cuttingSrcExtents = cuttingSrcCollider.bounds.extents.magnitude;
        this.cuttingSrc = cuttingSrc.GetComponent<CuttingTool>();
        hits = 0;
    }

    public void DisableCut()
    {
        canCut = false;
        hits = 0;
    }
}
