using System;

namespace GameConfig
{
    [Serializable]
    public sealed class ModifierConfigData : IConfig<int>
    {
        public int Id { get; set; }

        int IConfig<int>.Id => Id;

        public float Duration { get; set; }
        public int MaxStack { get; set; }
        public GameConfig.ModifierStackRule StackRule { get; set; }
        public System.Collections.Generic.List<string> Tags { get; set; }
        public GameConfig.ModifierOpType OpType { get; set; }
        public System.Collections.Generic.List<GameConfig.ModifierKeyValueOp> ValueOps { get; set; }
        public bool UseSpecLogic { get; set; }
        public string LogicType { get; set; }
        public string name { get; set; }
        public UnityEngine.HideFlags hideFlags { get; set; }
    }
}
