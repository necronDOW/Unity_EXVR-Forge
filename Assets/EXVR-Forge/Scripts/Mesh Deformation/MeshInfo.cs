using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityThreading;

[RequireComponent(typeof(MeshFilter))]
public class MeshInfo : MonoBehaviour
{
    public int loopSpacing, loopCount;
    public MeshFilter mFilter { get; private set; }
    private Vector3[] meshVertices;

    private void Start()
    {
        mFilter = GetComponent<MeshFilter>();
        meshVertices = mFilter.sharedMesh.vertices;
    }

    public int GetClosestVertexIndex(Vector3 pt)
    {
        pt = transform.InverseTransformPoint(pt);
        mFilter = GetComponent<MeshFilter>();
        Vector3[] vertices = mFilter.sharedMesh.vertices;

        return GetClosestVertexIndex_recursive(pt, vertices, 0, vertices.Length, vertices.Length / 10);
    }

    private int GetClosestVertexIndex_recursive(Vector3 localizedPt, Vector3[] vertices, int start, int end, int step)
    {
        if (step >= end - start || step == 0)
            step = 1;

        int currentMinIndex = start;
        float currentMinDistance = Vector3.Distance(vertices[currentMinIndex], localizedPt);

        for (int i = start + step; i <= end - step; i += step) {
            float thisDistance = Vector3.Distance(vertices[i], localizedPt);
            if (thisDistance < currentMinDistance) {
                currentMinIndex = i;
                currentMinDistance = thisDistance;
            }
        }

        start = Mathf.Clamp(currentMinIndex - step, 0, vertices.Length);
        end = Mathf.Clamp(currentMinIndex + step, 0, vertices.Length);

        if (step == 1)
            return currentMinIndex;
        else return GetClosestVertexIndex_recursive(localizedPt, vertices, start, end, (step * 2) / 10);
    }

    private Thread SearchThread(int iStart, int iEnd, Vector3 pt, ref int resultIndex, out float resultDistance)
    {
        int rI = -1; float rD = -1f;
        Thread t = new Thread(() => { rI = SearchThread(iStart, iEnd, pt, out rD); });
        t.Start();

        resultIndex = rI;
        resultDistance = rD;

        return t;
    }

    private int SearchThread(int iStart, int iEnd, Vector3 pt, out float resultDistance)
    {
        resultDistance = 0.0f;
        return 0;
    }

    public int ClosestLoopToPoint(Vector3 point)
    {
        int minLoop = 0;
        Vector3 localizedPoint = transform.InverseTransformPoint(point);
        float minDist = Vector3.Distance(localizedPoint, VertexAtLoop(minLoop));

        for (int i = 1; i < loopCount; i++)
        {
            float thisDist = Vector3.Distance(localizedPoint, VertexAtLoop(i));
            if (thisDist < minDist)
            {
                minDist = thisDist;
                minLoop = i;
            }
        }

        return minLoop;
    }

    public Vector3 VertexAtLoop(int loopIndex)
    {
        return mFilter.sharedMesh.vertices[loopSpacing * loopIndex];
    }
}
