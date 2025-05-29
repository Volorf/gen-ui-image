using UnityEngine;

namespace Volorf.GenUIImage
{
    [CreateAssetMenu(fileName = "ApiKeysProviderData", menuName = "Gen UI Image/Create ApiKeysProviderData")]
    public class ApiKeysProviderData : ScriptableObject
    {
        public string OpenAiApiKey;
    }
}
