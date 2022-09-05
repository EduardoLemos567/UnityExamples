using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Useful class to work with inspector defined ranges.
    /// You can check if a value is range, pick a random value in range. Lerp and ILerp it.
    /// </summary>
    [Serializable]
    public struct RangeInt : IEnumerable<int>, IEquatable<RangeInt>, IEqualityComparer, IEqualityComparer<RangeInt>
    {
        public int min;
        public int max;
        public int Size
        {
            get => max - min;
            set => max = min + value;
        }
        public int Mid
        {
            get => min + Size / 2;
            set => min = value - Size / 2;
        }
        public RangeInt(int min, int max)
        {
            // DebugManager.Assert(min <= max, $"min {min} is not lower than max {max}");
            this.min = min; this.max = max;
        }
        public static RangeInt CreateGuess(int value1, int value2) => new RangeInt(Math.Min(value1, value2), Math.Max(value1, value2));
        public IEnumerator<int> GetEnumerator()
        {
            for (var i = min; i < max; i++)
            {
                yield return i;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public bool InRange(int value) => min <= value && value <= max;
        public bool InRange(RangeInt value) => InRange(value.min) || InRange(value.max);
        public int Clamp(int value) => Mathf.Clamp(value, min, max);
        public float InverseLerp(int value) => Mathf.InverseLerp(min, max, value);
        public int Lerp(float value) => (int)Mathf.Lerp(min, max, value);
        public int SampleRandom(Random rng = null) => min + (int)((double)Size * (rng ?? Random.globalState).GetValueDouble());
        public override bool Equals(object other) => (other is RangeInt) && this == (RangeInt)other;
        public bool Equals(RangeInt other) => this == other;
        public new bool Equals(object x, object y) => (x is RangeInt && y is RangeInt) && (RangeInt)x == (RangeInt)y;
        public bool Equals(RangeInt x, RangeInt y) => x == y;
        public override int GetHashCode() => base.GetHashCode();
        public int GetHashCode(object obj) => obj.GetHashCode();
        public int GetHashCode(RangeInt obj) => obj.GetHashCode();
        public RangeInt Scaled(int scale) => new RangeInt(min * scale, max * scale);
        public static bool operator ==(RangeInt one, RangeInt two) => one.min == two.min && one.max == two.max;
        public static bool operator !=(RangeInt one, RangeInt two) => !(one == two);
        public static RangeInt ByMidSize(int mid, int size)
        {
            var half = size / 2;
            return new RangeInt(mid - half, mid + half);
        }
        public override string ToString() => $"RangeInt(min={min}, max={max})";
    }
    [Serializable]
    public struct RangeFloat : IEnumerable<float>, IEquatable<RangeFloat>, IEqualityComparer, IEqualityComparer<RangeFloat>
    {
        public float min;
        public float max;
        public float Size
        {
            get => max - min;
            set => max = min + value;
        }
        public float Mid
        {
            get => min + Size / 2;
            set => min = value - Size / 2;
        }
        public RangeFloat(float min, float max)
        {
            // DebugManager.Assert(min <= max, $"min {min} is not lower than max {max}");
            this.min = min; this.max = max;
        }
        public static RangeFloat CreateGuess(float value1, float value2) => new RangeFloat(Math.Min(value1, value2), Math.Max(value1, value2));
        public IEnumerator<float> GetEnumerator()
        {
            for (var i = min; i < max; i++)
            {
                yield return i;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public bool InRange(float value) => min <= value && value <= max;
        public bool InRange(RangeFloat value) => InRange(value.min) || InRange(value.max);
        public float Clamp(float value) => Mathf.Clamp(value, min, max);
        public float InverseLerp(float value) => Mathf.InverseLerp(min, max, value);
        public float Lerp(float value) => Mathf.Lerp(min, max, value);
        public float SampleRandom(Random rng = null) => min + Size * (float)(rng ?? Random.globalState).GetValueDouble();
        public override bool Equals(object other) => (other is RangeInt) && this == (RangeFloat)other;
        public bool Equals(RangeFloat other) => this == other;
        public new bool Equals(object x, object y) => (x is RangeFloat && y is RangeFloat) && (RangeFloat)x == (RangeFloat)y;
        public bool Equals(RangeFloat x, RangeFloat y) => x == y;
        public override int GetHashCode() => base.GetHashCode();
        public int GetHashCode(object obj) => obj.GetHashCode();
        public int GetHashCode(RangeFloat obj) => obj.GetHashCode();
        public RangeFloat Scaled(float scale) => new RangeFloat(min * scale, max * scale);
        public static bool operator ==(RangeFloat one, RangeFloat two) => one.min == two.min && one.max == two.max;
        public static bool operator !=(RangeFloat one, RangeFloat two) => !(one == two);
        public static RangeFloat ByMidSize(float mid, float size)
        {
            var half = size / 2;
            return new RangeFloat(mid - half, mid + half);
        }
        public override string ToString() => $"RangeFloat(min={min}, max={max})";
    }
}