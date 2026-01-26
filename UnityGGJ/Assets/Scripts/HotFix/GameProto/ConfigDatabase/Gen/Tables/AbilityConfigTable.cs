using System.Collections.Generic;

namespace GameConfig
{
    public sealed class AbilityConfigTable : IConfigTable<int, AbilityConfigData>
    {
        private readonly Dictionary<int, AbilityConfigData> _dict = new Dictionary<int, AbilityConfigData>();

        public IReadOnlyDictionary<int, AbilityConfigData> Dict => _dict;

        public AbilityConfigTable(IEnumerable<GameConfig.AbilityConfig> sources)
        {
            foreach (var src in sources)
            {
                var cfg = AbilityConfigDataAdapter.FromSource(src);
                _dict[cfg.Id] = cfg;
            }
        }

        public bool TryGet(int id, out AbilityConfigData cfg)
        {
            return _dict.TryGetValue(id, out cfg);
        }

        public AbilityConfigData Get(int id)
        {
            return _dict[id];
        }

    }
}
