using System;
using System.Collections.Generic;
using Godot;
using PdEventBus.Impls;
using VContainer.Internal;
using VContainer.Utils;

namespace VContainer.Godot
{
    struct ComponentDestination
    {
        public Node Parent;
        public Func<Node> ParentFinder;

        public Node GetParent()
        {
            if (Parent != null)
                return Parent;
            if (ParentFinder != null)
                return ParentFinder();
            return null;
        }
    }

    public interface IComponentRegistrationBuilder : IRegistrationBuilder
    {
        IComponentRegistrationBuilder UnderTransform(Node parent);
        IComponentRegistrationBuilder UnderTransform(Func<Node> parentFinder);
        IComponentRegistrationBuilder OnInstantiated(Action<object> callback);
    }

    internal struct ComponentRegistrationBuilder : IComponentRegistrationBuilder
    {
        public Type ImplementationType { get; }
        public Lifetime Lifetime { get; }

        public List<Type> InterfaceTypes { get; private set; }
        public List<IInjectParameter> Parameters { get; private set; }
        public BindingCondition Condition { get; private set; }

        private readonly object _instance;
        private readonly PackedScene _prefab;
        private readonly string _gameObjectName;
        private readonly PackedScene _scene;

        private ComponentDestination? _destination;
        private Action<object> _callback;

        public ComponentRegistrationBuilder(object instance)
        {
            _instance = instance;
            ImplementationType = instance.GetType();
            Lifetime = Lifetime.Singleton;
            _prefab = null;
            _gameObjectName = null;
            _destination = null;
            _callback = null;
            _scene = null;
            InterfaceTypes = null;
            Parameters = null;
            Condition = default;
        }

        public ComponentRegistrationBuilder(in PackedScene scene, Type implementationType)
        {
            _scene = scene;
            ImplementationType = implementationType;
            Lifetime = Lifetime.Scoped;
            _instance = null;
            _prefab = null;
            _gameObjectName = null;
            _destination = null;
            _callback = null;
            InterfaceTypes = null;
            Parameters = null;
            Condition = default;
        }

        public ComponentRegistrationBuilder(
            PackedScene prefab,
            Type implementationType,
            Lifetime lifetime)
        {
            _prefab = prefab;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            _instance = null;
            _gameObjectName = null;
            _destination = null;
            _callback = null;
            _scene = null;
            InterfaceTypes = null;
            Parameters = null;
            Condition = default;
        }

        public ComponentRegistrationBuilder(
            Object unityObjectAsPrefab,
            Type implementationType,
            Lifetime lifetime)
        {
            ImplementationType = implementationType;
            Lifetime = lifetime;
            _instance = null;
            _prefab = null;
            _gameObjectName = null;
            _scene = null;
            _destination = null;
            _callback = null;
            InterfaceTypes = null;
            Parameters = null;
            Condition = default;
        }

        internal ComponentRegistrationBuilder(
            string gameObjectName,
            Type implementationType,
            Lifetime lifetime)
        {
            _gameObjectName = gameObjectName;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            _instance = null;
            _prefab = null;
            _scene = null;
            _destination = null;
            _callback = null;
            InterfaceTypes = null;
            Parameters = null;
            Condition = default;
        }

        public IRegistration Build()
        {
            var injector = InjectorCache.GetOrBuild(ImplementationType);
            IInstanceProvider provider = null;

            if (_instance != null)
            {
                provider = new ExistingComponentProvider(_instance, injector, Parameters);
            }
            else if (_scene != null)
            {
                // provider = new FindComponentProvider(ImplementationType, injector, Parameters, _scene, _destination);
            }
            else if (_prefab != null)
            {
                provider = new PrefabComponentProvider(_prefab, injector, Parameters, _callback, _destination);
            }
            else
            {
                // provider = new NewGameObjectProvider(ImplementationType, injector, Parameters, in _destination, _gameObjectName);
            }
            return new Registration(ImplementationType, Lifetime, InterfaceTypes, provider, Condition);
        }

        public IComponentRegistrationBuilder UnderTransform(Node parent)
        {
            _destination = new ComponentDestination()
            {
                Parent = parent
            };
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IComponentRegistrationBuilder UnderTransform(Func<Node> parentFinder)
        {
            _destination = new ComponentDestination()
            {
                ParentFinder = parentFinder
            };
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IComponentRegistrationBuilder OnInstantiated(Action<object> callback)
        {
            _callback = callback;
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder As<TInterface>()
            => As(typeof(TInterface));

        public IRegistrationBuilder As<TInterface1, TInterface2>()
            => As(typeof(TInterface1), typeof(TInterface2));

        public IRegistrationBuilder As<TInterface1, TInterface2, TInterface3>()
            => As(typeof(TInterface1), typeof(TInterface2), typeof(TInterface3));

        public IRegistrationBuilder As<TInterface1, TInterface2, TInterface3, TInterface4>()
            => As(typeof(TInterface1), typeof(TInterface2), typeof(TInterface3), typeof(TInterface4));

        public IRegistrationBuilder AsSelf()
        {
            AddInterfaceType(ImplementationType);
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder AsImplementedInterfaces()
        {
            InterfaceTypes = InterfaceTypes ?? new List<Type>();
            InterfaceTypes.AddRange(ImplementationType.GetInterfaces());
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder As(Type interfaceType)
        {
            AddInterfaceType(interfaceType);
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder As(Type interfaceType1, Type interfaceType2)
        {
            AddInterfaceType(interfaceType1);
            AddInterfaceType(interfaceType2);
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder As(Type interfaceType1, Type interfaceType2, Type interfaceType3)
        {
            AddInterfaceType(interfaceType1);
            AddInterfaceType(interfaceType2);
            AddInterfaceType(interfaceType3);
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder As(params Type[] interfaceTypes)
        {
            foreach (var interfaceType in interfaceTypes)
            {
                AddInterfaceType(interfaceType);
            }
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder WithParameter(string name, object value)
        {
            Parameters = Parameters ?? new List<IInjectParameter>();
            Parameters.Add(new NamedParameter(name, value));
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder WithParameter(Type type, object value)
        {
            Parameters = Parameters ?? new List<IInjectParameter>();
            Parameters.Add(new TypedParameter(type, value));
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder WithParameter<TParam>(TParam value)
        {
            return WithParameter(typeof(TParam), value);
        }

        public IRegistrationBuilder WhenInjectedInto<T>()
        {
            Condition = new BindingCondition(EBindingConditionType.WhenInjectedTo,
                t => t != null && t.DerivesFromOrEqual(typeof(T)));
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public IRegistrationBuilder WhenNotInjectedInto<T>()
        {
            Condition = new BindingCondition(EBindingConditionType.WhenInjectedTo,
                t => t == null || !t.DerivesFromOrEqual(typeof(T)));
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
            return this;
        }

        public void AddInterfaceType(Type interfaceType)
        {
            if (!interfaceType.IsAssignableFrom(ImplementationType))
            {
                throw new VContainerException(interfaceType, $"{ImplementationType} is not assignable from {interfaceType}");
            }
            InterfaceTypes = InterfaceTypes ?? new List<Type>();
            if (!InterfaceTypes.Contains(interfaceType))
                InterfaceTypes.Add(interfaceType);
            Event<RegistrationBuilderUpdated>.Fire(new RegistrationBuilderUpdated(this));
        }
    }
}
