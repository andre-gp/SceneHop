using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

namespace SceneHop.Editor
{
    [Serializable]
    public class SceneOverlayData
    {
        #region Default Values

        public const string DEFAULT_PATH = "Assets/Scenes/";

        #endregion

        #region Callbacks

        public Action<SceneOverlayData> OnUpdateValue;

        #endregion

        [SerializeField] List<SceneGroup> favoritesData = new List<SceneGroup>();

        [SerializeField] private string currentPath = DEFAULT_PATH;
        public string CurrentPath { get => this.currentPath; set { currentPath = value; InternalOnUpdateValue(); } }

        [SerializeField] private string currentName = "SampleScene";
        public string CurrentName { get => this.currentName; set { currentName = value; InternalOnUpdateValue(); } }


        [SerializeField] private bool foldoutState = true;
        /// <summary>
        /// This field preserves the foldout value between multiple creations of the overlay
        /// </summary>
        [CreateProperty] public bool FoldoutState { get => this.foldoutState; set { foldoutState = value; InternalOnUpdateValue(); } }

        [SerializeField] private float buttonScale = 1f;
        [CreateProperty] public float ButtonScale { get => this.buttonScale; set { buttonScale = value; InternalOnUpdateValue(); } }
        

        private void InternalOnUpdateValue()
        {
            Debug.Log("On Save Value");
            OnUpdateValue?.Invoke(this);            
        }

        bool ShallowEquals(SceneOverlayData other)
        {
            if (other != null &&
               this.CurrentPath == other.CurrentPath &&
               this.CurrentName == other.CurrentName &&
               this.FoldoutState == other.FoldoutState &&
               this.ButtonScale == other.ButtonScale
               )
            {
                return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            
        }

    }
}

[Serializable]
public class SceneGroup
{
    [SerializeField] string groupName;
    [SerializeField] List<string> guids;

    public SceneGroup(string groupName, List<string> guids)
    {
        this.groupName = groupName;
        this.guids = guids;
    }

    public bool Add(string guid)
    {
        if (!guids.Contains(guid))
        {
            guids.Add(guid);

            return true;
        }

        return false;
    }
}
