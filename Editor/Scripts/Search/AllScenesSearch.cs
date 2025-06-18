using System.Linq;
using UnityEditor;
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

        public override SceneButton[] InstantiateButtons(VisualElement root)
        {
            var guids = AssetDatabase.FindAssets("t:scene", new string[] { "Assets/" });

            return guids.Select(x => new SceneButton(root, x)).ToArray();
        }


        public override void InitSearch()
        {
            
        }

    }
}
