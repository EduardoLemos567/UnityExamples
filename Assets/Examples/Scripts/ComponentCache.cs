using System;
using System.Collections.Generic;
using Game.Extensions;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class ComponentCache<T> where T : Component, ICacheable
    {
        public GameObject prefab;
        public uint cacheLimit;
        public Transform cacheTransform;
        readonly List<(T instance, float time)> cacheList = new();
        int LastIndex => cacheList.Count - 1;
        public T Rent(Transform new_parent)
        {
            T component;
            if (cacheList.Count > 0)
            {
                component = cacheList.PopLast().instance;
                component.transform.SetParent(new_parent);
                component.gameObject.SetActive(true);
            }
            else
            {
                component = GameObject.Instantiate(prefab, new_parent).GetComponent<T>();
            }
            component.IsCached = false;
            return component;
        }
        public void Return(T component)
        {
            component.IsCached = true;
            if (cacheList.Count + 1 <= cacheLimit)
            {
                cacheList.Add((instance: component, time: Time.time));
                component.gameObject.SetActive(false);
                component.transform.SetParent(cacheTransform);
            }
            else
            {
                GameObject.Destroy(component.gameObject);
            }
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