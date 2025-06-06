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

        #region Default Values
        const string DEFAULT_PATH = "Assets/Scenes/";
        const string DEFAULT_NAME = "SampleScene";
        #endregion

        #region Member Fields
        string pathValue = DEFAULT_PATH;
        string nameValue = DEFAULT_NAME;
        SearchFilter searchFilter = SearchFilter.Path;
        #endregion

        #region Public Methods
        public VisualElement GetSearchField(Action onRefreshOverlay)
        {
            VisualElement root = new VisualElement();

            EnumField dropdownField = new EnumField(searchFilter);
            dropdownField.AddToClassList("dropdown");

            root.Add(dropdownField);

            #region Text Field
            VisualElement textField = new VisualElement();

            var inputField = new TextField();
            UpdateInputText(inputField);
            inputField.textEdition.isDelayed = true;
            inputField.textEdition.placeholder = DEFAULT_NAME;
            inputField.style.marginRight = 10;

            inputField.RegisterValueChangedCallback(callback =>
            {
                if (searchFilter == SearchFilter.Path)
                {
                    pathValue = callback.newValue;
                }
                else if (searchFilter == SearchFilter.Name)
                {
                    nameValue = callback.newValue;
                }

                onRefreshOverlay();
            });

            UpdateTextFieldVisibility(textField, inputField);
            textField.Add(inputField);

            root.Add(textField);
            #endregion

            dropdownField.RegisterValueChangedCallback(callback =>
            {
                searchFilter = (SearchFilter)callback.newValue;

                UpdateTextFieldVisibility(textField, inputField);

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
                    return nameValue + " t:scene";
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
            inputField.value = searchFilter == SearchFilter.Path ? pathValue : nameValue;
        }

        private void UpdateTextFieldVisibility(VisualElement root, TextField inputField)
        {
            switch (searchFilter)
            {
                case SearchFilter.Path:
                    root.style.display = DisplayStyle.Flex;
                    inputField.textEdition.placeholder = "Assets/Scenes/";
                    break;
                case SearchFilter.Name:
                    root.style.display = DisplayStyle.Flex;
                    inputField.textEdition.placeholder = "SampleScene";
                    break;
                case SearchFilter.AllScenes:
                    root.style.display = DisplayStyle.None;
                    break;
                default:
                    break;
            }
        }

        private string[] SplitPath()
        {
            string[] paths = pathValue.Split(';');

            if (string.IsNullOrEmpty(paths[0]))
            {
                paths[0] = DEFAULT_PATH;
            }

            return paths;
        }

        #endregion
    }
}