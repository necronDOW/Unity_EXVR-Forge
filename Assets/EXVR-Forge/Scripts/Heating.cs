using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heating : MonoBehaviour {

    public Color startColor;
    public Color EndColor;
    public GameObject HeatSource;
    public CoolingSource[] coolingSources;
    public int Heat_Detection_Accuracy;
    private Color[] colors;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] worldPositions;
    public float[] rodLoopTemprature;

    void Awake ()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
            colors[i] = startColor;

        HeatSource = GameObject.Find("FireLocation");

        coolingSources = new CoolingSource[2];
        coolingSources[0] = GameObject.Find("WaterSource").GetComponent<CoolingSource>();
        coolingSources[1] = GameObject.Find("OilSource").GetComponent<CoolingSource>();

        rodLoopTemprature = new float[Heat_Detection_Accuracy];

        for (int i = 0; i < rodLoopTemprature.Length; i++)
            rodLoopTemprature[i] = 0;

        mesh.colors = colors;
    }


    void Update()
    {
        //Initialize rod
        if (vertices.Length == 0)
            Awake();

        HeatRod();

        //update colours
        mesh.colors = colors;
    }

    void InitRod()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        colors = new Color[vertices.Length];
    }

   // Initlaise colours when rod is cut

    void HeatRod()
    {
        InitRod();
        //only check every 100 points on the rod for fire collision
        int Length = (vertices.Length / Heat_Detection_Accuracy);

        for (int i = 0, j = 0; i < vertices.Length - Length && j < rodLoopTemprature.Length; i += Length, j++)
        {
            //Get world space location of this point
            vertices[i] = transform.TransformPoint(mesh.vertices[i]);
            //check fire distance
            float FireDistance = Vector3.Distance(HeatSource.transform.position, vertices[i]);
            
            //If in the fire
            if (FireDistance <= 0.55f)
            {
                if (rodLoopTemprature[j] < 100)
                    rodLoopTemprature[j] += Fire.temperature / 1000;
            }
            else
            {
                if (rodLoopTemprature[j] > 0)
                    rodLoopTemprature[j] -= Fire.temperature / 10000;
            }

            foreach (CoolingSource c in coolingSources) {
                if (c != null) {
                    if (c.bounds.Contains(vertices[i]) && rodLoopTemprature[j] > 0) {
                        rodLoopTemprature[j] -= Fire.temperature / 100;
                        c.EmitSteam(vertices[i]);
                    }
                }
            }
        }

        //loop over temprature array 
        for (int i = 0; i < rodLoopTemprature.Length ; i++)
        {
            //update colours for each temprature point
            for (int j = 0; j < Length; j++)
            {
                colors[(i * Length) + j] = Color.Lerp(startColor, EndColor, (rodLoopTemprature[i] / 100));
            }
        }
    }
}


//For performance reasons, consider using colors32 instead. 
//This will avoid byte-to-float conversions in colors, and use less temporary memory.