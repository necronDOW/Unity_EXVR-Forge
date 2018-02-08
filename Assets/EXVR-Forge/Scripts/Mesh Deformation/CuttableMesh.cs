using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(MeshInfo))]
public class CuttableMesh : MonoBehaviour
{
    public int hitsToCut = 4;
    public GameObject rodPrefab;

    private int hits;
    private MeshInfo mInfo;
    private float minImpactDistance;
    private bool canCut = false;
    private CuttingTool cuttingSrc;
    private float cuttingSrcExtents;
    private DeformableMesh deformableMesh;

    private void Start()
    {
        mInfo = GetComponent<MeshInfo>();

        Collider c = GetComponent<Collider>();
        minImpactDistance = Mathf.Min(c.bounds.extents.x, Mathf.Min(c.bounds.extents.y, c.bounds.extents.z)) * 2.0f;

        deformableMesh = GetComponent<DeformableMesh>();
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
                if (hits++ >= hitsToCut) {
                    PerformCut();
                }
            }
        }
    }

    public virtual void PerformCut()
    {
        Network_CuttableMesh ncm = GetComponent<Network_CuttableMesh>();
        if (ncm) {
            ncm.CmdOnCut(cuttingSrc.transform.position, cuttingSrc.transform.right, cuttingSrc.cuttingDiameter);
        }
    }

    public void EnableCut(Transform cuttingSrc, Collider cuttingSrcCollider)
    {
        canCut = true;
        cuttingSrcExtents = cuttingSrcCollider.bounds.extents.magnitude;
        this.cuttingSrc = cuttingSrc.GetComponent<CuttingTool>();
        hits = 0;

        if (deformableMesh)
            deformableMesh.enabled = false;
    }

    public void DisableCut()
    {
        canCut = false;
        hits = 0;

        if (deformableMesh)
            deformableMesh.enabled = true;
    }
}
