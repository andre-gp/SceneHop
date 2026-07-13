using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace SceneHop.Editor
{
    public class SceneButton
    {
        protected Button button;

        protected string guid;
        public string Guid => this.guid;

        protected string path;

        public SceneButton(VisualElement root, string guid)
        {
            this.guid = guid;
            this.path = AssetDatabase.GUIDToAssetPath(guid);

            button = new Button();

            button.clickable.clicked += () =>
            {
                OnClickButton();
            };

            button.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent callback) =>
            {
                callback.menu.AppendAction("Load Scene", (x) =>
                {
                    OpenScene();
                });

                callback.menu.AppendAction("Select Asset", (x) =>
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                });
            }));

            string fileName = Path.GetFileNameWithoutExtension(path);

            // Regex to add white spaces to 'CamelCasedNames -> Camel Cased Names'
            string btnName = Regex.Replace(fileName, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");

            btnName = btnName.Replace('_', ' ');
            btnName = btnName.Replace('-', ' ');

            button.text = btnName;
            button.AddToClassList("scene-button");

            root.Add(button);
        }

        protected virtual void OnClickButton()
        {
            OpenScene();
        }

        private void OpenScene()
        {
            // Re-resolve the path in case the scene was deleted or moved after this button was created.
            path = AssetDatabase.GUIDToAssetPath(guid);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"SceneHop: The scene '{button.text}' no longer exists.");
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
            }
        }

        public virtual void DestroyButton()
        {
            button.RemoveFromHierarchy();
        }
    }
}
