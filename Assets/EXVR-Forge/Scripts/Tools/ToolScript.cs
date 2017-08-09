using UnityEngine;
using System.Collections;
using NewtonVR;

public abstract class ToolScript : MonoBehaviour
{
    [HideInInspector]
    public GameObject attachPoint { get; private set; }

    protected bool readyToUse = false;
    protected BoxCollider bCollider;
    protected GameObject[] maluableObjects;
    protected GameObject attached;

    private GameObject collidedObject;
    private Vector3 lockVector;
    private Vector3 eulerLock;

    public virtual void Start()
    {
        bCollider = GetComponent<BoxCollider>();

        if (transform.FindChild("attachPoint"))
            attachPoint = transform.FindChild("attachPoint").gameObject;
        else
        {
            attachPoint = new GameObject();
            attachPoint.name = "attachPoint";
            attachPoint.transform.parent = transform;
            attachPoint.transform.localPosition = Vector3.zero;
            attachPoint.transform.rotation = Quaternion.identity;
        }

        maluableObjects = GameObject.FindGameObjectsWithTag("Maluable");
    }

    public virtual void Update()
    {
        SetInteractability();

        if (attached)
        {
            attached.transform.position = lockVector;
            attached.transform.eulerAngles = eulerLock;
        }

        if (collidedObject && !collidedObject.GetComponent<NVRInteractable>().IsAttached)
        {
            attached = collidedObject;

            lockVector = attached.transform.position;
            eulerLock = attached.transform.eulerAngles;

            OnAttach();
        }
        else if (attached && attached.GetComponent<NVRInteractable>().IsAttached)
        {
            attached = null;

            OnDetach();
        }

        collidedObject = null;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Maluable")
            collidedObject = other.transform.parent.parent.gameObject;
    }

    public virtual void SetReady(bool value)
    {
        readyToUse = value;
    }

    private void SetInteractability()
    {
        if (readyToUse)
        {
            for (int i = 0; i < maluableObjects.Length; i++)
            {
                NVRInteractable interactable = maluableObjects[i].GetComponent<NVRInteractable>();

                if (interactable && interactable.IsAttached)
                {
                    bCollider.isTrigger = true;
                    break;
                }

                bCollider.isTrigger = false;
            }
        }
    }

    protected virtual void OnAttach() { }
    protected virtual void OnDetach() { }
}
