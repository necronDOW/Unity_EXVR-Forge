using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMeshFactory
{
    private int xSize, ySize, zSize;
    private float meshScale;
    private float xScaled, yScaled, zScaled;
    private float roundnessReal;
    private Vector3 centeringOffset;
    private string meshName;
    private Vector3[] vertices, normals;
    
    public Mesh result {
        get;
        private set;
    }

    public CubeMeshFactory(int xSize, int ySize, int zSize, float meshScale, float roundness, string meshName = "gCube")
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.zSize = zSize;

        this.meshScale = meshScale;
        xScaled = xSize * meshScale;
        yScaled = ySize * meshScale;
        zScaled = zSize * meshScale;

        roundnessReal = (Mathf.Min(new float[] { xScaled, yScaled, zScaled }) / 2.0f) * roundness;
        centeringOffset = new Vector3(xScaled, yScaled, zScaled) * -0.5f;

        this.meshName = meshName;

        Generate();
    }

    private void Generate()
    {
        result = new Mesh();
        CreateVertices();
        CreateTriangles();
    }

    private void CreateVertices()
    {
        int cornerVertices = 8;
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;

        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];

        int v = 0;
        // Bottom face
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
                SetVertex(v++, x, 0, z);
        }

        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
                SetVertex(v++, x, y, 0);

            for (int z = 1; z <= zSize; z++)
                SetVertex(v++, xSize, y, z);

            for (int x = xSize - 1; x >= 0; x--)
                SetVertex(v++, x, y, zSize);

            for (int z = zSize - 1; z > 0; z--)
                SetVertex(v++, 0, y, z);
        }

        // Top face
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
                SetVertex(v++, x, ySize, z);
        }

        result.vertices = vertices;
        result.normals = normals;
    }

    private void SetVertex(int i, float x, float y, float z)
    {
        x *= meshScale;
        y *= meshScale;
        z *= meshScale;

        Vector3 inner = vertices[i] = new Vector3(x, y, z);

        if (x < roundnessReal)
            inner.x = roundnessReal;
        else if (x > xScaled - roundnessReal)
            inner.x = xScaled - roundnessReal;

        if (y < roundnessReal)
            inner.y = roundnessReal;
        else if (y > yScaled - roundnessReal)
            inner.y = yScaled - roundnessReal;

        if (z < roundnessReal)
            inner.z = roundnessReal;
        else if (z > zScaled - roundnessReal)
            inner.z = zScaled - roundnessReal;

        normals[i] = (vertices[i] - inner).normalized;
        vertices[i] = centeringOffset + inner + normals[i] * roundnessReal;
    }

    private void CreateTriangles()
    {
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        int[] triangles = new int[quads * 6];
        int ring = (xSize + zSize) * 2;
        int t = 0, v = (xSize - 1) * (zSize - 1);

        t = CreateBottomFace(result.vertexCount, triangles, t, ring);

        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < ring - 1; q++, v++)
                t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
            t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
        }

        t = CreateTopFace(triangles, t, ring);

        result.triangles = triangles;
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
        int faceSize = (xSize - 1) * (zSize - 1);
        int v = ring * ySize + faceSize;
        for (int x = 0; x < xSize - 1; x++, v++)
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
        t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (ySize + 1) - 1 + faceSize;
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
        int faceSize = (xSize - 1) * (zSize - 1);
        int v = faceSize;
        int vMid = 0;

        t = SetQuad(triangles, t, v + ring - 1, vMid, v, ++v);
        for (int x = 1; x < xSize - 1; x++, v++, vMid++)
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

        int vMin = faceSize + ring - 2;
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
}