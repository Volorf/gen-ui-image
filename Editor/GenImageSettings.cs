using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Volorf.GenUIImage
{
    public class GenImageSettings : EditorWindow
    {
        VisualTreeAsset _ui;
        static GenImageSettings _wnd;
        TextField _field;
        

        [MenuItem("Tools/Gen Image/Settings", false, 1)]
        public static void ShowSettings()
        {
            _wnd = GetWindow<GenImageSettings>();
            _wnd.titleContent = new GUIContent("Gen Image Settings");
        }

        [MenuItem("Tools/Gen Image/Update All", false, 2)]
        public static void UpdateAll()
        {
            // IFigmaImageUpdatable[] figmaImageUpdatables =
            //     FindObjectsOfType<MonoBehaviour>(false).OfType<IFigmaImageUpdatable>().ToArray();
            //
            // foreach (IFigmaImageUpdatable updatable in figmaImageUpdatables)
            // {
            //     updatable.UpdateFigmaImage();
            // }
            //
            // Debug.Log("All textures have been updated.");
        }

        public void CreateGUI()
        {
            _ui = Resources.Load<VisualTreeAsset>("GenImageSettings");
            VisualElement root = rootVisualElement;
            VisualElement labelFromUXML = _ui.Instantiate();
            root.Add(labelFromUXML);

            Button save = root.Q<Button>("save");
            _field = root.Q<TextField>("key");
            _field.value = PlayerPrefs.GetString(Utils.OpenAiApiKeyName);

            save.RegisterCallback<ClickEvent>(SaveOpenAiApiKey);
        }

        void SaveOpenAiApiKey(ClickEvent ev)
        {
            PlayerPrefs.SetString(Utils.OpenAiApiKeyName, _field.value);
            Debug.Log($"Saved OpenAI API key: {_field.value}");
            _wnd.Close();
        }

    }
}
