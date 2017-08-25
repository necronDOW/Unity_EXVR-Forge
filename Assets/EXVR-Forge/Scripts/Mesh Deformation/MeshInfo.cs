using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshInfo : MonoBehaviour
{
    public struct Range
    {
        public int start { get; private set; }
        public int end { get; private set; }

        public Range(int _start, int _end)
        {
            start = _start;
            end = _end;
        }
    }

    public Range vxr_topCap, vxr_bottomCap;
    public Range tr_topCap, tr_bottomCap;
    public int loopSpacing, loopCount;
    public MeshFilter mFilter { get; private set; }

    public int[] sortedVertexIndices;

    private void Start()
    {
        mFilter = GetComponent<MeshFilter>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="topCapStart">The starting vertex for the top-most cap (inclusive).</param>
    /// <param name="topCapEnd">The ending vertex for the top-most cap (exclusive).</param>
    /// <param name="bottomCapStart">The starting vertex for the bottom-most cap (inclusive).</param>
    /// <param name="bottomCapEnd">The ending vertex for the bottom-most cap (exclusive).</param>
    public void UpdateSortedVertexIndices(int length, int s_topCap, int e_topCap, int s_botCap, int e_botCap)
    {
        sortedVertexIndices = new int[length];
        for (int i = 0; i < length; i++)
            sortedVertexIndices[i] = -1;

        int v = 0;
        for (int i = s_topCap; i < e_topCap; i++)
            sortedVertexIndices[v++] = i;

        v = length - (e_botCap - s_botCap);
        for (int i = s_botCap; i < e_botCap; i++)
            sortedVertexIndices[v++] = i;

        v = 0;
        for (int i = 0; i < length; i++)
        {
            if (sortedVertexIndices[i] == -1)
                sortedVertexIndices[i] = v++;
        }
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
