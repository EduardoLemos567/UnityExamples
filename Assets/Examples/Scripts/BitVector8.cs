namespace Game
{
    /// <summary>
    /// Value is kept on a struct byte, you can easily read/write for bits
    /// </summary>
    public struct BitVector8
    {
        public byte Data { get; private set; }
        public bool IsEmpty => Data == 0;
        public bool this[int index]
        {
            get
            {
                // DebugManager.Assert(index >= 0 && index < 8, "Index must be between [0..7]");
                return ((Data >> index) & 1) == 1;
            }
            set
            {
                // DebugManager.Assert(index >= 0 && index < 8, "Index must be between [0..7]");
                Data = (byte)(value ? Data | (1 << index) : (Data & (Data ^ (1 << index))));
            }
        }
        public BitVector8(byte data) => Data = (byte)data;
        public BitVector8(params bool[] data)
        {
            // DebugManager.Assert(data.Length > 0 && data.Length <= 8, "data size must be between ]0..8]");
            Data = 0;
            for (int i = 0; i < data.Length; i++)
            { this[i] = data[i]; }
        }
        public BitVector8(params int[] data)
        {
            // DebugManager.Assert(data.Length > 0 && data.Length <= 8, "data size must be between ]0..8]");
            Data = 0;
            for (int i = 0; i < data.Length; i++)
            { this[data[i]] = true; }
        }
        public static bool operator ==(BitVector8 one, BitVector8 other) => one.Data == other.Data;
        public static bool operator !=(BitVector8 one, BitVector8 other) => one.Data != other.Data;
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => Data.GetHashCode();
        public override string ToString() => Data.ToString();
    }
}