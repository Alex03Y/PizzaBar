using UnityEditor;

namespace NaughtyAttributes.Editor
{
    [PropertyDrawer(typeof(SliderAttribute))]
    public class SliderPropertyDrawer : PropertyDrawer
    {
        public override void DrawProperty(SerializedProperty property)
        {
            EditorDrawUtility.DrawHeader(property);
            var sliderAttribute = PropertyUtility.GetAttribute<SliderAttribute>(property);

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                EditorGUILayout.IntSlider(property, (int) sliderAttribute.MinValue, (int) sliderAttribute.MaxValue);
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                EditorGUILayout.Slider(property, sliderAttribute.MinValue, sliderAttribute.MaxValue);
            }
            else
            {
                var warning = sliderAttribute.GetType().Name + " can be used only on int or float fields";
                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, PropertyUtility.GetTargetObject(property));
                EditorDrawUtility.DrawPropertyField(property);
            }
        }
    }
}