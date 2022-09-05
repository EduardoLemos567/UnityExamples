using UnityEngine;

namespace Game
{
    /// <summary>
    /// A bit useless class, just used to keep your code clean and verbose.
    /// Good to check if some specific amount of time has passed, reset it and etc.
    /// </summary>
    public struct TimeCounter
    {
        float limitSeconds;
        readonly System.Diagnostics.Stopwatch stopwatch;
        public TimeCounter(float limit_seconds)
        {
            limitSeconds = limit_seconds;
            stopwatch = System.Diagnostics.Stopwatch.StartNew();
        }
        public bool IsTime => stopwatch.ElapsedMilliseconds * 0.001f >= limitSeconds;
        public void Reset() => stopwatch.Restart();
        public void Reset(float limit_seconds)
        {
            limitSeconds = limit_seconds;
            Reset();
        }
    }
    public struct TimeCounterUnity
    {
        float limitSeconds;
        float startTime;
        public TimeCounterUnity(float limit_seconds)
        {
            limitSeconds = limit_seconds;
            startTime = Time.unscaledTime;
        }
        public bool IsTime => Time.unscaledTime - startTime >= limitSeconds;
        public void Reset() => startTime = Time.unscaledTime;
        public void Reset(float limit_seconds)
        {
            limitSeconds = limit_seconds;
            Reset();
        }
    }
}