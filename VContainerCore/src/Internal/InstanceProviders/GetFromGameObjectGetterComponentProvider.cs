using System;
using Godot;

namespace VContainer.Internal
{
    internal readonly struct GetFromGameObjectGetterComponentProvider : IInstanceProvider
    {
        private readonly Type _componentType;
        private readonly Node _gameObject;

        public GetFromGameObjectGetterComponentProvider(
            Type componentType, Node gameObject)
        {
            _componentType = componentType;
            _gameObject = gameObject;
        }

        public object SpawnInstance(IObjectResolver resolver)
        {
            return _gameObject;
        }
    }
}
