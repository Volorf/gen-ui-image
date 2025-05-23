using UnityEngine;

namespace Volorf.GenImage
{
    [CreateAssetMenu(fileName = "GenImageSettings", menuName = "GenImage/Create GenImageSettings")]
    public class GenImageSettings : ScriptableObject
    {
        public string OpenAiApiKey;
    }
}
