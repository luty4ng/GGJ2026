using System;

namespace GameConfig
{
    [Serializable]
    public sealed class AbilityConfigData : IConfig<int>
    {
        public int Id { get; set; }

        int IConfig<int>.Id => Id;

        public string AbilityName { get; set; }
        public string AbilityDescription { get; set; }
        public GameConfig.AbilityCategory AbilityCategory { get; set; }
        public GameConfig.AbilityElement AbilityElement { get; set; }
        public string AbilityTypeFullName { get; set; }
        public float Cooldown { get; set; }
        public float CastCost { get; set; }
        public float CastRange { get; set; }
        public bool RequireTarget { get; set; }
        public float ChannelTime { get; set; }
        public float EffectTime { get; set; }
        public UnityEngine.Sprite Icon { get; set; }
        public string name { get; set; }
        public UnityEngine.HideFlags hideFlags { get; set; }
    }
}
