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

        #region Member Fields        

        [SerializeField] private string currentPath = DEFAULT_PATH;
        public string CurrentPath 
        { 
            get => this.currentPath; 
            set 
            {
                if (value == currentPath)
                    return;

                currentPath = value; 
                InternalOnUpdateValue(); 
            } 
        }

        [SerializeField] private string currentName = "SampleScene";
        public string CurrentName 
        { 
            get => this.currentName; 
            set 
            {
                if (value == currentName)
                    return;

                currentName = value; 
                InternalOnUpdateValue(); 
            } 
        }

        #endregion

        #region Bound Fields

        [SerializeField] private int dropdownIndex = 0;
        [CreateProperty]
        public int DropdownIndex
        {
            get => this.dropdownIndex;
            set
            {
                if (value == dropdownIndex)
                    return;

                dropdownIndex = value;
                InternalOnUpdateValue();
            }
        }


        [SerializeField] private bool foldoutState = true;
        /// <summary>
        /// This field preserves the foldout value between multiple creations of the overlay
        /// </summary>
        [CreateProperty] public bool FoldoutState 
        { 
            get => this.foldoutState; 
            set 
            {
                if (value == foldoutState)
                    return;

                foldoutState = value;
                InternalOnUpdateValue(); 
            } 
        }

        [SerializeField] private float buttonScale = 1f;
        [CreateProperty] public float ButtonScale 
        { 
            get => this.buttonScale; 
            set 
            {
                if (value == buttonScale)
                    return;

                buttonScale = value; 
                InternalOnUpdateValue(); 
            } 
        }

        #endregion
        
        private void InternalOnUpdateValue()
        {            
            OnUpdateValue?.Invoke(this);            
        }
    }
}

