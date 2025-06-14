using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class NameSearch : SearchType
    {
        public NameSearch(SearchField searchField) : base(searchField)
        {
        }

        public override string Label => "Name";

        public override string TextValue 
        { 
            get => searchField.Data.CurrentName;
            set => searchField.Data.CurrentName = value; 
        }

        public override string[] RetrieveGuids()
        {

            return AssetDatabase.FindAssets(TextValue + " t:scene", new string[] { "Assets/" });
        }

        public override void InitSearch()
        {
            searchField.InputField.style.display = DisplayStyle.Flex;
            searchField.InputField.textEdition.placeholder = "SampleScene";
            searchField.InputField.value = TextValue;
        }
    }
}
