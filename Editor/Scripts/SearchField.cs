using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    class SearchField
    {
        #region Member Fields
        private SceneOverlayData data = null;

        private List<SearchType> searches;

        private DropdownField dropdown;
        #endregion

        #region Public Methods
        public SearchField(SceneOverlayData data)
        {
            this.data = data;

            searches = new List<SearchType>() { new PathSearch(data), new NameSearch(data), new AllScenesSearch(data) };
        }

        public void InitSearchField(VisualElement root, Action onRefreshOverlay)
        {
            dropdown = root.Q<DropdownField>("search-filter");
            UpdateDropdownChoices();

            var inputField = root.Q<TextField>("input-field");

            inputField.RegisterValueChangedCallback(callback =>
            {
                searches[dropdown.index].TextValue = callback.newValue;

                onRefreshOverlay();
            });

            dropdown.RegisterValueChangedCallback(callback =>
            {
                searches[dropdown.index].InitSearch(inputField);

                onRefreshOverlay();

            });

            dropdown.index = data.DropdownIndex;

            dropdown.SetBinding(nameof(dropdown.index), new DataBinding()
            {
                bindingMode = BindingMode.TwoWay,
                dataSourcePath = PropertyPath.FromName(nameof(data.DropdownIndex))
            });

            searches[dropdown.index].InitSearch(inputField);

            var button = root.Q<Button>("button-add");
            button.clickable.clicked += () =>
            {
                if (EditorUtility.DisplayDialog("Favorite Scenes", "Create a new favorite scenes group?", "Create", "Cancel"))
                {
                    searches.Add(new FavoriteScenesSearch(data));
                    UpdateDropdownChoices();

                    dropdown.index = searches.Count - 1;
                }
            };
        }

        private void UpdateDropdownChoices()
        {
            dropdown.choices = searches.Select(x => x.Label).ToList();
        }

        public string[] RetrieveGuids()
        {
            return searches[dropdown.index].RetrieveGuids();
        }
        

        #endregion
    }
}