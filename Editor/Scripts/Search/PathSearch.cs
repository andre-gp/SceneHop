using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class PathSearch : SearchType
    {
        public PathSearch(SceneOverlayData data) : base(data)
        {
        }

        public override string Label => "Path";

        public override string TextValue
        {
            get => data.CurrentPath;
            set => data.CurrentPath = value;
        }

        public override string[] RetrieveGuids()
        {
            string[] paths = data.CurrentPath.Split(';');

            if (string.IsNullOrEmpty(paths[0]))
            {
                paths[0] = SceneOverlayData.DEFAULT_PATH;
            }

            return AssetDatabase.FindAssets("t:scene", paths);
        }

        public override void InitSearch(TextField textField)
        {
            textField.style.display = DisplayStyle.Flex;
            textField.textEdition.placeholder = SceneOverlayData.DEFAULT_PATH;
            textField.value = TextValue;
        }
    }
}
