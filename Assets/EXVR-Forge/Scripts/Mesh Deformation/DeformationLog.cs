using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformationLog : MonoBehaviour
{
    public class DeformOperation
    {
        protected Transform objectTransform;

        public DeformOperation(Transform objectTransform)
        {
            this.objectTransform = objectTransform;
        }

        public virtual bool ApplyOperation()
        {
            return objectTransform != null;
        }

        public virtual string ParamsToString()
        {
            return "";
        }
    }

    public class MeshDeform : DeformOperation
    {
        public Vector3 impactVector { get; private set; }
        public Vector3 simplifiedVector { get; private set; }
        public bool applyOnlyToCollider { get; private set; }

        public MeshDeform(Transform objectTransform, Vector3 impactVector, Vector3 simplifiedVector, bool applyOnlyToCollider)
            : base(objectTransform)
        {
            this.impactVector = impactVector;
            this.simplifiedVector = simplifiedVector;
            this.applyOnlyToCollider = applyOnlyToCollider;
        }

        public override bool ApplyOperation()
        {
            if (base.ApplyOperation()) {
                DeformableMesh deformationScript = objectTransform.GetComponent<DeformableMesh>();

                if (deformationScript != null) {
                    deformationScript.Deform(impactVector, simplifiedVector);
                    return true;
                }
            }

            return false;
        }

        public override string ParamsToString()
        {
            return base.ParamsToString();
        }
    }

    public class BendDeform : DeformOperation
    {
        public float curvature { get; private set; }
        public float length { get; private set; }
        public float amount { get; private set; }
        public bool direction { get; private set; }
        public bool applyOnlyToCollider { get; private set; }

        public BendDeform(Transform objectTransform, float curvature, float length, float amount, bool direction, bool applyOnlyToCollider)
            : base(objectTransform)
        {
            this.curvature = curvature;
            this.length = length;
            this.amount = amount;
            this.direction = direction;
            this.applyOnlyToCollider = applyOnlyToCollider;
        }

        public override bool ApplyOperation()
        {
            if (base.ApplyOperation()) {
                Network_BendTool netBendTool = objectTransform.GetComponent<Network_BendTool>();

                if (netBendTool && netBendTool.bendTool.bendPrefab != null) {
                    GameObject bendObj = Instantiate(netBendTool.bendTool.bendPrefab);
                    BendInstance bendInstance = bendObj.GetComponentInChildren<BendInstance>();

                    if (bendInstance) {
                        bendInstance.curvature = curvature;
                        bendInstance.length = length;
                        bendInstance.amount = amount;
                        bendInstance.direction = direction;

                        if (!applyOnlyToCollider) {
                            bendInstance.DeformAll();
                        }
                        else {
                            bendInstance.Deform(1);
                        }

                        Destroy(bendObj);
                        return true;
                    }
                }
            }

            return false;
        }

        public override string ParamsToString()
        {
            return base.ParamsToString();
        }
    }

    public enum DeformType
    {
        Mesh,
        Bend
    }

    public List<DeformOperation> deformOperations { get; private set; }

    private System.Type EnumToType(DeformType type)
    {
        switch (type)
        {
            case DeformType.Mesh:
                return typeof(MeshDeform);

            case DeformType.Bend:
                return typeof(BendDeform);

            default:
                return typeof(DeformOperation);
        }
    }

    public void Awake()
    {
        if (deformOperations == null) {
            deformOperations = new List<DeformOperation>();
        }
    }

    public DeformationLog(DeformationLog other)
    {
        if (deformOperations == null) {
            deformOperations = new List<DeformOperation>();
        }

        foreach (DeformOperation d in other.deformOperations) {
            if (d.GetType() == typeof(MeshDeform)) {
                MeshDeform md = ((MeshDeform)d);
                AddOperation(md.impactVector, md.simplifiedVector, md.applyOnlyToCollider);
            }
            else if (d.GetType() == typeof(BendDeform)) {
                BendDeform bd = ((BendDeform)d);
                AddOperation(bd.curvature, bd.length, bd.amount, bd.direction, bd.applyOnlyToCollider);
            }
        }
    }

    // Mesh Deform Operation
    public void AddOperation(Vector3 impactVector, Vector3 simplifiedVector, bool applyOnlyToCollider = false)
    {
        deformOperations.Add(new MeshDeform(transform, impactVector, simplifiedVector, applyOnlyToCollider));
    }

    // Bend Operation
    public void AddOperation(float curvature, float length, float amount, bool direction, bool applyOnlyToCollider = false)
    {
        deformOperations.Add(new BendDeform(transform, curvature, length, amount, direction, applyOnlyToCollider));
    }

    public void ApplyOperations(DeformType type)
    {
        System.Type applyType = EnumToType(type);

        if (applyType != typeof(DeformOperation)) {
            int completedOperations = 0;
            int failedOperations = 0;

            for (int i = 0; i < deformOperations.Count; i++) {
                if (deformOperations[i].GetType() == applyType) {
                    if (deformOperations[i].ApplyOperation()) {
                        completedOperations++;
                    }
                    else {
                        Debug.LogWarning("FAILED OPERATION: type=" + applyType.ToString() + ", number=" + i + ", params=" + deformOperations[i].ParamsToString());
                        failedOperations++;
                    }
                }
            }

            Debug.Log("OPERATIONS DONE: type=" + applyType.ToString() + ", success=" + completedOperations + ", failed=" + failedOperations);
        }
        else {
            Debug.LogWarning("INVALID OPERATION TYPE!");
        }
    }
}
