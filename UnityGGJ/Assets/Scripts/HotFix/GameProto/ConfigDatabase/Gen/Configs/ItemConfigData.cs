using System;

namespace GameConfig
{
    [Serializable]
    public sealed class ItemConfigData : IConfig<int>
    {
        public int Id { get; set; }

        int IConfig<int>.Id => Id;

        public UnityEngine.Sprite Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanSell { get; set; }
        public int Price { get; set; }
        public bool CanStack { get; set; }
        public int MaxStack { get; set; }
        public GameConfig.ItemTier Tier { get; set; }
        public GameConfig.ItemPhysicalState PhysicalState { get; set; }
        public GameConfig.ItemStats Stats { get; set; }
        public GameConfig.ItemStorageCondition StorageCondition { get; set; }
        public string name { get; set; }
        public UnityEngine.HideFlags hideFlags { get; set; }
    }
}
