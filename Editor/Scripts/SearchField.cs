using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class SearchField
    {
        #region Member Fields
        private FavoriteScenesData favoriteScenes;

        private SceneOverlayData data = null;
        public SceneOverlayData Data => this.data;

        private List<SearchType> searches;

        #endregion

        #region Components

        private DropdownField dropdown;

        private TextField inputField;
        public TextField InputField => this.inputField;

        private VisualElement favoritesToolbar;
        public VisualElement FavoritesToolbar => this.favoritesToolbar;

        #endregion

        #region Public Methods
        public SearchField(SceneOverlayData data)
        {
            favoriteScenes = new FavoriteScenesData();

            this.data = data;

            searches = new List<SearchType>() { new PathSearch(this), new NameSearch(this), new AllScenesSearch(this) };
        }

        public void InitSearchField(VisualElement root, Action onRefreshOverlay)
        {
            favoritesToolbar = root.Q<VisualElement>("toolbar-favorites");

            dropdown = root.Q<DropdownField>("search-filter");
            UpdateDropdownChoices();

            inputField = root.Q<TextField>("input-field");

            inputField.RegisterValueChangedCallback(callback =>
            {
                searches[dropdown.index].TextValue = callback.newValue;

                onRefreshOverlay();
            });

            dropdown.RegisterValueChangedCallback(callback =>
            {
                DeactivateAllOptions();

                searches[dropdown.index].InitSearch();

                onRefreshOverlay();

            });

            data.DropdownIndex = Mathf.Clamp(data.DropdownIndex, 0, dropdown.choices.Count - 1);
            dropdown.index = data.DropdownIndex;


            dropdown.SetBinding(nameof(dropdown.index), new DataBinding()
            {
                bindingMode = BindingMode.TwoWay,
                dataSourcePath = PropertyPath.FromName(nameof(data.DropdownIndex))
            });

            searches[dropdown.index].InitSearch();

            var button = root.Q<Button>("button-add");
            button.clickable.clicked += () =>
            {
                if (EditorUtility.DisplayDialog("Favorite Scenes", "Create a new favorite scenes group?", "Create", "Cancel"))
                {
                    searches.Add(new FavoriteScenesSearch(this, favoriteScenes.AddNewSceneGroup()));
                    UpdateDropdownChoices();

                    dropdown.index = searches.Count - 1;
                }
            };
        }

        private void DeactivateAllOptions()
        {
            InputField.style.display = DisplayStyle.None;

            favoritesToolbar.style.display = DisplayStyle.None;
        }

        public void UpdateDropdownChoices()
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