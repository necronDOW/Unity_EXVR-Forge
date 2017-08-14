using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CubeMeshGenerator : MonoBehaviour
{
    private MeshFilter mFilter;
    private MeshCollider mCollider;
    
    public int xSize = 1, ySize = 1, zSize = 1;
    public bool centered = true;

    private void Awake()
    {
        mFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();

        GenerateMesh();
    }
    
    private void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = name + "_generated-mesh";
        mFilter.sharedMesh = mesh;

        CreateVertices(ref mesh);
        CreateTriangles(ref mesh);
    }

    private void CreateVertices(ref Mesh mesh)
    {
        int cornerVertices = 8;
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;

        Vector3[] vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        int v = 0;
        for (int y = 0; y <= ySize; y++) {
            for (int x = 0; x <= xSize; x++)
                vertices[v++] = new Vector3(x, y, 0.0f);

            for (int z = 1; z <= zSize; z++)
                vertices[v++] = new Vector3(xSize, y, z);

            for (int x = xSize - 1; x >= 0; x--)
                vertices[v++] = new Vector3(x, y, zSize);

            for (int z = zSize - 1; z > 0; z--)
                vertices[v++] = new Vector3(0.0f, y, z);
        }

        for (int z = 1; z < zSize; z++) {
            for (int x = 1; x < xSize; x++)
                vertices[v++] = new Vector3(x, ySize, z);
        }

        for (int z = 1; z < zSize; z++) {
            for (int x = 1; x < xSize; x++)
                vertices[v++] = new Vector3(x, 0, z);
        }

        mesh.vertices = vertices;
    }

    private void CreateTriangles(ref Mesh mesh)
    {
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        int[] triangles = new int[quads * 6];
        int ring = (xSize + zSize) * 2;
        int t = 0, v = 0;

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
            t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
        }

        t = CreateTopFace(triangles, t, ring);
        t = CreateBottomFace(mesh.vertexCount, triangles, t, ring);

        mesh.triangles = triangles;
    }

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private int CreateTopFace(int[] triangles, int t, int ring)
    {
        int v = ring * ySize;
        for (int x = 0; x < xSize - 1; x++, v++)
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            for (int x = 1; x < xSize - 1; x++, vMid++)
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
            t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
        }

        int vTop = vMin - 2;
        t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
            t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
        t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    private int CreateBottomFace(int vertexCount, int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = vertexCount - (xSize - 1) * (zSize - 1);
        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xSize - 1; x++, v++, vMid++)
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = ring - 2;
        vMid -= xSize - 2;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
            for (int x = 1; x < xSize - 1; x++, vMid++)
                t = SetQuad(triangles, t, vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
            t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
        }

        int vTop = vMin - 1;
        t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
        t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

        return t;
    }

    private void UpdateCollider()
    {
        if (mCollider)
        {
            mFilter.sharedMesh.RecalculateBounds();
            mCollider.sharedMesh = mFilter.sharedMesh;
        }
    }

    private void OnValidate()
    {
        if (xSize < 1) xSize = 1;
        if (ySize < 1) ySize = 1;
        if (zSize < 1) zSize = 1;
    }
}
