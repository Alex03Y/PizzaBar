using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NaughtyAttributes.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class InspectorEditor : UnityEditor.Editor
    {
        private IEnumerable<FieldInfo> fields;
        private HashSet<FieldInfo> groupedFields;
        private Dictionary<string, List<FieldInfo>> groupedFieldsByGroupName;
        private IEnumerable<MethodInfo> methods;
        private IEnumerable<PropertyInfo> nativeProperties;
        private IEnumerable<FieldInfo> nonSerializedFields;
        private SerializedProperty script;
        private Dictionary<string, SerializedProperty> serializedPropertiesByFieldName;
        private bool useDefaultInspector;

        private void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");

            // Cache serialized fields
            fields = ReflectionUtility.GetAllFields(target, f => serializedObject.FindProperty(f.Name) != null);

            // If there are no NaughtyAttributes use default inspector
            if (fields.All(f => f.GetCustomAttributes(typeof(NaughtyAttribute), true).Length == 0))
            {
                useDefaultInspector = true;
            }
            else
            {
                useDefaultInspector = false;

                // Cache grouped fields
                groupedFields =
                    new HashSet<FieldInfo>(
                        fields.Where(f => f.GetCustomAttributes(typeof(GroupAttribute), true).Length > 0));

                // Cache grouped fields by group name
                groupedFieldsByGroupName = new Dictionary<string, List<FieldInfo>>();

                foreach (var groupedField in groupedFields)
                {
                    var groupName =
                        (groupedField.GetCustomAttributes(typeof(GroupAttribute), true)[0] as GroupAttribute).Name;

                    if (groupedFieldsByGroupName.ContainsKey(groupName))
                        groupedFieldsByGroupName[groupName].Add(groupedField);
                    else
                        groupedFieldsByGroupName[groupName] = new List<FieldInfo>
                        {
                            groupedField
                        };
                }

                // Cache serialized properties by field name
                serializedPropertiesByFieldName = new Dictionary<string, SerializedProperty>();

                foreach (var field in fields)
                    serializedPropertiesByFieldName[field.Name] = serializedObject.FindProperty(field.Name);
            }

            // Cache non-serialized fields
            nonSerializedFields = ReflectionUtility.GetAllFields(
                target,
                f => f.GetCustomAttributes(typeof(DrawerAttribute), true).Length > 0 &&
                     serializedObject.FindProperty(f.Name) == null);

            // Cache the native properties
            nativeProperties = ReflectionUtility.GetAllProperties(
                target, p => p.GetCustomAttributes(typeof(DrawerAttribute), true).Length > 0);

            // Cache methods with DrawerAttribute
            methods = ReflectionUtility.GetAllMethods(
                target, m => m.GetCustomAttributes(typeof(DrawerAttribute), true).Length > 0);
        }

        private void OnDisable()
        {
            PropertyDrawerDatabase.ClearCache();
        }

        public override void OnInspectorGUI()
        {
            if (useDefaultInspector)
            {
                DrawDefaultInspector();
            }
            else
            {
                serializedObject.Update();

                if (script != null)
                {
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(script);
                    GUI.enabled = true;
                }

                // Draw fields
                var drawnGroups = new HashSet<string>();

                foreach (var field in fields)
                    if (groupedFields.Contains(field))
                    {
                        // Draw grouped fields
                        var groupName = (field.GetCustomAttributes(typeof(GroupAttribute), true)[0] as GroupAttribute)
                           .Name;

                        if (!drawnGroups.Contains(groupName))
                        {
                            drawnGroups.Add(groupName);
                            var grouper = GetPropertyGrouperForField(field);

                            if (grouper != null)
                            {
                                grouper.BeginGroup(groupName);
                                ValidateAndDrawFields(groupedFieldsByGroupName[groupName]);
                                grouper.EndGroup();
                            }
                            else
                            {
                                ValidateAndDrawFields(groupedFieldsByGroupName[groupName]);
                            }
                        }
                    }
                    else
                    {
                        // Draw non-grouped field
                        ValidateAndDrawField(field);
                    }

                serializedObject.ApplyModifiedProperties();
            }

            // Draw non-serialized fields
            foreach (var field in nonSerializedFields)
            {
                var drawerAttribute = (DrawerAttribute) field.GetCustomAttributes(typeof(DrawerAttribute), true)[0];
                var drawer = FieldDrawerDatabase.GetDrawerForAttribute(drawerAttribute.GetType());
                if (drawer != null) drawer.DrawField(target, field);
            }

            // Draw native properties
            foreach (var property in nativeProperties)
            {
                var drawerAttribute = (DrawerAttribute) property.GetCustomAttributes(typeof(DrawerAttribute), true)[0];
                var drawer = NativePropertyDrawerDatabase.GetDrawerForAttribute(drawerAttribute.GetType());
                if (drawer != null) drawer.DrawNativeProperty(target, property);
            }

            // Draw methods
            foreach (var method in methods)
            {
                var drawerAttribute = (DrawerAttribute) method.GetCustomAttributes(typeof(DrawerAttribute), true)[0];
                var methodDrawer = MethodDrawerDatabase.GetDrawerForAttribute(drawerAttribute.GetType());
                if (methodDrawer != null) methodDrawer.DrawMethod(target, method);
            }
        }

        private void ValidateAndDrawFields(IEnumerable<FieldInfo> fields)
        {
            foreach (var field in fields) ValidateAndDrawField(field);
        }

        private void ValidateAndDrawField(FieldInfo field)
        {
            if (!ShouldDrawField(field)) return;

            ValidateField(field);
            ApplyFieldMeta(field);
            DrawField(field);
        }

        private void ValidateField(FieldInfo field)
        {
            var validatorAttributes =
                (ValidatorAttribute[]) field.GetCustomAttributes(typeof(ValidatorAttribute), true);

            foreach (var attribute in validatorAttributes)
            {
                var validator = PropertyValidatorDatabase.GetValidatorForAttribute(attribute.GetType());
                if (validator != null) validator.ValidateProperty(serializedPropertiesByFieldName[field.Name]);
            }
        }

        private bool ShouldDrawField(FieldInfo field)
        {
            // Check if the field has draw conditions
            var drawCondition = GetPropertyDrawConditionForField(field);

            if (drawCondition != null)
            {
                var canDrawProperty = drawCondition.CanDrawProperty(serializedPropertiesByFieldName[field.Name]);

                if (!canDrawProperty) return false;
            }

            // Check if the field has HideInInspectorAttribute
            var hideInInspectorAttributes =
                (HideInInspector[]) field.GetCustomAttributes(typeof(HideInInspector), true);

            if (hideInInspectorAttributes.Length > 0) return false;

            return true;
        }

        private void DrawField(FieldInfo field)
        {
            EditorGUI.BeginChangeCheck();
            var drawer = GetPropertyDrawerForField(field);

            if (drawer != null)
                drawer.DrawProperty(serializedPropertiesByFieldName[field.Name]);
            else
                EditorDrawUtility.DrawPropertyField(serializedPropertiesByFieldName[field.Name]);

            if (EditorGUI.EndChangeCheck())
            {
                var onValueChangedAttributes =
                    (OnValueChangedAttribute[]) field.GetCustomAttributes(typeof(OnValueChangedAttribute), true);

                foreach (var onValueChangedAttribute in onValueChangedAttributes)
                {
                    var meta = PropertyMetaDatabase.GetMetaForAttribute(onValueChangedAttribute.GetType());

                    if (meta != null)
                        meta.ApplyPropertyMeta(serializedPropertiesByFieldName[field.Name], onValueChangedAttribute);
                }
            }
        }

        private void ApplyFieldMeta(FieldInfo field)
        {
            // Apply custom meta attributes
            var metaAttributes = field
                                .GetCustomAttributes(typeof(MetaAttribute), true)
                                .Where(attr => attr.GetType() != typeof(OnValueChangedAttribute))
                                .Select(obj => obj as MetaAttribute)
                                .ToArray();

            Array.Sort(metaAttributes, (x, y) => { return x.Order - y.Order; });

            foreach (var metaAttribute in metaAttributes)
            {
                var meta = PropertyMetaDatabase.GetMetaForAttribute(metaAttribute.GetType());
                if (meta != null) meta.ApplyPropertyMeta(serializedPropertiesByFieldName[field.Name], metaAttribute);
            }
        }

        private PropertyDrawer GetPropertyDrawerForField(FieldInfo field)
        {
            var drawerAttributes = (DrawerAttribute[]) field.GetCustomAttributes(typeof(DrawerAttribute), true);

            if (drawerAttributes.Length > 0)
            {
                var drawer = PropertyDrawerDatabase.GetDrawerForAttribute(drawerAttributes[0].GetType());

                return drawer;
            }

            return null;
        }

        private PropertyGrouper GetPropertyGrouperForField(FieldInfo field)
        {
            var groupAttributes = (GroupAttribute[]) field.GetCustomAttributes(typeof(GroupAttribute), true);

            if (groupAttributes.Length > 0)
            {
                var grouper = PropertyGrouperDatabase.GetGrouperForAttribute(groupAttributes[0].GetType());

                return grouper;
            }

            return null;
        }

        private PropertyDrawCondition GetPropertyDrawConditionForField(FieldInfo field)
        {
            var drawConditionAttributes =
                (DrawConditionAttribute[]) field.GetCustomAttributes(typeof(DrawConditionAttribute), true);

            if (drawConditionAttributes.Length > 0)
            {
                var drawCondition =
                    PropertyDrawConditionDatabase.GetDrawConditionForAttribute(drawConditionAttributes[0].GetType());

                return drawCondition;
            }

            return null;
        }
    }
}