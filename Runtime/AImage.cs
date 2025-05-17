using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Volorf.AImage
{
    public enum Model
    {
        DallE3,
        DallE2,
        GtpImage1
    }
    
    [AddComponentMenu("Volorf/AImage")]
    [RequireComponent(typeof(RawImage))]
    public class AImage : MonoBehaviour
    {
        string sk = "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";
        
        [TextArea(3, 9)] public string prompt;
        public Model model = Model.DallE3;
        public int width = 1024;
        public int height = 1024;
        public bool respectRatio;
    
        private static readonly HttpClient _http = new HttpClient();

        Texture2D _texture;
        RawImage _rawImage;
        
        static string GetModelName(Model model)
        {
            return model switch
            {
                Model.DallE3 => "dall-e-3",
                Model.DallE2 => "dall-e-2",
                Model.GtpImage1 => "gpt-image-1",
                _ => "dall-e-3"
            };
        }
        
        async void Start()
        {
            _rawImage = GetComponent<RawImage>();
            
            Vector2 size = _rawImage.rectTransform.rect.size;
            print("size: " + size);
            
            // _http.DefaultRequestHeaders.Authorization =
            //     new AuthenticationHeaderValue("Bearer", sk);

            try
            {
                _texture = await GenerateTexture2D(prompt, sk, GetModelName(model), width, height);
                if (_rawImage != null) _rawImage.texture = _texture;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating image: {e.Message}");
            }
        }

        public static async Task<Texture2D> GenerateTexture2D(string desc, string apiKey, string model, int width, int height)
        {
            string url = "https://api.openai.com/v1/images/generations";
            
            GenRequest reqBody = new GenRequest
            {
                model = model,
                prompt = desc,
                n = 1,
                size = $"{width}x{height}"
            };
            
            var json = JsonUtility.ToJson(reqBody);
            Debug.Log(json);
            
            using var post = new HttpRequestMessage(HttpMethod.Post, url);
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
            ImageResponse imageResponse = JsonUtility.FromJson<ImageResponse>(payload);
            string imgUrl = imageResponse.data[0].url;

            using HttpResponseMessage imgResp = await _http.GetAsync(imgUrl);
            imgResp.EnsureSuccessStatusCode();
            byte[] imgBytes = await imgResp.Content.ReadAsByteArrayAsync();
            
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
            if (!texture.LoadImage(imgBytes))
            {
                Debug.LogError("Failed to load image from bytes.");
                return null;
            }
            
            texture.Apply();
            return texture;
        }
        
        [Serializable] private class ImageResponse
        {
            public ImageData[] data;
        }
        [Serializable] private class ImageData
        {
            public string url;       // filled because we didn’t ask for "b64_json"
            public string b64_json;
        }
        
        [Serializable]             // <-- required for JsonUtility
        private class GenRequest
        {
            public string model;
            public string prompt;
            public int    n;
            public string size;
        }
        
    }
}

