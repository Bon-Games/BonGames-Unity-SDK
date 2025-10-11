using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BonGames.Tools
{
    public static class AssemblyUtils
    {
        public static IEnumerable<Assembly> GetDependentAssemblies(Assembly targetAssembly)
        {
            var targetName = targetAssembly.GetName().Name;

            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetReferencedAssemblies().Any(r => r.Name == targetName));
        }

        public static IEnumerable<Type> GetTypesByAttribute<TAttribute>(Type baseType, bool includeDependent) where TAttribute : Attribute
        {
            Assembly baseAssembly = Assembly.GetAssembly(baseType);
            IEnumerable<Type> typesInBaseAssembly = Assembly.GetAssembly(baseType).GetTypes().Where(t => t.GetCustomAttribute<TAttribute>() != null);
            if (includeDependent)
            {
                IEnumerable<Assembly> dependentAssemblies = AssemblyUtils.GetDependentAssemblies(baseAssembly);
                foreach (Assembly dependent in dependentAssemblies)
                {
                    typesInBaseAssembly.Union(dependent.GetTypes().Where(t => t.GetCustomAttribute<TAttribute>() != null));
                }
            }
            return typesInBaseAssembly;
        }

        public static IEnumerable<Type> GetTypes(Type baseType, bool includeDependent)
        {
            Assembly baseAssembly = Assembly.GetAssembly(baseType);
            IEnumerable<Type> typesInBaseAssembly = Assembly.GetAssembly(baseType).GetTypes();
            if (includeDependent)
            {
                IEnumerable<Assembly> dependentAssemblies = AssemblyUtils.GetDependentAssemblies(baseAssembly);
                foreach (Assembly dependent in dependentAssemblies)
                {
                    typesInBaseAssembly.Union(dependent.GetTypes());
                }
            }
            return typesInBaseAssembly;
        }

        public static object GetValueByPath(object target, params string[] parts)
        {
            if (target == null) return null;

            object current = target;

            foreach (var part in parts)
            {
                Type type = current.GetType();

                // public property first
                PropertyInfo prop = type.GetProperty(part, BindingFlags.Public | BindingFlags.Instance);

                if (prop != null)
                {
                    current = prop.GetValue(current);
                    continue;
                }

                // field (public or private)
                FieldInfo field = type.GetField(part, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    current = field.GetValue(current);
                    continue;
                }

                // not found
                return null;
            }

            return current;
        }
    }
}
