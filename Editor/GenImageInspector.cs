using UnityEditor;
using UnityEngine;

namespace Volorf.GenImage
{
    [CustomEditor(typeof(GenImage))]
    public class GenImageInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GenImage genImage = (GenImage)target;
            DrawDefaultInspector();
            EditorGUILayout.Space();
            // Add a button to generate an image
            if (GUILayout.Button("Generate"))
            {
                Debug.Log("Generating image...");
                genImage.Generate();
            }

            // TODO: Implement Assets saving
            if (genImage.Texture != null && !genImage.IsGenerating)
            {
                if (GUILayout.Button("Save As Asset"))
                {
                    Debug.Log("Saving image as asset...");
                }
            }
        }
    }
}

