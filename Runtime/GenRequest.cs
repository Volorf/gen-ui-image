using System;

namespace  Volorf.GenImage
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

