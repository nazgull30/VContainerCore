using System;
using System.Collections.Generic;
using Godot;
using PdEventBus.Impls;
using VContainer.Utils;

namespace VContainer.Internal
{
	public struct FromComponentOnRegistrationBuilder : IRegistrationBuilder
	{
		private readonly Node _gameObject;
		
		public Type ImplementationType { get; }
		public Lifetime Lifetime { get; }

		public List<Type> InterfaceTypes  { get; private set; }
		public List<IInjectParameter> Parameters { get; private set; }
		public BindingCondition Condition { get; private set; }

		public FromComponentOnRegistrationBuilder(
			Node gameObject,
			Type implementationType, Lifetime lifetime)
		{
			_gameObject = gameObject;
			ImplementationType = implementationType;
			Lifetime = lifetime;
			InterfaceTypes = null;
			Parameters = null;
			Condition = default;
		}
		
		
		public IRegistration Build()
		{
			var spawner = new GetFromGameObjectGetterComponentProvider(ImplementationType, _gameObject);
			return new Registration(ImplementationType, Lifetime, InterfaceTypes, spawner, Condition);
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