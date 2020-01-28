using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [FieldDrawer(typeof(ShowNonSerializedFieldAttribute))]
    public class ShowNonSerializedFieldFieldDrawer : FieldDrawer
    {
        public override void DrawField(Object target, FieldInfo field)
        {
            var value = field.GetValue(target);

            if (value == null)
            {
                var warning = string.Format("{0} doesn't support {1} types",
                                            typeof(ShowNonSerializedFieldFieldDrawer).Name, "Reference");

                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, target);
            }
            else if (!EditorDrawUtility.DrawLayoutField(value, field.Name))
            {
                var warning = string.Format("{0} doesn't support {1} types",
                                            typeof(ShowNonSerializedFieldFieldDrawer).Name, field.FieldType.Name);

                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, target);
            }
        }
    }
}