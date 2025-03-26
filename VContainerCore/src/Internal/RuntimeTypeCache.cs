using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer.Internal
{
    static class RuntimeTypeCache
    {
        static readonly ConcurrentDictionary<Type, Type> OpenGenericTypes = new ConcurrentDictionary<Type, Type>();
        static readonly ConcurrentDictionary<Type, Type[]> GenericTypeParameters = new ConcurrentDictionary<Type, Type[]>();
        static readonly ConcurrentDictionary<Type, Type> ArrayTypes = new ConcurrentDictionary<Type, Type>();
        static readonly ConcurrentDictionary<Type, Type> EnumerableTypes = new ConcurrentDictionary<Type, Type>();
        static readonly ConcurrentDictionary<Type, Type> ReadOnlyListTypes = new ConcurrentDictionary<Type, Type>();
        static readonly ConcurrentDictionary<Type, Type> ListTypes = new ConcurrentDictionary<Type, Type>();
        static readonly ConcurrentDictionary<Type, Type> IListTypes = new ConcurrentDictionary<Type, Type>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type OpenGenericTypeOf(Type closedGenericType)
            => OpenGenericTypes.GetOrAdd(closedGenericType, key => key.GetGenericTypeDefinition());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GenericTypeParametersOf(Type closedGenericType)
            => GenericTypeParameters.GetOrAdd(closedGenericType, key => key.GetGenericArguments());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ArrayTypeOf(Type elementType)
            => ArrayTypes.GetOrAdd(elementType, key => key.MakeArrayType());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type EnumerableTypeOf(Type elementType)
            => EnumerableTypes.GetOrAdd(elementType, key => typeof(IEnumerable<>).MakeGenericType(key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ReadOnlyListTypeOf(Type elementType)
            => ReadOnlyListTypes.GetOrAdd(elementType, key => typeof(IReadOnlyList<>).MakeGenericType(key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type ListTypeOf(Type elementType)
            => ListTypes.GetOrAdd(elementType, key => typeof(List<>).MakeGenericType(key));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type IListTypeOf(Type elementType)
            => IListTypes.GetOrAdd(elementType, key => typeof(IList<>).MakeGenericType(key));
    }
}
