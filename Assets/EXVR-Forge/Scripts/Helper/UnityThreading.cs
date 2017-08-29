using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace UnityThreading
{
    public class ts_Transform
    {
        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }
        public Vector3 lossyScale { get; private set; }

        public ts_Transform(Transform copy) { CopyValues(copy); }
        public ts_Transform(Vector3 position, Quaternion rotation, Vector3 lossyScale) { CopyValues(position, rotation, lossyScale); }

        public void CopyValues(Vector3 position, Quaternion rotation, Vector3 lossyScale)
        {
            this.position = position;
            this.rotation = rotation;
            this.lossyScale = lossyScale;
        }

        public void CopyValues(Transform copy)
        {
            if (copy)
            {
                position = copy.position;
                rotation = copy.rotation;
                lossyScale = copy.lossyScale;
            }
        }

        public Vector3 TransformPoint(Vector3 pt) { return position + rotation * Vector3.Scale(lossyScale, pt); }
        public Vector3 TransformPoint(float x, float y, float z) { return TransformPoint(new Vector3(x, y, z)); }
        public Vector3 TransformDirection(Vector3 dir) { return TransformPoint(dir) - position; }
        public Vector3 TransformDirection(float x, float y, float z) { return TransformPoint(new Vector3(x, y, z)) - position; }
        public Vector3 InverseTransformPoint(Vector3 pt) { return Vector3.Scale(new Vector3(1 / lossyScale.x, 1 / lossyScale.y, 1 / lossyScale.z), Quaternion.Inverse(rotation) * (pt - position)); }
        public Vector3 InverseTransformPoint(float x, float y, float z) { return InverseTransformPoint(new Vector3(x, y, z)); }
    }

    public static class ThreadTools
    {
        public static void WaitForThreads(ref List<Thread> threads)
        {
            foreach (Thread t in threads)
                t.Join();
            threads.Clear();
        }
    }
}
