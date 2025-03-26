using System;
using System.Collections.Generic;
using Godot;

namespace VContainer.Godot
{
    internal readonly struct PrefabComponentProvider : IInstanceProvider
    {
        private readonly IInjector _injector;
        private readonly IReadOnlyList<IInjectParameter> _customParameters;
        private readonly PackedScene _prefab;
        private readonly Action<object> _callback;
        private readonly ComponentDestination? _destination;

        public PrefabComponentProvider(
            PackedScene prefab,
            IInjector injector,
            IReadOnlyList<IInjectParameter> customParameters,
            Action<object> callback,
            in ComponentDestination? destination)
        {
            _injector = injector;
            _customParameters = customParameters;
            _callback = callback;
            _prefab = prefab;
            _destination = destination;
        }

        public object SpawnInstance(IObjectResolver resolver)
        {
            if (_prefab != null)
            {
                return SpawnInstanceWithPrefab(resolver);
            }

            throw new ArgumentException(
                $"_prefab is null {_prefab == null}");
        }

        private object SpawnInstanceWithPrefab(IObjectResolver resolver)
        {
            var parent = _destination?.GetParent();
            var node = _prefab.Instantiate();
            parent?.AddChild(node);

            _injector.Inject(node, resolver, _customParameters);

            _callback?.Invoke(node);

            return node;
        }
    }
}
