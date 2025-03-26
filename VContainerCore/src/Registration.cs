using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using VContainer.Internal;

namespace VContainer
{
    public interface IRegistration
    {
        Type ImplementationType { get; }
        List<Type> InterfaceTypes { get; }
        Lifetime Lifetime { get; }
        IInstanceProvider Provider { get; }
        BindingCondition? Condition { get; }
        
        object SpawnInstance(IObjectResolver resolver);
    }

    public readonly struct Registration : IRegistration
    {
        public Type ImplementationType { get; }
        public List<Type> InterfaceTypes { get; }
        public Lifetime Lifetime { get; }
        public IInstanceProvider Provider { get; }
        public BindingCondition? Condition { get; }
        
        internal Registration(
            Type implementationType,
            Lifetime lifetime,
            List<Type> interfaceTypes,
            IInstanceProvider provider,
            BindingCondition? condition)
        {
            ImplementationType = implementationType;
            InterfaceTypes = interfaceTypes;
            Lifetime = lifetime;
            Provider = provider;
            Condition = condition;
        }

        public override string ToString()
        {
            var contractTypes = InterfaceTypes != null ? string.Join(", ", InterfaceTypes) : "";
            return $"Registration {ImplementationType.Name} ContractTypes=[{contractTypes}] {Lifetime} {Provider}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object SpawnInstance(IObjectResolver resolver) => Provider.SpawnInstance(resolver);
    }
}
