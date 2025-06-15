using System.Linq;
using UnityEditor;
using UnityEngine;
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
            } 
        }
        public override SceneButton[] InstantiateButtons(VisualElement root)
        {
            if (searchField.FavoritesToolbar.IsEditing)
            {
                var guids = AssetDatabase.FindAssets("t:scene", new string[] { "Assets/" });

                return guids.Select(x => new FavoriteSceneButton(root, x, sceneGroup, searchField.FavoritesToolbar)).ToArray();
            }
            else
            {
                return SceneGroup.Guids.Select(x => new SceneButton(root, x)).ToArray();
            }
        }

        public override void InitSearch()
        {
            searchField.FavoritesToolbar.EnableToolbar(true);

            searchField.InputField.textEdition.placeholder = "Group Name";
            searchField.InputField.value = TextValue;
        }
    }
}
