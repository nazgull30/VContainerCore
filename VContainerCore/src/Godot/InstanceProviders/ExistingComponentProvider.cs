using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer.Godot
{
    internal readonly struct ExistingComponentProvider : IInstanceProvider
    {
        readonly object instance;
        readonly IInjector injector;
        readonly IReadOnlyList<IInjectParameter> customParameters;

        public ExistingComponentProvider(
            object instance,
            IInjector injector,
            IReadOnlyList<IInjectParameter> customParameters)
        {
            this.instance = instance;
            this.customParameters = customParameters;
            this.injector = injector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object SpawnInstance(IObjectResolver resolver)
        {
            injector.Inject(instance, resolver, customParameters);
            return instance;
        }
    }
}
