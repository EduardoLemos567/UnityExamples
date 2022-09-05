namespace Game
{
    /// <summary>
    /// A bit useless class, just used to keep your code clean and verbose.
    /// If you call TryCall, it'll keep track on how many times you called, if its
    /// above the callRate, it'll return true. You can reset to restart.
    /// </summary>
    public struct CallCounter
    {
        readonly int callRate;
        int counter;
        public CallCounter(int call_rate, int initial_counter = 0)
        {
            callRate = call_rate;
            counter = initial_counter;
        }
        public bool TryCall() => ++counter > callRate;
        public void Rollback() => counter--;
        public void Reset() => counter = 0;
    }
}