using System;

namespace VContainer.Internal
{
    internal readonly struct ContainerLocalInstanceProvider : IInstanceProvider
    {
        readonly Type wrappedType;
        readonly IRegistration valueRegistration;

        public ContainerLocalInstanceProvider(Type wrappedType, IRegistration valueRegistration)
        {
            this.wrappedType = wrappedType;
            this.valueRegistration = valueRegistration;
        }

        public object SpawnInstance(IObjectResolver resolver)
        {
            var value = resolver.Resolve(valueRegistration);
            var parameterValues = CappedArrayPool<object>.Shared8Limit.Rent(1);
            try
            {
                parameterValues[0] = value;
                return Activator.CreateInstance(wrappedType, parameterValues);
            }
            finally
            {
                CappedArrayPool<object>.Shared8Limit.Return(parameterValues);
            }
        }
    }
}