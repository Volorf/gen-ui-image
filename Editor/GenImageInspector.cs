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
            if (GUILayout.Button("Generate Image"))
            {
                Debug.Log("Generating image...");
            }

            // Add a button to save the image
            if (GUILayout.Button("Save Image"))
            {
                Debug.Log("Saving image...");
            }
        }
    }
}

