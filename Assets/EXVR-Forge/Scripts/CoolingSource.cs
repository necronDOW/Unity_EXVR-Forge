using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolingSource : MonoBehaviour
{
    public float resetAfterSeconds = 0.2f;
    public float limitInstantiationSeconds = 0.1f;
    public Object audioPrefab;
    public Bounds bounds { get { return GetComponent<Collider>().bounds; } }
    private ParticleSystem steamParticleSystem;
    private Vector3 resetPosition;
    private float resetTimer = 0.0f;
    private float instantiationTimer;
    private bool isReset = true;
    private List<GameObject> audioInstances;

    private void Awake()
    {
        steamParticleSystem = GetComponentInChildren<ParticleSystem>();
        resetPosition = steamParticleSystem.transform.position;
        audioInstances = new List<GameObject>();
    }

    private void Update()
    {
        instantiationTimer += Time.deltaTime;

        if (!isReset) {
            resetTimer += Time.deltaTime;

            if (resetTimer >= resetAfterSeconds)
                Reset();
        }
    }

    public void EmitSteam(Vector3 worldPosition)
    {
        isReset = false;
        resetTimer = 0.0f;

        steamParticleSystem.transform.position = worldPosition;

        if (audioPrefab && instantiationTimer >= limitInstantiationSeconds) {
            audioInstances.Add((GameObject)Instantiate(audioPrefab, worldPosition, Quaternion.identity));
            instantiationTimer = 0.0f;
        }
    }

    private void Reset()
    {
        isReset = true;
        resetTimer = 0.0f;
        steamParticleSystem.transform.position = resetPosition;

        foreach (GameObject g in audioInstances)
            Destroy(g);
        audioInstances.Clear();
    }
}
