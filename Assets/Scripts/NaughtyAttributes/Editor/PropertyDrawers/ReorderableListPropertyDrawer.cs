using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [PropertyDrawer(typeof(ReorderableListAttribute))]
    public class ReorderableListPropertyDrawer : PropertyDrawer
    {
        private readonly Dictionary<string, ReorderableList> reorderableListsByPropertyName =
            new Dictionary<string, ReorderableList>();

        private string GetPropertyKeyName(SerializedProperty property)
        {
            return property.serializedObject.targetObject.GetInstanceID() + "/" + property.name;
        }

        public override void DrawProperty(SerializedProperty property)
        {
            EditorDrawUtility.DrawHeader(property);

            if (property.isArray)
            {
                var key = GetPropertyKeyName(property);

                if (!reorderableListsByPropertyName.ContainsKey(key))
                {
                    var reorderableList =
                        new ReorderableList(property.serializedObject, property, true, true, true, true)
                        {
                            drawHeaderCallback = rect =>
                            {
                                EditorGUI.LabelField(
                                    rect, string.Format("{0}: {1}", property.displayName, property.arraySize),
                                    EditorStyles.label);
                            },
                            drawElementCallback = (rect, index, isActive, isFocused) =>
                            {
                                var element = property.GetArrayElementAtIndex(index);
                                rect.y += 2f;

                                EditorGUI.PropertyField(
                                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
                            }
                        };

                    reorderableListsByPropertyName[key] = reorderableList;
                }

                reorderableListsByPropertyName[key].DoLayoutList();
            }
            else
            {
                var warning = typeof(ReorderableListAttribute).Name + " can be used only on arrays or lists";
                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, PropertyUtility.GetTargetObject(property));
                EditorDrawUtility.DrawPropertyField(property);
            }
        }

        public override void ClearCache()
        {
            reorderableListsByPropertyName.Clear();
        }
    }
}