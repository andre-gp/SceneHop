using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class NameSearch : SearchType
    {
        public NameSearch(SceneOverlayData data) : base(data)
        {
        }

        public override string Label => "Name";

        public override string TextValue 
        { 
            get => data.CurrentName; 
            set => data.CurrentName = value; 
        }

        public override string[] RetrieveGuids()
        {

            return AssetDatabase.FindAssets(TextValue + " t:scene", new string[] { "Assets/" });
        }

        public override void InitSearch(TextField textField)
        {
            textField.style.display = DisplayStyle.Flex;
            textField.textEdition.placeholder = "SampleScene";
            textField.value = TextValue;
        }
    }
}
