using System;

namespace VContainer.Internal
{
	public enum EBindingConditionType
	{
		None,
		WhenInjectedTo
	}
	
	public readonly struct BindingCondition
	{
		public readonly EBindingConditionType ConditionType;
		public readonly Func<Type, bool> Value;

		public BindingCondition(EBindingConditionType conditionType, Func<Type, bool> value)
		{
			ConditionType = conditionType;
			Value = value;
		}
	}
}