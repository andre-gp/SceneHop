using System.IO;
using System.Text.RegularExpressions;
using Unity.Properties;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    [Icon(ASSETS_PATH + "SceneHopIcon.png")]
    [Overlay(typeof(EditorWindow), "", minWidth = 173, maxWidth = 9999, minHeight = 160, maxHeight = 9999)]
    public class SceneHopOverlay : Overlay
    {
        #region Default Values
        private const string ASSETS_PATH = "Packages/com.gaton.editor.scenehop/Editor/Assets/";

        private const string USS_PATH = ASSETS_PATH + "SceneHop.uss";

        private Vector2 defaultBtnSize = new Vector2(60, 40);
        #endregion

        #region Fields
        private SceneOverlayData data;

        private VisualTreeAsset mainWindow;
        private VisualTreeAsset buttonAsset;

        private StyleSheet styleSheet;

        private SearchField searchField;

        private VisualElement scenesGrid;

        private string savePath;

        #endregion

        #region Constructor
        SceneHopOverlay()
        {
            savePath = Application.dataPath + "/../Library/SceneHop/data.json";

            LoadData();
            searchField = new SearchField(data);
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
            mainWindow = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ASSETS_PATH + "SceneHop.uxml");
            buttonAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ASSETS_PATH + "SceneButton.uxml");           
        }
        private void LoadData()
        {
            if (File.Exists(savePath))
            {
                data = JsonUtility.FromJson<SceneOverlayData>(File.ReadAllText(savePath));
            }
            else
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

            EditorApplication.projectChanged += RefreshOverlay;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            EditorApplication.projectChanged -= RefreshOverlay;
        }

        public override VisualElement CreatePanelContent()
        {
            var root = mainWindow.CloneTree();
            root.dataSource = this.data;

            // The uxml already loads the style sheet, but to guarantee that it is going to be
            // linked, I am still adding it again here.
            root.styleSheets.Add(styleSheet);

            CreateConfigurations(root);

            CreateScenesGrid(root);

            return root;
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

            Slider scaleSlider = root.Q<Slider>("slider-scale");
            scaleSlider.SetBinding(nameof(scaleSlider.value), new DataBinding()
            {
                bindingMode = BindingMode.TwoWay,
                dataSourcePath = PropertyPath.FromName(nameof(this.data.ButtonScale))
            });
            scaleSlider.RegisterValueChangedCallback(callback =>
            {
                UpdateButtonsScale(callback.newValue);
            });

            searchField.InitSearchField(root, () => { RefreshOverlay(); });
        }

        private void UpdateButtonsScale(float scale)
        {
            foreach (var btn in scenesGrid.Children())
            {
                btn.style.width = defaultBtnSize.x * scale;
                btn.style.height = defaultBtnSize.y * scale;
                btn.style.fontSize = 8 * (scale + 0.1f);
            }
        }

        private void CreateScenesGrid(VisualElement root)
        {
            scenesGrid = root.Q<VisualElement>("grid-content");

            RefreshGrid(scenesGrid);
        }

        private void RefreshOverlay()
        {
            RefreshGrid(scenesGrid);
        }

        private void RefreshGrid(VisualElement grid)
        {
            grid.Clear();

            var paths = searchField.GetSearchPaths();

            var assets = AssetDatabase.FindAssets(searchField.GetSearchFilter(), searchField.GetSearchPaths());

            if (assets.Length <= 0)
            {
                grid.Add(new Label("Could not find any scenes!"));
            }

            for (int i = 0; i < assets.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[i]);

                var button = new Button(() =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(path);
                    }
                });

                button.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent callback) =>
                {
                    callback.menu.AppendAction("Load Scene", (x) => 
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(path);
                        }
                    });

                    callback.menu.AppendAction("Select Asset", (x) => 
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(path); 
                    });
                }));

                string fileName = Path.GetFileNameWithoutExtension(path);

                // Regex to add white spaces to 'CamelCasedNames -> Camel Cased Names'
                string btnName = Regex.Replace(fileName, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

                btnName = btnName.Replace('_', ' ');
                btnName = btnName.Replace('-', ' ');

                button.text = btnName;
                button.AddToClassList("scene-button");

                grid.Add(button);
            }

            UpdateButtonsScale(data.ButtonScale);
        }

        #endregion
    }
}