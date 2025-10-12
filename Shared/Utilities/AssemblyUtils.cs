using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BonGames.Tools
{
    public static class AssemblyUtils
    {
        public const string UnityAssemblyCSharp = "Assembly-CSharp";

        public static IEnumerable<Assembly> GetDependentAssemblies(Assembly targetAssembly)
        {
            string targetName = targetAssembly.GetName().Name;
            return AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetName().Name.Equals(UnityAssemblyCSharp) || a.GetReferencedAssemblies().Any(r => r.Name.Equals(targetName)));
        }

        public static IEnumerable<Type> GetTypesByAttribute<TAttribute>(Type baseType, bool includeDependent) where TAttribute : Attribute
        {
            return GetTypes(baseType, includeDependent).Where(t => t.GetCustomAttribute<TAttribute>() != null);
        }

        public static IEnumerable<Type> GetTypes(Type baseType, bool includeDependent)
        {
            Assembly baseAssembly = Assembly.GetAssembly(baseType);
            List<Type> types = new List<Type>(baseAssembly.GetTypes());
            if (includeDependent)
            {
                IEnumerable<Assembly> dependentAssemblies = AssemblyUtils.GetDependentAssemblies(baseAssembly);
                foreach (Assembly dependent in dependentAssemblies)
                {
                    types.AddRange(dependent.GetTypes()); 
                }
            }
            return types;
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
