using System;

namespace VContainer.Utils
{
	public static class TypeExtensions
	{
		public static bool DerivesFromOrEqual(this Type a, Type b)
		{
#if UNITY_WSA && ENABLE_DOTNET && !UNITY_EDITOR
            return b == a || b.GetTypeInfo().IsAssignableFrom(a.GetTypeInfo());
#else
			return b == a || b.IsAssignableFrom(a);
#endif
		}
	}
}
