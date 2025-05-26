using UnityEngine;

namespace Volorf.GenImage
{
    public class ApiKeysProvider : MonoBehaviour
    {
        [SerializeField] private GenImageSettings _genImageSettings;

        void Awake()
        {
            if (_genImageSettings == null)
            {
                Debug.LogError("GenImageSettings is not assigned in ApiKeysProvider.");
                return;
            }

            if (string.IsNullOrEmpty(_genImageSettings.OpenAiApiKey))
            {
                Debug.LogError("OpenAI API key is not set in GenImageSettings.");
            }
            else
            {
                PlayerPrefs.SetString(Utils.OpenAiApiKeyName, _genImageSettings.OpenAiApiKey);
            }
        }
    }
}

