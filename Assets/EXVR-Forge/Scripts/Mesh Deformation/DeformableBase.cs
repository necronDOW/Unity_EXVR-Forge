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

    protected void Awake()
    {

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
        if (!mFilter || !mCollider)
        {
            mFilter = GetComponent<MeshFilter>();
            mCollider = GetComponent<MeshCollider>();
            originalTriangles = mFilter.sharedMesh.triangles;
            originalVertices = mFilter.sharedMesh.vertices;

            GenerateLowPolyMesh();
        }
    }

    protected void GenerateLowPolyMesh()
    {
        simplifiedMesh = new CubeMeshFactory(2, 120, 2, 0.0125f, 0.0f, "simplified_").result;

        mCollider.sharedMesh = null;
        mCollider.sharedMesh = simplifiedMesh;
    }

    public void UpdateMeshCollider(bool recalculate = false)
    {
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = null;
        mCollider.sharedMesh = simplifiedMesh;
    }
}
