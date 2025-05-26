using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Volorf.GenImage
{
    public class GenImageUtility
    {
        [MenuItem("GameObject/UI/Gen Image")]
        public static void AddGenImage()
        {
            GameObject genImageObject = new ("Gen Image");
            genImageObject.AddComponent<RawImage>();
            genImageObject.AddComponent<GenImage>();
            
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
            
            Undo.RegisterCreatedObjectUndo(genImageObject, "Create a Gen Image");
            
            Selection.activeGameObject = genImageObject;
        }
    }
}