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
        public Action RefreshOverlay;

        private FavoriteScenesData favoriteScenes;

        private SceneOverlayData data = null;
        public SceneOverlayData Data => this.data;

        private List<SearchType> searches;

        #endregion

        #region Components

        private VisualElement root;
        public VisualElement Root => this.root;

        private DropdownField searchTypeDropdown;
        public DropdownField SearchTypeDropdown => this.searchTypeDropdown;

        private TextField inputField;
        public TextField InputField => this.inputField;

        private FavoriteScenesToolbar favoritesToolbar;
        public FavoriteScenesToolbar FavoritesToolbar => this.favoritesToolbar;

        #endregion

        #region Public Methods
        public SearchField(SceneOverlayData data)
        {
            favoriteScenes = new FavoriteScenesData();

            this.data = data;

            searches = new List<SearchType>() { new PathSearch(this), new NameSearch(this), new AllScenesSearch(this) };
        }

        public void InitSearchField(VisualElement root, Action RefreshOverlay)
        {
            this.root = root;
            this.RefreshOverlay = RefreshOverlay;

            favoritesToolbar = new FavoriteScenesToolbar(this);

            searchTypeDropdown = root.Q<DropdownField>("search-filter");
            UpdateDropdownChoices();

            inputField = root.Q<TextField>("input-field");

            inputField.RegisterValueChangedCallback(callback =>
            {
                searches[searchTypeDropdown.index].TextValue = callback.newValue;

                RefreshOverlay();
            });

            searchTypeDropdown.RegisterValueChangedCallback(callback =>
            {
                DeactivateAllOptions();

                searches[searchTypeDropdown.index].InitSearch();

                RefreshOverlay();

            });

            DeactivateAllOptions();

            data.DropdownIndex = Mathf.Clamp(data.DropdownIndex, 0, searchTypeDropdown.choices.Count - 1);
            searchTypeDropdown.index = data.DropdownIndex;


            searchTypeDropdown.SetBinding(nameof(searchTypeDropdown.index), new DataBinding()
            {
                bindingMode = BindingMode.TwoWay,
                dataSourcePath = PropertyPath.FromName(nameof(data.DropdownIndex))
            });

            searches[searchTypeDropdown.index].InitSearch();

            var button = root.Q<Button>("button-add");
            button.clickable.clicked += () =>
            {
                if (EditorUtility.DisplayDialog("Favorite Scenes", "Create a new favorite scenes group?", "Create", "Cancel"))
                {
                    searches.Add(new FavoriteScenesSearch(this, favoriteScenes.AddNewSceneGroup()));
                    UpdateDropdownChoices();

                    searchTypeDropdown.index = searches.Count - 1;
                }
            };
        }

        private void DeactivateAllOptions()
        {
            InputField.style.display = DisplayStyle.None;

            favoritesToolbar.EnableToolbar(false);
        }

        public void UpdateDropdownChoices()
        {
            searchTypeDropdown.choices = searches.Select(x => x.Label).ToList();
        }

        public string[] RetrieveGuids()
        {                       
            return searches[searchTypeDropdown.index].RetrieveGuids();
        }
        

        #endregion
    }
}