using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendMesh : DeformableBase
{
    public bool visualizeDebug = false;
    public int pointResolution = 10;
    [Range(0, 1)] public float bendSharpness = 0.0f;

    private Transform[] splines;
    private Vector3[] splinesV3;
    private Vector3 originSpline0;

    protected override void Start()
    {
        base.Start();

        splines = new Transform[transform.childCount];
        splinesV3 = new Vector3[splines.Length];

        for (int i = 0; i < splines.Length; i++)
        {
            splines[i] = transform.GetChild(i).GetComponent<Transform>();
            splinesV3[i] = splines[i].position;
        }

        originSpline0 = splinesV3[0];
    }

    private void Update()
    {
        for (int i = 0; i < splines.Length; i++)
            splinesV3[i] = splines[i].position;

        BendVertices();
    }

    private void BendVertices()
    {
        splinesV3[0] = Vector3.Lerp(splinesV3[0], splinesV3[1], bendSharpness);
        splinesV3[3] = Vector3.Lerp(splinesV3[3], splinesV3[2], bendSharpness);

        Vector3[] points = new Vector3[pointResolution];
        points[0] = splines[0].position;
        points[points.Length - 1] = splines[3].position;

        for (int i = 1; i < pointResolution - 1; i++)
            points[i] = GetPoint(splinesV3, (float)i / (pointResolution));

        if (visualizeDebug)
        {
            for (int i = 0; i < points.Length - 1; i++)
                Debug.DrawLine(points[i], points[i + 1], Color.blue);
        }
    }

    private Vector3 GetPoint(Vector3[] pts, float t)
    {
        return (pts[0] * Mathf.Pow(1 - t, 3)) 
            + (pts[1] * 3 * Mathf.Pow(1 - t, 2) * t) 
            + (pts[2] * 3 * (1 - t) * Mathf.Pow(t, 2)) 
            + (pts[3] * Mathf.Pow(t, 3));
    }
}
