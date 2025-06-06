using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop
{
    [Icon(ASSETS_PATH + "SceneHopIcon.png")]
    [Overlay(typeof(EditorWindow), "", minWidth = 173, maxWidth = 9999, minHeight = 160, maxHeight = 9999)]
    public class SceneHopOverlay : Overlay
    {
        #region Default Values
        const string ASSETS_PATH = "Packages/com.gaton.editor.scenehop/Editor/Assets/";

        const string USS_PATH = ASSETS_PATH + "SceneHop.uss";
        const string SETTINGS_LABEL = "Settings";
        #endregion

        #region Fields
        StyleSheet styleSheet;

        SearchField searchField;

        VisualElement scenesGrid;

        bool foldoutState = true;
        #endregion

        #region Constructor
        SceneHopOverlay()
        {
            searchField = new SearchField();
            styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USS_PATH);
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
            var root = new VisualElement();

            root.styleSheets.Add(styleSheet);

            root.Add(CreateConfigurations());

            root.Add(CreateSeparator());

            root.Add(CreateScenesGrid());

            return root;
        }

        #endregion

        #region Private Methods

        private VisualElement CreateConfigurations()
        {
            Foldout foldout = new Foldout()
            {
                value = foldoutState,
                text = SETTINGS_LABEL
            };

            foldout.AddToClassList("foldout");

            foldout.RegisterValueChangedCallback(callback =>
            {
                foldoutState = callback.newValue;
            });

            foldout.Add(searchField.GetSearchField(() => { RefreshOverlay(); }));

            foldout.style.flexGrow = 0;
            foldout.style.flexShrink = 0;

            return foldout;
        }


        private VisualElement CreateScenesGrid()
        {
            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            scenesGrid = new VisualElement();
            scenesGrid.AddToClassList("grid");

            RefreshGrid(scenesGrid);

            scrollView.Add(scenesGrid);

            return scrollView;
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

                string fileName = Path.GetFileNameWithoutExtension(path);

                // Regex to add white spaces to 'CamelCasedNames -> Camel Cased Names'
                string btnName = Regex.Replace(fileName, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

                btnName = btnName.Replace('_', ' ');
                btnName = btnName.Replace('-', ' ');

                button.text = btnName;
                button.AddToClassList("scene-button");

                grid.Add(button);
            }
        }

        private VisualElement CreateSeparator()
        {
            var separator = new VisualElement();
            separator.AddToClassList("separator");

            return separator;
        }

        #endregion
    }
}