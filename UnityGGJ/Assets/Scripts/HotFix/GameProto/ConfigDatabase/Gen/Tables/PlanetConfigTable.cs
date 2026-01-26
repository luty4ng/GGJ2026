using System.Collections.Generic;

namespace GameConfig
{
    public sealed class PlanetConfigTable : IConfigTable<int, PlanetConfigData>
    {
        private readonly Dictionary<int, PlanetConfigData> _dict = new Dictionary<int, PlanetConfigData>();

        public IReadOnlyDictionary<int, PlanetConfigData> Dict => _dict;

        public PlanetConfigTable(IEnumerable<GameConfig.PlanetConfig> sources)
        {
            foreach (var src in sources)
            {
                var cfg = PlanetConfigDataAdapter.FromSource(src);
                _dict[cfg.id] = cfg;
            }
        }

        public bool TryGet(int id, out PlanetConfigData cfg)
        {
            return _dict.TryGetValue(id, out cfg);
        }

        public PlanetConfigData Get(int id)
        {
            return _dict[id];
        }

    }
}
