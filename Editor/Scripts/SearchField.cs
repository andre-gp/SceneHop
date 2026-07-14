using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        private const string ADD_GROUP_OPTION = "Add new group...";

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

        private bool requestedRefresh = false;

        /// <summary>
        /// Last selected search type, used to restore the dropdown when the
        /// 'Add new group...' option is cancelled.
        /// </summary>
        private int lastValidIndex = 0;

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
            favoritesToolbar.OnDelete += OnDeleteFavorites;

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
                // The last choice is not a search type; it opens the group creation dialog.
                if (searchTypeDropdown.index >= searches.Count)
                {
                    AddNewGroupFromDropdown();
                    return;
                }

                lastValidIndex = searchTypeDropdown.index;

                DeactivateAllOptions();

                CurrentSearchType.InitSearch();

                RefreshOverlay();

            });

            DeactivateAllOptions();

            data.DropdownIndex = Mathf.Clamp(data.DropdownIndex, 0, searches.Count - 1);
            searchTypeDropdown.index = data.DropdownIndex;
            lastValidIndex = data.DropdownIndex;

            searchTypeDropdown.SetBinding(nameof(searchTypeDropdown.index), new DataBinding()
            {
                bindingMode = BindingMode.TwoWay,
                dataSourcePath = PropertyPath.FromName(nameof(data.DropdownIndex))
            });

            searches[searchTypeDropdown.index].InitSearch();

            RefreshOverlay();
        }

        private void LoadFavorites()
        {
            favoriteScenesSavePath = Application.dataPath + "/../Library/SceneHop/favorites.json";

            try
            {
                if (File.Exists(favoriteScenesSavePath))
                {
                    favoriteScenes = JsonUtility.FromJson<FavoriteScenesData>(File.ReadAllText(favoriteScenesSavePath));
                }
            }
            catch (Exception e)
            {
                File.Copy(favoriteScenesSavePath, favoriteScenesSavePath + ".bak", true);
                Debug.LogWarning($"SceneHop: Failed to load '{favoriteScenesSavePath}'. Falling back to empty favorites. " +
                    $"The previous file was backed up as 'favorites.json.bak'.\n{e.Message}");
            }

            if (favoriteScenes == null)
            {
                favoriteScenes = new FavoriteScenesData();
            }

            favoriteScenes.Normalize();
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

        private void OnDeleteFavorites()
        {
            if(CurrentSearchType is FavoriteScenesSearch favoriteSearch)
            {
                if (EditorUtility.DisplayDialog("Delete this group?",
                    $"Are you sure you want to delete '{favoriteSearch.SceneGroup.GroupName}'",
                    "Delete",
                    "Cancel"))
                {
                    int previousIndex = searchTypeDropdown.index;
                    favoriteScenes.SceneGroups.Remove(favoriteSearch.SceneGroup);
                    searches.Remove(favoriteSearch);

                    SaveFavoritesDataOnDisk();

                    UpdateDropdownChoices();

                    searchTypeDropdown.index = Mathf.Clamp(previousIndex, 0, searches.Count - 1);

                    DeactivateAllOptions();

                    CurrentSearchType.InitSearch();

                    RefreshOverlay();
                }
            }
        }

        public void SaveFavoritesDataOnDisk()
        {
            new FileInfo(favoriteScenesSavePath).Directory.Create();
            File.WriteAllText(favoriteScenesSavePath, JsonUtility.ToJson(favoriteScenes));
        }

        public void UpdateDropdownChoices()
        {
            var choices = searches.Select(x => x.Label).ToList();
            choices.Add(ADD_GROUP_OPTION);

            searchTypeDropdown.choices = choices;
        }

        /// <summary>
        /// Always refreshes next frame, to avoid executing duplicate calls.
        /// </summary>
        public void RefreshOverlay()
        {
            if (!requestedRefresh)
            {
                requestedRefresh = true;

                // The continued execution is intended.
                RefreshNextFrame();
            }
        }

        private async Task RefreshNextFrame()
        {
            await Task.Delay(1);

            try
            {
                scenesGrid?.RefreshGrid(CurrentSearchType);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                requestedRefresh = false;
            }
        }


        #endregion

        #region Private Methods

        private void DeactivateAllOptions()
        {
            InputField.style.display = DisplayStyle.None;

            favoritesToolbar.EnableToolbar(false);
        }

        private void AddNewGroupFromDropdown()
        {
            if (EditorUtility.DisplayDialog("Favorite Scenes", "Create a new favorite scenes group?", "Create", "Cancel"))
            {
                searches.Add(new FavoriteScenesSearch(this, favoriteScenes.AddNewSceneGroup()));

                UpdateDropdownChoices();

                SaveFavoritesDataOnDisk();

                searchTypeDropdown.index = searches.Count - 1;
            }
            else
            {
                searchTypeDropdown.index = lastValidIndex;
            }
        }

        #endregion
    }
}