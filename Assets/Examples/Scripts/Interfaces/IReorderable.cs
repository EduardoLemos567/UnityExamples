using System;

namespace Game
{
    public interface IReordeable<E> where E : Enum
    {
        E Order { get; }
    }
}