#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IndentAttribute))]
public class IndentAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return
            EditorGUIUtility.singleLineHeight +
            EditorGUIUtility.standardVerticalSpacing +
            EditorGUI.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        IndentAttribute indentAttribute = (IndentAttribute)attribute;
        if (!string.IsNullOrEmpty(indentAttribute.label))
        {
            Rect labelRect = position;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PrefixLabel(labelRect, new GUIContent(indentAttribute.label));

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            position.height -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
        position.x += 20.0f * indentAttribute.indents;
        EditorGUI.PropertyField(position, property, label, true);
    }
}
#endif