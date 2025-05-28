using UnityEngine;
using UnityEngine.Serialization;

namespace Volorf.GenImage
{
    public class ApiKeysProvider : MonoBehaviour
    {
        [FormerlySerializedAs("_genImageSettingsData")] [FormerlySerializedAs("_genImageSettings")] [SerializeField] private GenImageApiKeys _genImageApiKeys;

        void Awake()
        {
            if (_genImageApiKeys == null)
            {
                Debug.LogError("GenImageSettings is not assigned in ApiKeysProvider.");
                return;
            }

            if (string.IsNullOrEmpty(_genImageApiKeys.OpenAiApiKey))
            {
                Debug.LogError("OpenAI API key is not set in GenImageSettings.");
            }
            else
            {
                PlayerPrefs.SetString(Utils.OpenAiApiKeyName, _genImageApiKeys.OpenAiApiKey);
            }
        }
    }
}

