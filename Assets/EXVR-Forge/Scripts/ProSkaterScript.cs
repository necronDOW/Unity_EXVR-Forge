using UnityEngine;
using System.Collections;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Interactable))]
public class ProSkaterScript : MonoBehaviour
{
    public float variance;
    public float floatSpeed;
    public Vector3 rotation;
    public Color activeEmission;
    public float inactiveRamp = 0.5f;

    private float t = 0.5f;
    private Vector3 lowerBound;
    private Vector3 upperBound;
    private Color defaultColor;
    private Color defaultEmission;
    private MeshRenderer mR;
    private Collider thisCollider;

    void Start()
    {
        lowerBound = transform.position - new Vector3(0, variance, 0);
        upperBound = transform.position + new Vector3(0, variance, 0);

        mR = GetComponent<MeshRenderer>();
        mR.material.EnableKeyword("_EMISSION");
        defaultColor = mR.material.color;
        defaultEmission = mR.material.GetColor("_EmissionColor");
        thisCollider = GetComponent<Collider>();

        SetActive(false);
    }
    
    void Update()
    {
        t += floatSpeed;

        transform.position = new Vector3(transform.position.x, 
            Mathf.Lerp(lowerBound.y, upperBound.y, t), transform.position.z);

        if (t >= 1f || t <= 0)
            floatSpeed *= -1;

        transform.Rotate(rotation);
    }

    private void OnHandHoverBegin(Hand hand)
    {
        mR.material.SetColor("_EmissionColor", activeEmission);
    }

    private void OnHandHoverEnd(Hand hand)
    {
        mR.material.SetColor("_EmissionColor", defaultEmission);
    }

    public void SetActive(bool value)
    {
        thisCollider.enabled = value;

        if (value)
        {
            mR.material.color = defaultColor;
            mR.material.SetColor("_EmissionColor", defaultEmission);
        }
        else
        {
            mR.material.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a) * inactiveRamp;
            mR.material.SetColor("_EmissionColor", new Color(defaultEmission.r, defaultEmission.g, defaultEmission.b) * inactiveRamp);
        }
    }
}
