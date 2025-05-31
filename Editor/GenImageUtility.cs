using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Volorf.GenUIImage
{
    public class GenImageUtility
    {
        [MenuItem("GameObject/UI/Gen UI Image")]
        public static void AddGenImage()
        {
            GameObject genImageObject = new ("Gen UI Image");
            genImageObject.AddComponent<RawImage>();
            genImageObject.AddComponent<GenUIImage>();
            
            if (Selection.activeGameObject == null)
            {
                Canvas canvas = GameObject.FindObjectOfType<Canvas>();
                
                if (canvas != null)
                {
                    genImageObject.transform.SetParent(canvas.transform);
                    genImageObject.transform.localPosition = Vector3.zero;
                    genImageObject.transform.localScale = Vector3.one;
                }
            }
            else
            {
                genImageObject.transform.SetParent(Selection.activeGameObject.transform);
                genImageObject.transform.localPosition = Vector3.zero;
                genImageObject.transform.localScale = Vector3.one;
            }
            
            Undo.RegisterCreatedObjectUndo(genImageObject, "Create a Gen UI Image");
            
            Selection.activeGameObject = genImageObject;
        }
    }
}