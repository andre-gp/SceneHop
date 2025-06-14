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

                searchField.UpdateDropdownChoices();
            } 
        }

        public override void InitSearch()
        {
            searchField.FavoritesToolbar.style.display = DisplayStyle.Flex;
        }

        public override string[] RetrieveGuids()
        {
            return new string[] { };
        }
    }
}
