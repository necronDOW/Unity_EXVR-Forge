using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]

public class Fire : MonoBehaviour
{
    public static float temperature;
    public float coalIncrement = 0.025f;
    public static float maxTemprature = 100;
    public int coalLifetimeIncriment = 10000;
    public static float coalsInFire;
    private int extraTime;
    private static ParticleSystem[] flames;
    private static float[] startValues;
    private static AudioSource sound;
    private Light Fire_Light;
    private static float Light_Intensity = 0.0f;

    private void Start()
    {
        temperature = 0f;
        coalsInFire = 0;
        GetComponent<BoxCollider>().isTrigger = true;
        extraTime = coalLifetimeIncriment;
        flames = GetComponentsInChildren<ParticleSystem>();
        startValues = new float[flames.Length];

        Fire_Light = GetComponentInChildren<Light>();
        Fire_Light.intensity = 0;

        for (int i = 0; i < flames.Length; i++)
        {
            startValues[i] = flames[i].emission.rateOverTime.constantMax;         
        }

        sound = GetComponent<AudioSource>();

        SetEmissionRate(0);
    }

    private void Update()
    {
        Fire_Light.intensity = Light_Intensity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coal")
        {
            if (coalsInFire < 100.0f)
            {
                other.GetComponent<CoalScript>().IncreaseLifetime(extraTime);
                coalsInFire += 1.0f;
                temperature = coalsInFire;
                SetEmissionRate(coalsInFire);
              
            }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Coal")
        {
            other.GetComponent<CoalScript>().IncreaseLifetime(-extraTime);
            coalsInFire -= 1.0f;
            temperature = coalsInFire;
            SetEmissionRate(coalsInFire);
        }
    }

    public static void SetEmissionRate(float emissionRate)
    {
        if (emissionRate > 100.0f)
            emissionRate = 100.0f;

        Light_Intensity = (emissionRate / 100) *3;

        for (int i = 0; i < flames.Length; i++)
        {
            var rate = flames[i].emission;
            rate.rateOverTime = startValues[i] * (emissionRate / maxTemprature);
        }
        sound.volume = (emissionRate / 100);
        
    }
}
