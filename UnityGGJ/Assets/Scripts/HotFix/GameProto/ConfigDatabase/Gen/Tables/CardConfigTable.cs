using System.Collections.Generic;

namespace GameConfig
{
    public sealed class CardConfigTable : IConfigTable<int, CardConfigData>
    {
        private readonly Dictionary<int, CardConfigData> _dict = new Dictionary<int, CardConfigData>();

        public IReadOnlyDictionary<int, CardConfigData> Dict => _dict;

        public CardConfigTable(IEnumerable<GameConfig.CardConfig> sources)
        {
            foreach (var src in sources)
            {
                var cfg = CardConfigDataAdapter.FromSource(src);
                _dict[cfg.Id] = cfg;
            }
        }

        public bool TryGet(int id, out CardConfigData cfg)
        {
            return _dict.TryGetValue(id, out cfg);
        }

        public CardConfigData Get(int id)
        {
            return _dict[id];
        }

    }
}
