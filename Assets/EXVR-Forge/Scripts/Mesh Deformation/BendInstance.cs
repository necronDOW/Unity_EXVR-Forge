using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityThreading;

public class BendInstance : MonoBehaviour
{
    struct BendParameters
    {
        public Vector3 P, N, BiN, T;
        public Vector3 bendEnd;
    }

    private static int maxWorkGroupSize = 1024;
    private static int numBendSegments = 3;

    public float curvature = 0;
    public float length = 1;
    public float amount = 1;
    public bool showGizmo = true;
    public GameObject target;

    private Mesh mesh;
    private Vector3[] origVerts;
    private Vector3[] origPts = new Vector3[numBendSegments];
    private Vector3[] pts = new Vector3[numBendSegments];
    private Vector3[] normals = new Vector3[numBendSegments];
    private Vector3[] binormals = new Vector3[numBendSegments];
    private Vector3[] tangents = new Vector3[numBendSegments];

    private List<Thread> threads = new List<Thread>();
    private ts_Transform ts_transform, ts_targetTransform;

    private Thread StartThread(int iStart, int iEnd, ts_Transform thisT, ts_Transform otherT, BendParameters bp)
    {
        Thread t = new Thread(() => DeformThread(iStart, iEnd, thisT, otherT, bp));
        t.Start();
        return t;
    }

    // Use this for initialization
    private void Start()
    {
        ProjectTarget();
        
        ts_transform = new ts_Transform(transform);
        ts_targetTransform = new ts_Transform(target.transform);
    }
    
    // LateUpdate happens after all Update functions
    private void LateUpdate()
    {
        ts_transform.CopyValues(transform);
        ts_targetTransform.CopyValues(target.transform);

        Deform();
    }

    private void ProjectTarget()
    {
        mesh = target.GetComponent<MeshFilter>().sharedMesh;
        origVerts = mesh.vertices;
    }

    private void UpdatePts()
    {
        origVerts = mesh.vertices;
    }

    Vector3[] thread_pts;
    private void Deform()
    {
        thread_pts = new Vector3[origVerts.Length];

        BendParameters bp = new BendParameters();
        bp.N = ts_transform.TransformDirection(1, 0, 0);
        bp.BiN = ts_transform.TransformDirection(0, 0, -1);
        bp.T = ts_transform.TransformDirection(0, 1, 0);
        bp.bendEnd = ts_targetTransform.TransformPoint(0, length, 0);

        for (int i = 0; i < origVerts.Length; i += maxWorkGroupSize) {
            int workgroupSize = (i + maxWorkGroupSize < origVerts.Length) ? maxWorkGroupSize : origVerts.Length - i;
            threads.Add(StartThread(i, i + workgroupSize, ts_transform, ts_targetTransform, bp));
        }

        ThreadTools.WaitForThreads(ref threads);
        mesh.vertices = thread_pts;
    }

    private void DeformThread(int iStart, int iEnd, ts_Transform thisT, ts_Transform otherT, BendParameters bp)
    {
        Vector3 v;
        Vector3 P2, N2, T2;

        for (int i = iStart; i < iEnd; i++)
        {
            Vector3 wsPt = otherT.TransformPoint(origVerts[i]);

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

            thread_pts[i] = Vector3.Lerp(origVerts[i], otherT.InverseTransformPoint(P2 + dN * N2 + dBiN * bp.BiN + dT * T2), amount);
        }
    }

    private void PlotPoints()
    {
        for (int i = 0; i < origPts.Length; i++)
        {
            float u = (float)i / (float)(origPts.Length - 1);
            //Puts the bend in the same space as the mesh
            origPts[i] = transform.TransformPoint(new Vector3(0, u * length, 0));
            normals[i] = transform.TransformDirection(new Vector3(0, 1, 0));

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
                pt = origPts[i];
                normal = transform.TransformDirection(new Vector3(1, 0, 0));
            }

            binormal = transform.TransformDirection(new Vector3(0, 0, -1));
            tangent = Vector3.Cross(normal, binormal);

            pts[i] = pt;
            normals[i] = normal;
            binormals[i] = binormal;
            tangents[i] = tangent;
        }
    }

    private void DrawBend()
    {
        PlotPoints();

        for (int i = 0; i < pts.Length; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pts[i], pts[i] + .1F * normals[i] * length);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(pts[i], pts[i] + .1F * tangents[i] * length);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pts[i], pts[i] + .1F * binormals[i] * length);

            Gizmos.color = new Color(1, 1, 1, .5F);
            if (i < pts.Length - 1)
                Gizmos.DrawLine(pts[i], pts[i + 1]);
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

    public void Finish()
    {
        mesh.RecalculateNormals();

        if (target.transform.GetComponent<MeshCollider>() != null)
            target.transform.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
            DrawBend();
    }
}
