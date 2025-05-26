using System;
using UnityEngine;
using UnityEngine.UI;

namespace Volorf.GenImage
{
    [ExecuteInEditMode]
    [AddComponentMenu("Volorf/Gen Image")]
    [RequireComponent(typeof(RawImage))]
    public class GenImage : MonoBehaviour
    {
        public Provider provider = Provider.OpenAI;
        public Model model = Model.DallE3;
        
        [Space(10)]
        public Quality quality = Quality.Medium;
        public Size size = Size.Square;
        public FillMode fillMode = FillMode.Stretch;
        
        [Space(10)]
        public bool generateOnStart = true;
        
        [Space(10)]
        [TextArea(3, 9)] public string prompt;
        
        
        Texture2D _texture;
        RawImage _rawImage;
        GenRequestManager _genRequestManager;
        bool _isGenerating;
        
        GameObject _loaderInstance;
        CanvasRenderer _canvasRenderer;
        MaterialPropertyBlock _propertyBlock;

        void Start()
        {
            _genRequestManager = new GenRequestManager();
            _rawImage = GetComponent<RawImage>();
            _canvasRenderer = _rawImage.canvasRenderer;
            
            if (generateOnStart) 
                Generate();
        }
        
        public async void Generate()
        {
            try
            {
                if (_isGenerating) return;
                _isGenerating = true;
                _genRequestManager ??= new GenRequestManager();
                
                _rawImage.material.SetFloat("_PIStrength", 1f);
                _rawImage.material.SetTexture("_MainTex", _rawImage.texture);
                _rawImage.texture = null;
                
                _texture = await _genRequestManager.GenerateTexture2D(
                    provider: provider,
                    model: model,
                    quality: quality,
                    size: size,
                    prompt: prompt);
                
                if (_rawImage == null) 
                    _rawImage = GetComponent<RawImage>();
                
                // Material rawImageMaterial = new (_rawImage.material);
                _rawImage.material.SetFloat("_PIStrength", 0f);
                _rawImage.texture = _texture;
                
                UpdateRawImageUV();
                _isGenerating = false;
                
                #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(_rawImage);
                #endif
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating image: {e.Message}");
                _isGenerating = false;
            }
        }

        void UpdateRawImageUV()
        {
            Vector2 rectSize = _rawImage.rectTransform.rect.size;
            _rawImage.uvRect = Utils.GetFixedUVRect(rectSize, fillMode, Utils.GetSize(size, model));
        }
    }
}

