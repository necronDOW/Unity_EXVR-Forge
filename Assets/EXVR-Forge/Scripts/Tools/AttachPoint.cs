using UnityEngine;
using Valve.VR.InteractionSystem;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]


public class AttachPoint : MonoBehaviour
{
    




    //private MeshRenderer mRenderer;
    //private Interactable inCollider;
    //private GameObject attached;

    //void Start()
    //{
    //    mRenderer = GetComponent<MeshRenderer>();
    //}

    //void Update()
    //{
    //    if (attached)
    //    {
    //        attached.transform.position = transform.position;
    //        attached.transform.forward = transform.forward;

    //        if (attached.GetComponent<Interactable>().atta)
    //            DetachObject();
    //    }

    //    if (!inCollider)
    //    {
    //        for (int i = 0; i < toolNames.Length; i++)
    //        {
    //            if (GameObject.Find(toolNames[i]).GetComponent<NVRInteractable>().IsAttached && !mRenderer.enabled)
    //            {
    //                mRenderer.enabled = true;
    //                break;
    //            }
    //            else if (!GameObject.Find(toolNames[i]).GetComponent<NVRInteractable>().IsAttached && mRenderer.enabled)
    //                mRenderer.enabled = false;
    //        }
    //    }
    //    else
    //    {
    //        if (!inCollider.IsAttached && !attached)
    //            AttachObject(inCollider.gameObject);
    //    }
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    for (int i = 0; i < toolNames.Length; i++)
    //    {
    //        if (other.name == toolNames[i])
    //        {
    //            inCollider = other.GetComponent<Interactable>();

    //            if (other.GetComponent<NVRInteractable>().IsAttached)
    //                mRenderer.enabled = false;
    //        }
    //    }
    //}

    //void OnTriggerExit(Collider other)
    //{
    //    for (int i = 0; i < toolNames.Length; i++)
    //    {
    //        if (other.name == toolNames[i])
    //        {
    //            inCollider = null;

    //            if (other.GetComponent<NVRInteractable>().IsAttached)
    //                mRenderer.enabled = true;
    //        }
    //    }
    //}

    //public void AttachObject(GameObject o)
    //{
    //    if (!attached)
    //    {
    //        attached = o;
    //        attached.transform.position = transform.position;
    //        attached.transform.up = transform.up;

    //        if (attached.GetComponent<ToolScript>())
    //            attached.GetComponent<ToolScript>().SetReady(true);
    //    }
    //}

    //public void DetachObject()
    //{
    //    if (attached)
    //    {
    //        if (attached.GetComponent<ToolScript>())
    //            attached.GetComponent<ToolScript>().SetReady(false);

    //        attached = null;
    //    }
    //}
}
