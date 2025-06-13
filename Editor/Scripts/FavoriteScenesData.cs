using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneHop.Editor
{
    [System.Serializable]
    public class FavoriteScenesData
    {
        [SerializeField] List<SceneGroup> favoritesData = new List<SceneGroup>();
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
