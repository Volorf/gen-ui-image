using UnityEngine;

namespace Volorf.GenImage
{
    [RequireComponent(typeof(GenImage))]
    public class GenImageProgressIndicator : MonoBehaviour
    {
        GameObject _progressIndicatorPrefab;
        GameObject _progressIndicatorInstance;

        void Start()
        {
            _progressIndicatorPrefab = Resources.Load<GameObject>("GenImageProgressIndicator");
            _progressIndicatorInstance = Instantiate(_progressIndicatorPrefab, transform);
        }
    }
}

