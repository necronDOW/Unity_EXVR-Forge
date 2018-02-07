using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshInfo))]
public class CubeMeshGenerator : MonoBehaviour
{
    private MeshFilter mFilter;
    private MeshCollider mCollider;
    private float xScaled, yScaled, zScaled;
    private float roundnessReal = 0.0f;
    private Vector3 centeringOffset;
    
    public int xSize = 1, ySize = 1, zSize = 1;
    [Range(0.01f, 1)] public float roundness = 0.01f;
    public float meshScale = 1.0f;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        mFilter = GetComponent<MeshFilter>();
        mCollider = GetComponent<MeshCollider>();

        if (GenerateMesh())
            Destroy(this);
    }

    private bool GenerateMesh()
    {
        Mesh mesh = new CubeMeshFactory(xSize, ySize, zSize, meshScale, roundness, name).result;
        if (mesh == null) {
            Debug.LogError("Mesh Generation failed! (name: " + name + ").");
            return false;
        }

        if (mFilter) {
            Debug.LogWarning("No mesh filter attached to CubeMeshGenerator (" + gameObject.name + ").");
            mFilter.sharedMesh = mesh;
        }

        if (mCollider) {
            Debug.LogWarning("No mesh collider attached to CubeMeshGenerator (" + gameObject.name + ").");
            mCollider.sharedMesh = mesh;
        }
        
        return true;
    }

    private void UpdateCollider()
    {
        if (mCollider) {
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