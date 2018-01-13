using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class AnvilTool : MonoBehaviour
{
    public string lookupTag = "";
    public Collider[] colliders; // 0 = parent, 1 = this.
    public GameObject attachedRod { get; private set; }
    private Coroutine lastRanCoroutine;

    protected virtual void Start()
    {
        colliders = transform.parent.GetComponentsInChildren<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == lookupTag) {
            colliders[0].enabled = false;
            lastRanCoroutine = StartCoroutine(FreezeCheck(other));
        }
    }

    private IEnumerator FreezeCheck(Collider other)
    {
        if (lastRanCoroutine != null)
            StopCoroutine(lastRanCoroutine);

        while (IsAttached(other.gameObject))
            yield return null;

        Debug.Log("freeze");
        Freeze(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == lookupTag) {
            colliders[0].enabled = true;
            lastRanCoroutine = StartCoroutine(UnfreezeCheck(other));
        }
    }

    private IEnumerator UnfreezeCheck(Collider other)
    {
        if (lastRanCoroutine != null)
            StopCoroutine(lastRanCoroutine);

        while (!IsAttached(other.gameObject))
            yield return null;

        Debug.Log("unfreeze");
        Unfreeze(other.gameObject);
    }

    protected virtual void Freeze(GameObject o)
    {
        o.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //make kinematic
        //make non convex mesh collider for accurate collisions
        attachedRod = o;
    }

    protected virtual void Unfreeze(GameObject o)
    {
        o.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //make non kinematic
        //make convex mesh collider for accurate collisions
        attachedRod = null;
    }

    private bool IsAttached(GameObject other)
    {
        Throwable t = other.GetComponent<Throwable>();
        Network_InteractableObject nio = other.GetComponent<Network_InteractableObject>();

        return ((t && t.attached) || (nio && nio.isAttached));
    }
}
