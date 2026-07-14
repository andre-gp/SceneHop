using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class FavoriteScenesSearch : SearchType
    {
        private SceneButton[] scenes;

        private SceneGroup sceneGroup;
        public SceneGroup SceneGroup => this.sceneGroup;

        public FavoriteScenesSearch(SearchField searchField, SceneGroup sceneGroup) : base(searchField)
        {
            this.sceneGroup = sceneGroup;
        }

        public override string Label => sceneGroup.GroupName;

        public override string TextValue 
        { 
            get => this.sceneGroup.GroupName; 
            set 
            {
                this.sceneGroup.GroupName = value;
                
                int previousIndex = searchField.SearchTypeDropdown.index;
                searchField.UpdateDropdownChoices();
                searchField.SearchTypeDropdown.index = previousIndex;
                searchField.SaveFavoritesDataOnDisk();
            } 
        }

        #region Public Methods

        public override SceneButton[] InstantiateButtons(VisualElement root)
        {
            if (searchField.FavoritesToolbar.IsEditing)
            {
                var guidsList = AssetDatabase.FindAssets("t:scene", new string[] { "Assets/" }).ToList();

                // Show favorites first
                guidsList.Sort((x, y) => SceneGroup.Guids.Contains(y).CompareTo(SceneGroup.Guids.Contains(x)));

                return guidsList.Select(x => new FavoriteSceneButton(root, x, sceneGroup, searchField.FavoritesToolbar)).ToArray();
            }
            else
            {
                return SceneGroup.Guids
                    .Where(x => !string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(x)))
                    .Select(x => new SceneButton(root, x)).ToArray();
            }
        }

        public override void InitSearch()
        {
            searchField.FavoritesToolbar.EnableToolbar(true);

            searchField.InputField.textEdition.placeholder = "Group Name";
            searchField.InputField.value = TextValue;
        }

        public override void AddNoSceneElements(VisualElement root)
        {
            Label label = new Label("Click the edit button to add some favorite scenes to this group!");
            label.AddToClassList("empty-label");
            root.Add(label);
        }

        #endregion
    }
}
