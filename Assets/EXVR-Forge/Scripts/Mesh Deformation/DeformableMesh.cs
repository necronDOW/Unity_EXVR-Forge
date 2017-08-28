using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityThreading;

public class DeformableMesh : DeformableBase
{
    struct DeformVectors
    {
        public Vector3 impact;
        public Vector3 simplified;

        public DeformVectors(Transform transform, Vector3 otherPosition, Vector3 hitPoint)
        {
            impact = (otherPosition - hitPoint).normalized;
            simplified = DivideVector3(hitPoint - transform.position, transform.lossyScale);

            RotatePointAroundPivot(ref impact, transform.position, -transform.eulerAngles);
            RotatePointAroundPivot(ref simplified, transform.position, -transform.eulerAngles);
        }

        private void RotatePointAroundPivot(ref Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(angles) * dir;
            point = dir + pivot;
        }
    }

    private static int maxWorkGroupSize = 1024;

    public float maxInfluence = 1.0f;
    public float forceFactor = 1.0f;
    public float distanceLimiter = 0.0f;

    private Collider currentImpactCollider;
    private Vector3 currentHitPoint;

    private List<Thread> threads = new List<Thread>();

    private Thread StartThread(int iStart, int iEnd, DeformVectors vectors, float force)
    {
        Thread t = new Thread(() => DisplaceVertices(iStart, iEnd, vectors.impact, vectors.simplified, force));
        t.Start();
        return t;
    }

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
        DeformableAgent da = other.GetComponent<DeformableAgent>();
        float dispersion = (da) ? da.dispersion : 1.0f;

        if (!currentImpactCollider) {
            currentImpactCollider = other;
            Debug.Log("test");
            RaycastHit hitInfo;
            float rayLength = other.bounds.extents.magnitude;

            if (Physics.Raycast(other.transform.position, other.transform.forward, out hitInfo, rayLength)
                || Physics.Raycast(other.transform.position, -other.transform.forward, out hitInfo, rayLength)) {
                if (hitInfo.transform.gameObject == gameObject) {
                    Vector3[] pts = new Vector3[9];
                    Vector3 otherRight = other.transform.right * other.bounds.extents.x * dispersion;
                    Vector3 otherUp = other.transform.up * other.bounds.extents.y * dispersion;

                    int p = 0;
                    for (int i = -1; i <= 1; i += 1) {
                        for (int j = -1; j <= 1; j += 1)
                            pts[p++] = hitInfo.point + (otherRight * i) + (otherUp * j);
                    }

                    Deform(other.transform, pts);

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

    Vector3[] thread_vertices;
    private void DisplaceVertices(int iStart, int iEnd, Vector3 impactVectorNormalized, Vector3 simplifiedHitPoint, float force)
    {
        for (int i = iStart; i < iEnd; i++) {
            Vector3 diff = simplifiedHitPoint - thread_vertices[i];
            float dist = diff.magnitude;
            float influence = Mathf.Clamp01(maxInfluence - dist);
            Vector3 displacement = -impactVectorNormalized * force * influence;

            thread_vertices[i] += displacement;
        }
    }

    private void Deform(Transform otherObject, Vector3[] hitPoints)
    {
        thread_vertices = mFilter.mesh.vertices;
        for (int i = 0; i < hitPoints.Length; i++)
        {
            for (int j = 0; j < thread_vertices.Length; j += maxWorkGroupSize)
            {
                DeformVectors vectors = new DeformVectors(transform, otherObject.transform.position, hitPoints[i]);
                int workgroupSize = (j + maxWorkGroupSize < thread_vertices.Length) ? maxWorkGroupSize : thread_vertices.Length - j;
                threads.Add(StartThread(j, j + workgroupSize, vectors, forceFactor / hitPoints.Length));
            }
        }

        ThreadTools.WaitForThreads(ref threads);
        UpdateMesh(thread_vertices);
    }
    
    private void Deform(Transform otherObject, Vector3 hitPoint)
    {
        thread_vertices = mFilter.sharedMesh.vertices;

        DeformVectors vectors = new DeformVectors(transform, otherObject.transform.position, hitPoint);

        for (int i = 0; i < thread_vertices.Length; i += maxWorkGroupSize)
        {
            int workgroupSize = (i + maxWorkGroupSize < thread_vertices.Length) ? maxWorkGroupSize : thread_vertices.Length - i;
            threads.Add(StartThread(i, i + workgroupSize, vectors, forceFactor));
        }

        ThreadTools.WaitForThreads(ref threads);
        UpdateMesh(thread_vertices);
    }

    private void UpdateMesh(Vector3[] vertices)
    {
        mFilter.sharedMesh.vertices = vertices;
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = mFilter.sharedMesh;
    }
}
