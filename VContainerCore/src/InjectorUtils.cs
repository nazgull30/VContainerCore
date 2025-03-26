using System.Reflection;

namespace VContainer
{
    public static class InjectorUtils
    {
        public static void InjectNotPublicField(object instance, string fieldName, object value)
        {
            var fieldInfo = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            fieldInfo?.SetValue(instance, value);
        }
        
        public static void InjectNotPublicProperty(object instance, string propertyName, object value)
        {
            var propertyInfo = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            propertyInfo?.SetValue(instance, value);
        }
        
        public static void InjectNotPublicMethod(object instance, string methodName, params object[] values)
        {
            var methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            methodInfo?.Invoke(instance, values);
        }
    }
}