using System;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class FavoriteScenesToolbar
    {
        #region Callbacks

        public Action<bool> OnEnableEditing;

        public Action OnSave;

        public Action OnDelete;

        #endregion

        #region Member Fields
        private bool isEditing = false;
        public bool IsEditing => this.isEditing;
        #endregion

        #region Components
        private SearchField searchField;
        private VisualElement toolbarRoot;
        private Button editButton;
        private Button saveButton;
        private Button cancelButton;
        #endregion

        public FavoriteScenesToolbar(SearchField searchField)
        {
            this.searchField = searchField;

            VisualElement root = searchField.Root;

            toolbarRoot = root.Q<VisualElement>("toolbar-favorites");            

            editButton = root.Q<Button>("button-favorites-edit");
            editButton.clickable.clicked += EditFavorites;

            saveButton = root.Q<Button>("button-favorites-save");
            saveButton.clickable.clicked += SaveFavorites;

            cancelButton = root.Q<Button>("button-favorites-delete");
            cancelButton.clickable.clicked += DeleteFavoriteGroup;
        }

        #region Public Methods
        public void EnableToolbar(bool enable)
        {
            toolbarRoot.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;

            if (enable)
                ActivateEditMode(false);
        }
        #endregion

        #region Private Methods
        private void ActivateEditMode(bool activate)
        {
            isEditing = activate;

            editButton.style.display = activate ? DisplayStyle.None : DisplayStyle.Flex;
            saveButton.style.display = activate ? DisplayStyle.Flex : DisplayStyle.None;
            cancelButton.style.display = activate ? DisplayStyle.Flex : DisplayStyle.None;
            searchField.InputField.style.display = activate ? DisplayStyle.Flex : DisplayStyle.None;

            searchField.RefreshOverlay();

            OnEnableEditing?.Invoke(activate);
        }

        public void EditFavorites()
        {
            ActivateEditMode(true);
        }

        private void SaveFavorites()
        {
            OnSave?.Invoke();

            ActivateEditMode(false);
        }

        private void DeleteFavoriteGroup()
        {
            OnDelete?.Invoke();
        }
        #endregion

    }
}
