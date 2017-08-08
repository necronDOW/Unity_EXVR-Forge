using UnityEngine;
using System.Collections;

public class CoalScript : MonoBehaviour
{
    [HideInInspector]
    public int inFireIndex;

    public int startLifetime = 1;
    public bool destroyed { get; private set; }
    private int lifetime;

    void Start()
    {
        inFireIndex = -1;
        lifetime = startLifetime;
    }

    void Update()
    {
        ReduceLifetime(100);

        if (destroyed)
            Destroy(gameObject, 0.05f);
    }

    public void IncreaseLifetime(int amount)
    {
        lifetime += amount;

        if (lifetime > startLifetime)
            lifetime = startLifetime;
    }

    public void ReduceLifetime(int amount)
    {
        lifetime -= amount;

        if (lifetime <= 0)
            destroyed = true;
    }
}