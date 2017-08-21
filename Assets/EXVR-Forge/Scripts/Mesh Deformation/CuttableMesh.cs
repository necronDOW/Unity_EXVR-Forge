using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshInfo))]
public class CuttableMesh : MonoBehaviour
{
    public Material capMaterial;

    public int hitsToCut = 4;
    public float cutCooldown = 5.0f;
    [HideInInspector] public CuttingTool cuttingTool;
    [HideInInspector] public float minImpactDistance;

    private int hits = 0;
    private bool canCut = true;
    private MeshInfo mInfo;

    private void Start()
    {
        mInfo = GetComponent<MeshInfo>();

        Collider c = GetComponent<Collider>();
        minImpactDistance = Mathf.Min(c.bounds.extents.x, Mathf.Min(c.bounds.extents.y, c.bounds.extents.z)) * 2.0f;
    }

    float cooldownTimer = 0.0f;
    private void Update()
    {
        if (!canCut)
        {
            if ((cooldownTimer += Time.deltaTime) >= cutCooldown)
            {
                canCut = true;
                cooldownTimer = 0.0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cuttingTool && other.transform.gameObject != cuttingTool.transform.gameObject)
        {
            if (!canCut)
                return;

            if (Vector3.Distance(other.transform.position, cuttingTool.transform.position) 
                < (minImpactDistance + other.bounds.extents.magnitude) * 1.1f)
            {
                if (hits++ >= hitsToCut)
                {
                    PerformCut();
                    canCut = false;
                    hits = 0;
                }
            }
        }
    }

    public void PerformCut()
    {
        int cuttingLoop = mInfo.ClosestLoopToPoint(cuttingTool.transform.position);
        MeshCutter.MeshCut.Cut(gameObject, cuttingTool.transform.position, cuttingTool.transform.right, capMaterial);
    }
}
