using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshInfo))]
public class CubeMeshGenerator : MonoBehaviour
{
    [System.Serializable]
    public class GeneratorParams
    {
        [SerializeField, Range(2, 10000)] public int xSize = 2;
        [SerializeField, Range(2, 10000)] public int ySize = 2;
        [SerializeField, Range(2, 10000)] public int zSize = 2;
        [SerializeField, Range(0, 1)] public float roundness = 0.01f;
        [SerializeField] public float meshScale = 1.0f;

        public GeneratorParams() { }
        public GeneratorParams(int xSize, int ySize, int zSize, float roundness, float meshScale)
        {
            this.xSize = xSize;
            this.ySize = ySize;
            this.zSize = zSize;
            this.roundness = roundness;
            this.meshScale = meshScale;
        }

        public void ApplySimplification(float simplificationFactor)
        {
            simplificationFactor = Mathf.Clamp01(simplificationFactor);

            float oldXSize = (float)xSize;
            xSize = Mathf.Clamp((int)(xSize / simplificationFactor), 0, xSize);
            ySize = Mathf.Clamp((int)(ySize / simplificationFactor), 0, ySize);
            zSize = Mathf.Clamp((int)(zSize / simplificationFactor), 0, zSize);

            meshScale *= oldXSize / xSize;
        }
    }

    private MeshFilter mFilter;
    private MeshCollider mCollider;

    public GeneratorParams gParams;

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
        Mesh mesh = new CubeMeshFactory(gParams.xSize, gParams.ySize, gParams.zSize, gParams.meshScale, gParams.roundness, name).result;
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
}