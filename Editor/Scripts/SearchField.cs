using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Properties;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class SearchField
    {
        #region Default Values

        #endregion

        #region Member Fields
        public SearchType CurrentSearchType => searches[searchTypeDropdown.index];

        private FavoriteScenesData favoriteScenes;

        private SceneOverlayData data = null;
        public SceneOverlayData Data => this.data;

        private List<SearchType> searches;

        ScenesGrid scenesGrid;
        public ScenesGrid ScenesGrid => scenesGrid;

        private string favoriteScenesSavePath;

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
            LoadFavorites();

            this.data = data;

            searches = new List<SearchType>() { new PathSearch(this), new NameSearch(this), new AllScenesSearch(this) };

            foreach (var sceneGroup in this.favoriteScenes.SceneGroups)
            {
                searches.Add(new FavoriteScenesSearch(this, sceneGroup));
            }
        }

        public void InitSearchField(VisualElement root)
        {
            scenesGrid = new ScenesGrid(root, data);

            this.root = root;

            favoritesToolbar = new FavoriteScenesToolbar(this);
            favoritesToolbar.OnSave += OnSaveFavorites;

            searchTypeDropdown = root.Q<DropdownField>("search-filter");
            UpdateDropdownChoices();

            inputField = root.Q<TextField>("input-field");

            inputField.RegisterValueChangedCallback(callback =>
            {
                CurrentSearchType.TextValue = callback.newValue;

                RefreshOverlay();
            });

            searchTypeDropdown.RegisterValueChangedCallback(callback =>
            {
                DeactivateAllOptions();

                CurrentSearchType.InitSearch();

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

                    SaveFavoritesDataOnDisk();
                }
            };
        }

        private void LoadFavorites()
        {
            favoriteScenesSavePath = Application.dataPath + "/../Library/SceneHop/favorites.json";

            if (File.Exists(favoriteScenesSavePath))
            {
                favoriteScenes = JsonUtility.FromJson<FavoriteScenesData>(File.ReadAllText(favoriteScenesSavePath));
            }
            else
            {
                favoriteScenes = new FavoriteScenesData();
            }
        }

        private void OnSaveFavorites()
        {
            var favoriteScenesGuid = ScenesGrid.InstantiatedButtons
                .OfType<FavoriteSceneButton>()
                .Where(x => x.IsFavorite)
                .Select(x => x.Guid)
                .ToList();

            if (CurrentSearchType is FavoriteScenesSearch favoriteSearch)
            {
                favoriteSearch.SceneGroup.UpdateGuids(favoriteScenesGuid);
            }

            SaveFavoritesDataOnDisk();
        }

        public void SaveFavoritesDataOnDisk()
        {
            new FileInfo(favoriteScenesSavePath).Directory.Create();
            File.WriteAllText(favoriteScenesSavePath, JsonUtility.ToJson(favoriteScenes));
        }

        public void UpdateDropdownChoices()
        {
            searchTypeDropdown.choices = searches.Select(x => x.Label).ToList();
        }

        public void RefreshOverlay()
        {
            scenesGrid.RefreshGrid(CurrentSearchType);
        }


        #endregion

        #region Private Methods

        private void DeactivateAllOptions()
        {
            InputField.style.display = DisplayStyle.None;

            favoritesToolbar.EnableToolbar(false);
        }

        #endregion
    }
}