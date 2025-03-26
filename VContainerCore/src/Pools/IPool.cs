namespace VContainer.Pools {
  public interface IPool<T> {
    T Spawn();

    void Despawn(T item);
  }
}
