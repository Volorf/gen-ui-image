using System.IO;
using UnityEditor;
using UnityEngine;

namespace Volorf.GenImage
{
    [CustomEditor(typeof(GenImage))]
    public class GenImageInspector : Editor
    {
        bool _canSave;
        
        public override void OnInspectorGUI()
        {
            GenImage genImage = (GenImage)target;
            DrawDefaultInspector();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Generate"))
            {
                genImage.Generate();
                _canSave = true;
            }
            
            if (genImage.Texture != null && !genImage.IsGenerating && _canSave)
            {
                if (GUILayout.Button("Save As Asset"))
                {
                    if (!_canSave) return;
                    _canSave = false; // Prevent multiple saves
                    
                    string path = "Assets/Volorf/Gen Image/Generated Images";
                    string fileName = $"{Utils.GetModelName(genImage.model)}_{Utils.GetQualityName(genImage.quality, genImage.model)}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
                    
                    if (!Directory.Exists(path))
                    {
                       Directory.CreateDirectory(path); 
                    }
                    
                    File.WriteAllBytesAsync(path + "/" + fileName, genImage.Texture.EncodeToPNG());
                    AssetDatabase.Refresh();
                    
                    Debug.Log($"Image saved as {path}/{fileName}");
                }
            }
        }
    }
}

