using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Volorf.GenImage
{
    [ExecuteAlways]
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

        void Start()
        {
            _genRequestManager = new GenRequestManager();
            _rawImage = GetComponent<RawImage>();
            
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
                
                _texture = await _genRequestManager.GenerateTexture2D(
                    provider: provider,
                    model: model,
                    quality: quality,
                    size: size,
                    prompt: prompt);
                
                if (_rawImage == null) 
                    _rawImage = GetComponent<RawImage>();
                
                Material rawImageMaterial = Instantiate(_rawImage.material);
                rawImageMaterial.name = "Gen Image Material";
                rawImageMaterial.SetTexture("_MainTex", _texture);
                _rawImage.material = rawImageMaterial;
                
                UpdateRawImageUV();
                _isGenerating = false;
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

