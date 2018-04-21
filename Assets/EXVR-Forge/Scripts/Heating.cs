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
    public float[] rodLoopTemprature;
    private int[] rodLoopindex;

    private const int maxHeat = 100;
    private const float maxHeatDist = 0.75f;
    
    void Awake()
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
        rodLoopindex = new int[Heat_Detection_Accuracy];

        for (int i = 0; i < rodLoopTemprature.Length; i++)
            rodLoopTemprature[i] = 0;

        for (int i = 0; i < rodLoopindex.Length; i++)
            rodLoopindex[i] = 0;

        for (int i = 0, j = 0; i < vertices.Length - (vertices.Length / Heat_Detection_Accuracy) && j < rodLoopindex.Length; i += (vertices.Length / Heat_Detection_Accuracy), j++)
        {
            rodLoopindex[j] = i;
        }
        

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
            rodLoopindex[j] = i;
            //check fire distance
            float FireDistance = Vector3.Distance(HeatSource.transform.position, vertices[i]);
            float tDistance = 1.0f - (FireDistance / maxHeatDist);

            //If in the fire
            if (FireDistance <= maxHeatDist)
            {
                if (rodLoopTemprature[j] < maxHeat)
                    rodLoopTemprature[j] += (Fire.temperature * tDistance * Time.deltaTime) * 0.1f;
            }
            else
            {
                if (rodLoopTemprature[j] > 0)
                    rodLoopTemprature[j] -= (Fire.temperature * Time.deltaTime) * 0.015f;
            }

            foreach (CoolingSource c in coolingSources) {
                if (c != null) {
                    if (c.bounds.Contains(vertices[i]) && rodLoopTemprature[j] > 0) {
                        rodLoopTemprature[j] -= Fire.temperature * Time.deltaTime;
                        c.EmitSteam(vertices[i]);
                    }
                }
            }
        }

        //loop over temprature array 
        for (int i = 0; i < rodLoopTemprature.Length ; i++)
        {
            float t = (rodLoopTemprature[i] / maxHeat);

            //update colours for each temprature point
            for (int j = 0; j < Length; j++)
            {
                colors[(i * Length) + j] = new Color(
                    EaseInSin(t, startColor.r, EndColor.r - startColor.r, 1.0f),
                    EaseInSin(t, startColor.g, EndColor.g - startColor.g, 2.0f),
                    EaseInSin(t, startColor.b, EndColor.b - startColor.b, 20.0f));
                //EaseInSin((rodLoopTemprature[i] / maxHeat), startColor, EndColor - startColor, 2.5f);
                //Color.Lerp(startColor, EndColor, (rodLoopTemprature[i] / maxHeat));
            }
        }
    }


    public float ClosestHeatAtPoint(Vector3 worldPoint)
    {
        worldPoint = transform.InverseTransformPoint(worldPoint);
        float closestDistance = float.MaxValue;
        int closestVertex = 0;

        for (int i = 0; i < rodLoopindex.Length; i++) {
            float distance = Vector3.Distance(worldPoint, VertexAtLoopIndex(i));
            if (distance < closestDistance) {
                closestVertex = i;
                closestDistance = distance;
            }
        }

        return rodLoopTemprature[closestVertex];
    }

    public float ClosestHeatAtPointNormalized(Vector3 worldPoint)
    {
        return Mathf.Clamp01(ClosestHeatAtPoint(worldPoint) / maxHeat);
    }

    private float EaseInSin(float t, float start, float change, float d)
    {
        return -change * Mathf.Cos(t / d * (Mathf.PI / 2)) + change + start;
    }

    private Color EaseInSin(float t, Color start, Color change, float d)
    {
        Color minChange = new Color(-change.r, -change.g, -change.b, -change.a);
        return minChange * Mathf.Cos(t / d * (Mathf.PI / 2)) + change + start;
    }

    public Vector3 VertexAtLoopIndex(int i)
    {
        return mesh.vertices[rodLoopindex[i]];
    }

    public Vector3 WorldVertexAtLoopIndex(int i)
    {
        return transform.TransformPoint(VertexAtLoopIndex(i));
    }

    int lastClosest = -1;
    public int FindClosestLoopIndexAtPoint(Vector3 worldPos, int inset = 0)
    {
        Vector3 localizedPos = transform.InverseTransformPoint(worldPos);
        int closestIndex = inset;
        float closestDist = Vector3.Distance(VertexAtLoopIndex(inset), localizedPos);

        for (int i = inset + 1; i < rodLoopindex.Length - inset; i++) {
            float dist = Vector3.Distance(VertexAtLoopIndex(i), localizedPos);
            if (dist < closestDist) {
                closestIndex = i;
                closestDist = dist;
            }
        }

        return lastClosest = closestIndex;
    }

    public int OffsetLoopIndex(int index, int offset)
    {
        index += offset;
        if (index >= rodLoopindex.Length || index < 0)
            index = -1;

        return lastClosest = index;
    }

    private void OnDrawGizmos()
    {
        if (rodLoopindex != null)
        {
            for (int i = 0; i < rodLoopindex.Length; i += 2) {
                Gizmos.color = new Color(0.0f, i * 0.01f, 0.0f);
                Gizmos.DrawCube(WorldVertexAtLoopIndex(i), Vector3.one * 0.01f);
            }

            if (lastClosest != -1) {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(WorldVertexAtLoopIndex(lastClosest), 0.02f);
            }
        }
    }
}


//For performance reasons, consider using colors32 instead. 
//This will avoid byte-to-float conversions in colors, and use less temporary memory.