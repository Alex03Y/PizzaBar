using System.Collections.Generic;
using UnityEditor;

namespace NaughtyAttributes.Editor
{
    [PropertyDrawCondition(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawCondition : PropertyDrawCondition
    {
        public override bool CanDrawProperty(SerializedProperty property)
        {
            var showIfAttribute = PropertyUtility.GetAttribute<ShowIfAttribute>(property);
            var target = PropertyUtility.GetTargetObject(property);
            var conditionValues = new List<bool>();

            foreach (var condition in showIfAttribute.Conditions)
            {
                var conditionField = ReflectionUtility.GetField(target, condition);

                if (conditionField != null &&
                    conditionField.FieldType == typeof(bool))
                    conditionValues.Add((bool) conditionField.GetValue(target));

                var conditionMethod = ReflectionUtility.GetMethod(target, condition);

                if (conditionMethod != null &&
                    conditionMethod.ReturnType == typeof(bool) &&
                    conditionMethod.GetParameters().Length == 0)
                    conditionValues.Add((bool) conditionMethod.Invoke(target, null));
            }

            if (conditionValues.Count > 0)
            {
                bool draw;

                if (showIfAttribute.ConditionOperator == ConditionOperator.And)
                {
                    draw = true;
                    foreach (var value in conditionValues) draw = draw && value;
                }
                else
                {
                    draw = false;
                    foreach (var value in conditionValues) draw = draw || value;
                }

                if (showIfAttribute.Reversed) draw = !draw;

                return draw;
            }

            var warning = showIfAttribute.GetType().Name +
                          " needs a valid boolean condition field or method name to work";

            EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, target);

            return true;
        }
    }
}