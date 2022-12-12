using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(HideInSubClassAttribute))]
public class HideInSubClassAttributeDrawer : PropertyDrawer
{

    private bool ShouldShow(SerializedProperty property)
    { 
        Type type = property.serializedObject.targetObject.GetType();
        FieldInfo field = type.GetField(property.name);
        Type declaringType = field.DeclaringType;
        return type == declaringType;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Somehow EditorGUI doesn't show the array
        if (ShouldShow(property))
            EditorGUI.PropertyField(position, property); //fun fact: base.OnGUI doesn't work! Check for yourself!
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
            return base.GetPropertyHeight(property, label);
        else
            return 0;
    }
}

public class HideInSubClassAttribute : PropertyAttribute { }

[System.Serializable]
public class FloatArrayWrapper
{
    public float[] array;
}

[System.Serializable]
public class TerrainArrayWrapper
{
    public Terrain[] array;
}