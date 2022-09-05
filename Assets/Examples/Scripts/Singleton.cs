using UnityEngine;

namespace Game
{
    /// <summary>
    /// Implement a simple version of singleton pattern.
    /// </summary>
    /// <typeparam name="T">Expect the derivated class to be accessed by the Instance prop.</typeparam>
    public class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; protected set; }
        public static bool IsInstanced => Instance != null;
        protected virtual void Awake() => Instance = this as T;
        protected virtual void OnDestroy() => Instance = null;
    }
}