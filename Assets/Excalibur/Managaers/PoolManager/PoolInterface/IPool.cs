using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public interface IPool
    {
        void Clear();
    }

    public interface IMonoPool : IPool
    {
        void InitPool<T>(T t) where T : Component;
        void InitPool<T>(T t, Transform parent) where T : Component;
        void EnQueue<T>(T t) where T : Component;
        T DeQueue<T>() where T : Component;
    }

    public interface IQuotePool<T> : IPool where T : IQuoteConstraint<T>, new()
    {
        void EnQueue(T t);
        T DeQueue();
    }

    public interface IQuoteConstraint<T> where T : new()
    {
        void Free();
    }
}