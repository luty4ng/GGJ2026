using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameLogic
{
    [System.Serializable]
    public class Serialization<T>
    {
        [SerializeField] List<T> target;

        public List<T> ToList()
        {
            return target;
        }

        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }
    
    [System.Serializable]
    public class SerializedDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] List<TKey> keys;
        [SerializeField] List<TValue> values;

        Dictionary<TKey, TValue> target;

        public Dictionary<TKey, TValue> ToDictionary()
        {
            return target;
        }

        public SerializedDictionary()
        {
            this.target = new Dictionary<TKey, TValue>();
        }

        public SerializedDictionary(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            var count = System.Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            for (var i = 0; i < count; ++i)
            {
                target.Add(keys[i], values[i]);
            }
        }
    }
}