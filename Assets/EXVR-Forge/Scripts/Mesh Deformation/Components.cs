using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Components
{
    public static Component Copy(GameObject target, Component original)
    {
        System.Type type = original.GetType();
        System.Reflection.FieldInfo[] fields = type.GetFields();

        Component copy = target.AddComponent(type);
        foreach (System.Reflection.FieldInfo field in fields)
            field.SetValue(copy, field.GetValue(original));

        return copy;
    }

    public static Component[] Copy(GameObject target, params Component[] originals)
    {
        Component[] copies = new Component[originals.Length];
        for (int i = 0; i < originals.Length; i++)
            copies[i] = Copy(target, originals[i]);

        return copies;
    }
}
