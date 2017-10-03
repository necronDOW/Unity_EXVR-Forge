using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heating : MonoBehaviour {

    public Color startColor;
    public GameObject HeatSoruce;
    private Color[] colors;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] worldPositions;
    private float minX, maxX;
    private float R, G, B;

    void Awake ()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = startColor;

        mesh.colors = colors;

        minX = HeatSoruce.transform.position.x - 0.5f;
        maxX = HeatSoruce.transform.position.x + 0.5f;
    }


    void Update()
    {
        if (vertices.Length == 0)
            Awake();

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = transform.TransformPoint(mesh.vertices[i]);
            if (vertices[i].x > minX && vertices[i].x < maxX)
            {
                colors[i] = new Color((R += Fire.temperature), 0, 0);
            }
            else
            {
                if (R < 0)
                colors[i] = new Color((R -= 0.0001f), 0, 0);
            }
        }
        mesh.colors = colors;
    }

}
//For performance reasons, consider using colors32 instead. 
//This will avoid byte-to-float conversions in colors, and use less temporary memory.