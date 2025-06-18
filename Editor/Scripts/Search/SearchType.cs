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

        public abstract SceneButton[] InstantiateButtons(VisualElement root);

        public abstract void InitSearch();

        public virtual void AddNoSceneElements(VisualElement root)
        {
            root.Add(new Label("Could not find any scenes!"));
        }
    }
}
