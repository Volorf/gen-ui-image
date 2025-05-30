using System.IO;
using System.Threading.Tasks;
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
                    
                    SaveTextureAsAsset(genUIImage.Texture, path, fileName).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.LogError($"Failed to save image: {task.Exception}");
                        }
                        else
                        {
                            Debug.Log($"Image saved as {path}/{fileName}");
                        }
                    });
                }
            }
        }
        
        private async Task SaveTextureAsAsset(Texture2D texture, string path, string fileName)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            string fullPath = Path.Combine(path, fileName);
            await File.WriteAllBytesAsync(fullPath, texture.EncodeToPNG());
            AssetDatabase.Refresh();
        }
    }
}

