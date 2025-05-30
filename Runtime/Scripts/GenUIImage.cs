using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Volorf.GenUIImage
{
    [ExecuteAlways]
    [AddComponentMenu("Volorf/Gen UI Image")]
    [RequireComponent(typeof(RawImage))]
    public class GenUIImage : MonoBehaviour
    {
        public Provider provider = Provider.OpenAI;
        public Model model = Model.DallE3;
        
        [Space(10)]
        public Quality quality = Quality.Medium;
        public Size size = Size.Square;
        public FillMode fillMode = FillMode.Stretch;
        
        [Space(10)]
        public bool generateOnStart;
        
        [Space(10)]
        [TextArea(3, 9)] public string prompt = "A cute red panda eating apples.";
        
        public Texture2D Texture { get; private set; }
        public bool IsGenerating { get; private set; }
        
        GenRequestManager _genRequestManager;
        Material _genMaterial;
        RawImage _rawImage;
        
        void Start()
        {
            _genMaterial = new Material(Resources.Load<Material>("GenImageMaterial"));
            _genRequestManager = new GenRequestManager();
            _rawImage = GetComponent<RawImage>();
            _rawImage.material = _genMaterial;
            
            if (generateOnStart) 
                Generate();
        }
        
        public async void Generate(string apiKey = "")
        {
            try
            {
                if (IsGenerating) return;
                IsGenerating = true;
                _genRequestManager ??= new GenRequestManager();
                _rawImage.material.SetFloat("_PIStrength", 1f);
                
                if (_rawImage.texture != null)
                {
                    _rawImage.material.SetTexture("_MainTex", _rawImage.texture);
                }
                
                _rawImage.texture = null;
                
                prompt = string.IsNullOrWhiteSpace(prompt) ? "A cute red panda eating apples." : prompt;
                
                Texture = await _genRequestManager.GenerateTexture2D(
                    provider: provider,
                    model: model,
                    quality: quality,
                    size: size,
                    prompt: prompt,
                    apiKey: apiKey);

                _rawImage.material.SetFloat("_PIStrength", 0f);
                _rawImage.texture = Texture;
                
                UpdateRawImageUV();
                IsGenerating = false;
                
                #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(_rawImage);
                #endif
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating image: {e.Message}");
                _rawImage.material.SetFloat("_PIStrength", 0f);
                IsGenerating = false;
            }
        }

        void UpdateRawImageUV()
        {
            Vector2 rectSize = _rawImage.rectTransform.rect.size;
            _rawImage.uvRect = Utils.GetFixedUVRect(rectSize, fillMode, Utils.GetSize(size, model));
        }
    }
}

