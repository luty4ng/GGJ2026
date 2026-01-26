using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityGameFramework.Runtime
{
    public sealed class ObservableList<T> : IList<T>, IReadOnlyList<T>
    {
        private readonly List<T> m_items;
        public Action<T> OnElementAdd { get; set; }
        public Action<T> OnElementRemove { get; set; }
        public Action OnElementClear { get; set; }

        public ObservableList()
        {
            m_items = new List<T>();
        }

        public ObservableList(int capacity)
        {
            m_items = new List<T>(capacity);
        }

        public ObservableList(IEnumerable<T> items)
        {
            m_items = items == null ? new List<T>() : new List<T>(items);
        }

        public int Count => m_items.Count;
        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => m_items[index];
            set
            {
                var oldValue = m_items[index];
                if (EqualityComparer<T>.Default.Equals(oldValue, value))
                    return;
                m_items[index] = value;
                OnElementRemove?.Invoke(oldValue);
                OnElementAdd?.Invoke(value);
            }
        }

        public void Add(T item)
        {
            m_items.Add(item);
            OnElementAdd?.Invoke(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                return;
            foreach (var item in items)
            {
                m_items.Add(item);
                OnElementAdd?.Invoke(item);
            }
        }

        public void Clear()
        {
            if (m_items.Count == 0)
                return;
            m_items.Clear();
            OnElementClear?.Invoke();
        }

        public bool Contains(T item) => m_items.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => m_items.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => m_items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_items.GetEnumerator();
        public int IndexOf(T item) => m_items.IndexOf(item);

        public void Insert(int index, T item)
        {
            m_items.Insert(index, item);
            OnElementAdd?.Invoke(item);
        }

        public bool Remove(T item)
        {
            if (!m_items.Remove(item))
                return false;
            OnElementRemove?.Invoke(item);
            return true;
        }

        public void RemoveAt(int index)
        {
            var item = m_items[index];
            m_items.RemoveAt(index);
            OnElementRemove?.Invoke(item);
        }

        public void ReplaceAll(IEnumerable<T> items)
        {
            if (m_items.Count > 0)
            {
                m_items.Clear();
                OnElementClear?.Invoke();
            }
            if (items == null)
                return;
            foreach (var item in items)
            {
                m_items.Add(item);
                OnElementAdd?.Invoke(item);
            }
        }

        public void NotifyClear() => OnElementClear?.Invoke();
    }

    public sealed class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> m_dict;
        public event Action<TKey, TValue> OnElementAdd;
        public event Action<TKey, TValue> OnElementRemove;
        public event Action OnElementClear;

        public ObservableDictionary()
        {
            m_dict = new Dictionary<TKey, TValue>();
        }

        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            m_dict = new Dictionary<TKey, TValue>(comparer);
        }

        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer = null)
        {
            m_dict = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public int Count => m_dict.Count;
        public bool IsReadOnly => false;
        public ICollection<TKey> Keys => m_dict.Keys;
        public ICollection<TValue> Values => m_dict.Values;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => m_dict.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => m_dict.Values;

        public TValue this[TKey key]
        {
            get => m_dict[key];
            set
            {
                if (m_dict.TryGetValue(key, out var existing))
                {
                    if (EqualityComparer<TValue>.Default.Equals(existing, value))
                        return;
                    m_dict[key] = value;
                    OnElementRemove?.Invoke(key, existing);
                    OnElementAdd?.Invoke(key, value);
                    return;
                }
                m_dict[key] = value;
                OnElementAdd?.Invoke(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            m_dict.Add(key, value);
            OnElementAdd?.Invoke(key, value);
        }

        public bool ContainsKey(TKey key) => m_dict.ContainsKey(key);

        public bool Remove(TKey key)
        {
            if (!m_dict.TryGetValue(key, out var value))
                return false;
            if (!m_dict.Remove(key))
                return false;
            OnElementRemove?.Invoke(key, value);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value) => m_dict.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)m_dict).Add(item);
            OnElementAdd?.Invoke(item.Key, item.Value);
        }

        public void Clear()
        {
            if (m_dict.Count == 0)
                return;
            m_dict.Clear();
            OnElementClear?.Invoke();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)m_dict).Contains(item);
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)m_dict).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (!((IDictionary<TKey, TValue>)m_dict).Remove(item))
                return false;
            OnElementRemove?.Invoke(item.Key, item.Value);
            return true;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_dict.GetEnumerator();

        public void NotifyClear() => OnElementClear?.Invoke();
    }

    public sealed class ObservableQueue<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly Queue<T> m_queue;
        public event Action<T> OnElementAdd;
        public event Action<T> OnElementRemove;
        public event Action OnElementClear;

        public ObservableQueue()
        {
            m_queue = new Queue<T>();
        }

        public ObservableQueue(int capacity)
        {
            m_queue = new Queue<T>(capacity);
        }

        public ObservableQueue(IEnumerable<T> items)
        {
            m_queue = items == null ? new Queue<T>() : new Queue<T>(items);
        }

        public int Count => m_queue.Count;

        public void Enqueue(T item)
        {
            m_queue.Enqueue(item);
            OnElementAdd?.Invoke(item);
        }

        public T Dequeue()
        {
            var item = m_queue.Dequeue();
            OnElementRemove?.Invoke(item);
            return item;
        }

        public bool TryDequeue(out T item)
        {
            if (!m_queue.TryDequeue(out item))
                return false;
            OnElementRemove?.Invoke(item);
            return true;
        }

        public T Peek() => m_queue.Peek();

        public void Clear()
        {
            if (m_queue.Count == 0)
                return;
            m_queue.Clear();
            OnElementClear?.Invoke();
        }

        public IEnumerator<T> GetEnumerator() => m_queue.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_queue.GetEnumerator();

        public void NotifyClear() => OnElementClear?.Invoke();
    }

    public sealed class ObservableHashSet<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        private readonly HashSet<T> m_set;
        public event Action<T> OnElementAdd;
        public event Action<T> OnElementRemove;
        public event Action OnElementClear;

        public ObservableHashSet()
        {
            m_set = new HashSet<T>();
        }

        public ObservableHashSet(IEqualityComparer<T> comparer)
        {
            m_set = new HashSet<T>(comparer);
        }

        public ObservableHashSet(IEnumerable<T> items, IEqualityComparer<T> comparer = null)
        {
            m_set = items == null ? new HashSet<T>(comparer) : new HashSet<T>(items, comparer);
        }

        public int Count => m_set.Count;
        public bool IsReadOnly => false;

        public bool Add(T item)
        {
            if (!m_set.Add(item))
                return false;
            OnElementAdd?.Invoke(item);
            return true;
        }

        void ICollection<T>.Add(T item) => Add(item);

        public void Clear()
        {
            if (m_set.Count == 0)
                return;
            m_set.Clear();
            OnElementClear?.Invoke();
        }

        public bool Contains(T item) => m_set.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => m_set.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            if (!m_set.Remove(item))
                return false;
            OnElementRemove?.Invoke(item);
            return true;
        }

        public IEnumerator<T> GetEnumerator() => m_set.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_set.GetEnumerator();

        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
                return;
            foreach (var item in other)
            {
                if (m_set.Add(item))
                    OnElementAdd?.Invoke(item);
            }
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
                return;
            foreach (var item in other)
            {
                if (m_set.Remove(item))
                    OnElementRemove?.Invoke(item);
            }
        }

        public void NotifyClear() => OnElementClear?.Invoke();
    }
}


