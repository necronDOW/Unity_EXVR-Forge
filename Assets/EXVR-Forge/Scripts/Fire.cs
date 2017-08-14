using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]

public class Fire : MonoBehaviour
{
    public float temperature;
    public float coalIncrement = 0.025f;
    public float maxTemprature = 1;
    public int coalLifetimeIncriment = 1000000000;
    private float coalsInFire;
    //private List<CoalScript> coalsInFire;
    //global fire temprature
    //when coal enters fire
    //On Trigger Enter
    //add lifetime to coal through IncreaseLifetime(int amount)
    //for each bit of coal added, add to temprature
    //link particle effects to temprature of the fire within reason

    private void Start()
    {
        temperature = 0f;
        coalsInFire = 0f;
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Update()
    {
        if (coalsInFire > 0)
            coalsInFire -= 0.0005f;

        // Calculate temprature
        //Debug.Log(coalsInFire);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coal")
        {
            other.GetComponent<CoalScript>().IncreaseLifetime(coalLifetimeIncriment);
            if (coalsInFire < 100)
                coalsInFire += 1.0f;
        }
       
    }


    //public float coalIncrement = 0.001f;
    //public float maxTemperature = 0.01f;
    //public ParticleSystem linkedParticles;
    //public float temperature
    //{
    //    get { return _temperature; }
    //    set
    //    {
    //        _temperature = value;

    //        if (_temperature > maxTemperature)
    //            _temperature = maxTemperature;

    //        if (linkedParticles != null)
    //        {
    //            SetEmissionRate(linkedParticles, 10000f * temperature);
    //            linkedParticles.maxParticles = (int)linkedParticles.emission.rate.constantMax;
    //        }
    //    }
    //}

    //private float _temperature = 0.01f;
    //private List<CoalScript> coalsInFire;

    //void Start()
    //{
    //    temperature = 0f;
    //    coalsInFire = new List<CoalScript>();
    //}



    //void Update()
    //{
    //    for (int i = 0; i < coalsInFire.Count; i++)
    //    {
    //        coalsInFire[i].IncreaseLifetime(99);

    //        if (coalsInFire[i].destroyed)
    //            coalsInFire.RemoveAt(i);
    //    }

    //    temperature = (coalsInFire.Count * coalIncrement) / 5f;
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    CoalScript coalScript = other.GetComponent<CoalScript>();

    //    if (coalScript && coalScript.inFireIndex < 0)
    //    {
    //        coalScript.inFireIndex = coalsInFire.Count;
    //        coalsInFire.Add(coalScript);
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    CoalScript coalScript = other.GetComponent<CoalScript>();

    //    if (coalScript && coalScript.inFireIndex != -1)
    //    {
    //        coalsInFire.RemoveAt(coalScript.inFireIndex);
    //        coalScript.inFireIndex = -1;
    //    }
    //}

    //private void SetEmissionRate(ParticleSystem particleSystem, float emissionRate)
    //{
    //    if (emissionRate > 1000f)
    //        emissionRate = 1000f;

    //    ParticleSystem.EmissionModule emission = particleSystem.emission;
    //    var rate = emission.rate;
    //    rate.constantMax = emissionRate;
    //    emission.rate = rate;
    //}
}
