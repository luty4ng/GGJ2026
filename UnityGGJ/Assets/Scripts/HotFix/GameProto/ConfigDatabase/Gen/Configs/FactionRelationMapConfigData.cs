using System;

namespace GameConfig
{
    [Serializable]
    public sealed class FactionRelationMapConfigData : IConfig<int>
    {
        public int Id { get; set; }

        int IConfig<int>.Id => Id;

        public UnityEngine.Vector2Int Hostile { get; set; }
        public UnityEngine.Vector2Int Friendly { get; set; }
        public System.Collections.Generic.List<GameConfig.FactionRelationMap.FactionPairState> InitialRelations { get; set; }
        public System.Collections.Generic.List<GameConfig.FactionDefinition> Factions { get; set; }
        public string name { get; set; }
        public UnityEngine.HideFlags hideFlags { get; set; }
    }
}
