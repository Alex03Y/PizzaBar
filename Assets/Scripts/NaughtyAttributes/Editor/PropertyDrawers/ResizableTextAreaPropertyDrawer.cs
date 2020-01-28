using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [PropertyDrawer(typeof(ResizableTextAreaAttribute))]
    public class ResizableTextAreaPropertyDrawer : PropertyDrawer
    {
        public override void DrawProperty(SerializedProperty property)
        {
            EditorDrawUtility.DrawHeader(property);

            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUILayout.LabelField(property.displayName);
                EditorGUI.BeginChangeCheck();

                var textAreaValue = EditorGUILayout.TextArea(property.stringValue,
                                                             GUILayout.MinHeight(
                                                                 EditorGUIUtility.singleLineHeight * 3f));

                if (EditorGUI.EndChangeCheck()) property.stringValue = textAreaValue;
            }
            else
            {
                var warning = PropertyUtility.GetAttribute<ResizableTextAreaAttribute>(property).GetType().Name +
                              " can only be used on string fields";

                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, PropertyUtility.GetTargetObject(property));
                EditorDrawUtility.DrawPropertyField(property);
            }
        }
    }
}