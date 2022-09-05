using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Help you keep track of your coroutines, sometimes want to stop/restart or check
    /// if a coroutine is done.
    /// </summary>
    public class CoroutineRunner
    {
        public delegate IEnumerator Coroutine(Action on_finished);
        IEnumerator currentCoroutine;
        readonly MonoBehaviour ownerComponent;
        readonly Coroutine coroutineMethod;
        readonly Action onFinishedCallback;
        public bool IsRunning => currentCoroutine != null;
        public CoroutineRunner(
            MonoBehaviour owner_component,
            Coroutine coroutine_method,
            Action on_finished_callback = null
        )
        {
            ownerComponent = owner_component;
            coroutineMethod = coroutine_method;
            onFinishedCallback = on_finished_callback;
        }
        public void Start()
        {
            // DebugManager.Assert(!IsRunning, "Cant start a new coroutine while another is running.");
            if (IsRunning) { return; }
            ownerComponent.StartCoroutine(currentCoroutine = coroutineMethod(OnFinished));
        }
        public void Stop()
        {
            if (IsRunning)
            {
                ownerComponent.StopCoroutine(currentCoroutine);
                OnFinished();
            }
        }
        public void Restart()
        {
            Stop();
            Start();
        }
        void OnFinished()
        {
            currentCoroutine = null;
            onFinishedCallback?.Invoke();
        }
    }
}