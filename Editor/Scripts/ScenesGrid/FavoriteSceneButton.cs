using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class FavoriteSceneButton : SceneButton
    {
        private SceneGroup sceneGroup;

        private bool isEditing;

        private bool isFavorite;
        public bool IsFavorite => this.isFavorite;
        public FavoriteSceneButton(VisualElement root, string guid, SceneGroup sceneGroup, FavoriteScenesToolbar toolbar) : base(root, guid)
        {
            this.sceneGroup = sceneGroup;

            sceneGroup.OnModifyGroup += OnModifyGroup;

            toolbar.OnEnableEditing += SetEditing;

            SetEditing(toolbar.IsEditing);

            OnModifyGroup();

            UpdateVisual();
        }

        private void OnModifyGroup()
        {
            this.isFavorite = sceneGroup.Guids.Contains(guid);
        }

        private void SetEditing(bool isEditing)
        {
            this.isEditing = isEditing;
        }

        private void UpdateVisual()
        {
            if (isEditing && isFavorite)
                button.AddToClassList("favorite-button");
            else
                button.RemoveFromClassList("favorite-button");
        }

        protected override void OnClickButton()
        {
            if (isEditing)
            {
                isFavorite = !isFavorite;
                UpdateVisual();
            }
            else
            {
                base.OnClickButton();
            }
        }

        public override void DestroyButton()
        {
            base.DestroyButton();

            if (sceneGroup != null)
            {
                sceneGroup.OnModifyGroup -= OnModifyGroup;
            }
        }
    }
}
