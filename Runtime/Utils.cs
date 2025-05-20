using UnityEngine;

namespace Volorf.GenImage
{
    public class Utils
    {
        public static string GetModelName(Model model)
        {
            return model switch
            {
                Model.DallE3 => "dall-e-3",
                Model.DallE2 => "dall-e-2",
                Model.GptImage1 => "gpt-image-1",
                _ => "dall-e-3"
            };
        }
        
        public static string GetQualityName(Quality quality)
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

        public static Vector2Int GetSize(Size size, Model modelContext)
        {
            return size switch
            {
                Size.Square => modelContext switch
                {
                    Model.GptImage1 => new Vector2Int(1024, 1024),
                    Model.DallE3 => new Vector2Int(1024, 1024),
                    _ => new Vector2Int(512, 512)
                },
                Size.Landscape => modelContext switch
                {
                    Model.GptImage1 => new Vector2Int(1536, 1024),
                    Model.DallE3 => new Vector2Int(1792, 1024),
                    _ => new Vector2Int(512, 512),
                },
                Size.Portrait => modelContext switch
                {
                    Model.GptImage1 => new Vector2Int(1024, 1536),
                    Model.DallE3 => new Vector2Int(1024, 1792),
                    _ => new Vector2Int(512, 512)
                }
            };
        }
        
        public static string GetEndPoint(Provider provider)
        {
            return provider switch
            {
                Provider.OpenAI => "https://api.openai.com/v1/images/generations",
                _ => "https://api.openai.com/v1/images/generations"
            };
        }
    }
}