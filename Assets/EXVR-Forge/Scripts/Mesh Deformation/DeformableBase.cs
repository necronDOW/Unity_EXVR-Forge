﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class DeformableBase : MonoBehaviour
{
    public static int generatedVerticesLength = 0;
    public float colliderSimplificationFactor = 0.4f;

    private MeshFilter _mFilter;
    protected MeshFilter mFilter
    {
        get {
            if (!_mFilter)
                _mFilter = GetComponent<MeshFilter>();
            return _mFilter;
        }
    }

    private MeshCollider _mCollider;
    protected MeshCollider mCollider {
        get {
            if (!_mCollider)
                _mCollider = GetComponent<MeshCollider>();
            return _mCollider;
        }
    }

    protected int[] originalTriangles;
    protected Vector3[] originalVertices;
    protected Mesh simplifiedMesh;
    
    private CubeMeshGenerator.GeneratorParams simplifiedGParams {
        get {
            return CubeMeshGenerator.gParams.ApplySimplification(0.4f);
        }
    }

    protected virtual void Start()
    {
        UpdateComponents();
    }

    protected virtual void OnApplicationQuit()
    {
        ResetMesh();
    }

    public static Vector3 DivideVector3(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    protected void ResetMesh()
    {
        mFilter.sharedMesh.triangles = originalTriangles;
        mFilter.sharedMesh.vertices = originalVertices;
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = mFilter.sharedMesh;
    }

    protected void UpdateComponents()
    {
        if (!_mFilter || !_mCollider) {
            _mFilter = GetComponent<MeshFilter>();
            _mCollider = GetComponent<MeshCollider>();
            originalTriangles = mFilter.sharedMesh.triangles;
            originalVertices = mFilter.sharedMesh.vertices;

            if (!simplifiedMesh) {
                GenerateLowPolyMesh();
            }
        }
    }

    protected void GenerateLowPolyMesh()
    {
        simplifiedMesh = new CubeMeshFactory(simplifiedGParams, "simplified_").result;

        mCollider.sharedMesh = null;
        mCollider.sharedMesh = simplifiedMesh;
    }

    public void UpdateMeshCollider(bool recalculate = false)
    {
        mFilter.sharedMesh.RecalculateBounds();
        mCollider.sharedMesh = null;
        mCollider.sharedMesh = simplifiedMesh;
    }

    public void SetMeshCollider(Mesh colliderMesh)
    {
        simplifiedMesh = colliderMesh;

        if (_mCollider == null) {
            _mCollider = GetComponent<MeshCollider>();
            mCollider.sharedMesh = null;
            mCollider.sharedMesh = simplifiedMesh;
        }
    }

    public static int FindClosestVertex(Vector3 localPt, Vector3[] verts)
    {
        int closestIndex = 0;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < verts.Length; i++) {
            float dist = Vector3.Distance(verts[i], localPt);
            if (dist < minDist) {
                closestIndex = i;
                minDist = dist;
            }
        }

        return closestIndex;
    }

    public static float FindClosestHeatFactor(Heating heatScript, Vector3 worldCheckLocation)
    {
        if (heatScript != null) {
            return heatScript.ClosestHeatAtPointNormalized(worldCheckLocation);
        }

        return 1.0f;
    }
}
