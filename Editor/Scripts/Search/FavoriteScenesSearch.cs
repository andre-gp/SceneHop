using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class FavoriteScenesSearch : SearchType
    {
        public FavoriteScenesSearch(SceneOverlayData data) : base(data)
        {
        }

        public override string Label => "Favorite";

        public override string TextValue { get => ""; set { } }

        public override void InitSearch(TextField textField)
        {
            
        }

        public override string[] RetrieveGuids()
        {
            return new string[] { };
        }
    }
}
