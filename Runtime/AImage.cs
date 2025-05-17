using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Volorf.AImage
{
    public enum Model
    {
        DallE3,
        DallE2,
        GptImage1
    }
    
    public enum Provider
    {
        OpenAI
    }
    
    public enum FillMode
    {
        Stretch,
        PreserveAspect,
        None
    }
    
    public enum Quality
    {
        Low,
        Medium,
        High,
        Auto
    }
    
    public enum Size
    {
        Square,
        Landscape,
        Portrait
    }
    
    [AddComponentMenu("Volorf/AImage")]
    [RequireComponent(typeof(RawImage))]
    public class AImage : MonoBehaviour
    {
        string sk = "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";
        
        [Header("Generation")]
        [TextArea(3, 9)] public string prompt;
        public Model model = Model.DallE3;
        public Quality quality = Quality.Auto;
        public Provider provider = Provider.OpenAI;
        // public int width = 1024;
        // public int height = 1024;
        public Vector2Int size = new Vector2Int(1024, 1024);
        public FillMode fillMode = FillMode.Stretch;
    
        private static readonly HttpClient _http = new HttpClient();

        Texture2D _texture;
        RawImage _rawImage;
        
        static string GetModelName(Model model)
        {
            return model switch
            {
                Model.DallE3 => "dall-e-3",
                Model.DallE2 => "dall-e-2",
                Model.GptImage1 => "gpt-image-1",
                _ => "dall-e-3"
            };
        }
        
        static string GetQualityName(Quality quality)
        {
            return quality switch
            {
                Quality.Low => "low",
                Quality.Medium => "medium",
                Quality.High => "high",
                Quality.Auto => "auto",
                _ => "auto"
            };
        }
        
        static string GetSizeName(Size size)
        {
            return size switch
            {
                Size.Square => "1024x1024",
                Size.Landscape => "1536x1024",
                Size.Portrait => "1024x1536",
                _ => "1024x1024"
            };
        }
        
        async void Start()
        {
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
                _texture = await GenerateTexture2D(prompt, sk, GetModelName(model), size, GetQualityName(quality));
                if (_rawImage != null) _rawImage.texture = _texture;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating image: {e.Message}");
            }
        }

        public static async Task<Texture2D> GenerateTexture2D(string desc, string apiKey, string model, Vector2Int size, string quality)
        {
            const string endPoint = "https://api.openai.com/v1/images/generations";
            
            GenRequest reqBody = new GenRequest
            {
                model = model,
                prompt = desc,
                n = 1,
                size = $"{size.x}x{size.y}"
                // response_format = "url"
            };
            
            var json = JsonUtility.ToJson(reqBody);
            Debug.Log(json);
            
            using var post = new HttpRequestMessage(HttpMethod.Post, endPoint);
            post.Content = new StringContent(json, Encoding.UTF8, "application/json");
            post.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            
            using HttpResponseMessage genResponse = await _http.SendAsync(post);
            
            if (!genResponse.IsSuccessStatusCode)
            {
                Debug.LogError($"Failed to generate image: {genResponse.StatusCode}");
                return null;
            }
            
            // genResponse.EnsureSuccessStatusCode();
            
            string payload = await genResponse.Content.ReadAsStringAsync();
            Debug.Log($"Response: {payload}");
            ImageResponse imageResponse = JsonUtility.FromJson<ImageResponse>(payload);
            
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
            
            if (model != "gpt-image-1")
            {
                string imgUrl = imageResponse.data[0].url;
                using HttpResponseMessage imgResp = await _http.GetAsync(imgUrl);
                imgResp.EnsureSuccessStatusCode();
                byte[] imgBytes = await imgResp.Content.ReadAsByteArrayAsync();
            
                
                if (!texture.LoadImage(imgBytes))
                {
                    Debug.LogError("Failed to load image from bytes.");
                    return null;
                }
            }
            else
            {
                string b64 = imageResponse.data[0].b64_json;
                byte[] imgBytes = Convert.FromBase64String(b64);
                
                if (!texture.LoadImage(imgBytes))
                {
                    Debug.LogError("Failed to load image from base64.");
                    return null;
                }
            }
            
            
            
            texture.Apply();
            return texture;
        }

        [Serializable]
        private class ImageResponse
        {
            public ImageData[] data;
        }
    }
}

