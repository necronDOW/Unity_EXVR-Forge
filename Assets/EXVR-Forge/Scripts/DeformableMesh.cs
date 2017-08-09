using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class DeformableMesh : MonoBehaviour
{
    public float maxInfluence = 1.0f;
    public float forceFactor = 1.0f;
    private MeshFilter mFilter;

    private void Start()
    {
        mFilter = GetComponent<MeshFilter>();
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
            Vector3 diff = mVertices[i] - hitPoint;
            float dist = diff.magnitude;
            float influence = maxInfluence - dist;

            if (influence < 0.0f)
                influence = 0.0f;

            mVertices[i] += diff * (1.0f / dist) * influence * forceFactor;
        }

        mFilter.sharedMesh.vertices = mVertices;
        mFilter.sharedMesh.RecalculateBounds();
    }
}
