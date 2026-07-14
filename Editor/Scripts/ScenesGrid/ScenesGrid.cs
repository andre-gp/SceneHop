using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class ScenesGrid
    {
        private Vector2 defaultBtnSize = new Vector2(60, 40);

        private VisualElement gridRoot;
        private SceneOverlayData data;
        private float minScale;

        private List<SceneButton> instantiatedButtons = new List<SceneButton>();
        public List<SceneButton> InstantiatedButtons => this.instantiatedButtons;

        public ScenesGrid(VisualElement root, SceneOverlayData data)
        {
            this.data = data;

            gridRoot = root.Q<VisualElement>("grid-content");

            SetGridEnabled(!EditorApplication.isPlayingOrWillChangePlaymode);

            InitScaleSlider(root);
        }

        public void SetGridEnabled(bool enabled)
        {
            gridRoot.SetEnabled(enabled);
        }

        private void InitScaleSlider(VisualElement root)
        {
            Slider scaleSlider = root.Q<Slider>("slider-scale");
            minScale = scaleSlider.lowValue;
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

        public void ClearGrid()
        {
            foreach (var btn in instantiatedButtons)
            {
                btn.DestroyButton();
            }

            instantiatedButtons.Clear();
        }

        public void RefreshGrid(SearchType searchType)
        {
            ClearGrid();
            gridRoot.Clear(); // Ensure that have removed everything, including the label

            instantiatedButtons.AddRange(searchType.InstantiateButtons(gridRoot));

            if (instantiatedButtons.Count <= 0)
            {
                searchType.AddNoSceneElements(gridRoot);
            }

            UpdateButtonsScale(data.ButtonScale);
        }

        private void UpdateButtonsScale(float scale)
        {
            // At the slider's minimum, collapse the grid into a compact list
            bool listMode = scale <= minScale;

            gridRoot.EnableInClassList("grid--list", listMode);

            foreach (var btn in gridRoot.Children())
            {
                btn.EnableInClassList("scene-button--list", listMode);

                if (listMode)
                {
                    // Clear inline sizes so the list USS rules take over.
                    btn.style.width = StyleKeyword.Null;
                    btn.style.height = StyleKeyword.Null;
                    btn.style.fontSize = StyleKeyword.Null;
                }
                else
                {
                    btn.style.width = defaultBtnSize.x * scale;
                    btn.style.height = defaultBtnSize.y * scale;
                    btn.style.fontSize = 9 * scale;
                }
            }
        }
    }
}
