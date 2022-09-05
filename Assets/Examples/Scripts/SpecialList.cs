using System;
using System.Collections.Generic;
using Game.Extensions;

namespace Game
{
    /// <summary>
    /// My own implementation of List.
    /// Mostly so i could return a ArraySegment directly from the
    /// internal array, less garbage generated.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpecialList<T>
    {
        const int MIN_CAPACITY = 4;
        const int MAX_ARRAY_LENGTH = int.MaxValue;
        T[] array;
        public int Count { get; private set; }
        public int LastIndex => Count - 1;
        public T LastItem => array[LastIndex];
        public SpecialList() { array = new T[MIN_CAPACITY]; }
        public SpecialList(int capacity)
        { array = new T[Math.Clamp(capacity, MIN_CAPACITY, MAX_ARRAY_LENGTH)]; }
        public int Capacity
        {
            get => array.Length;
            set
            {
                if (value > Count && value < MAX_ARRAY_LENGTH)
                { Array.Resize(ref array, value); }
            }
        }
        public T this[int index]
        {
            get => array[index];
            set => array[index] = value;
        }
        public void Add(T item) => Insert(Count, item);
        public void AddRange(ArraySegment<T> range)
        {
            EnsureRangeCapacity(Count + range.Count);
            Array.Copy(range.Array, range.Offset, array, Count, range.Count);
            Count += range.Count;
        }
        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                Add(item);
            }
        }
        public void Insert(int index, T item)
        {
            if (Count >= Capacity)
            {
                var temp = new T[IncreaseCapacityStrategy(Count + 1)];
                if (index > 0)
                {
                    // Copy items from old array at left of the index to the new array (temp)
                    Array.Copy(array, 0, temp, 0, index);
                }
                if (index < Count)
                {
                    // Copy item from old array at right of the index to the new array (temp)
                    // With 1 offset.
                    Array.Copy(array, index, temp, index + 1, Count - index);
                }
                array = temp;
            }
            else if (index < Count)
            {
                // We dont need to copy any items  at left of the index.
                // Copy items at right of the index with 1 offset
                Array.Copy(array, index, array, index + 1, Count - index);
            }
            array[index] = item;
            Count++;
        }
        public void InsertRange(int index, ArraySegment<T> segment)
        {
            InsertRangePrepareSpace(index, segment.Count);
            // Copy the items informed on the segment into the open space
            Array.Copy(segment.Array, segment.Offset, array, index, segment.Count);
            Count += segment.Count;
        }
        public void InsertRange(int index, int count, IEnumerator<T> enumerator)
        {
            InsertRangePrepareSpace(index, count);
            // Copy the items informed on the segment into the open space
            for (int i = 0; i < count; i++)
            {
                if (!enumerator.MoveNext())
                {
                    throw new Exception($"Given enumerator ended before given count '{count}' calls.");
                }
                array[index + i] = enumerator.Current;
            }
            Count += count;
        }
        void InsertRangePrepareSpace(int index, int count)
        {
            if (Count + count >= Capacity)
            {
                var temp = new T[IncreaseCapacityStrategy(Count + count)];
                if (index > 0)
                {
                    // Copy items from old array at left of the index to the new array (temp)
                    Array.Copy(array, 0, temp, 0, index);
                }
                if (index < Count)
                {
                    // Copy item from old array at right of the index to the new array (temp)
                    // With segment.Count offset.
                    Array.Copy(array, index, temp, index + count, Count - index);
                }
                array = temp;
            }
            else if (index < Count)
            {
                // We dont need to copy any items  at left of the index.
                // Copy items at right of the index with segment.Count offset
                Array.Copy(array, index, array, index + count, Count - index);
            }
        }
        public void RemoveAt(int index) => RemoveRange(index, 1);
        public void RemoveRange(int index, int count)
        {
            // index must be between [0..Count[
            // count > 0 else: There's zero items to remove
            // Count > 0 else: There's nothing to copy or clear
            // Count >= index + count else: either index or count is greater than the original array
            if ((index >= 0 && index < Count) && count > 0 && Count > 0 && Count >= index + count)
            {
                // (index+count) < Count else: There's nothing at right to save/copy in place
                if (index + count < Count)
                { Array.Copy(array, index + count, array, index, Count - (index + count)); }
                Array.Clear(array, Count - count, count);
                Count -= count;
            }
        }
        public void EnsureRangeCapacity(int range)
        {
            if (array.Length < range)
            { Capacity = (int)IncreaseCapacityStrategy(range); }
        }
        public T Pop(int index)
        {
            try { return array[index]; }
            finally { RemoveAt(index); }
        }
        public T PopLast() => Pop(LastIndex);
        public ArraySegment<T> AsSegment(int index = 0, int count = 0)
        { return new(array, index, count == 0 ? (Count - index) : count); }
        public void Clear()
        {
            Array.Clear(array, 0, Count);
            Count = 0;
        }
        public void Reverse() => Array.Reverse(array, 0, Count);
        public IEnumerable<T> GetEnumerable()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return array[i];
            }
        }
        static long IncreaseCapacityStrategy(long requested)
        {
            return Math.Clamp((requested) / 2 * 3, MIN_CAPACITY, MAX_ARRAY_LENGTH);
        }
#if UNITY_EDITOR
        public void Print()
        {
            var items = new List<string>();
            for (int i = 0; i < Count; i++)
            {
                items.Add(array[i].ToString());
            }
            UnityEngine.Debug.Log($"[{string.Join(", ", items)}]");
        }
#endif
    }
}