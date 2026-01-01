using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif


namespace Volorf.GenUIImage
{
    [AddComponentMenu("Volorf/Gen UI Texture Updater")]
    [RequireComponent(typeof(GenUIImage))]
    public class GenUIImageTexture : MonoBehaviour
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
        public Texture2D texture;
        
        Action<Texture2D> _onResult;
        Action<string> _onError;
        
        [Space(10)]
        [TextArea(3, 9)] public string prompt = "Apply a sci-fi style to this texture, respecting boundaries of its pixel areas.";

        void Start()
        {
            Debug.Log($"Is Texture readable: {texture.isReadable}");
            if (!texture) { Debug.LogError("Texture is not assigned."); return; }
            
            _onResult = null;
            _onError = null;
            _onResult += tex =>
            {
                Debug.Log("Texture result");
                // OverwriteTextureAsset(texture, tex);
            };
            _onError += tex => Debug.LogError($"Error: {tex}");
            
            StartCoroutine(EditImage(texture, prompt, _onResult, _onError));
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
        
        public IEnumerator EditImage(Texture2D input, string prompt, Action<Texture2D> onResult, Action<string> onError)
    {
        byte[] pngBytes = MakeReadable(input).EncodeToPNG();

        // Build multipart form
        var form = new WWWForm();
        form.AddField("model", Utils.GetModelName(model));          // example model name
        form.AddField("prompt", prompt);
        // Optional fields some endpoints support:
        // form.AddField("size", "1024x1024");
        // form.AddField("n", "1");

        form.AddBinaryData("image", pngBytes, "input.png", "image/png");

        using var req = UnityWebRequest.Post("https://api.openai.com/v1/images/edits", form);
        req.SetRequestHeader("Authorization", $"Bearer {Utils.GetOpenAiApiKey()}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(req.error + "\n" + req.downloadHandler.text);
            yield break;
        }
        
        // var json = req.downloadHandler.text;
        // Debug.Log(json);
        //
        // // extract "url"
        // const string key = "\"url\":";
        // int i = json.IndexOf(key, StringComparison.Ordinal);
        // if (i < 0) { onError?.Invoke("url not found: " + json); yield break; }
        //
        // i = json.IndexOf('"', i + key.Length);
        // if (i < 0) { onError?.Invoke("url start quote not found: " + json); yield break; }
        // i++;
        //
        // int j = json.IndexOf('"', i);
        // if (j < 0) { onError?.Invoke("url end quote not found: " + json); yield break; }
        //
        // string url = json.Substring(i, j - i).Replace("\\/", "/");
        //
        // // download the image bytes
        // using (var get = UnityWebRequest.Get(url))
        // {
        //     yield return get.SendWebRequest();
        //
        //     if (get.result != UnityWebRequest.Result.Success)
        //     {
        //         onError?.Invoke("Failed to download image: " + get.error);
        //         yield break;
        //     }
        //
        //     byte[] outPng = get.downloadHandler.data;
        //     var outTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        //     outTex.LoadImage(outPng);
        //     onResult?.Invoke(outTex);
        //     
        //     if (get.result != UnityWebRequest.Result.Success)
        //     {
        //         onError?.Invoke(get.error);
        //     }
        // }
        
        
        

        // Parse JSON response, then decode the returned image (often base64)
        // Typical response: { data: [ { b64_json: "...." } ] }
        try
        {
            var json = req.downloadHandler.text;
            Debug.Log(json);
            ImageResponse imageResponse = JsonUtility.FromJson<ImageResponse>(json);
            // string b64 = ExtractB64JsonFromImagesResponse(json); // implement below
            string b64 = imageResponse.data[0].b64_json;
            byte[] outPng = Convert.FromBase64String(b64);
            
            // var outTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            // outTex.LoadImage(outPng);
            // onResult?.Invoke(outTex);
            
            ReplaceTextureAsset(texture, outPng);
            

        }
        catch (Exception e)
        {
            onError?.Invoke("Failed to parse image response: " + e);
        }
    }

        // Minimal JSON extraction without extra libs (works for simple responses).
        // For production, use a JSON library (e.g., Newtonsoft JSON).
        private static string ExtractB64JsonFromImagesResponse(string json)
        {
            const string key = "\"b64_json\":\"";
            int i = json.IndexOf(key, StringComparison.Ordinal);
            if (i < 0) throw new Exception("b64_json not found.");
            i += key.Length;
            int j = json.IndexOf("\"", i, StringComparison.Ordinal);
            if (j < 0) throw new Exception("b64_json end not found.");
            return json.Substring(i, j - i).Replace("\\n", "").Replace("\\/", "/");
        }
        
        Texture2D MakeReadable(Texture source)
        {
            var rt = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.Linear
            );

            Graphics.Blit(source, rt);

            var prev = RenderTexture.active;
            RenderTexture.active = rt;

            var tex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            return tex;
        }
        
        void ReplaceTextureAsset(Texture2D asset, byte[] pngBytes)
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path))
                throw new Exception("Texture is not an asset on disk");

            File.WriteAllBytes(path, pngBytes);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
#endif
        }


    }
}


