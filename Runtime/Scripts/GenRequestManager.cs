using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Volorf.GenUIImage
{
    public class GenRequestManager
    {
        HttpClient _http;
        
        public GenRequestManager()
        {
            _http = new HttpClient();
        }
        
        /// <summary>
        /// Creates a request to an AI provider with a provided set of arguments.
        /// </summary>
        /// <param name="provider">Your AI Provider.</param>
        /// <param name="model">Supported model for image generation.</param>
        /// <param name="quality">Model-depending parameter. Defines the quality of generation itself (amount of tokens used).</param>
        /// <param name="prompt">Description of what you want to be generated.</param>
        /// <param name="size">Model-depending parameter. Defines an available size variation for a given model.</param>
        /// <param name="apiKey">Key to access your AI provider.</param>
        public async Task<Texture2D> GenerateTexture2D(
            Provider provider,
            Model model,
            Quality quality,
            Size size,
            string prompt,
            string apiKey = "")
        {
            string endPoint = Utils.GetEndPoint(provider);
            Vector2Int genSize = Utils.GetSize(size, model);
            
            Dictionary<string, string> req = new Dictionary<string, string>();
            req.Add("model", Utils.GetModelName(model));
            req.Add("prompt", prompt);
            req.Add("n", "1");
            req.Add("size", $"{genSize.x}x{genSize.y}");
            
            if (model != Model.DallE2)
            {
                req.Add("quality", Utils.GetQualityName(quality, model));
            }
            
            string genReq = Utils.DictionaryToJson(req);
            using var post = new HttpRequestMessage(HttpMethod.Post, endPoint);
            post.Content = new StringContent(genReq, Encoding.UTF8, "application/json");
            string ak = string.IsNullOrEmpty(apiKey) ? Utils.GetOpenAiApiKey() : apiKey;
            post.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ak);
            
            using HttpResponseMessage genResponse = await _http.SendAsync(post);
            
            if (!genResponse.IsSuccessStatusCode)
            {
                Debug.LogError($"Failed to generate image: {genResponse.StatusCode}");
                return null;
            }
            
            string payload = await genResponse.Content.ReadAsStringAsync();
            Debug.Log($"Response: {payload}");
            ImageResponse imageResponse = JsonUtility.FromJson<ImageResponse>(payload);
            
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, mipChain: false);
            
            if (Utils.GetModelName(model) != "gpt-image-1")
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
        
        /// <summary>
        /// Dummy method to emulate a request.
        /// </summary>
        public async Task<Texture2D> DummyGenerateTexture2D(
            Provider provider,
            Model model,
            Quality quality,
            Size size,
            string prompt,
            string apiKey = "")
        {
            Vector2Int genSize = Utils.GetSize(size, model);
            Texture2D texture = new Texture2D(genSize.x, genSize.y, TextureFormat.RGBA32, mipChain: false);
            
            Color ranColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
            
            for (int y = 0; y < genSize.y; y++)
            {
                for (int x = 0; x < genSize.x; x++)
                {
                    texture.SetPixel(x, y, ranColor);
                }
            }
            
            texture.Apply();

            await Task.Delay(2000);
            
            return texture;
        }
    }
}