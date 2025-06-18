using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class ScenesGrid
    {
        private Vector2 defaultBtnSize = new Vector2(60, 40);
        private Color favoriteSceneColor = new Color(0.9f, 0.9f, 0.4f);

        private VisualElement gridRoot;
        private SceneOverlayData data;

        private List<SceneButton> instantiatedButtons = new List<SceneButton>();
        public List<SceneButton> InstantiatedButtons => this.instantiatedButtons;

        public ScenesGrid(VisualElement root, SceneOverlayData data)
        {
            this.data = data;

            gridRoot = root.Q<VisualElement>("grid-content");

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
            foreach (var btn in gridRoot.Children())
            {
                btn.style.width = defaultBtnSize.x * scale;
                btn.style.height = defaultBtnSize.y * scale;
                btn.style.fontSize = 9 * (scale + 0.1f);
            }
        }
    }
}
