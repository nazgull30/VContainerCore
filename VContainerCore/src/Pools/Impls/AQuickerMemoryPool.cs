using System;
using System.Collections.Generic;
using VContainer.Godot;

namespace VContainer.Pools.Impls
{
    public abstract class AQuickerMemoryPool<T> : IPool<T>, IInitializable, IDisposable
    {
        private readonly int _capacity;
        private readonly Stack<T> _pool;

        protected AQuickerMemoryPool() : this(10)
        {
        }

        protected AQuickerMemoryPool(int capacity)
        {
            _capacity = capacity;
            _pool = new Stack<T>(capacity);
        }

        public virtual void Initialize()
        {
            for (var i = 0; i < _capacity; i++)
            {
                var item = InternalCreate();
                _pool.Push(item);
            }
        }

        public T Spawn()
        {
            var newItem = _pool.Count > 0 ? _pool.Pop() : InternalCreate();
            OnSpawned(newItem);
            return newItem;
        }

        protected virtual void OnSpawned(T item)
        {
        }

        private T InternalCreate()
        {
            var item = AllocNew();
            OnCreated(item);
            return item;
        }

        protected virtual void OnCreated(T item)
        {
        }

        public void Despawn(T item)
        {
            OnDespawned(item);
            _pool.Push(item);
        }

        protected virtual void OnDespawned(T item)
        {
        }

        protected abstract T AllocNew();

        public virtual void Dispose()
        {
        }
    }
}
