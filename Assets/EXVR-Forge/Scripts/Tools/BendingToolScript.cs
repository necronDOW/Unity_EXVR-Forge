using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BendingToolScript : ToolScript
{
    //public Object bendPrefab;

    //private GameObject bendInstance;

    //public override void Start()
    //{
    //    base.Start();
    //}
    
    //protected override void OnAttach()
    //{
    //    if (bendPrefab)
    //    {
    //        bendInstance = Instantiate(bendPrefab) as GameObject;
    //        bendInstance.transform.parent = attached.transform;
    //        bendInstance.transform.position = attachPoint.transform.position;
    //        bendInstance.transform.localScale = Vector3.one;

    //        Bend bendScript = bendInstance.GetComponent<Bend>();

    //        Transform[] attachedTransforms = attached.transform.GetComponentsInChildren<Transform>();
    //        List<GameObject> attachedChildren = new List<GameObject>();
    //        for (int i = 0; i < attachedTransforms.Length; i++)
    //        {
    //            if (attachedTransforms[i].GetComponent<MeshFilter>())
    //                attachedChildren.Add(attachedTransforms[i].gameObject);
    //        }

    //        bendScript.SetMeshes(attachedChildren.ToArray());
    //        bendInstance.transform.LookAt(attached.transform.position);
    //        bendInstance.transform.Rotate(new Vector3(-90f, 0f, 0f));
    //    }
    //}

    //protected override void OnDetach()
    //{
    //    if (bendInstance)
    //    {
    //        Destroy(bendInstance);
    //        bendInstance = null;
    //    }
    //}
}
