using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fire : MonoBehaviour
{
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
