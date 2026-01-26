using System;

namespace GameConfig
{
    [Serializable]
    public sealed class PlanetConfigData : IConfig<int>
    {
        public int id { get; set; }

        int IConfig<int>.Id => id;

        public UnityEngine.Sprite icon { get; set; }
        public string localizationId { get; set; }
        public string planetName { get; set; }
        public string description { get; set; }
        public UnityEngine.Vector2 radiusRange { get; set; }
        public UnityEngine.Vector2Int massRange { get; set; }
        public UnityEngine.Vector2Int integrityRange { get; set; }
        public UnityEngine.Vector2Int stabilityRange { get; set; }
        public UnityEngine.Vector2Int fluxRange { get; set; }
        public System.Collections.Generic.List<GameConfig.AbilityAndStragtegy> presetAbilities { get; set; }
        public bool canUseActiveAbility { get; set; }
        public bool canUsePassiveAbility { get; set; }
        public string name { get; set; }
        public UnityEngine.HideFlags hideFlags { get; set; }
    }
}
