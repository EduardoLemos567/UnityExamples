using System;
using UnityEngine;
using UnityEngine.UI;
using Game.Extensions;
using System.Collections.Generic;

namespace Game
{
    /// <summary>
    /// Utility static class, where i throw all functions that didnt belong to any other class.
    /// </summary>
    public static class Utils
    {
        public const float CLOSE_ENOUGH = 0.0001f;
        public const float SQR_CLOSE_ENOUGH = CLOSE_ENOUGH * CLOSE_ENOUGH;
        public const int NULL_INDEX = -1;
        public readonly static WaitForSeconds WaitOneSecond = new(1);
        public readonly static WaitForSeconds WaitOneTenth = new(0.1f);
        public readonly static WaitForSeconds WaitFiveTenth = new(0.5f);
        public readonly static WaitForSeconds WaitOneHundreth = new(0.01f);
        public readonly static WaitForSecondsRealtime WaitRealOneSecond = new(1);
        public readonly static WaitForSecondsRealtime WaitRealOneTenth = new(0.1f);
        public readonly static WaitForSecondsRealtime WaitRealFiveTenth = new(0.5f);
        public readonly static WaitForSecondsRealtime WaitRealOneHundreth = new(0.01f);
        public readonly static Vector2 HalfVector2 = new(0.5f, 0.5f);
        /// <summary>
        /// Turn 1.2345 into 1.23 if places = 2, into 1.234 if places = 3
        /// </summary>
        /// <param name="value"></param>
        /// <param name="places"></param>
        /// <returns></returns>
        public static float Truncate(float value, int places)
        {
            places = (int)Math.Pow(10, places);
            return (int)(value * places) / (float)places;
        }
        /// <summary>
        /// Turn 123456 into 123000 if places = 3
        /// </summary>
        /// <param name="value"></param>
        /// <param name="places"></param>
        /// <returns></returns>
        public static int IntegerTruncate(int value, int places)
        {
            places = (int)Math.Pow(10, places);
            return value / places * places;
        }
        /// <summary>
        /// Remap one value from [imin...imax] into [omin...omax]
        /// </summary>
        /// <param name="value">Value to be interpolated</param>
        /// <param name="imin">Minimum input expected</param>
        /// <param name="imax">Maximum input expected</param>
        /// <param name="omin">Minimum output expected</param>
        /// <param name="omax">Maximum output expected</param>
        /// <returns></returns>
        public static float Remap(in float value, in float imin, in float imax, in float omin, in float omax)
        {
            return Mathf.Lerp(omin, omax, Mathf.InverseLerp(imin, imax, value));
        }
        /// <summary>
        /// Given bounds and a position in the same unit of the bounds.
        /// Find the relative normalized [0...1] position.
        /// Doesnt check if position is inside the bounds.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 FindUV(in Bounds bounds, in Vector3 position)
        {
            return (position - bounds.min).ToXZ().Divide(bounds.size.ToXZ());
        }
        /// <summary>
        /// Find the bound position from UV value in percent radius, percent.x = 1, means extends.x.
        /// </summary>
        /// <param name="bounds">Bounds from which to take the percent</param>
        /// <param name="percent">relative percent value, x:1 = extends.x</param>
        /// <returns>Position from the percent of the bounds.</returns>
        public static Vector2 FindPositionFromUV(in Bounds bounds, in Vector2 percent)
        {
            return Vector3.Scale(bounds.extents, percent.FromXZ()).ToXZ();
        }
        public static void LimitMod(ref Vector2 vec, float unit)
        {
            vec.x %= unit;
            vec.y %= unit;
        }
        public static void DebugDrawCross(in Vector3 position, Color color, float radius = 5f, float seconds = 10f)
        {
            // X
            Debug.DrawLine(position + Vector3.right * radius, position - Vector3.right * radius, color, seconds);
            // Y
            Debug.DrawLine(position + Vector3.up * radius, position - Vector3.up * radius, color, seconds);
            // Z
            Debug.DrawLine(position + Vector3.forward * radius, position - Vector3.forward * radius, color, seconds);
        }
        public static Vector2 GetCharacterSize(char c, Font font, int font_size, FontStyle font_style)
        {
            CharacterInfo ci;
            if (!font.GetCharacterInfo(c, out ci, font_size))
            {
                font.RequestCharactersInTexture(new(c, 1), font_size, font_style);
                font.GetCharacterInfo(c, out ci, font_size);
            }
            return new(ci.advance, ci.glyphHeight);
        }
        public static Vector2 GetWordSize(string s, Font font, int font_size, FontStyle font_style)
        {
            float width = 0f;
            foreach (char c in s)
            {
                Vector2 temp = GetCharacterSize(c, font, font_size, font_style);
                width += temp.x;
            }
            return new(width, font_size + font.lineHeight);
        }
        public static Vector2 GetStringSize(string s, Font font, int font_size, FontStyle font_style)
        {
            var result = new Vector2();
            string[] phrases = s.Split('\n');
            foreach (var phrase in phrases)
            {
                Vector2 temp = GetWordSize(phrase, font, font_size, font_style);
                result.y += temp.y;
                if (temp.x > result.x) { result.x = temp.x; }
            }
            return result;
        }
        public static Vector2 GetTextSize(Text t)
        {
            return GetStringSize(t.text, t.font, t.fontSize, t.fontStyle);
        }
        public static Vector2 GetPositionAsPercentOfScreen(Vector2 position)
        {
            return new(position.x / Screen.width, position.y / Screen.height);
        }
        public static Vector2 GetPercentOfScreenAsPosition(Vector2 percent)
        {
            return new(Screen.width * percent.x, Screen.height * percent.y);
        }
        public static Vector2 GetMousePositionAsPercentOfScreen()
        {
            return GetPositionAsPercentOfScreen(Input.mousePosition);
        }
        public static Vector2 GetScreenSize(float scale = 1.0f)
        {
            return new(Screen.width * scale, Screen.height * scale);
        }
        public static Color ColorMixAdditive(in Color color1, in Color color2, float alpha = 1.0f)
        {
            Color color;
            color.r = (color1.r + color2.r) / 2;
            color.g = (color1.g + color2.g) / 2;
            color.b = (color1.b + color2.b) / 2;
            color.a = alpha;
            return color;
        }
        public static Color ColorMixSubtractive(in Color color1, in Color color2, float alpha = 1.0f)
        {
            Color color;
            color.r = color1.r - (color1.r - color2.r) / 2;
            color.g = color1.g - (color1.g - color2.g) / 2;
            color.b = color1.b - (color1.b - color2.b) / 2;
            color.a = alpha;
            return color;
        }
        public static string GetRelativePath(string base_path, string full_path)
        {
            base_path = base_path.Replace('\\', '/');
            full_path = full_path.Replace('\\', '/');
            if (full_path.StartsWith(base_path))
            {
                int size = base_path.Length + (base_path.EndsWith("/") ? 0 : 1);
                return full_path.Substring(size, full_path.Length - size);
            }
            else
            {
                return null;
            }
        }
        public static bool IsRelativePath(string base_path, string full_path)
        {
            base_path = base_path.Replace('\\', '/');
            full_path = full_path.Replace('\\', '/');
            return full_path.StartsWith(base_path);
        }
        public static bool CheckArrayEquals<T>(T[] arr1, T[] arr2) where T : IEquatable<T>
        {
            if (arr1.Length == arr2.Length)
            {
                for (int i = 0; i < arr1.Length; i++)
                {
                    if (!arr1[i].Equals(arr2[i]))
                        return false;
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Normalize Vector2Int to have a Length of exactly one, cant normalize diagonal vectors 
        /// (vectors where both axis are non zero) as those will have length != 1.
        /// </summary>
        /// <param name="vec2">Value to be adjusted</param>
        /// <returns>True if it was able to adjust. False if the vector cant be normalized to length 1.</returns>
        public static bool Normalize(ref Vector2Int vec2)
        {
            if (vec2 != Vector2Int.zero)
            {
                if (vec2.x == 0)
                {
                    vec2.y = Math.Sign(vec2.y) * 1;
                    return true;
                }
                else if (vec2.y == 0)
                {
                    vec2.x = Math.Sign(vec2.x) * 1;
                    return true;
                }
            }
            return false;
        }
        public static T GetArrayPosition<T>(T[,] array, Vector2Int pos)
        {
            return array[pos.x, pos.y];
        }
        public static int EnumLength<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).Length;
        }
        public static E[] ListEnumValues<E>() where E : Enum
        {
            return (E[])Enum.GetValues(typeof(E));
        }
        public static bool CheckIsSorted<T>(T[] array) where T : IComparable<T>
        {
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1].CompareTo(array[i]) >= 0)
                {
                    return false;
                }
            }
            return true;
        }
        public static int[] CreateRangeArray(int size)
        {
            var array = new int[size];
            for (int i = 0; i < size; i++) { array[i] = i; }
            return array;
        }
        public static string FormatSecondsToTime(int seconds)
        {
            if (seconds < 0)
            { return $"- {-seconds / 60}:{(-seconds % 60).ToString().PadLeft(2, '0')}"; }
            else
            { return $"{seconds / 60}:{(seconds % 60).ToString().PadLeft(2, '0')}"; }
        }
        public static int FormatedTimeToSeconds(string formated)
        {
            var negative = false;
            if (formated.StartsWith("- "))
            {
                negative = true;
                formated = formated.Substring(2);
            }
            var parts = formated.Split(':');
            return (negative ? -1 : 1) * (int.Parse(parts[0]) * 60 + int.Parse(parts[1]));
        }
        public static float SpringInterpolation(float a, float b, float t)
        {
            t = Mathf.Clamp01(t);
            t = (Mathf.Sin(t * Mathf.PI * (.2f + 2.5f * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t) * (1f + (1.2f * (1f - t)));
            return a + (b - a) * t;
        }
        public static float SinInterpolation(float bottom_extremities, float peak_center, float t)
        {
            return Remap(Mathf.Sin(Mathf.PI * t), -1, 1, bottom_extremities, peak_center);
        }
        public static bool CheckEqualsAny<T>(T value, params T[] values)
        {
            for (int i = 0; i < values.Length; i++)
            { if (value.Equals(values[i])) { return true; } }
            return false;
        }
        public static void CheckAndReorder<T, E>(T[] array) 
        where T : IReordeable<E>
        where E : Enum 
        {
            static int CompareItems(T item0, T item1)
            {
                var result = Convert.ToInt32(item0.Order).CompareTo(Convert.ToInt32(item1.Order));
                // DebugManager.Assert(result != 0, $"Item0.Order '{item0.Order}' and Item1.Order '{item1.Order}', shouldn't be equal.");
                return result;
            }
#if UNITY_EDITOR
            //First check: length
            // DebugManager.Assert(array.Length == Utils.EnumLength<E>(), $"List size '{array.Length}' doesnt match to enum array '{Utils.EnumLength<E>()}'");
#endif
            Array.Sort(array, CompareItems);
        }
        public static int FindNearestPowerOf2(int value)
        {
            int result = 8; // min
            for (int i = 0; i < 27 && result < value; i++)
            { result = result << 1; }
            return result >= value ? result : int.MaxValue; // max
        }
    }
}
