using System;
using System.Collections.Generic;

namespace GameLogic
{
    public delegate ref int IntRefGetter<TStats>(ref TStats stats);
    public sealed class IntRefRegistry<TStats>
    {
        private readonly Dictionary<string, IntRefGetter<TStats>> _byName = new(StringComparer.Ordinal);
        public void Register(string key, IntRefGetter<TStats> getter)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;
            if (getter == null)
                return;
            _byName.Add(key, getter);
        }

        public bool HasKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;
            return _byName.ContainsKey(key);
        }

        public IntRefGetter<TStats> Resolve(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;
            if (_byName.TryGetValue(key, out var g))
                return g;
            return null;
        }
    }
}
