using System.Runtime.CompilerServices;

namespace VContainer.Internal
{
    internal readonly struct ExistingInstanceProvider : IInstanceProvider
    {
        readonly object implementationInstance;

        public ExistingInstanceProvider(object implementationInstance)
        {
            this.implementationInstance = implementationInstance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object SpawnInstance(IObjectResolver resolver) => implementationInstance;
    }
}
