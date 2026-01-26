using System.Collections.Generic;

namespace GameConfig
{
    public sealed class ItemConfigTable : IConfigTable<int, ItemConfigData>
    {
        private readonly Dictionary<int, ItemConfigData> _dict = new Dictionary<int, ItemConfigData>();

        public IReadOnlyDictionary<int, ItemConfigData> Dict => _dict;

        public ItemConfigTable(IEnumerable<GameConfig.ItemConfig> sources)
        {
            foreach (var src in sources)
            {
                var cfg = ItemConfigDataAdapter.FromSource(src);
                _dict[cfg.Id] = cfg;
            }
        }

        public bool TryGet(int id, out ItemConfigData cfg)
        {
            return _dict.TryGetValue(id, out cfg);
        }

        public ItemConfigData Get(int id)
        {
            return _dict[id];
        }

    }
}
