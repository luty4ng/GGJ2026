using System;

namespace GameConfig
{
    [Serializable]
    public sealed class CardConfigData : IConfig<int>
    {
        public int Id { get; set; }

        int IConfig<int>.Id => Id;

        public UnityEngine.Sprite icon { get; set; }
        public string cardName { get; set; }
        public string description { get; set; }
        public int factionBelong { get; set; }
        public GameConfig.ETargetType targetType { get; set; }
        public GameConfig.ECardType cardType { get; set; }
        public int costWorkload { get; set; }
        public System.Collections.Generic.List<GameConfig.CostPair> costList { get; set; }
        public System.Collections.Generic.List<int> modifierIdList { get; set; }
        public string name { get; set; }
        public UnityEngine.HideFlags hideFlags { get; set; }
    }
}
