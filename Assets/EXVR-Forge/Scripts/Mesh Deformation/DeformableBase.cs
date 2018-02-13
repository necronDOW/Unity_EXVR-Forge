using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class DeformableBase : MonoBehaviour
{
    public float colliderSimplificationFactor = 0.4f;

    protected MeshFilter mFilter;
    protected MeshCollider mCollider;
    protected int[] originalTriangles;
    protected Vector3[] originalVertices;
    protected Mesh simplifiedMesh;
    
    private CubeMeshGenerator.GeneratorParams simplifiedGParams {
        get {
            return CubeMeshGenerator.gParams.ApplySimplification(0.4f);
        }
    }

    protected virtual void Start()
    {
        UpdateComponents();
    }

    protected virtual void OnApplicationQuit()
    {
        ResetMesh();
    }

    public static Vector3 DivideVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    protected void ResetMesh()
    {
        mFilter.sharedMesh.triangles = originalTriangles;
        mFilter.sharedMesh.vertices = originalVertices;
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = mFilter.sharedMesh;
    }

    protected void UpdateComponents()
    {
        if (!mFilter || !mCollider) {
            mFilter = GetComponent<MeshFilter>();
            mCollider = GetComponent<MeshCollider>();
            originalTriangles = mFilter.sharedMesh.triangles;
            originalVertices = mFilter.sharedMesh.vertices;

            if (!simplifiedMesh) {
                GenerateLowPolyMesh();
            }
        }
    }

    protected void GenerateLowPolyMesh()
    {
        simplifiedMesh = new CubeMeshFactory(simplifiedGParams, "simplified_").result;

        mCollider.sharedMesh = null;
        mCollider.sharedMesh = simplifiedMesh;
    }

    public void UpdateMeshCollider(bool recalculate = false)
    {
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = null;
        mCollider.sharedMesh = simplifiedMesh;
    }

    public void SplitCollider(Vector3 worldPt, int halfIndex, Vector3[] newVertices)
    {
        if (halfIndex < 0 || halfIndex > 1) {
            Debug.LogWarning("Invalid half index (" + halfIndex + "). Index should be either 0 or 1, to represent one of two halves.");
            return;
        }

        Vector3 localPt = transform.InverseTransformPoint(worldPt);
        int closestVertex = FindClosestVertex(localPt, newVertices);
        float percent = (closestVertex != 0 ? (closestVertex / newVertices.Length) : 0.001f);

        if (halfIndex == 1) {
            percent = 1.0f - percent;
        }

        // NEED ACCESS TO PREVIOUS DEFORMATION INFO!!
    }

    public static int FindClosestVertex(Vector3 localPt, Vector3[] verts)
    {
        int closestIndex = 0;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < verts.Length; i++) {
            float dist = Vector3.Distance(verts[i], localPt);
            if (dist < minDist) {
                closestIndex = i;
                minDist = dist;
            }
        }

        return closestIndex;
    }

    public static float FindClosestHeatFactor(Heating heatScript, Vector3 worldCheckLocation)
    {
        if (heatScript != null) {
            return heatScript.ClosestHeatAtPointNormalized(worldCheckLocation);
        }

        return 1.0f;
    }
}
