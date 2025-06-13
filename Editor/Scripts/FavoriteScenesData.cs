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
