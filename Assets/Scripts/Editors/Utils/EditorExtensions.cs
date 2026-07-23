#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Reflection;


public static class EditorUtils
{
    /// <summary>
    /// Returns attributes of type <typeparamref name="TAttribute"/> on <paramref name="serializedProperty"/>.
    /// </summary>
    public static TAttribute[] GetAttributes<TAttribute>(this SerializedProperty serializedProperty, bool inherit)
        where TAttribute : Attribute
    {
        if (serializedProperty == null)
        {
            return null;
        }

        var iterateObjectType = serializedProperty.serializedObject.targetObject.GetType();

        if (iterateObjectType == null)
        {
            return null;
        }

        FieldInfo iterateFieldInfo = null;

        if (serializedProperty.propertyPath.Contains("ff") && serializedProperty.propertyPath.Contains('['))
        {
            UnityEngine.Debug.DebugBreak();
        }

        int itemsToSkip = 0;
        foreach (var pathSegment in serializedProperty.propertyPath.Split('.'))
        {
            if (0 < itemsToSkip)
            {
                --itemsToSkip;
                continue;
            }

            if (iterateObjectType.IsArray)
            {
                iterateObjectType = iterateObjectType.GetElementType();
                itemsToSkip = 1;
                continue;
            }

            iterateFieldInfo = iterateObjectType.GetField(pathSegment, (BindingFlags)(-1));
            if (iterateFieldInfo != null)
            {
                iterateObjectType = iterateFieldInfo.FieldType;
            }
            else
            {
                return null;
            }
        }

        if (iterateFieldInfo == null)
            return null;

        return (TAttribute[])iterateFieldInfo.GetCustomAttributes<TAttribute>(inherit);
    }
}
#endif