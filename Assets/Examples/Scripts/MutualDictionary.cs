using System.Collections;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Two dictionaries kept in sync to allow fast lookup of values by keys 
    /// and lookup of keys by values.
    /// Key and Value types must be different.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class MutualDictionary<K, V> : IEnumerable<KeyValuePair<K, V>>
    {
        readonly Dictionary<K, V> keyValue = new();
        readonly Dictionary<V, K> valueKey = new();
        public int Count => keyValue.Count;
        public V this[K key] => keyValue[key];
        public K this[V value] => valueKey[value];
        public MutualDictionary()
        {
            if (typeof(K) == typeof(V))
            {
                throw new System.ArgumentException("Key and Value types must be different");
            }
        }
        public void Add(K key, V value)
        {
            keyValue.Add(key, value);
            valueKey.Add(value, key);
        }
        public void Remove(K key)
        {
            if (keyValue.TryGetValue(key, out var value))
            {
                keyValue.Remove(key);
                valueKey.Remove(value);
            }
        }
        public void Remove(V value)
        {
            if (valueKey.TryGetValue(value, out var key))
            {
                valueKey.Remove(value);
                keyValue.Remove(key);
            }
        }
        public bool TryFind(K key, out V value) => keyValue.TryGetValue(key, out value);
        public bool TryFind(V value, out K key) => valueKey.TryGetValue(value, out key);
        public void Clear()
        {
            keyValue.Clear();
            valueKey.Clear();
        }
        public IEnumerable<K> Keys => keyValue.Keys;
        public IEnumerable<V> Values => keyValue.Values;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return keyValue.GetEnumerator();
        }
    }
}