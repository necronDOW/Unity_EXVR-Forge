using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VertexLookup : MonoBehaviour
{
    public int xVertexCount = 0;
    public int yVertexCount = 0;
    [Range(0, 4071)] public int drawLines = 4071;

    private MeshFilter mFilter;
    private int vertexLoopStep;
    private int capVertexCount;
    private int zVertexCount;
    private int[] lookupIndices;

    private List<Vector3> caps = new List<Vector3>();

    private void Start()
    {
        mFilter = GetComponent<MeshFilter>();
        vertexLoopStep = (xVertexCount * 2) + (yVertexCount * 2) - 4;
        capVertexCount = (xVertexCount - 2) * (yVertexCount - 2);
        zVertexCount = (mFilter.sharedMesh.vertexCount - capVertexCount) / vertexLoopStep;
        lookupIndices = new int[zVertexCount * vertexLoopStep + (capVertexCount * 2)];

        Mesh mesh = mFilter.sharedMesh;

        Vector3 vertexDistances = new Vector3((mesh.bounds.extents.x * 2.0f) / xVertexCount,
            (mesh.bounds.extents.y * 2.0f) / yVertexCount,
            (mesh.bounds.extents.z * 2.0f) / zVertexCount);

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 normalizedVertex = mesh.bounds.extents - mesh.vertices[i];
            int lookupIndexX = (int)(normalizedVertex.x / vertexDistances.x);
            int lookupIndexY = (int)(normalizedVertex.y / vertexDistances.y);
            int lookupIndexZ = (int)(normalizedVertex.z / vertexDistances.z);
            
            if (lookupIndexZ <= 0 || lookupIndexZ >= zVertexCount-1)
                caps.Add(mesh.vertices[i]);
            else lookupIndices[(vertexLoopStep * lookupIndexZ) + SzudzikPairing(lookupIndexX, lookupIndexY)] = i;
        }
    }

    // Credit: Matthew Szudzik (http://szudzik.com/ElegantPairing.pdf).
    private int SzudzikPairing(int a, int b)
    {
        return (a >= b) ? (a * a + a + b) : (a + b * b);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (lookupIndices != null)
        {
            for (int i = mFilter.sharedMesh.vertexCount - 1; i > drawLines; i--)
            {
                Gizmos.DrawSphere(mFilter.sharedMesh.vertices[lookupIndices[i]], 0.001f);
            }
        }
        Gizmos.color = Color.red;
        for (int i = 0; i < caps.Count; i++)
            Gizmos.DrawSphere(caps[i], 0.001f);
    }
}
