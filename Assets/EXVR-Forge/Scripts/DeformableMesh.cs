using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableMesh : DeformableBase
{
    public float maxInfluence = 1.0f;
    public float forceFactor = 1.0f;
    public float distanceLimiter = 0.0f;
    public bool advancedDeform = false;

    private Collider currentImpactCollider;
    private Vector3 currentHitPoint;

    protected override void Start()
    {
        base.Start();

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
        if (!currentImpactCollider) {
            currentImpactCollider = other;

            RaycastHit hitInfo;
            float rayLength = other.bounds.extents.magnitude;

            if (Physics.Raycast(other.transform.position, other.transform.forward, out hitInfo, rayLength)
                || Physics.Raycast(other.transform.position, -other.transform.forward, out hitInfo, rayLength)) {
                if (hitInfo.transform.gameObject == gameObject) {
                    if (advancedDeform)
                    {
                        Vector3[] pts = new Vector3[4];
                        Vector3 otherRight = other.transform.right * other.bounds.extents.x;
                        Vector3 otherUp = other.transform.up * other.bounds.extents.y;

                        int p = 0;
                        for (int i = -1; i <= 1; i += 2)
                        {
                            for (int j = -1; j <= 1; j += 2)
                                pts[p++] = hitInfo.point + (otherRight * i) + (otherUp * j);
                        }

                        Deform(other.transform, pts);
                    }
                    else Deform(other.transform, hitInfo.point);

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

    private void DisplaceVertices(ref Vector3[] vertices, Transform otherObject, Vector3 hitPoint, float force)
    {
        Vector3 impactVectorNormalized = (otherObject.position - hitPoint).normalized;
        Vector3 simplifiedHitPoint = DivideVector3(hitPoint - transform.position, transform.lossyScale);

        RotatePointAroundPivot(ref impactVectorNormalized, transform.position, -transform.eulerAngles);
        RotatePointAroundPivot(ref simplifiedHitPoint, transform.position, -transform.eulerAngles);

        for (int i = 0; i < vertices.Length; i++) {
            Vector3 diff = simplifiedHitPoint - vertices[i];
            float dist = diff.magnitude;
            float influence = Mathf.Clamp01(maxInfluence - dist);

            Vector3 displacement = -impactVectorNormalized * force * influence;
            vertices[i] += displacement;
        }
    }

    private void Deform(Transform otherObject, Vector3[] hitPoints)
    {
        Vector3[] mVertices = mFilter.mesh.vertices;
        for (int i = 0; i < hitPoints.Length; i++)
            DisplaceVertices(ref mVertices, otherObject, hitPoints[i], forceFactor / hitPoints.Length);
        UpdateMesh(mVertices);
    }
    
    private void Deform(Transform otherObject, Vector3 hitPoint)
    {
        Vector3[] mVertices = mFilter.sharedMesh.vertices;
        DisplaceVertices(ref mVertices, otherObject, hitPoint, forceFactor);
        UpdateMesh(mVertices);
    }

    private void UpdateMesh(Vector3[] vertices)
    {
        mFilter.sharedMesh.vertices = vertices;
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = mFilter.sharedMesh;
    }

    private void RotatePointAroundPivot(ref Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
    }
}
