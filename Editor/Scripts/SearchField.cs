using System;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    class SearchField : VisualElement
    {
        enum SearchFilter
        {
            Path = 0,
            Name = 1,
            AllScenes = 2
        }

        #region Member Fields
        private SceneOverlayData data = null;

        private SearchFilter searchFilter = SearchFilter.Path;
        #endregion

        #region Public Methods
        public SearchField(SceneOverlayData data)
        {
            this.data = data;
        }

        public VisualElement InitSearchField(VisualElement root, Action onRefreshOverlay)
        {
            EnumField dropdownField = root.Q<EnumField>("search-filter");
            dropdownField.Init(searchFilter);
            
            #region Text Field

            var inputField = root.Q<TextField>("input-field");
            UpdateInputText(inputField);

            inputField.RegisterValueChangedCallback(callback =>
            {
                if (searchFilter == SearchFilter.Path)
                {
                    data.CurrentPath = callback.newValue;
                }
                else if (searchFilter == SearchFilter.Name)
                {
                    data.CurrentName = callback.newValue;
                }

                onRefreshOverlay();
            });

            UpdateTextFieldVisibility(inputField);

            #endregion

            dropdownField.RegisterValueChangedCallback(callback =>
            {
                searchFilter = (SearchFilter)callback.newValue;

                UpdateTextFieldVisibility(inputField);

                UpdateInputText(inputField);

                onRefreshOverlay();

            });

            return root;
        }

        public string GetSearchFilter()
        {
            switch (searchFilter)
            {
                case SearchFilter.Path:
                case SearchFilter.AllScenes:
                default:
                    return "t:scene";

                case SearchFilter.Name:
                    return data.CurrentName + " t:scene";
            }
        }

        public string[] GetSearchPaths()
        {
            switch (searchFilter)
            {
                case SearchFilter.Path:
                    return SplitPath();

                case SearchFilter.Name:
                case SearchFilter.AllScenes:
                default:
                    return new string[] { "Assets/" };

            }
        }

        

        #endregion

        #region Private Methods
        private void UpdateInputText(TextField inputField)
        {
            inputField.value = searchFilter == SearchFilter.Path ? data.CurrentPath : data.CurrentName;
        }

        private void UpdateTextFieldVisibility(TextField inputField)
        {
            switch (searchFilter)
            {
                case SearchFilter.Path:
                    inputField.style.display = DisplayStyle.Flex;
                    inputField.textEdition.placeholder = SceneOverlayData.DEFAULT_PATH;
                    break;
                case SearchFilter.Name:
                    inputField.style.display = DisplayStyle.Flex;
                    inputField.textEdition.placeholder = "SampleScene";
                    break;
                case SearchFilter.AllScenes:
                    inputField.style.display = DisplayStyle.None;
                    break;
                default:
                    break;
            }
        }

        private string[] SplitPath()
        {
            string[] paths = data.CurrentPath.Split(';');

            if (string.IsNullOrEmpty(paths[0]))
            {
                paths[0] = SceneOverlayData.DEFAULT_PATH;
            }

            return paths;
        }

        #endregion
    }
}