using BonGames.CommandLine;
using BonGames.EasyBuilder.Argument;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BonGames.EasyBuilder
{
    public static class ArgumentsExpander
    {
        private static readonly Dictionary<string, string> s_argumentNames = new();

        public static Dictionary<string, string> GetDefinedArguments()
        {
            if (!UnityEngine.Application.isBatchMode || s_argumentNames.Count == 0)
            {
                s_argumentNames.Clear(); 
                Dictionary<string, string> collection =
                           typeof(BuildArguments.Key)           .GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Union(typeof(BuildArguments.Android)       .GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                    .Union(typeof(BuildArguments.IOS)           .GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                    .Union(typeof(BuildArguments.Standalone)    .GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
                    .ToDictionary(p => p.Name, p => $"{p.GetRawConstantValue()}");


                foreach (KeyValuePair<string, string> it in collection)
                {
                    s_argumentNames[it.Key] = it.Value;
                }
            }
            return s_argumentNames;
        }

        /// <summary>
        /// Replace the placeholder tag with actual value
        /// Eg: [ProductCode]_[BuildVersionString] => SupperCoolGame_0.0.1
        /// </summary>
        /// <param name="pattern">This is made up by keys defined in <see cref="BuildArguments.Key"/>, <see cref="BuildArguments.Android"/>, ect.</param>
        /// <returns></returns>
        public static string ExpandArguments(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return string.Empty;

            StringBuilder res = new StringBuilder(pattern.Trim());
            Dictionary<string, string> argDefines = GetDefinedArguments();
            foreach (KeyValuePair<string, string> define in argDefines) 
            {
                string tag = $"[{define.Key}]";
                res = res.Replace(tag, ArgumentsResolver.GetEnvironmentArgument(define.Value));
            }
            return res.ToString();
        }
    }
}
