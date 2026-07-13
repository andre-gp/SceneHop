using System.Linq;
using UnityEditor;
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
            string currentPath = searchField.Data.CurrentPath;

            if (string.IsNullOrEmpty(currentPath))
            {
                currentPath = SceneOverlayData.DEFAULT_PATH;
            }

            string[] paths = currentPath.Split(';')
                .Select(x => x.Trim().TrimEnd('/'))
                .Where(x => !string.IsNullOrEmpty(x) && AssetDatabase.IsValidFolder(x))
                .ToArray();

            if (paths.Length == 0)
            {
                return new SceneButton[0];
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
