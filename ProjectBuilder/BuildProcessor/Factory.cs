using BonGames.Tools;
using log4net.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public interface ITask
    {

    }

    class ConfigurationAttribute : Attribute 
    {
        public string Id;
    }

    public class Factory : MonoBehaviour
    {

        private Dictionary<string, Type> _taskById;

        public void Get()
        {
            Type baseType = typeof(ITask);
            IEnumerable<Type> types = AssemblyUtils.GetTypes(baseType, true);
            foreach (Type taskType in types)
            {
                if (!IsValidType(baseType, taskType)) continue;

                ConfigurationAttribute att = taskType.GetCustomAttribute<ConfigurationAttribute>();
                string id = att == null || string.IsNullOrEmpty(att.Id) ? taskType.Name : att.Id;
                if (_taskById.ContainsKey(id))
                {
                    UnityEngine.Debug.LogError("Configuration Id {id} is duplicated");
                    continue;
                }
                _taskById.Add(id, taskType);
            }
        }

        private bool IsValidType(Type baseType, Type t)
        {
            return !t.IsAbstract && !t.IsGenericTypeDefinition && baseType.IsAssignableFrom(t);
        }
    }
}
