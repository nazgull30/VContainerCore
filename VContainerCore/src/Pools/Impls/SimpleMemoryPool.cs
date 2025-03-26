namespace VContainer.Pools.Impls {
  public abstract class SimpleMemoryPool<T> : AQuickerMemoryPool<T>
    where T : new() {
    protected SimpleMemoryPool() {
    }

    protected SimpleMemoryPool(int capacity) : base(capacity) {
    }

    protected override T AllocNew() => new T();
  }
}
