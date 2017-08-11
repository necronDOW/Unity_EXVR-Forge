using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class DeformableMesh : MonoBehaviour
{
    public float maxInfluence = 1.0f;
    public float forceFactor = 1.0f;
    public float distanceLimiter = 0.0f;

    private MeshFilter mFilter;
    private MeshCollider mCollider;
    private Vector3[] originalVertices;
    private Collider currentImpactCollider;
    private Vector3 currentHitPoint;

    private void Start()
    {
        mFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();
        originalVertices = mFilter.sharedMesh.vertices;
        currentImpactCollider = null;
        currentHitPoint = Vector3.zero;
    }

    private void OnCollisionExit(Collision collision)
    {
        Vector3 centralContact = Vector3.zero;
        for (int i = 0; i < collision.contacts.Length; i++)
            centralContact += collision.contacts[i].point;
        centralContact /= collision.contacts.Length;

        Deform(collision.transform, centralContact);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!currentImpactCollider)
        {
            currentImpactCollider = other;

            RaycastHit hitInfo;
            float rayLength = other.bounds.extents.magnitude;

            if (Physics.Raycast(other.transform.position, other.transform.forward, out hitInfo, rayLength)
                || Physics.Raycast(other.transform.position, -other.transform.forward, out hitInfo, rayLength))
            {
                if (hitInfo.transform.gameObject == gameObject)
                {
                    Deform(other.transform, hitInfo.point);
                    currentHitPoint = hitInfo.point;
                    return;
                }
            }

            Deform(other.transform, other.transform.position);
            currentHitPoint = other.transform.position;
        }
    }

    private void Update()
    {
        if (currentImpactCollider && Vector3.Distance(currentImpactCollider.transform.position, currentHitPoint) >= distanceLimiter)
            currentImpactCollider = null;
    }

    private void OnApplicationQuit()
    {
        ResetMesh();
    }

    float startTime = 0.0f;
    private void Deform(Transform otherObject, Vector3 hitPoint)
    {
        startTime = Time.realtimeSinceStartup;

        Vector3 impactVectorNormalized = (otherObject.position - hitPoint).normalized;
        Vector3 simplifiedHitPoint = DivideVector3(hitPoint - transform.position, transform.lossyScale);
        Vector3[] mVertices = mFilter.sharedMesh.vertices;

        for (int i = 0; i < mVertices.Length; i++)
        {
            Vector3 diff = simplifiedHitPoint - mVertices[i];
            float dist = diff.magnitude;
            float influence = Mathf.Clamp01(maxInfluence - dist);

            // This line helps with sideways displacment but needs some work.
            //Vector3 displacement = diff * (1.0f / dist) * forceFactor * influence;

            Vector3 displacement = -impactVectorNormalized * forceFactor * influence;
            mVertices[i] += displacement;
        }

        mFilter.sharedMesh.vertices = mVertices;
        mFilter.sharedMesh.RecalculateBounds();

        Debug.Log(Time.realtimeSinceStartup - startTime);

        mCollider.sharedMesh = mFilter.sharedMesh;
    }

    private Vector3 DivideVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    private void ResetMesh()
    {
        mFilter.sharedMesh.vertices = originalVertices;
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = mFilter.sharedMesh;
    }
}
