using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace NaughtyAttributes.Editor
{
    public static class PropertyUtility
    {
        public static T GetAttribute<T>(SerializedProperty property) where T : Attribute
        {
            var attributes = GetAttributes<T>(property);

            return attributes.Length > 0 ? attributes[0] : null;
        }

        public static T[] GetAttributes<T>(SerializedProperty property) where T : Attribute
        {
            var fieldInfo = ReflectionUtility.GetField(GetTargetObject(property), property.name);

            return (T[]) fieldInfo.GetCustomAttributes(typeof(T), true);
        }

        public static Object GetTargetObject(SerializedProperty property)
        {
            return property.serializedObject.targetObject;
        }
    }
}