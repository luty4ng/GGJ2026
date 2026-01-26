using System.Collections.Generic;

namespace GameConfig
{
    public sealed class FactionRelationMapConfigTable : IConfigTable<int, FactionRelationMapConfigData>
    {
        private readonly Dictionary<int, FactionRelationMapConfigData> _dict = new Dictionary<int, FactionRelationMapConfigData>();

        public IReadOnlyDictionary<int, FactionRelationMapConfigData> Dict => _dict;

        public FactionRelationMapConfigTable(IEnumerable<GameConfig.FactionRelationMap> sources)
        {
            foreach (var src in sources)
            {
                var cfg = FactionRelationMapConfigDataAdapter.FromSource(src);
                _dict[cfg.Id] = cfg;
            }
        }

        public bool TryGet(int id, out FactionRelationMapConfigData cfg)
        {
            return _dict.TryGetValue(id, out cfg);
        }

        public FactionRelationMapConfigData Get(int id)
        {
            return _dict[id];
        }

    }
}
