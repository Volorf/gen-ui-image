using System.IO;
using UnityEditor;
using UnityEngine;

namespace Volorf.GenUIImage
{
    [CustomEditor(typeof(GenUIImage))]
    public class GenImageInspector : Editor
    {
        bool _canSave;
        
        public override void OnInspectorGUI()
        {
            GenUIImage genUIImage = (GenUIImage)target;
            DrawDefaultInspector();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Generate"))
            {
                genUIImage.Generate();
                _canSave = true;
            }
            
            if (genUIImage.Texture != null && !genUIImage.IsGenerating && _canSave)
            {
                if (GUILayout.Button("Save As Asset"))
                {
                    if (!_canSave) return;
                    _canSave = false;
                    
                    string path = "Assets/Volorf/Gen UI Image/Generated Images";
                    string fileName = $"{Utils.GetModelName(genUIImage.model)}_{Utils.GetQualityName(genUIImage.quality, genUIImage.model)}_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
                    
                    if (!Directory.Exists(path))
                    {
                       Directory.CreateDirectory(path); 
                    }
                    
                    File.WriteAllBytesAsync(path + "/" + fileName, genUIImage.Texture.EncodeToPNG());
                    AssetDatabase.Refresh();
                    
                    Debug.Log($"Image saved as {path}/{fileName}");
                }
            }
        }
    }
}

