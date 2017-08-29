using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heating : MonoBehaviour {

    public Color startColor;
    private Color[] colors;
    private Mesh mesh;
    private Vector3[] vertices;

    void Start ()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = startColor;

        mesh.colors = colors;       
    }

    private void Update()
    {
        for (int i = 0; i < vertices.Length/2; i++)
        {
            colors[i] = new Color(Fire.temperature, 0,0);
        }
           
        mesh.colors = colors;
    }

}
//For performance reasons, consider using colors32 instead. 
//This will avoid byte-to-float conversions in colors, and use less temporary memory.