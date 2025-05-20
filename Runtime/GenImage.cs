using System;
using UnityEngine;
using UnityEngine.UI;

namespace Volorf.GenImage
{
    [AddComponentMenu("Volorf/GenImage")]
    [RequireComponent(typeof(RawImage))]
    public class GenImage : MonoBehaviour
    {
        public Provider provider = Provider.OpenAI;
        public Model model = Model.DallE3;
        public Quality quality = Quality.Auto;
        public Size size = Size.Square;
        public FillMode fillMode = FillMode.Stretch;
        [TextArea(3, 9)] public string prompt;
        
        Texture2D _texture;
        RawImage _rawImage;
        HttpRequestsManager _httpRequestsManager;

        async void Start()
        {
            _httpRequestsManager = new HttpRequestsManager();
            _rawImage = GetComponent<RawImage>();
            Vector2 rectSize = _rawImage.rectTransform.rect.size;
            _rawImage.uvRect = Utils.GetFixedUVRect(rectSize, fillMode, Utils.GetSize(size, model));

            try
            {
                _texture = await _httpRequestsManager.GenerateTexture2D(
                    provider: provider,
                    model: model,
                    quality: quality,
                    size: size,
                    prompt: prompt);
                
                if (_rawImage != null) _rawImage.texture = _texture;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating image: {e.Message}");
            }
        }
    }
}

