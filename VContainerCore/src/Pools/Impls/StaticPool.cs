namespace VContainer.Pools.Impls {
  public class StaticPool<T> : SimpleMemoryPool<T> where T : new() {
    public static StaticPool<T> Instance { get; } = new();
  }
}
