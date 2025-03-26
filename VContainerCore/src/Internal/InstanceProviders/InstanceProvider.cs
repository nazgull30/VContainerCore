using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer.Internal
{
    internal readonly struct InstanceProvider : IInstanceProvider
    {
        private readonly IInjector _injector;
        private readonly IReadOnlyList<IInjectParameter> _customParameters;

        public InstanceProvider(
            IInjector injector,
            IReadOnlyList<IInjectParameter> customParameters = null)
        {
            _injector = injector;
            _customParameters = customParameters;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object SpawnInstance(IObjectResolver resolver)
            => _injector.CreateInstance(resolver, _customParameters);
    }
}