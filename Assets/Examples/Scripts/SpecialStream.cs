using System;
using System.Buffers;
using System.IO;
using Game.Extensions;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// My own MemoryStream implementation, mostly so i could grab 
    /// ReadOnlySpan directly from the internal array.
    /// </summary>
    public class SpecialStream : Stream
    {
        byte[] array;  // array and capacity
        long position;  // position where to read or write
        long inUse;     // from left to right, amount of the array in use
        long? maxCapacity;
        bool canRead;
        bool canWrite;
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => inUse;
        public long Capacity
        {
            get => array.LongLength;
            set => Resize(value);
        }
        public long UnreadData => inUse - position;   // Amount of data left to read
        public SpecialStream(long initial_capacity = 4096, long? max_capacity = null)
        {
            array = new byte[initial_capacity];
            maxCapacity = max_capacity;
        }
        public override long Position
        {
            get => position;
            set => position = Math.Clamp(value, 0, inUse);
        }
        public override void Flush()
        {
            position = 0;
            inUse = 0;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
            {
                throw new Exception("Buffer has not enough space");
            }
            count = Math.Min(count, (int)UnreadData);
            if (count > 0)
            {
                Array.Copy(array, position, buffer, offset, count);
                position += count;
            }
            return count;
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = Math.Min(offset, inUse);
                    break;
                case SeekOrigin.Current:
                    position = Math.Clamp(position + offset, 0, inUse);
                    break;
                case SeekOrigin.End:
                    position = Math.Max(inUse - offset, 0);
                    break;
            }
            return position;
        }
        public override void SetLength(long value)
        {
            if (value > array.LongLength)
            {
                Resize(value);
            }
            inUse = value;
            position = Math.Min(inUse, position);
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
            {
                throw new Exception("Buffer has not enough space");
            }
            if (count >= (array.LongLength - position))
            {
                Resize(IncreaseCapacityStrategy(position + count));
            }
            Array.Copy(buffer, offset, array, position, count);
            position += count;
            inUse = Math.Max(inUse, position);
        }
        public void ClearCompactUnread(long initial_position)
        {
            position = initial_position;
            // Nothing to copy
            if (position == 0) { return; }
            if (UnreadData == 0) { position = inUse = 0; }
            else
            {
                Array.Copy(array, position, array, 0, UnreadData);
                position = inUse = UnreadData;
            }
        }
        void Resize(long value)
        {
            if (maxCapacity.HasValue && value > maxCapacity.Value)
            {
                throw new Exception("Resize requested exceeds maximum capacity previously set.");
            }
            if (value < inUse)
            {
                throw new Exception("Cant resize bellow Length, may lose data in use, use SetLength instead.");
            }
            var temp = new byte[value];
            Array.Copy(array, temp, inUse);
            array = temp;
        }
        public void CopyTo(SpecialStream destination)
        {
            /*
              _            
            0,1,2,3,4,5,6
                _
            0,1,2,3,5,6,7,8
            */
            var amount = UnreadData;
            if (amount == 0) { return; }
            if ((destination.position + amount) > destination.Capacity)
            { destination.Resize(IncreaseCapacityStrategy(destination.position + amount)); }
            Array.Copy(array, position, destination.array, destination.position, amount);
            position += amount;
            destination.inUse = Math.Max(destination.position + amount, destination.inUse);
            destination.position += amount;
        }
        public ReadOnlySpan<byte> AsReadOnlySpan(int offset = 0, int? count = null)
        {
            return new ReadOnlySpan<byte>(array, offset, (int)(count.HasValue ? Math.Min(count.Value, inUse) : inUse));
        }
        static long IncreaseCapacityStrategy(long requested) => (requested) / 2 * 3;
    }
}