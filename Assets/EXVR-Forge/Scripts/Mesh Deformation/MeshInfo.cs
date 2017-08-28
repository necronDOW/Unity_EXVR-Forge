using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshInfo : MonoBehaviour
{
    public int loopSpacing, loopCount;
    public MeshFilter mFilter { get; private set; }

    public int[] sortedVertexIndices;

    private void Start()
    {
        mFilter = GetComponent<MeshFilter>();
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
