using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class PathSearch : SearchType
    {
        public PathSearch(SearchField searchField) : base(searchField)
        {
        }

        public override string Label => "Path";

        public override string TextValue
        {
            get => searchField.Data.CurrentPath;
            set => searchField.Data.CurrentPath = value;
        }
        public override SceneButton[] InstantiateButtons(VisualElement root)
        {
            string[] paths = searchField.Data.CurrentPath.Split(';');

            if (string.IsNullOrEmpty(paths[0]))
            {
                paths[0] = SceneOverlayData.DEFAULT_PATH;
            }

            var guids = AssetDatabase.FindAssets("t:scene", paths);

            return guids.Select(x => new SceneButton(root, x)).ToArray();
        }

        public override void InitSearch()
        {
            searchField.InputField.style.display = DisplayStyle.Flex;
            searchField.InputField.textEdition.placeholder = SceneOverlayData.DEFAULT_PATH;
            searchField.InputField.value = TextValue;
        }

    }
}
