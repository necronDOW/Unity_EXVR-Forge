using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshInfo : MonoBehaviour
{
    public struct VertexRange
    {
        public int start { get; private set; }
        public int end { get; private set; }

        public VertexRange(int _start, int _end)
        {
            start = _start;
            end = _end;
        }
    }

    public VertexRange vxr_topCap, vxr_bottomCap;
    public int loopSpacing, loopCount;

    private MeshFilter mFilter;

    private void Start()
    {
        mFilter = GetComponent<MeshFilter>();

        for (int i = 0; i < 20; i++)
            Debug.DrawLine(VertexAtLoop(i), VertexAtLoop(i + 1), Color.red, Mathf.Infinity);
    }

    public int ClosestLoopToPoint(Vector3 point)
    {
        int minLoop = 0;
        Vector3 localizedPoint = transform.TransformPoint(point);
        float minDist = Vector3.Distance(localizedPoint, VertexAtLoop(minLoop));

        for (int i = 1; i < loopCount; i++) {
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
