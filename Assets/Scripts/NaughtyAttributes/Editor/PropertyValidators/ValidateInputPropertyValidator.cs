using UnityEditor;

namespace NaughtyAttributes.Editor
{
    [PropertyValidator(typeof(ValidateInputAttribute))]
    public class ValidateInputPropertyValidator : PropertyValidator
    {
        public override void ValidateProperty(SerializedProperty property)
        {
            var validateInputAttribute = PropertyUtility.GetAttribute<ValidateInputAttribute>(property);
            var target = PropertyUtility.GetTargetObject(property);
            var validationCallback = ReflectionUtility.GetMethod(target, validateInputAttribute.CallbackName);

            if (validationCallback != null &&
                validationCallback.ReturnType == typeof(bool) &&
                validationCallback.GetParameters().Length == 1)
            {
                var fieldInfo = ReflectionUtility.GetField(target, property.name);
                var fieldType = fieldInfo.FieldType;
                var parameterType = validationCallback.GetParameters()[0].ParameterType;

                if (fieldType == parameterType)
                {
                    if (!(bool) validationCallback.Invoke(target, new[] {fieldInfo.GetValue(target)}))
                    {
                        if (string.IsNullOrEmpty(validateInputAttribute.Message))
                            EditorDrawUtility.DrawHelpBox(property.name + " is not valid", MessageType.Error, target);
                        else
                            EditorDrawUtility.DrawHelpBox(validateInputAttribute.Message, MessageType.Error, target);
                    }
                }
                else
                {
                    var warning = "The field type is not the same as the callback's parameter type";
                    EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, target);
                }
            }
            else
            {
                var warning =
                    validateInputAttribute.GetType().Name +
                    " needs a callback with boolean return type and a single parameter of the same type as the field";

                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, target);
            }
        }
    }
}