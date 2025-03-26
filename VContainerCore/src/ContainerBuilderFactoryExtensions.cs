using System.Runtime.CompilerServices;
using VContainer.Factory;

namespace VContainer
{
    public static class ContainerBuilderFactoryExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRegistrationBuilder RegisterFactory<TContract, TFactory>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Singleton) where TFactory: PlaceholderFactory<TContract>
        {
            return builder.Register<TFactory>(lifetime).AsSelf();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRegistrationBuilder RegisterFactory<TParam1, TContract, TFactory>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Singleton) where TFactory: PlaceholderFactory<TParam1, TContract>
        {
            builder.Register<TContract>(Lifetime.Transient);
            return builder.Register<TFactory>(lifetime).AsSelf();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRegistrationBuilder RegisterFactory<TParam1, TParam2, TContract, TFactory>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Singleton) where TFactory: PlaceholderFactory<TParam1, TParam2, TContract>
        {
            return builder.Register<TFactory>(lifetime).AsSelf();

        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRegistrationBuilder RegisterFactory<TParam1, TParam2, TParam3, TContract, TFactory>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Singleton) where TFactory: PlaceholderFactory<TParam1, TParam2, TParam3, TContract>
        {
            return builder.Register<TFactory>(lifetime).AsSelf();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRegistrationBuilder RegisterFactory<TParam1, TParam2, TParam3, TParam4, TContract, TFactory>(
            this IContainerBuilder builder,
            Lifetime lifetime = Lifetime.Singleton) where TFactory: PlaceholderFactory<TParam1, TParam2, TParam3, TParam4, TContract>
        {
            return builder.Register<TFactory>(lifetime).AsSelf();
        }
    }
}
