using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewBend : MonoBehaviour
{
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

    // Use this for initialization
    private void Start()
    {
        ProjectTarget();

        transform.position -= transform.TransformPoint(0, length, 0) * 0.5f;
    }

    // LateUpdate happens after all Update functions
    private void LateUpdate()
    {
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

    private void Deform()
    {
        Vector3[] pts = new Vector3[origVerts.Length];
        Vector3 P, N, BiN, T;
        Vector3 P2, N2, BiN2, T2;
        Vector3 bendEnd = target.transform.TransformPoint(0, length, 0);

        N = transform.TransformDirection(new Vector3(1, 0, 0));
        BiN = transform.TransformDirection(new Vector3(0, 0, -1));
        T = transform.TransformDirection(new Vector3(0, 1, 0));

        for (int i = 0; i < origVerts.Length; i++) {
            Vector3 wsPt = target.transform.TransformPoint(origVerts[i]);

            float u = PtLineProject(wsPt, transform.TransformPoint(new Vector3(0, 0, 0)), transform.TransformPoint(new Vector3(0, 1 * length, 0)));
            u = Mathf.Min(1.0F, Mathf.Max(u, 0.0F));
            
            float tmp = Vector3.Dot(wsPt, bendEnd);
            tmp = Mathf.Min(Mathf.Max(tmp, 0), bendEnd.magnitude);

            P = transform.TransformPoint(new Vector3(0, 0, 0) + u * (new Vector3(0, 1 * length, 0) - new Vector3(0, 0, 0)));

            float dN, dBiN, dT;
            dN = PtLineProject(wsPt, P, P + N);
            dBiN = PtLineProject(wsPt, P, P + BiN);
            dT = PtLineProject(wsPt, P, P + T);
            
            float x, y, z;

            if (curvature != 0) {
                x = (Mathf.Cos(u * curvature / Mathf.PI) * Mathf.PI / curvature - Mathf.PI / curvature) * length;
                y = (Mathf.Sin(u * curvature / Mathf.PI) * Mathf.PI / curvature) * length;
                z = 0;
                P2 = transform.TransformPoint(new Vector3(x, y, z));
                N2 = transform.TransformDirection(Vector3.Normalize(new Vector3(x, y, z) - new Vector3(-Mathf.PI / curvature * length, 0, 0)));

                if (curvature < 0)
                    N2 *= -1;

                BiN2 = BiN;
                T2 = Vector3.Cross(N2, BiN2);
            }
            else {
                P2 = P;
                N2 = N;
                BiN2 = BiN;
                T2 = T;
            }
            
            pts[i] = Vector3.Lerp(origVerts[i], target.transform.InverseTransformPoint(P2 + dN * N2 + dBiN * BiN2 + dT * T2), amount);
        }

        mesh.vertices = pts;

        if (target.transform.GetComponent<MeshCollider>() != null)
            target.transform.GetComponent<MeshCollider>().sharedMesh = mesh;
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

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (showGizmo)
            DrawBend();
    }
}
