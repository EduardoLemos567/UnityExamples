using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

/// Some useful extensions, mostly to keep your code readable.
/// Eg: Vector3.ToXZ(), returns a Vector2 with values X and Z
/// or List.LastIndex, more readable than Count - 1

namespace Game.Extensions
{
    public enum DEBUG_LEVEL
    {
        INFO,
        WARNING,
        ERROR,
    }
    public static class StringExtensions
    {
        public static string TrimStart(this string value, string trim)
        {
            if (value.StartsWith(trim))
            { return value.Remove(0, trim.Length); }
            return value;
        }
        public static string TrimEnd(this string value, string trim)
        {
            if (value.EndsWith(trim))
            { return value.Remove(value.Length - trim.Length, trim.Length); }
            return value;
        }
        public static string FirstLetterUpper(this string value)
        {
            return char.ToUpper(value[0]) + value[1..].ToLower();
        }
        public static string RemoveAccents(this string word)
        {
            var normalizedString = word.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(word.Length);
            for (int i = 0; i < normalizedString.Length; i++)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(normalizedString[i]);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static void Print(this string message, DEBUG_LEVEL level = DEBUG_LEVEL.INFO)
        {
            switch (level)
            {
                case DEBUG_LEVEL.INFO:
                    Debug.Log(message);
                    break;
                case DEBUG_LEVEL.WARNING:
                    Debug.LogWarning(message);
                    break;
                case DEBUG_LEVEL.ERROR:
                    Debug.LogError(message);
                    break;
            }
        }
    }
    public static class ListExtensions
    {
        public static void EnsureRangeCapacity<T>(this List<T> list, int range)
        {
            if (list.Count + range > list.Capacity)
            {
                list.Capacity = list.Count + range;
            }
        }
        public static T LastItem<T>(this IList<T> list) => list[list.Count - 1];
        public static int LastIndex<T>(this IList<T> list) => list.Count - 1;
        public static T FromLast<T>(this IList<T> list, int index) => list[list.Count - 1 - index];
        public static T Pop<T>(this IList<T> list, int index)
        {
            try { return list[index]; }
            finally { list.RemoveAt(index); }
        }
        public static T PopLast<T>(this IList<T> list) => list.Pop(list.Count - 1);
        public static List<T> PopLastRange<T>(this List<T> list, int from_index)
        {
            try { return list.GetRange(from_index, list.Count - from_index); }
            finally { list.RemoveRange(from_index, list.Count - from_index); }
        }
        public static IEnumerable<T> GetRangeEnumerable<T>(this List<T> list, int from_index, int count)
        {
            for (int i = 0; i < count && (from_index + i) < list.Count; i++)
            {
                yield return list[from_index + i];
            }
        }
    }
    public static class Vector2Extensions
    {
        public static float SqrDistance(this in Vector2 this_vector, in Vector2 other_vector)
        {
            return Vector2.SqrMagnitude(this_vector - other_vector);
        }
        public static Vector2Int ToVector2Int(this in Vector2 this_vector)
        {
            return new((int)this_vector.x, (int)this_vector.y);
        }
        public static Vector3 ToVector3WithZ(this in Vector2 xy, float z = 0) => new(xy.x, xy.y, z);
        public static Vector3 FromXZ(this in Vector2 this_vector)
        {
            return new(this_vector.x, 0, this_vector.y);
        }
        public static Vector2 Divide(this in Vector2 this_vector, in Vector2 other_vector)
        {
            return new(this_vector.x / other_vector.x, this_vector.y / other_vector.y);
        }
    }
    public static class Vector3Extensions
    {
        public static float SqrDistance(this in Vector3 this_vector, in Vector3 other_vector)
        {
            return Vector3.SqrMagnitude(this_vector - other_vector);
        }
        public static Vector2 ToXZ(this in Vector3 position)
        {
            return new(position.x, position.z);
        }
        public static Vector3 Divide(this in Vector3 one, in Vector3 two)
        {
            return new(one.x / two.x, one.y / two.y, one.z / two.z);
        }
    }
    public static class RectExtensions
    {
        public static bool IsInsideOf(this in Rect inside_rect, in Rect outside_rect)
        {
            return outside_rect.xMin <= inside_rect.xMin
            && inside_rect.xMax <= outside_rect.xMax
            && outside_rect.yMin <= inside_rect.yMin
            && inside_rect.yMax <= outside_rect.yMax;
        }
        public static Vector2 GetTopLeft(this in Rect this_rect)
        {
            return new(this_rect.xMin, this_rect.yMax);
        }
        public static Vector2 GetBottomRight(this in Rect this_rect)
        {
            return new(this_rect.xMax, this_rect.yMin);
        }
    }
    public static class TransformExtensions
    {
        public static IEnumerable<T> FindComponentsInChildren<T>(this Transform parent)
        {
            var stack = new Stack<int>();
            T component;
            int i = 0, count = parent.childCount;
            if (parent.TryGetComponent<T>(out component)) { yield return component; }
            while (true)
            {
                if (i < count)
                {
                    var child = parent.GetChild(i);
                    if (child.TryGetComponent<T>(out component)) { yield return component; }
                    // If child has children, push into stack
                    if (child.childCount > 0)
                    {
                        stack.Push(i + 1);
                        parent = child;
                        i = 0;
                        count = parent.childCount;
                    }
                    else { i++; }
                }
                else
                {
                    // After iterating over all children, return to continue iterating into parent
                    if (stack.Count == 0) { yield break; }
                    parent = parent.parent;
                    i = stack.Pop();
                    count = parent.childCount;
                }
            }
        }
    }
    public static class RectTransformExtensions
    {
        public static Rect GetWorldRect(this RectTransform rect_transform)
        {
            return new(rect_transform.TransformVector(rect_transform.rect.min), rect_transform.TransformVector(rect_transform.rect.max));
        }
        public static void CopyTransformsInto(this RectTransform from, RectTransform to)
        {
            to.anchorMin = from.anchorMin;
            to.anchorMax = from.anchorMax;
            to.anchoredPosition = from.anchoredPosition;
            to.sizeDelta = from.sizeDelta;
        }
        public static Rect GetOffsetRect(this RectTransform this_rect_transform)
        {
            return new Rect { min = this_rect_transform.offsetMin, max = this_rect_transform.offsetMax };
        }
        public static void SetOffsetRect(this RectTransform this_rect_transform, in Rect rect)
        {
            this_rect_transform.offsetMin = rect.min;
            this_rect_transform.offsetMax = rect.max;
        }
    }
    public static class ColorExtensions
    {
        public static ColorHSV ToColorHSV(this in Color color) => new ColorHSV(color);
    }
    public static class IEnumerableExtensions
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Print<T>(this IEnumerable<T> list)
        {
            var items = new List<string>();
            foreach (var item in list)
            {
                items.Add(item.ToString());
            }
            Debug.Log($"[{string.Join(", ", items)}]");
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Print<K, V>(this IEnumerable<KeyValuePair<K, V>> dict)
        {
            var items = new List<string>();
            foreach (var item in dict)
            {
                items.Add($"{item.Key.ToString()}:{item.Value.ToString()}");
            }
            Debug.Log($"{{{string.Join(", ", items)}}}");
        }
    }
    public static class ReadOnlySpanExtensions
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Print<T>(this ReadOnlySpan<T> rospan)
        {
            var items = new List<string>();
            foreach (var item in rospan)
            {
                items.Add(item.ToString());
            }
            Debug.Log($"[{string.Join(", ", items)}]");
        }
    }
}