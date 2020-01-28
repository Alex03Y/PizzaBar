using UnityEditor;

namespace NaughtyAttributes.Editor
{
    [PropertyValidator(typeof(RequiredAttribute))]
    public class RequiredPropertyValidator : PropertyValidator
    {
        public override void ValidateProperty(SerializedProperty property)
        {
            var requiredAttribute = PropertyUtility.GetAttribute<RequiredAttribute>(property);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue == null)
                {
                    var errorMessage = property.name + " is required";
                    if (!string.IsNullOrEmpty(requiredAttribute.Message)) errorMessage = requiredAttribute.Message;

                    EditorDrawUtility.DrawHelpBox(errorMessage, MessageType.Error,
                                                  PropertyUtility.GetTargetObject(property));
                }
            }
            else
            {
                var warning = requiredAttribute.GetType().Name + " works only on reference types";
                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, PropertyUtility.GetTargetObject(property));
            }
        }
    }
}