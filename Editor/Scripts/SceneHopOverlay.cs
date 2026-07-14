using System;
using System.IO;
using System.Threading.Tasks;
using Unity.Properties;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    [Icon(ASSETS_PATH + "SceneHopIcon.png")]
    [Overlay(typeof(EditorWindow), "", minWidth = 155, maxWidth = 9999, minHeight = 100, maxHeight = 9999)]
    public class SceneHopOverlay : Overlay
    {
        #region Default Values

        private const string ASSETS_PATH = "Packages/com.gaton.editor.scenehop/Editor/Assets/";

        private const string USS_PATH = ASSETS_PATH + "SceneHop.uss";
        private const string UXML_PATH = ASSETS_PATH + "SceneHop.uxml";

        #endregion

        #region Fields
        private SceneOverlayData data;

        private VisualTreeAsset mainWindowTemplate;

        private StyleSheet styleSheet;

        private SearchField searchField;

        private string savePath;

        private VisualElement root;

        private bool hasInitializedOverlay;

        #endregion

        #region Constructor
        SceneHopOverlay()
        {
            savePath = Application.dataPath + "/../Library/SceneHop/data.json";

            LoadData();

            searchField = new SearchField(data);

            LoadAssets();
        }

        private void LoadAssets()
        {
            if (styleSheet != null && mainWindowTemplate != null)
                return;

            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            mainWindowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UXML_PATH);
        }

        private void LoadData()
        {
            try
            {
                if (File.Exists(savePath))
                {
                    data = JsonUtility.FromJson<SceneOverlayData>(File.ReadAllText(savePath));
                }
            }
            catch (Exception e)
            {
                File.Copy(savePath, savePath + ".bak", true);
                Debug.LogWarning($"SceneHop: Failed to load '{savePath}'. Falling back to default settings. " +
                    $"The previous file was backed up as 'data.json.bak'.\n{e.Message}");
            }

            if (data == null)
            {
                data = new SceneOverlayData();
            }

            data.OnUpdateValue += data =>
            {
                SaveData();
            };
        }

        private void SaveData()
        {
            new FileInfo(savePath).Directory.Create();
            File.WriteAllText(savePath, JsonUtility.ToJson(data));
        }

        #endregion

        #region Public Methods
        public override void OnCreated()
        {
            base.OnCreated();

            EditorApplication.projectChanged += OnRefreshProject;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorSceneManager.sceneOpened += OnSceneOpened;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            EditorApplication.projectChanged -= OnRefreshProject;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            // Rebuild the grid so the current-scene highlight follows the open scene.
            if (hasInitializedOverlay)
            {
                searchField.RefreshOverlay();
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            searchField.ScenesGrid?.SetGridEnabled(!EditorApplication.isPlayingOrWillChangePlaymode);
        }

        public void OnRefreshProject()
        {
            if (hasInitializedOverlay)
            {
                searchField.RefreshOverlay();
            }
            else
            {                
                LoadAssets();

                if (root == null)
                    return;

                root.Clear();

                root.Add(InternalGetPanelContent());
            }
        }

        public override VisualElement CreatePanelContent()
        {
            root = new VisualElement();

            root.Add(InternalGetPanelContent());

            return root;
        }

        private VisualElement InternalGetPanelContent()
        {
            if (styleSheet == null || mainWindowTemplate == null)
            {
                return CreateReloadButton();
            }

            hasInitializedOverlay = true;

            var mainWindow = mainWindowTemplate.CloneTree();
            mainWindow.dataSource = this.data;

            // The uxml already loads the style sheet, but to guarantee that it is going to be
            // linked, I am still adding it again here.
            mainWindow.styleSheets.Add(styleSheet);

            CreateConfigurations(mainWindow);

            return mainWindow;
        }

        private VisualElement CreateReloadButton()
        {
            /// When the package is first imported, these assets may not be available yet,
            /// so the UI is created after the project refresh.
            /// A button was added to prevent recursive script reloads in case the assets
            /// failed to be added to the project.

            Button button = new Button();

            button.text = "Initialize SceneHop";

            button.clicked += () =>
            {
                EditorUtility.RequestScriptReload();
            };

            return button;
        }

        #endregion

        #region Private Methods

        private void CreateConfigurations(VisualElement root)
        {
            Foldout foldout = root.Q<Foldout>("foldout-settings");
            foldout.SetBinding(nameof(foldout.value), new DataBinding()
            {
                bindingMode = BindingMode.TwoWay,
                dataSourcePath = PropertyPath.FromName(nameof(this.data.FoldoutState))
            });

            searchField.InitSearchField(root);
        }

        #endregion
    }
}