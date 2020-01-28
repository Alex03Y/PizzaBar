using UnityEditor;

namespace NaughtyAttributes.Editor
{
    [PropertyValidator(typeof(MinValueAttribute))]
    public class MinValuePropertyValidator : PropertyValidator
    {
        public override void ValidateProperty(SerializedProperty property)
        {
            var minValueAttribute = PropertyUtility.GetAttribute<MinValueAttribute>(property);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (property.intValue < minValueAttribute.MinValue)
                    property.intValue = (int) minValueAttribute.MinValue;
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                if (property.floatValue < minValueAttribute.MinValue) property.floatValue = minValueAttribute.MinValue;
            }
            else
            {
                var warning = minValueAttribute.GetType().Name + " can be used only on int or float fields";
                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, PropertyUtility.GetTargetObject(property));
            }
        }
    }
}