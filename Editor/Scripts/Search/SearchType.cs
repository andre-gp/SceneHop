using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public abstract class SearchType
    {
        protected SearchField searchField;

        public SearchType(SearchField data)
        {
            this.searchField = data;
        }

        public abstract string Label { get; }

        public abstract string TextValue { get; set; }

        public abstract string[] RetrieveGuids();

        

        public abstract void InitSearch();
    }
}
