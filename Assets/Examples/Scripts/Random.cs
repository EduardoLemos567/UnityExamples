using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Simple random implementation using the builtin C# random.
    /// Has a globalState (it was before the new builtin Shared in .NET6)
    /// Can create random strings, good for testing stuff.
    /// Can pick random elements from a list or shuffle it.
    /// </summary>
    public class Random
    {
        static Random m_globalState = new();
        public static Random globalState
        {
            get
            {
                return m_globalState;
            }
            set
            {
                if (!(value is null))
                    m_globalState = value;
            }
        }
        System.Random state;
        public Random() => state = new();
        public Random(int seed) => state = new(seed);
        public Random(string seed) => state = new(seed.GetHashCode());
        const int ALPHABET_COUNT = 26;
        static readonly RangeInt numberRange = new(48, 57);
        static readonly RangeInt lowerRange = new(97, 122);
        static readonly RangeInt upperRange = new(65, 90);
        public bool GetTrueByChance(float chance)
        {
            return state.NextDouble() <= chance;
        }
        public int GetValueInt()
        { // return value between 0 <= x < int.MaxValue
            return state.Next();
        }
        public double GetValueDouble()
        { // return value between 0 <= x < 1.0
            return state.NextDouble();
        }
        public double GetValueInRange(double min_value, double max_value)
        {   // return value between min_value <= value < max_value
            return min_value + (max_value - min_value) * state.NextDouble();
        }
        public int GetValueInRange(int min_value, int max_value)
        {   // return value between min_value <= value < max_value
            return state.Next(min_value, max_value);
        }
        public double GetValueInRange(double max_value)
        {   // return value between min_value <= value < max_value
            return (max_value) * state.NextDouble();
        }
        public int GetValueInRange(int max_value)
        {   // return value between min_value <= value < max_value
            return state.Next(0, max_value);
        }
        public string GetString(int count, bool include_lower = true, bool include_upper = true)
        {
            return string.Create<(bool lower, bool upper)>(count, (include_lower, include_upper), CreateString);
        }
        static void CreateString(Span<char> result, (bool lower, bool upper) included)
        {
            var range = 10 + (included.lower ? ALPHABET_COUNT : 0) + (included.upper ? ALPHABET_COUNT : 0);
            for (int i = 0; i < result.Length; i++)
            {
                var value = globalState.GetValueInRange(range);
                byte direction = 0;
                if (value >= 10)
                {
                    if (value < (10 + ALPHABET_COUNT))
                    {
                        value -= 10;
                        if (!included.lower)
                        {
                            direction = 2;
                        }
                        else
                        {
                            direction = 1;
                        }
                    }
                    else
                    {
                        value -= (10 + ALPHABET_COUNT);
                        direction = 2;
                    }
                }
                switch (direction)
                {
                    case 0:
                        result[i] = (char)(numberRange.min + value);
                        break;
                    case 1:
                        result[i] = (char)(lowerRange.min + value);
                        break;
                    case 2:
                        result[i] = (char)(upperRange.min + value);
                        break;
                }
            }
        }
        public Color GetColor()
        {
            return new(
                (float)state.NextDouble(),
                (float)state.NextDouble(),
                (float)state.NextDouble(),
                1.0f
            );
        }
        public Vector2 GetVector2(float min_value, float max_value)
        {
            return new((float)GetValueInRange(min_value, max_value), (float)GetValueInRange(min_value, max_value));
        }
        public Vector3 GetVector3(float min_value, float max_value)
        {
            return new((float)GetValueInRange(min_value, max_value), (float)GetValueInRange(min_value, max_value), (float)GetValueInRange(min_value, max_value));
        }
        public void ShuffleList<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = state.Next(n + 1);
                T temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }
        public T Pick<T>(IList<T> list)
        {
            if (list.Count > 0)
                return list[state.Next(list.Count)];
            return default(T);
        }
        public T Pick<T>(bool exclude_zero) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            if (values.Length > 0)
                return (T)values.GetValue(state.Next(exclude_zero ? 1 : 0, values.Length));
            return (T)values.GetValue(0);
        }
        public T Pick<T>(T[] array)
        {
            if (array.Length > 0)
                return array[state.Next(array.Length)];
            return default(T);
        }
    }
}