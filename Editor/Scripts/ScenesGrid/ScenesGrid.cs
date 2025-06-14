using System;
using System.IO;
using System.Text.RegularExpressions;
using Unity.Properties;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class ScenesGrid
    {
        public Func<string[]> RetrieveGuids;

        private Vector2 defaultBtnSize = new Vector2(60, 40);
        private Color favoriteSceneColor = new Color(0.9f, 0.9f, 0.4f);

        private VisualElement gridRoot;
        private SceneOverlayData data;

        public ScenesGrid(VisualElement root, SceneOverlayData data, Func<string[]> RetrieveGuids)
        {
            this.RetrieveGuids = RetrieveGuids;
            this.data = data;

            gridRoot = root.Q<VisualElement>("grid-content");

            RefreshGrid();

            InitScaleSlider(root);
        }

        private void InitScaleSlider(VisualElement root)
        {
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
        }

        public void RefreshGrid()
        {
            gridRoot.Clear();

            var guids = RetrieveGuids();

            if (guids.Length <= 0)
            {
                gridRoot.Add(new Label("Could not find any scenes!"));
            }

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                var button = new Button();

                button.clickable.clicked += () =>
                {
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    {
                        EditorSceneManager.OpenScene(path);
                    }
                };

                if (i % 2 == 0)
                {
                    Color color = new Color(0.9f, 0.9f, 0.4f);
                    button.style.borderBottomColor = color;
                    button.style.borderTopColor = color;
                    button.style.borderRightColor = color;
                    button.style.borderLeftColor = color;

                    
                }

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

                gridRoot.Add(button);
            }

            UpdateButtonsScale(data.ButtonScale);
        }

        private void UpdateButtonsScale(float scale)
        {
            foreach (var btn in gridRoot.Children())
            {
                btn.style.width = defaultBtnSize.x * scale;
                btn.style.height = defaultBtnSize.y * scale;
                btn.style.fontSize = 9 * (scale + 0.1f);
            }
        }
    }
}
