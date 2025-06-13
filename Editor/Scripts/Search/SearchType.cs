using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public abstract class SearchType
    {
        protected SceneOverlayData data;

        public SearchType(SceneOverlayData data)
        {
            this.data = data;
        }

        public abstract string Label { get; }

        public abstract string TextValue { get; set; }

        public abstract string[] RetrieveGuids();

        

        public abstract void InitSearch(TextField textField);
    }
}
