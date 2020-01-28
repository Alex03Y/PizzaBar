using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [NativePropertyDrawer(typeof(ShowNativePropertyAttribute))]
    public class ShowNativePropertyNativePropertyDrawer : NativePropertyDrawer
    {
        public override void DrawNativeProperty(Object target, PropertyInfo property)
        {
            var value = property.GetValue(target, null);

            if (value == null)
            {
                var warning = string.Format("{0} doesn't support {1} types",
                                            typeof(ShowNativePropertyNativePropertyDrawer).Name, "Reference");

                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, target);
            }
            else if (!EditorDrawUtility.DrawLayoutField(value, property.Name))
            {
                var warning = string.Format("{0} doesn't support {1} types",
                                            typeof(ShowNativePropertyNativePropertyDrawer).Name,
                                            property.PropertyType.Name);

                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, target);
            }
        }
    }
}