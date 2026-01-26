using System.Collections.Generic;
using UnityEngine;

namespace GameConfig
{
    public interface IConfig<TKey>
    {
        TKey Id { get; }
    }

    public interface IConfigTable<TKey, TConfig> where TConfig : IConfig<TKey>
    {
        IReadOnlyDictionary<TKey, TConfig> Dict { get; }
        TConfig Get(TKey id);
        bool TryGet(TKey id, out TConfig cfg);
    }
}
