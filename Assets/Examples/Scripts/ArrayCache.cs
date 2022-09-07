using System;
using System.Collections.Generic;
using Game.Extensions;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Allow allocation and cache of arrays (similar to ArrayPool) with some utilities like
    /// 'Returnable' IDiposable structs to be used on 'using' clauses.
    /// Also the cache is kept sorted by the last returned time, so if your cache is getting
    /// bigger and you have some unused arrays taking space, you can call 'TrimOlder' to 
    /// delete the most older unused arrays.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayCache<T>
    {
        public struct Returnable : IDisposable
        {
            public T[] Array { get; private set; }
            public Returnable(T[] array) => Array = array;
            public void Dispose() => Shared.Return(Array);
        }
        public struct ReturnableSegment : IDisposable
        {
            public ArraySegment<T> ArraySegment { get; private set; }
            public ReturnableSegment(ArraySegment<T> segment) => ArraySegment = segment;
            public void Dispose() => Shared.Return(ArraySegment.Array);
        }
        public static ArrayCache<T> Shared
        {
            get
            {
                if (_shared == null)
                { _shared = new(); }
                return _shared;
            }
        }
        public static bool HasShared => _shared != null;
        static ArrayCache<T> _shared;
        public int maxCachedArrays = 0; // 0 = unlimited
        public int maxCachedArrayPerLength = 0; // 0 = unlimited
        readonly SortedDictionary<int, List<(T[] array, float time)>> cacheSortedDictionary = new();
        public T[] Rent(int minimum_size)
        {
            minimum_size = Utils.FindNearestPowerOf2(minimum_size);
            foreach (var item in cacheSortedDictionary)
            {
                if (item.Key > minimum_size)
                {
                    if (cacheSortedDictionary[item.Key].Count > 0)
                    { return cacheSortedDictionary[item.Key].PopLast().array; }
                    else { break; }
                }
            }
            return new T[minimum_size];
        }
        public Returnable RentReturnable(int minimum_size) => new(Rent(minimum_size));
        public ArraySegment<T> RentSegment(int minimum_size) => new(Rent(minimum_size), 0, minimum_size);
        public ReturnableSegment RentReturnableSegment(int minimum_size) => new(RentSegment(minimum_size));
        public void Return(T[] array)
        {
            if (!cacheSortedDictionary.ContainsKey(array.Length))
            {
                cacheSortedDictionary.Add(array.Length, new());
            }
            cacheSortedDictionary[array.Length].Add((array: array, time: Time.time));
        }
        public void ReturnSegment(ArraySegment<T> segment) => Return(segment.Array);
        public void Trim()
        {
            cacheSortedDictionary.Clear();
        }
        public void TrimOlder(float older)
        {
            foreach (var pair in cacheSortedDictionary)
            {
                var should_trim = false;
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    if (pair.Value[i].time < older)
                    {
                        pair.Value.RemoveAt(i);
                        i--;
                        should_trim = true;
                    }
                }
                if (pair.Value.Count == 0) { cacheSortedDictionary.Remove(pair.Key); }
                else if (should_trim) { pair.Value.TrimExcess(); }
            }
        }
    }
}