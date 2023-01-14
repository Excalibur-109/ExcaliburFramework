using System.Collections.Generic;
using UnityEngine;

namespace Excalibur
{
    public sealed class PoolManager : Singleton<PoolManager>
    {
        Transform rootPoolParent;
        Dictionary<PoolType, IPool> _pool;

        public Transform RootPool => rootPoolParent;

        protected override void Init()
        {
            base.Init();

            rootPoolParent = Utility.CreateGameObject("RootPool", true, GameManager.Instance.transform).transform;
            rootPoolParent.position = Vector3.zero;
            rootPoolParent.localPosition = Vector3.zero;
            _pool = new Dictionary<PoolType, IPool>();
        }

        public void InitPoolMono<T>(PoolType poolType, T t) where T : Component
        {
            if (!_pool.ContainsKey(poolType))
            {
                _pool.Add(poolType, new MonoPool());
            }


            IMonoPool pool = (IMonoPool)_pool[poolType];
            pool.InitPool(t);
        }

        public void InitPoolMono<T>(PoolType poolType, T t, Transform parent) where T : Component
        {
            if (!_pool.ContainsKey(poolType))
            {
                _pool.Add(poolType, new MonoPool());
            }

            IMonoPool pool = (IMonoPool)_pool[poolType];
            pool.InitPool(t, parent);
        }

        public void EnQueueMono(PoolType poolType, Component t)
        {
            if (!_pool.ContainsKey(poolType))
            {
                _pool.Add(poolType, new MonoPool());
            }

            IMonoPool pool = (IMonoPool)_pool[poolType];
            pool.EnQueue(t);
        }

        public T DeQueueMono<T>(PoolType poolType) where T : Component
        {
            if (!_pool.ContainsKey(poolType))
            {
                throw new System.Exception($"不存在{typeof(T).Name}的对象池");
            }

            IPool pool = _pool[poolType];

            return (pool as IMonoPool).DeQueue<T>();
        }

        public void EnQueueQuote<T>(PoolType poolType, T t) where T : IQuoteConstraint<T>, new()
        {
            if (!_pool.ContainsKey(poolType))
            {
                _pool.Add(poolType, new QuotePool<T>());
            }

            IQuotePool<T> pool = (IQuotePool<T>)_pool[poolType];
            pool.EnQueue(t);
        }

        public T DeQueueQuote<T>(PoolType poolType) where T : IQuoteConstraint<T>, new()
        {
            if (_pool.ContainsKey(poolType))
            {
                _pool.Add(poolType, new QuotePool<T>());
            }

            IQuotePool<T> pool = (IQuotePool<T>)_pool[poolType];
            return pool.DeQueue();
        }

        public void Clear(PoolType poolType)
        {
            if (_pool.ContainsKey(poolType))
            {
                _pool[poolType].Clear();
            }
        }

        public void Clear()
        {
            foreach (var item in _pool)
            {
                item.Value.Clear();
            }
        }
    }

    sealed internal class MonoPool : IMonoPool
    {
        Transform _parent;
        Component _prefab;
        Queue<Component> _pool;
        List<Component> _components;

        public void InitPool<T>(T t) where T : Component
        {
            if (_pool == null)
            {
                _parent = Utility.CreateGameObject(t.name + "Pool", true, PoolManager.Instance.RootPool).transform;
                _prefab = t;
                _pool = new Queue<Component>();
                _components = new List<Component>();
            }
        }

        public void InitPool<T>(T t, Transform parent) where T : Component
        {
            if (_pool == null)
            {
                InitPool(t);
                if (parent != null)
                    _parent = parent;
            }
        }

        public void EnQueue<T>(T t) where T : Component
        {
            _pool.Enqueue(t);
            if (!_components.Contains(t))
            {
                _components.Add(t);
            }
            Utility.SetActive(t, false);
        }

        public T DeQueue<T>() where T : Component
        {
            if (_prefab == null)
            {
                throw new System.Exception("对象池未添加prefab.");
            }

            Component t = _pool.Count == 0 ? Object.Instantiate((T)_prefab, _parent) : _pool.Dequeue();

            if (!_components.Contains(t))
            {
                _components.Add(t);
            }

            if (t.name.Contains("(Clone)"))
                t.name = t.name.Replace("(Clone)", string.Empty);

            Utility.SetActive(t.gameObject, true);

            return (T)t;
        }

        public void Clear()
        {
            foreach (var item in _components)
            {
                Object.DestroyImmediate(item.gameObject);
            }

            _components.Clear();
            _pool.Clear();
        }
    }

    sealed internal class QuotePool<T> : IQuotePool<T> where T : IQuoteConstraint<T>, new()
    {
        Queue<T> _pool;
        List<T> _references;

        public void EnQueue(T t)
        {
            if (_pool == null)
            {
                _pool = new Queue<T>();
            }
            if (_references == null)
            {
                _references = new List<T>();
            }

            t.Free();

            if (!_references.Contains(t))
            {
                _references.Add(t);
            }

            _pool.Enqueue(t);
        }

        public T DeQueue()
        {
            if (_pool == null)
            {
                _pool = new Queue<T>();
            }
            if (_references == null)
            {
                _references = new List<T>();
            }

            T t = _pool.Count == 0 ? new T() : _pool.Dequeue();

            if (!_references.Contains(t))
            {
                _references.Add(t);
            }

            return t;
        }

        public void Clear()
        {

        }
    }
}