using System;

namespace Volorf.GenUIImage
{
    [Serializable]
    public class ImageData
    {
        public string url;
        public string b64_json;
    }
    
    [Serializable]
    public class ImageResponse
    {
        public ImageData[] data;
    }
}
