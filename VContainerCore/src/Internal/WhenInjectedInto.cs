using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer.Internal
{
	internal readonly struct WhenInjectedInto : IRegistration
	{
		public Type ImplementationType { get; }
		public List<Type> InterfaceTypes { get; }
		public Lifetime Lifetime { get; }
		public IInstanceProvider Provider { get; }
		public BindingCondition? Condition { get; }

		private readonly List<IRegistration> _registrations;

		internal WhenInjectedInto(
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
			_registrations = new List<IRegistration>();
		}

		public void AddRegistration(IRegistration registration)
		{
			_registrations.Add(registration);
		}
            
		public void Get(Type implementationType, out IRegistration registration, Type injectToType)
		{
			IRegistration res = null;
			foreach (var reg in _registrations)
			{
				if(!reg.InterfaceTypes.Contains(implementationType))
					continue;
                    
				if (reg.Condition.HasValue && reg.Condition.Value.Value(injectToType))
				{
					
					
					res = reg;
					break;
				}
			}
			registration = res;
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