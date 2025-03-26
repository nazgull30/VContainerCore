using System.Runtime.CompilerServices;
using VContainer.Pools;

namespace VContainer {
  public static class ContainerBuilderPoolExtensions {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IRegistrationBuilder RegisterPool<TItemContract, TPoolConcrete, TPoolContract>(
        this IContainerBuilder builder,
        Lifetime lifetime = Lifetime.Singleton) where TPoolContract : IPool<TItemContract>
        => builder.Register<TPoolConcrete>(lifetime).As<TPoolContract>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IRegistrationBuilder RegisterPool<TItemContract, TPool>(
        this IContainerBuilder builder,
        Lifetime lifetime = Lifetime.Singleton)
        where TPool : IPool<TItemContract> {
      return RegisterPool<TItemContract, TPool, TPool>(builder, lifetime);
    }
  }
}
