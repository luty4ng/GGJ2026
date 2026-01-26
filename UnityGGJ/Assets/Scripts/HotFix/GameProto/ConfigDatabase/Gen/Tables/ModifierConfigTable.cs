using System.Collections.Generic;

namespace GameConfig
{
    public sealed class ModifierConfigTable : IConfigTable<int, ModifierConfigData>
    {
        private readonly Dictionary<int, ModifierConfigData> _dict = new Dictionary<int, ModifierConfigData>();

        public IReadOnlyDictionary<int, ModifierConfigData> Dict => _dict;

        public ModifierConfigTable(IEnumerable<GameConfig.ModifierConfig> sources)
        {
            foreach (var src in sources)
            {
                var cfg = ModifierConfigDataAdapter.FromSource(src);
                _dict[cfg.Id] = cfg;
            }
        }

        public bool TryGet(int id, out ModifierConfigData cfg)
        {
            return _dict.TryGetValue(id, out cfg);
        }

        public ModifierConfigData Get(int id)
        {
            return _dict[id];
        }

    }
}
