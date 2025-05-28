using UnityEngine;

namespace Volorf.GenUIImage
{
    [CreateAssetMenu(fileName = "GenImageApiKeys", menuName = "GenImage/Create GenImageApiKeys")]
    public class GenImageApiKeys : ScriptableObject
    {
        public string OpenAiApiKey;
    }
}
