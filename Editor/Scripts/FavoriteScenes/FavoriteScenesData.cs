using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneHop.Editor
{
    [System.Serializable]
    public class FavoriteScenesData
    {
        [SerializeField] List<SceneGroup> favoritesData = new List<SceneGroup>();
        public List<SceneGroup> SceneGroups => this.favoritesData;

        public SceneGroup AddNewSceneGroup()
        {
            var group = new SceneGroup($"Favorites {favoritesData.Count + 1}");

            // An untitled/never-saved scene has an empty path, which would store an empty GUID.
            string activeScenePath = SceneManager.GetActiveScene().path;
            if (!string.IsNullOrEmpty(activeScenePath))
            {
                string guid = AssetDatabase.AssetPathToGUID(activeScenePath);
                if (!string.IsNullOrEmpty(guid))
                {
                    group.Guids.Add(guid);
                }
            }

            favoritesData.Add(group);
            return group;
        }

        public void Normalize()
        {
            if (favoritesData == null)
            {
                favoritesData = new List<SceneGroup>();
            }

            favoritesData.RemoveAll(group => group == null);

            foreach (var group in favoritesData)
            {
                group.Normalize();
            }
        }
    }
}

[Serializable]
public class SceneGroup
{
    #region Callbacks
    [NonSerialized] public Action OnModifyGroup;
    #endregion

    [SerializeField] string groupName;
    public string GroupName { get => this.groupName; set => this.groupName = value; }

    [SerializeField] List<string> guids;
    public List<string> Guids => this.guids;


    public SceneGroup(string groupName) : this(groupName, new List<string>())
    {

    }
    public SceneGroup(string groupName, List<string> guids)
    {
        this.groupName = groupName;
        this.guids = guids;
    }

    public void UpdateGuids(List<string> guids)
    {
        this.guids = guids;
    }

    public void Normalize()
    {
        if (groupName == null)
        {
            groupName = "Favorites";
        }

        if (guids == null)
        {
            guids = new List<string>();
        }
        else
        {
            guids.RemoveAll(guid => guid == null);
        }
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
