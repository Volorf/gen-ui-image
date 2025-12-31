using System;
using UnityEngine;

namespace Volorf.GenUIImage
{
    [ExecuteAlways]
    [AddComponentMenu("Volorf/Gen UI Texture Updater")]
    [RequireComponent(typeof(GenUIImage))]
    public class GenUITextureUpdater : MonoBehaviour
    {
        public Texture2D texture;

        void Start()
        {
            Debug.Log($"Is Texture readable: {texture.isReadable}");
        }
        
        public static byte[] Texture2DToPng(Texture2D tex)
        {
            if (tex == null)
            {
                Debug.LogWarning("Texture is null");
                return null;
            }

            if (!tex.isReadable)
            {
                Debug.LogWarning("Texture is not readable. Tick the Read/Write box in the Texture Settings.");
                return null;
            }
            
            return tex.EncodeToPNG();
        }

    }
}


