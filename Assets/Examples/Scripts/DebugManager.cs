//TODO: remove on release
#if GAME_USE_DEBUG_MANAGER
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game
{
    /// <summary>
    /// Utility singleton class to keep your asserts in one single place.
    /// Also emit logs based on a system of flags. You can enable/disable in editor
    /// which flags you want to allow emiting logs.
    /// </summary>
    public class DebugManager : Singleton<DebugManager>
    {
        public class AssertException : Exception { public AssertException(string msg) : base(msg) { } }
        [Serializable]
        public struct NameEnabledInfo
        {
            public string name;
            public bool enabled;
        }
        public NameEnabledInfo[] infos;
        Dictionary<string, bool> relevance;
        protected override void Awake()
        {
            base.Awake();
            //#if UNITY_EDITOR  //TODO: change on release
            relevance = new(infos.Length);
            foreach (var info in infos) { relevance[info.name] = info.enabled; }
            infos = null;
            //#endif
        }
        public static bool IsRelevant(string name)
        {
            return IsInstanced
            && Instance.relevance != null
            && Instance.relevance.ContainsKey(name)
            && Instance.relevance[name];
        }
        //[Conditional("GAME_USE_DEBUG_MANAGER")]
        public static void Log(string name, string msg)
        {
            if (IsRelevant(name))
            { UnityEngine.Debug.Log($"{name}:{msg}"); }
        }
        //[Conditional("GAME_USE_DEBUG_MANAGER")]
        public static void Log(string name, string place, string msg)
        {
            if (IsRelevant(name))
            { UnityEngine.Debug.Log($"{name}:{place}:{msg}"); }
        }
        //[Conditional("GAME_USE_DEBUG_MANAGER")]
        public static void Log(string name, object obj, string msg)
        {
            if (IsRelevant(name))
            { UnityEngine.Debug.Log($"{name}:{obj.GetType().Name}({obj.GetHashCode()}):{msg}"); }
        }
        //[Conditional("GAME_USE_DEBUG_MANAGER")]
        public static void Assert(bool condition, string msg)
        {
            if (!condition) { throw new AssertException(msg); }
        }
        //[Conditional("GAME_USE_DEBUG_MANAGER")]
        public static void Assert(bool pre_condition, Func<bool> condition, string msg)
        {
            if (pre_condition && !condition.Invoke()) { throw new AssertException(msg); }
        }
    }
}
#endif //GAME_USE_DEBUG_MANAGER