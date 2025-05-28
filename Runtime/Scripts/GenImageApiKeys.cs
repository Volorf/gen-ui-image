using UnityEngine;

namespace Volorf.GenImage
{
    [CreateAssetMenu(fileName = "GenImageApiKeys", menuName = "GenImage/Create GenImageApiKeys")]
    public class GenImageApiKeys : ScriptableObject
    {
        public string OpenAiApiKey;
    }
}
