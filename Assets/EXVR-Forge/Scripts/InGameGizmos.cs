using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameGizmos : MonoBehaviour
{
    public float gizmosScale = 1.0f;
    private static Material material;

    private static void CreateLineMaterial()
    {
        if (!material) {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            material = new Material(shader);
            material.hideFlags = HideFlags.HideAndDontSave;
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            material.SetInt("_ZWrite", 0);
        }
    }

    private void OnRenderObject()
    {
        CreateLineMaterial();
        material.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        DrawLine(Vector3.zero, transform.forward, Color.blue);
        DrawLine(Vector3.zero, transform.up, Color.green);
        DrawLine(Vector3.zero, transform.right, Color.red);

        GL.End();
        GL.PopMatrix();
    }

    private void DrawLine(Vector3 start, Vector3 dir, Color color)
    {
        start *= gizmosScale;
        dir *= gizmosScale;

        GL.Color(color);
        GL.Vertex(start);
        GL.Vertex(start + dir);
    }
}
