
namespace BonGames.UniConfigurator
{
    using System;
    using BonGames.Tools;
    using System.Collections.Generic;

    public class UniRecordTypes<T> where T : IUniRecord
    {
        private readonly Dictionary<string, Type> _typeByName = new();

        public UniRecordTypes()
        {
            GatherTypes();
        }

        ~UniRecordTypes()
        {
            Dispose();
        }

        private void GatherTypes()
        {
            Type baseType = typeof(T);
            IEnumerable<Type> types = AssemblyUtils.GetTypes(baseType, true);
            foreach (Type taskType in types)
            {
                if (!IsValidType(baseType, taskType)) continue;

                _typeByName.Add(taskType.FullName, taskType);
            }
        }

        private bool IsValidType(Type baseType, Type t)
        {
            return !t.IsAbstract && !t.IsGenericTypeDefinition && baseType.IsAssignableFrom(t);
        }

        public void Dispose()
        {
            if (_typeByName != null)
            {
                _typeByName.Clear();
            }
        }

        public T Create(string id)
        {
            if (_typeByName.ContainsKey(id))
            {
                return (T)Activator.CreateInstance(_typeByName[id]);
            }
            return default;
        }

        public T Create(string id, params object[] args)
        {
            if (_typeByName.ContainsKey(id))
            {
                return (T)Activator.CreateInstance(_typeByName[id], args);
            }
            return default;
        }

        public IEnumerable<string> CreatableIds() => _typeByName.Keys;
    }
}
