using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Volorf.GenImage
{
    public class HttpRequestsManager
    {
        HttpClient _http;
        string _apiKey = "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";

        public HttpRequestsManager()
        {
            _http = new HttpClient();
        }
        
        public async Task<Texture2D> GenerateTexture2D(
            Provider provider,
            Model model,
            Quality quality,
            Size size,
            string prompt)
        {
            string endPoint = Utils.GetEndPoint(provider);
            Vector2Int genSize = Utils.GetSize(size, model);
            
            GenRequest reqBody = new GenRequest
            {
                model = Utils.GetModelName(model),
                prompt = prompt,
                n = 1,
                size = $"{genSize.x}x{genSize.y}",
                // response_format = "url"
            };
            
            var json = JsonUtility.ToJson(reqBody);
            Debug.Log(json);
            
            using var post = new HttpRequestMessage(HttpMethod.Post, endPoint);
            post.Content = new StringContent(json, Encoding.UTF8, "application/json");
            post.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            
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
            
            if (reqBody.model != "gpt-image-1")
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
    }
}