using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshInfo))]
public class CuttableMesh : MonoBehaviour
{
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
        Mesh[] meshes = SplitMesh(mInfo.mFilter.sharedMesh, cuttingLoop * mInfo.loopSpacing);

        GameObject go = new GameObject("left");
        go.AddComponent<MeshFilter>().mesh = meshes[0];

        GameObject go2 = new GameObject("right");
        go2.AddComponent<MeshFilter>().mesh = meshes[1];
    }

    public Mesh[] SplitMesh(Mesh m, int splitIndex)
    {
        Vector3[] m_verts = m.vertices;
        int[] m_tris = m.triangles;
        int triangleSplitIndex = splitIndex * 6;

        Mesh left = SplitMesh(m_verts, 0, splitIndex, m_tris, 0, triangleSplitIndex);
        Mesh right = SplitMesh(m_verts, splitIndex, mInfo.vxr_topCap.start, m_tris, triangleSplitIndex, mInfo.tr_topCap.start);

        return new Mesh[2] { left, right };
    }

    private Mesh SplitMesh(Vector3[] verts, int vStart, int vEnd, int[] tris, int tStart, int tEnd)
    {
        Mesh m = new Mesh();
        m.vertices = GetVerticesInRange(verts, vStart, vEnd);
        m.triangles = GetTrianglesInRange(tris, tStart, tEnd, vStart);
        m.RecalculateNormals();
        m.RecalculateBounds();

        return m;
    }

    private Vector3[] GetVerticesInRange(Vector3[] verts, int start, int end)
    {
        end = Mathf.Min(end, verts.Length);
        Vector3[] subVertices = new Vector3[end - start];

        for (int i = start, sV = 0; i < end; i++, sV++)
            subVertices[sV] = verts[i];

        return subVertices;
    }

    private int[] GetTrianglesInRange(int[] tris, int start, int end, int vertexComp)
    {
        end = Mathf.Min(end, tris.Length);
        int[] subTris = new int[end - start];

        for (int i = start, sT = 0; i < end; i++, sT++)
            subTris[sT] = tris[i] - vertexComp;

        return subTris;
    }
}
