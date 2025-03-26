using System;
using System.Collections.Generic;
using System.Linq;

namespace VContainer.Internal
{
    public readonly struct Registry
    {
        [ThreadStatic]
        static IDictionary<Type, IRegistration> buildBuffer = new Dictionary<Type, IRegistration>(128);

        readonly FixedTypeKeyHashtable<IRegistration> hashTable;

        public static Registry Build(IRegistration[] registrations)
        {
            // ThreadStatic
            if (buildBuffer == null)
                buildBuffer = new Dictionary<Type, IRegistration>(128);
            buildBuffer.Clear();


            buildBuffer.Add(typeof(WhenInjectedInto), new WhenInjectedInto(
                    null,
                    Lifetime.Transient,
                    null, null, null));

            foreach (var registration in registrations)
            {
                if (registration.InterfaceTypes is IReadOnlyList<Type> interfaceTypes)
                {
                    // ReSharper disable once ForCanBeConvertedToForeach
                    for (var i = 0; i < interfaceTypes.Count; i++)
                    {
                        AddToBuildBuffer(buildBuffer, interfaceTypes[i], registration);
                    }

                    // Mark the ImplementationType with a guard because we need to check if it exists later.
                    if (!buildBuffer.ContainsKey(registration.ImplementationType))
                    {
                        buildBuffer.Add(registration.ImplementationType, null);
                    }
                }
                else
                {
                    AddToBuildBuffer(buildBuffer, registration.ImplementationType, registration);
                }
            }

            var hashTable = new FixedTypeKeyHashtable<IRegistration>(buildBuffer.ToArray());
            return new Registry(hashTable);
        }

        static void AddToBuildBuffer(IDictionary<Type, IRegistration> buf, Type service, IRegistration registration)
        {
            if (buf.TryGetValue(service, out var exists))
            {
                CollectionInstanceProvider collection;
                if (buf.TryGetValue(RuntimeTypeCache.EnumerableTypeOf(service), out var found) &&
                    found.Provider is CollectionInstanceProvider foundCollection)
                {
                    collection = foundCollection;
                }
                else
                {
                    collection = new CollectionInstanceProvider(service) { exists };
                    var newRegistration = new Registration(
                        RuntimeTypeCache.ArrayTypeOf(service),
                        Lifetime.Transient,
                        new List<Type>
                        {
                            RuntimeTypeCache.EnumerableTypeOf(service),
                            RuntimeTypeCache.ReadOnlyListTypeOf(service),
                            RuntimeTypeCache.ListTypeOf(service),
                            RuntimeTypeCache.IListTypeOf(service),
                        }, collection, null);
                    AddCollectionToBuildBuffer(buf, newRegistration);
                }
                collection.Add(registration);

                var injectedWhenInto = AddWhenInjectedInto(buf, registration);
                if (!injectedWhenInto)
                {
                    // Overwritten by the later registration
                    buf[service] = registration;
                }
            }
            else
            {
                var injectedWhenInto = AddWhenInjectedInto(buf, registration);
                if (!injectedWhenInto)
                {
                    // Overwritten by the later registration
                    buf[service] = registration;
                }
            }
        }

        static void AddCollectionToBuildBuffer(IDictionary<Type, IRegistration> buf, Registration collectionRegistration)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < collectionRegistration.InterfaceTypes.Count; i++)
            {
                var collectionType = collectionRegistration.InterfaceTypes[i];
                try
                {
                    buf.Add(collectionType, collectionRegistration);
                }
                catch (ArgumentException)
                {
                    throw new VContainerException(collectionType, $"Registration with the same key already exists: {collectionRegistration}");
                }
            }
        }

        Registry(FixedTypeKeyHashtable<IRegistration> hashTable)
        {
            this.hashTable = hashTable;
        }

        public bool TryGet(Type interfaceType, out IRegistration registration, Type injectToType = null)
        {
            var res = TryGetFromHashTable(interfaceType, out registration, injectToType);
            if (res)
                return registration != null;

            if (interfaceType.IsConstructedGenericType)
            {
                var openGenericType = RuntimeTypeCache.OpenGenericTypeOf(interfaceType);
                var typeParameters = RuntimeTypeCache.GenericTypeParametersOf(interfaceType);
                return TryFallbackToSingleElementCollection(interfaceType, openGenericType, typeParameters, out registration) ||
                       TryFallbackToContainerLocal(interfaceType, openGenericType, typeParameters, out registration);

            }
            return false;
        }

        public bool Exists(Type type) => hashTable.TryGet(type, out _);



























        bool TryFallbackToContainerLocal(
            Type closedGenericType,
            Type openGenericType,
            IReadOnlyList<Type> typeParameters,
            out IRegistration newRegistration)
        {
            if (openGenericType == typeof(ContainerLocal<>))
            {
                var valueType = typeParameters[0];
                if (TryGet(valueType, out var valueRegistration))
                {
                    var spawner = new ContainerLocalInstanceProvider(closedGenericType, valueRegistration);
                    newRegistration = new Registration(closedGenericType, Lifetime.Scoped, null, spawner, null);
                    return true;
                }
            }
            newRegistration = null;
            return false;
        }

        bool TryFallbackToSingleElementCollection(
            Type closedGenericType,
            Type openGenericType,
            IReadOnlyList<Type> typeParameters,
            out IRegistration newRegistration)
        {
            if (CollectionInstanceProvider.Match(openGenericType))
            {
                var elementType = typeParameters[0];
                var collection = new CollectionInstanceProvider(elementType);
                // ReSharper disable once InconsistentlySynchronizedField
                if (hashTable.TryGet(elementType, out var elementRegistration) && elementRegistration != null)
                {
                    collection.Add(elementRegistration);
                }
                newRegistration = new Registration(
                    RuntimeTypeCache.ArrayTypeOf(elementType),
                    Lifetime.Transient,
                    new List<Type>
                    {
                        RuntimeTypeCache.EnumerableTypeOf(elementType),
                        RuntimeTypeCache.ReadOnlyListTypeOf(elementType),
                        RuntimeTypeCache.ListTypeOf(elementType),
                        RuntimeTypeCache.IListTypeOf(elementType),
                    }, collection, null);
                return true;
            }
            newRegistration = null;
            return false;
        }

        private static bool AddWhenInjectedInto(IDictionary<Type, IRegistration> buf, IRegistration registration)
        {
            var ifWhenInjectedInto = registration.Condition is { ConditionType: EBindingConditionType.WhenInjectedTo };
            if (!ifWhenInjectedInto)
                return false;

            var whenInjectedIntoKey = buf[typeof(WhenInjectedInto)] as WhenInjectedInto?;
            whenInjectedIntoKey?.AddRegistration(registration);

            return true;
        }

        private bool TryGetFromHashTable(Type interfaceType, out IRegistration registration, Type injectToType)
        {
            if (injectToType == null)
            {
                if (hashTable.TryGet(interfaceType, out registration))
                    return registration != null;

                return false;
            }

            var whenInjectedIntoKey = hashTable.Get(typeof(WhenInjectedInto)) as WhenInjectedInto?;
            IRegistration res = null;
            whenInjectedIntoKey?.Get(interfaceType, out res, injectToType);
            if (res != null)
            {
                registration = res;
                return true;
            }

            if (hashTable.TryGet(interfaceType, out registration))
                return registration != null;

            return false;
        }
    }
}
