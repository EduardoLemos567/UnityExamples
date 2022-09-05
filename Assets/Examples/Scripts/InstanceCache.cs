using System;
using System.Collections.Generic;
using Game.Extensions;
using UnityEngine;

namespace Game
{
    public class InstanceCache<T> where T : class, ICacheable, new()
    {
        public struct Returnable : IDisposable
        {
            public Returnable(T instance) => Instance = instance;
            public T Instance { get; private set; }

            public void Dispose() => Shared.Return(Instance);
        }
        public static InstanceCache<T> Shared
        {
            get
            {
                if (shared == null)
                { shared = new(); }
                return shared;
            }
        }
        static InstanceCache<T> shared;
        public int maxCachedInstances = 0; // 0 = unlimited
        readonly List<(T instance, float time)> cacheList = new();
        public Returnable RentReturnable() => new(Rent());
        public T Rent()
        {
            T instance;
            if (cacheList.Count > 0)
            { instance = cacheList.PopLast().instance; }
            else { instance = new(); }
            instance.IsCached = false;
            return instance;
        }
        public void Return(T instance)
        {
            instance.IsCached = true;
            if (cacheList.Count < maxCachedInstances)
            {
                cacheList.Add((instance, Time.time));
            }
        }
        public void Trim()
        {
            cacheList.Clear();
            cacheList.TrimExcess();
        }
        public void TrimOlder(float older)
        {
            var should_trim = false;
            for (int i = 0; i < cacheList.Count; i++)
            {
                if (cacheList[i].time < older)
                {
                    cacheList.RemoveAt(i);
                    i--;
                    should_trim = true;
                }
            }
            if (should_trim) { cacheList.TrimExcess(); }
        }
    }
}