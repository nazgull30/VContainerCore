using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer.Internal
{
    internal readonly struct CollectionInstanceProvider : IInstanceProvider, IEnumerable<IRegistration>
    {
        public static bool Match(Type openGenericType) => openGenericType == typeof(IEnumerable<>) ||
                                                          openGenericType == typeof(IReadOnlyList<>) ||
                                                          openGenericType == typeof(List<>) ||
                                                          openGenericType == typeof(IList<>);

        public List<IRegistration>.Enumerator GetEnumerator() => registrations.GetEnumerator();
        IEnumerator<IRegistration> IEnumerable<IRegistration>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Type ImplementationType { get; }
        public IReadOnlyList<Type> InterfaceTypes => interfaceTypes;
        public Lifetime Lifetime => Lifetime.Transient; // Collection reference is transient. So its members can have each lifetimes.

        public Type ElementType { get; }

        readonly List<Type> interfaceTypes;
        private readonly List<IRegistration> registrations;

        public CollectionInstanceProvider(Type elementType)
        {
            ElementType = elementType;
            ImplementationType = elementType.MakeArrayType();
            interfaceTypes = new List<Type>
            {
                RuntimeTypeCache.EnumerableTypeOf(elementType),
                RuntimeTypeCache.ReadOnlyListTypeOf(elementType),
                RuntimeTypeCache.ListTypeOf(elementType),
                RuntimeTypeCache.IListTypeOf(elementType),
            };
            registrations = new List<IRegistration>();
        }

        public override string ToString()
        {
            var contractTypes = InterfaceTypes != null ? string.Join(", ", InterfaceTypes) : "";
            return $"CollectionRegistration {ImplementationType} ContractTypes=[{contractTypes}] {Lifetime}";
        }

        public void Add(IRegistration registration)
        {
            foreach (var x in registrations)
            {
                if (x.Lifetime == Lifetime.Singleton && x.ImplementationType == registration.ImplementationType
                                                     && registration.Condition == null)
                {
                    throw new VContainerException(registration.ImplementationType, $"Conflict implementation type : {registration}");
                }
            }
            registrations.Add(registration);
        }

        public void Merge(CollectionInstanceProvider other)
        {
            foreach (var x in other.registrations)
            {
                Add(x);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object SpawnInstance(IObjectResolver resolver)
        {
            if (registrations.Count == 0)
            {
                var listType = typeof(List<>);
                var constructedListType = listType.MakeGenericType(ElementType);
                var instance = Activator.CreateInstance(constructedListType);
                return instance;
            }

            var array = Array.CreateInstance(ElementType, registrations.Count);
            for (var i = 0; i < registrations.Count; i++)
            {
                array.SetValue(resolver.Resolve(registrations[i]), i);
            }
            return array;
        }
    }
}
