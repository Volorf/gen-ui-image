using System;

namespace  Volorf.AImage
{
    [Serializable]
    public class GenRequest
    {
        public string model;
        public string prompt;
        public int    n;
        public string size;
    }
}

