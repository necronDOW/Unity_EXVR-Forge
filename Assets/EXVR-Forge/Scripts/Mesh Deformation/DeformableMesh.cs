using System.Collections;
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

        public DeformVectors(Transform transform, Vector3 displaceDirection, Vector3 hitPoint)
        {
            impact = transform.InverseTransformDirection(displaceDirection); //transform.InverseTransformVector((otherPosition - hitPoint)).normalized;
            simplified = transform.InverseTransformPoint(hitPoint);

            //RotatePointAroundPivot(ref impact, transform.position, -transform.eulerAngles);
            //RotatePointAroundPivot(ref simplified, transform.position, -transform.eulerAngles);
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
    private Heating heatScript;

    private List<Thread> threads = new List<Thread>();

    private Thread StartThread(int iStart, int iEnd, Vector3 impactVector, Vector3 simplifiedVector, float force)
    {
        Thread t = new Thread(() => DisplaceVertices(iStart, iEnd, impactVector, simplifiedVector, force));
        t.Start();
        return t;
    }

    protected override void Start()
    {
        base.Start();

        currentImpactCollider = null;
        currentHitPoint = Vector3.zero;
        heatScript = GetComponent<Heating>();
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    DeformableAgent da = collision.gameObject.GetComponent<DeformableAgent>();

    //    if (da) {
    //        Vector3 centralContact = Vector3.zero;
    //        for (int i = 0; i < collision.contacts.Length; i++)
    //            centralContact += collision.contacts[i].point;
    //        centralContact /= collision.contacts.Length;

    //        Deform(collision.transform, centralContact);
    //    }
    //}
    
    private void OnTriggerEnter(Collider other)
    {
        DeformableAgent da = other.GetComponent<DeformableAgent>();
        //float dispersion = (da) ? da.dispersion : 1.0f;

        if (!currentImpactCollider && da) {
            currentImpactCollider = other;

            DeformVectors vectors = new DeformVectors(transform, other.transform.up, other.transform.position);
            Network_DeformableMesh ndm = GetComponent<Network_DeformableMesh>();

            if (ndm)
                ndm.CmdOnDeform(vectors.impact, vectors.simplified);
            else Deform(vectors.impact, vectors.simplified);

            currentHitPoint = other.transform.position;
        }

        //if (!currentImpactCollider && da) {
        //    currentImpactCollider = other;
        //    RaycastHit hitInfo;
        //    float rayLength = other.bounds.extents.magnitude;

        //    if (Physics.Raycast(other.transform.position, other.transform.forward, out hitInfo, rayLength)
        //        || Physics.Raycast(other.transform.position, -other.transform.forward, out hitInfo, rayLength))
        //    {
        //        if (hitInfo.transform.gameObject == gameObject)
        //        {
        //            Vector3[] pts = new Vector3[9];
        //            Vector3 otherRight = other.transform.right * other.bounds.extents.x * dispersion;
        //            Vector3 otherUp = other.transform.up * other.bounds.extents.y * dispersion;

        //            int p = 0;
        //            for (int i = -1; i <= 1; i += 1)
        //            {
        //                for (int j = -1; j <= 1; j += 1)
        //                    pts[p++] = hitInfo.point + (otherRight * i) + (otherUp * j);
        //            }
        //            Deform(other.transform, pts);

        //            currentHitPoint = hitInfo.point;
        //            return;
        //        }
        //    }

        //    Deform(other.transform, other.transform.position);
        //    currentHitPoint = other.transform.position;
        //}
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

    private void Deform_local(Transform otherObject, Vector3[] hitPoints, Mesh targetMesh)
    {
        UpdateComponents();
        
        thread_vertices = targetMesh.vertices;
        
        float finalForce = (forceFactor / hitPoints.Length) * FindClosestHeatFactor(heatScript, otherObject.position);

        for (int i = 0; i < hitPoints.Length; i++) {
            for (int j = 0; j < thread_vertices.Length; j += maxWorkGroupSize) {
                DeformVectors vectors = new DeformVectors(transform, otherObject.transform.up, hitPoints[i]);
                int workgroupSize = (j + maxWorkGroupSize < thread_vertices.Length) ? maxWorkGroupSize : thread_vertices.Length - j;
                threads.Add(StartThread(j, j + workgroupSize, vectors.impact, vectors.simplified, finalForce));
            }
        }
    }
    
    private void Deform_local(Vector3 impactVector, Vector3 simplifiedVector, Mesh targetMesh)
    {
        UpdateComponents();

        thread_vertices = targetMesh.vertices;
        
        float finalForce = forceFactor * FindClosestHeatFactor(heatScript, transform.TransformPoint(simplifiedVector));

        if (finalForce != 0.0f) {
            for (int i = 0; i < thread_vertices.Length; i += maxWorkGroupSize) {
                int workgroupSize = (i + maxWorkGroupSize < thread_vertices.Length) ? maxWorkGroupSize : thread_vertices.Length - i;
                threads.Add(StartThread(i, i + workgroupSize, impactVector, simplifiedVector, finalForce));
            }
        }
    }

    public void Deform(Vector3 impactVector, Vector3 simplifiedVector)
    {
        Deform_local(impactVector, simplifiedVector, mFilter.sharedMesh);
        ThreadTools.WaitForThreads(ref threads);
        threads.Clear();

        mFilter.sharedMesh.vertices = thread_vertices;
        mFilter.sharedMesh.RecalculateBounds();

        Deform_local(impactVector, simplifiedVector, simplifiedMesh);
        ThreadTools.WaitForThreads(ref threads);
        threads.Clear();

        simplifiedMesh.vertices = thread_vertices;
        UpdateMeshCollider();
    }
}
