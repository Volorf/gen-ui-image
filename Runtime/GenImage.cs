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
            print("size: " + rectSize);
            bool isLandscape = rectSize.x > rectSize.y;

            if (fillMode == FillMode.Stretch)
            {
                if (isLandscape)
                {
                    float ratio = rectSize.x / rectSize.y;
                    float yOffset = 1f / ratio;
                    Vector2 newUV = new Vector2(1f, yOffset);
                    print("newUV: " + newUV);
                    _rawImage.uvRect = new Rect(0f, (1f - newUV.y) / 2f, newUV.x, newUV.y);
                }
                else
                {
                    float ratio = rectSize.y / rectSize.x;
                    float xOffset = 1f / ratio;
                    Vector2 newUV = new Vector2(xOffset, 1f);
                    print("newUV: " + newUV);
                    _rawImage.uvRect = new Rect((1f - xOffset) / 2f, 0f, newUV.x, newUV.y);
                }
            }

            if (fillMode == FillMode.PreserveAspect)
            {
                if (isLandscape)
                {
                    float ratio = rectSize.x / rectSize.y;
                    Vector2 newUV = new Vector2(1f * ratio, 1f);
                    _rawImage.uvRect = new Rect((1f - newUV.x) / 2f, 0f, newUV.x, newUV.y);
                }
                else
                {
                    float ratio = rectSize.y / rectSize.x;
                    Vector2 newUV = new Vector2(1f, 1f * ratio);
                    _rawImage.uvRect = new Rect(0f, (1f - newUV.y) / 2f, newUV.x, newUV.y);
                }
            }

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

