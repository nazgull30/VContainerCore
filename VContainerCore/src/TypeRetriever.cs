using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VContainer.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExternalTypeRetrieverAttribute : Attribute
    {
        
    }

    public interface IExternalTypeRetriever
    {
        List<Type> GetTypes();
    }

    public class TypeRetriever
    {
        public List<Type> GetAllTypesToGenerate()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var externalTypes = GetExternalTypes(assemblies);
            var withGenerateInjectorAttribute = GetAllTypesWithGenerateInjector(assemblies);
            var res = new List<Type>();
            res.AddRange(externalTypes);
            res.AddRange(withGenerateInjectorAttribute);
            return res.Distinct().ToList();
        }

        private List<Type> GetExternalTypes(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var externalTypeRetriever = type.GetCustomAttribute<ExternalTypeRetrieverAttribute>();
                    if (externalTypeRetriever != null)
                    {
                        var retriever = (IExternalTypeRetriever)Activator.CreateInstance(type);
                        return retriever.GetTypes();
                    }
                }
            }
            return new List<Type>();
        }
        
        private List<Type> GetAllTypesWithGenerateInjector(Assembly[] assemblies)
        {
            var res = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var generateInjectorAttribute = type.GetCustomAttribute<GenerateInjectorAttribute>();
                    if(generateInjectorAttribute == null)
                        continue;
                    res.Add(type);
                }
            }
            return res;
        }
    }

}