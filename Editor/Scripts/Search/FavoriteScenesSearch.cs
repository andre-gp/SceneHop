using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class FavoriteScenesSearch : SearchType
    {
        SceneGroup sceneGroup;

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

        public override void InitSearch()
        {
            searchField.FavoritesToolbar.EnableToolbar(true);

            searchField.InputField.textEdition.placeholder = "Group Name";
            searchField.InputField.value = TextValue;
        }

        public override string[] RetrieveGuids()
        {
            if (searchField.FavoritesToolbar.IsEditing)
            {
                return AssetDatabase.FindAssets("t:scene", new string[] { "Assets/" });
            }
            else
            {
                return new string[] { };
            }
        }
    }
}
