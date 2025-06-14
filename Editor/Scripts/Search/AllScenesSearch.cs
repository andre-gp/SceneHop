using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class AllScenesSearch : SearchType
    {
        public AllScenesSearch(SearchField searchField) : base(searchField)
        {
        }

        public override string Label => "All Scenes";
        public override string TextValue { get => ""; set { } }

        public override string[] RetrieveGuids()
        {
            return AssetDatabase.FindAssets("t:scene", new string[] { "Assets/" });
        }

        public override void InitSearch()
        {
            
        }

    }
}
