using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class DeformableMesh : MonoBehaviour
{
    public float maxInfluence = 1.0f;
    public float forceFactor = 1.0f;
    private MeshFilter mFilter;
    private MeshCollider mCollider;

    private void Start()
    {
        mFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Deform(other.ClosestPointOnBounds(transform.position));
    }

    private void Deform(Vector3 hitPoint)
    {
        Vector3[] mVertices = mFilter.sharedMesh.vertices;

        for (int i = 0; i < mVertices.Length; i++)
        {
            Vector3 diff = transform.position + mVertices[i] - hitPoint;
            float dist = diff.magnitude;
            float influence = Mathf.Clamp01(maxInfluence - dist);

            mVertices[i] += (diff * (1.0f / dist) * forceFactor) * influence;
        }

        mFilter.sharedMesh.vertices = mVertices;
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = mFilter.sharedMesh;
    }
}
