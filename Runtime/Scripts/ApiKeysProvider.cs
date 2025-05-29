using UnityEngine;
using UnityEngine.Serialization;

namespace Volorf.GenUIImage
{
    public class ApiKeysProvider : MonoBehaviour
    {
        [FormerlySerializedAs("_genImageApiKeys")] [FormerlySerializedAs("_genImageSettingsData")] [FormerlySerializedAs("_genImageSettings")] [SerializeField] private ApiKeysProviderData _apiKeysProviderData;

        void Awake()
        {
            if (_apiKeysProviderData == null)
            {
                Debug.LogError("GenImageSettings is not assigned in ApiKeysProvider.");
                return;
            }

            if (string.IsNullOrEmpty(_apiKeysProviderData.OpenAiApiKey))
            {
                Debug.LogError("OpenAI API key is not set in GenImageSettings.");
            }
            else
            {
                PlayerPrefs.SetString(Utils.OpenAiApiKeyName, _apiKeysProviderData.OpenAiApiKey);
            }
        }
    }
}

