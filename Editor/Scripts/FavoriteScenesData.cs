using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneHop.Editor
{
    [System.Serializable]
    public class FavoriteScenesData
    {
        [SerializeField] List<SceneGroup> favoritesData = new List<SceneGroup>();

        public SceneGroup AddNewSceneGroup()
        {
            var group = new SceneGroup(favoritesData.Count.ToString());
            favoritesData.Add(group);
            return group;
        }
    }
}

[Serializable]
public class SceneGroup
{
    [SerializeField] string groupName;
    public string GroupName { get => this.groupName; set => this.groupName = value; }

    [SerializeField] List<string> guids;

    public SceneGroup(string groupName) : this(groupName, new List<string>())
    {
        
    }
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
