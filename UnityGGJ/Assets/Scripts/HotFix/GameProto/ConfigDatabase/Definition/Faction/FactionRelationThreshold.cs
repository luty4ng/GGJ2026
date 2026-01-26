using UnityEngine;
using Sirenix.OdinInspector;

namespace GameConfig
{
    [System.Serializable]
    public class FactionRelationThreshold
    {
        [MinMaxSlider(-100, 100, true)] public Vector2Int Hostile;
        [MinMaxSlider(-100, 100, true)] public Vector2Int Friendly;
    }
}