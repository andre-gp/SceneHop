using System;
using System.Collections.Generic;
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
    [Overlay(typeof(EditorWindow), "", minWidth = 155, maxWidth = 9999, minHeight = 100, maxHeight = 9999)]
    public class SceneHopOverlay : Overlay
    {
        #region Default Values

        private const string ASSETS_PATH = "Packages/com.gaton.editor.scenehop/Editor/Assets/";

        private const string USS_PATH = ASSETS_PATH + "SceneHop.uss";

        #endregion

        #region Fields
        private SceneOverlayData data;

        private VisualTreeAsset mainWindow;
        private VisualTreeAsset buttonAsset;

        private StyleSheet styleSheet;

        private SearchField searchField;

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

            EditorApplication.projectChanged += searchField.RefreshOverlay;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();

            EditorApplication.projectChanged -= searchField.RefreshOverlay;
        }

        public override VisualElement CreatePanelContent()
        {
            var root = mainWindow.CloneTree();
            root.dataSource = this.data;

            // The uxml already loads the style sheet, but to guarantee that it is going to be
            // linked, I am still adding it again here.
            root.styleSheets.Add(styleSheet);

            CreateConfigurations(root);

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

            searchField.InitSearchField(root);
        }

        #endregion
    }
}