using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityThreading;
using Valve.VR.InteractionSystem;

public class BendInstance : MonoBehaviour
{
    struct BendParameters {
        public Vector3 P, N, BiN, T;
        public Vector3 bendEnd;
    }

    class BendableMesh {
        public Vector3[] origPts = new Vector3[numBendSegments];
        public Vector3[] pts = new Vector3[numBendSegments];
        public Vector3[] normals = new Vector3[numBendSegments];
        public Vector3[] binormals = new Vector3[numBendSegments];
        public Vector3[] tangents = new Vector3[numBendSegments];

        public Mesh mesh {
            get;
            private set;
        }

        public Vector3[] origVerts {
            get;
            private set;
        }

        public int seperatingIndex {
            get;
            private set;
        }

        public BendableMesh(Mesh _mesh, Transform _meshTransform, Transform _bendTransform) {
            mesh = _mesh;
            origVerts = _mesh.vertices;
            seperatingIndex = DeformableBase.FindClosestVertex(_meshTransform.InverseTransformPoint(_bendTransform.position), origVerts);
        }

        public void UpdatePts()
        {
            origVerts = mesh.vertices;
        }
    }

    private static int maxWorkGroupSize = 1024;
    private static int numBendSegments = 3;

    public float curvature = 0;
    private float heatFactor = 0.0f;
    public float length = 1;
    public float amount = 1;
    public bool showGizmo = true;
    public GameObject target;
    public bool direction = false;

    private BendableMesh[] bMeshes;

    private List<Thread> threads = new List<Thread>();
    private ts_Transform ts_transform, ts_targetTransform;

    public RodGripScript rodGripScriptReference;
    private Network_BendInstance networkBendInstance;

    private Thread StartThread(int iStart, int iEnd, ts_Transform thisT, ts_Transform otherT, BendParameters bp, int meshIndex)
    {
        Thread t = new Thread(() => DeformThread(iStart, iEnd, thisT, otherT, bp, meshIndex));
        t.Start();
        return t;
    }

    private void Start()
    {
        networkBendInstance = GetComponentInParent<Network_BendInstance>();
    }

    // Use this for initialization
    public void Initialize()
    {
        if (!target) {
            target = transform.parent.parent.gameObject;
        }

        ProjectTarget();

        heatFactor = DeformableBase.FindClosestHeatFactor(target.GetComponent<Heating>(), target.transform.InverseTransformPoint(transform.position));

        ts_transform = new ts_Transform(transform);
        ts_targetTransform = new ts_Transform(target.transform);
    }

    // LateUpdate happens after all Update functions
    private float lastCurvature = 0;
    private void LateUpdate()
    {
        ts_transform.CopyValues(transform);
        ts_targetTransform.CopyValues(target.transform);

        if (rodGripScriptReference) {
            if (rodGripScriptReference.isGripped) {
                UpdateCurvature();
                DeformAll();
                networkBendInstance.UpdateNetworkDeform(this);
            }
            else {
                if (lastCurvature != curvature) {
                    networkBendInstance.NetworkUpdateMeshCollider();
                    lastCurvature = curvature;
                }
            }
        }
    }

    private void ProjectTarget()
    {
        List<Mesh> foundMeshes = new List<Mesh>();
        MeshFilter targetFilter = target.GetComponent<MeshFilter>();
        if (targetFilter) {
            foundMeshes.Add(targetFilter.sharedMesh);
        }

        MeshCollider targetMeshCollider = target.GetComponent<MeshCollider>();
        if (targetMeshCollider) {
            foundMeshes.Add(targetMeshCollider.sharedMesh);
        }

        if (foundMeshes.Count > 0) {
            bMeshes = new BendableMesh[foundMeshes.Count];
            for (int i = 0; i < foundMeshes.Count; i++)
                bMeshes[i] = new BendableMesh(foundMeshes[i], target.transform, transform);
            
            direction = DeformableBase.FindClosestVertex(target.transform.InverseTransformPoint(transform.position + (transform.up * 0.1f)), bMeshes[bMeshes.Length - 1].origVerts) > bMeshes[bMeshes.Length - 1].seperatingIndex;
        }
    }

    private void UpdatePts(int meshIndex)
    {
        bMeshes[meshIndex].UpdatePts();
    }

    public void DeformAll()
    {
        //curvature *= heatFactor;

        for (int i = 0; i < bMeshes.Length; i++) {
            Deform(i);
        }
    }

    Vector3[] thread_pts;
    public void Deform(int meshIndex)
    {
        if (meshIndex > bMeshes.Length-1)
            return;

        thread_pts = new Vector3[bMeshes[meshIndex].origVerts.Length];

        BendParameters bp = new BendParameters();
        bp.N = ts_transform.TransformDirection(1, 0, 0);
        bp.BiN = ts_transform.TransformDirection(0, 0, -1);
        bp.T = ts_transform.TransformDirection(0, 1, 0);
        bp.bendEnd = ts_targetTransform.TransformPoint(0, length, 0);

        int start = (direction) ? 0 : bMeshes[meshIndex].seperatingIndex;
        int end = (direction) ? bMeshes[meshIndex].seperatingIndex : bMeshes[meshIndex].origVerts.Length;

        for (int i = start; i < end; i += maxWorkGroupSize) {
            int workgroupSize = (i + maxWorkGroupSize < bMeshes[meshIndex].origVerts.Length) ? maxWorkGroupSize : bMeshes[meshIndex].origVerts.Length - i;
            threads.Add(StartThread(i, i + workgroupSize, ts_transform, ts_targetTransform, bp, meshIndex));
        }

        start = (direction) ? bMeshes[meshIndex].seperatingIndex : 0;
        end = (direction) ? bMeshes[meshIndex].origVerts.Length : bMeshes[meshIndex].seperatingIndex;

        for (int i = start; i < end; i++)
            thread_pts[i] = bMeshes[meshIndex].origVerts[i];

        ThreadTools.WaitForThreads(ref threads);
        bMeshes[meshIndex].mesh.vertices = thread_pts;
    }

    private void DeformThread(int iStart, int iEnd, ts_Transform thisT, ts_Transform otherT, BendParameters bp, int meshIndex)
    {
        Vector3 v;
        Vector3 P2, N2, T2;

        for (int i = iStart; i < iEnd; i++)
        {
            Vector3 wsPt = otherT.TransformPoint(bMeshes[meshIndex].origVerts[i]);

            float u = PtLineProject(wsPt, thisT.TransformPoint(Vector3.zero), thisT.TransformPoint(0, 1 * length, 0));
            u = Mathf.Min(1.0F, Mathf.Max(u, 0.0F));

            float tmp = Vector3.Dot(wsPt, bp.bendEnd);
            tmp = Mathf.Min(Mathf.Max(tmp, 0), bp.bendEnd.magnitude);

            bp.P = thisT.TransformPoint(u * new Vector3(0, 1 * length, 0));

            float dN, dBiN, dT;
            dN = PtLineProject(wsPt, bp.P, bp.P + bp.N);
            dBiN = PtLineProject(wsPt, bp.P, bp.P + bp.BiN);
            dT = PtLineProject(wsPt, bp.P, bp.P + bp.T);
            
            if (curvature != 0)
            {
                v.x = (Mathf.Cos(u * curvature / Mathf.PI) * Mathf.PI / curvature - Mathf.PI / curvature) * length;
                v.y = (Mathf.Sin(u * curvature / Mathf.PI) * Mathf.PI / curvature) * length;
                v.z = 0;

                P2 = thisT.TransformPoint(v);
                N2 = thisT.TransformDirection(Vector3.Normalize(v - new Vector3(-Mathf.PI / curvature * length, 0, 0)));

                if (curvature < 0)
                    N2 *= -1;
                
                T2 = Vector3.Cross(N2, bp.BiN);
            }
            else
            {
                P2 = bp.P;
                N2 = bp.N;
                T2 = bp.T;
            }

            thread_pts[i] = Vector3.Lerp(bMeshes[meshIndex].origVerts[i], otherT.InverseTransformPoint(P2 + dN * N2 + dBiN * bp.BiN + dT * T2), amount);
        }
    }

    private void PlotPoints(int meshIndex)
    {
        for (int i = 0; i < bMeshes[meshIndex].origPts.Length; i++)
        {
            float u = (float)i / (float)(bMeshes[meshIndex].origPts.Length - 1);
            //Puts the bend in the same space as the mesh
            bMeshes[meshIndex].origPts[i] = transform.TransformPoint(new Vector3(0, u * length, 0));
            bMeshes[meshIndex].normals[i] = transform.TransformDirection(new Vector3(0, 1, 0));

            float x, y, z;
            Vector3 pt, normal, binormal, tangent;

            if (curvature != 0)
            {
                x = (Mathf.Cos(u * curvature / Mathf.PI) * Mathf.PI / curvature - Mathf.PI / curvature) * length;
                y = (Mathf.Sin(u * curvature / Mathf.PI) * Mathf.PI / curvature) * length;
                z = 0;
                pt = transform.TransformPoint(new Vector3(x, y, z));
                normal = transform.TransformDirection(Vector3.Normalize(new Vector3(x, y, z) - new Vector3(-Mathf.PI / curvature * length, 0, 0)));

                if (curvature < 0)
                {
                    normal *= -1;
                }
            }
            else
            {
                pt = bMeshes[meshIndex].origPts[i];
                normal = transform.TransformDirection(new Vector3(1, 0, 0));
            }

            binormal = transform.TransformDirection(new Vector3(0, 0, -1));
            tangent = Vector3.Cross(normal, binormal);

            bMeshes[meshIndex].pts[i] = pt;
            bMeshes[meshIndex].normals[i] = normal;
            bMeshes[meshIndex].binormals[i] = binormal;
            bMeshes[meshIndex].tangents[i] = tangent;
        }
    }

    private void DrawBend(int meshIndex)
    {
        PlotPoints(meshIndex);

        for (int i = 0; i < bMeshes[meshIndex].pts.Length; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(bMeshes[meshIndex].pts[i], bMeshes[meshIndex].pts[i] + .1F * bMeshes[meshIndex].normals[i] * length);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(bMeshes[meshIndex].pts[i], bMeshes[meshIndex].pts[i] + .1F * bMeshes[meshIndex].tangents[i] * length);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(bMeshes[meshIndex].pts[i], bMeshes[meshIndex].pts[i] + .1F * bMeshes[meshIndex].binormals[i] * length);

            Gizmos.color = new Color(1, 1, 1, .5F);
            if (i < bMeshes[meshIndex].pts.Length - 1)
                Gizmos.DrawLine(bMeshes[meshIndex].pts[i], bMeshes[meshIndex].pts[i + 1]);
        }
    }

    private float PtLineProject(Vector3 pt, Vector3 start, Vector3 end)
    {
        //start and end point of bend
        float u = 0.0F;
        Vector3 AC = pt - start;
        Vector3 AB = end - start;
        u = Vector3.Dot(AC, AB) / Mathf.Pow(Vector3.Distance(start, end), 2.0F);
        return u;
    }

    public void Finish(int meshIndex)
    {
        bMeshes[meshIndex].mesh.RecalculateNormals();

        if (target.transform.GetComponent<MeshCollider>() != null)
            target.transform.GetComponent<MeshCollider>().sharedMesh = bMeshes[meshIndex].mesh;
    }

    public void UpdateMeshCollider()
    {
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        if (meshFilter)
        {
            meshFilter.mesh.RecalculateBounds();

            MeshCollider meshCollider = target.GetComponent<MeshCollider>();
            if (meshCollider)
                meshCollider.sharedMesh = meshFilter.mesh;
        }
    }

    Vector3 lastLocalizedQ;
    private void UpdateCurvature()
    {
        float angularMultiplier = 0.0548311372f;

        Vector3 localizedP = rodGripScriptReference.grippedPoint - transform.position;
        Vector3 localizedQ = rodGripScriptReference.target.transform.position - transform.position;
        
        if (Vector3.Angle(localizedP, localizedQ) < 175)
            lastLocalizedQ = localizedQ;

        curvature = -Mathf.Abs(Vector3.Angle(localizedP, lastLocalizedQ) * angularMultiplier);
    }
    
    private void OnDrawGizmos()
    {
        if (showGizmo) {
            if (bMeshes.Length > 0)
                DrawBend(0);
        }
    }
}
