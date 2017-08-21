using UnityEngine;
using System.Collections;

public class OilScript : MonoBehaviour
{
    [HideInInspector]
    public MeshFilter exportTarget;
    public ProSkaterScript proSkaterScript;

    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Rod")
        {
            GameObject target = other.attachedRigidbody.gameObject;

            if (target.GetComponent<MeshFilter>())
            {
                exportTarget = target.GetComponent<MeshFilter>();
                proSkaterScript.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Rod")
        {
            GameObject target = other.attachedRigidbody.gameObject;

            if (target.GetComponent<MeshFilter>())
            {
                exportTarget = null;
                proSkaterScript.SetActive(false);
            }
        }
    }
}
