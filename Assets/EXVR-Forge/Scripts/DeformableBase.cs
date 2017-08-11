using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class DeformableBase : MonoBehaviour
{
    protected MeshFilter mFilter;
    protected MeshCollider mCollider;
    protected Vector3[] originalVertices;

    protected virtual void Start()
    {
        mFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();
        originalVertices = mFilter.sharedMesh.vertices;
    }

    protected virtual void OnApplicationQuit()
    {
        ResetMesh();
    }

    protected Vector3 DivideVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    protected void ResetMesh()
    {
        mFilter.sharedMesh.vertices = originalVertices;
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = mFilter.sharedMesh;
    }
}
