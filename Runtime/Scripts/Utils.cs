using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Volorf.GenUIImage
{
    [ExecuteAlways]
    public class Utils
    {
        public static readonly string OpenAiApiKeyName = "OPENAI_API_KEY";
        
        public static string GetModelName(Model model)
        {
            return model switch
            {
                Model.DallE3 => "dall-e-3",
                Model.DallE2 => "dall-e-2",
                Model.GptImage1 => "gpt-image-1",
                _ => "dall-e-2"
            };
        }
        
        public static string GetQualityName(Quality quality, Model modelContext)
        {
            return quality switch
            {
                Quality.Low => modelContext switch
                {
                    Model.GptImage1 => "low",
                    Model.DallE3 => "standard",
                    _ => ""
                },
                Quality.Medium => modelContext switch
                {
                    Model.GptImage1 => "medium",
                    Model.DallE3 => "standard",
                    _ => ""
                },
                Quality.High => modelContext switch
                {
                    Model.GptImage1 => "high",
                    Model.DallE3 => "hd",
                    _ => ""
                }
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
                },
                _ => throw new ArgumentOutOfRangeException(nameof(size), size, null)
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

        public static Rect GetFixedUVRect(Vector2 imageSize, FillMode mode, Vector2Int genSize)
        {
            bool isLandscape = imageSize.x > imageSize.y;

            if (mode == FillMode.Stretch)
            {
                if (isLandscape)
                {
                    float ratio = imageSize.x / imageSize.y;
                    float genRatio = (float)genSize.x / (float)genSize.y;
                    float yOffset = 1f / ratio * genRatio;
                    Vector2 newUV = new Vector2(1f, yOffset);
                    return new Rect(0f, (1f - newUV.y) / 2f, newUV.x, newUV.y);
                }
                else
                {
                    float ratio = imageSize.y / imageSize.x;
                    float genRatio = (float)genSize.y / (float)genSize.x;
                    float xOffset = 1f / ratio * genRatio;
                    Vector2 newUV = new Vector2(xOffset, 1f);
                    return new Rect((1f - xOffset) / 2f, 0f, newUV.x, newUV.y);
                }
            }

            if (mode == FillMode.PreserveAspect)
            {
                if (isLandscape)
                {
                    float ratio = imageSize.x / imageSize.y;
                    float genRatio = (float)genSize.x / (float)genSize.y;
                    Vector2 newUV = new Vector2(1f * ratio / genRatio, 1f);
                    return new Rect((1f - newUV.x) / 2f, 0f, newUV.x, newUV.y);
                }
                else
                {
                    float ratio = imageSize.y / imageSize.x;
                    float genRatio = (float)genSize.y / (float)genSize.x;
                    Vector2 newUV = new Vector2(1f, 1f * ratio / genRatio);
                    return new Rect(0f, (1f - newUV.y) / 2f, newUV.x, newUV.y);
                }
            }
            
            return new Rect(0, 0, 1, 1);
        }
        
        
        public static string DictionaryToJson(Dictionary<string, string> dict)
        {
            var entries = dict.Select(d =>
            {
                if (d.Key == "n")
                    return $"\"{d.Key}\":{d.Value}";
                return $"\"{d.Key}\":\"{d.Value}\"";
            });
                
            return "{" + string.Join(",", entries) + "}";
        }

        public static string GetOpenAiApiKey()
        {
            return PlayerPrefs.GetString(OpenAiApiKeyName);
        }
    }
}