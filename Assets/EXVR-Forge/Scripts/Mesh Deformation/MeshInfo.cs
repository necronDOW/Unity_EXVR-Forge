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
        return 0;
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
