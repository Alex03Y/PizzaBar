using UnityEditor;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [PropertyDrawer(typeof(ShowAssetPreviewAttribute))]
    public class ShowAssetPreviewPropertyDrawer : PropertyDrawer
    {
        public override void DrawProperty(SerializedProperty property)
        {
            EditorDrawUtility.DrawPropertyField(property);

            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                if (property.objectReferenceValue != null)
                {
                    var previewTexture = AssetPreview.GetAssetPreview(property.objectReferenceValue);

                    if (previewTexture != null)
                    {
                        var showAssetPreviewAttribute =
                            PropertyUtility.GetAttribute<ShowAssetPreviewAttribute>(property);

                        var width = Mathf.Clamp(showAssetPreviewAttribute.Width, 0, previewTexture.width);
                        var height = Mathf.Clamp(showAssetPreviewAttribute.Height, 0, previewTexture.height);
                        GUILayout.Label(previewTexture, GUILayout.MaxWidth(width), GUILayout.MaxHeight(height));
                    }
                    else
                    {
                        var warning = property.name + " doesn't have an asset preview";

                        EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning,
                                                      PropertyUtility.GetTargetObject(property));
                    }
                }
            }
            else
            {
                var warning = property.name + " doesn't have an asset preview";
                EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, PropertyUtility.GetTargetObject(property));
            }
        }
    }
}