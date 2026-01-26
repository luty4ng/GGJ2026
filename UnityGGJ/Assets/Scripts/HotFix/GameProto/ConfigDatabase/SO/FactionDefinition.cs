using UnityEngine;

namespace GameConfig
{
    [CreateAssetMenu(menuName = "FoldingCosmos/Faction/FactionDefinition")]
    public class FactionDefinition : ScriptableObject
    {
        public int Id;
        public string Name;
        public Color Color;
        public Sprite Icon;
    }
}