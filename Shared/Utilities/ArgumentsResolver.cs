
namespace BonGames.CommandLine
{
    using System.Text;
    using BonGames.Tools;
    using System.Collections.Generic;

    public static class ArgumentsResolver
    {     
        private static Dictionary<string, string> s_startupArguments { get; set; }

        static ArgumentsResolver()
        {
            Load();
        }

        public static void Load()
        {
            s_startupArguments = GetCommandlineArgs();
            Print();
        }

        public static void Load(string profilePath)
        {
            s_startupArguments = GetCommandlineArgs(profilePath);
            Print();
        }

        public static void Print()
        {
            StringBuilder logger = new();
            if (s_startupArguments != null)
            {
                logger.AppendLine($"Command Arguments");
                foreach (var item in s_startupArguments)
                {
                    logger.AppendLine($"{item.Key}:{item.Value}");
                }
            }
            Domain.LogI($"{logger}");
        }

        public static Dictionary<string, string> GetCommandlineArgs(string defaultProfile = null)
        {
            Dictionary<string, string> argDictionary = new();

            string[] args = System.Environment.GetCommandLineArgs();
            const string dash = "-";

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith(dash))
                {
                    string value = i < args.Length - 1 ? args[i + 1] : null;
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.StartsWith(dash) ? null : value;
                    }
                    argDictionary.Add(arg, value);
                }
            }

            // Fill out default arguments if they do not exist
            BonGames.Tools.Domain.LogW($"Attemp to load default arguments");
            Dictionary<string, string> defaultArgs = string.IsNullOrEmpty(defaultProfile) || !System.IO.File.Exists(defaultProfile)
                ? LoadDefaultArguments() : LoadArguments(defaultProfile);
            foreach (KeyValuePair<string, string> it in defaultArgs)
            {
                string key = it.Key;
                string keyLower = it.Key.ToLower();
                if (argDictionary.ContainsKey(key) || argDictionary.ContainsKey(keyLower))
                {
                    BonGames.Tools.Domain.LogW($"Default arugment {key} is ignore due to value is passed in Value:{it.Value}");
                    continue;
                }

                argDictionary.Add(key, it.Value);
            }

            return argDictionary;
        }

        public static string GetEnvironmentArgument(string key)
        {
            if (s_startupArguments == null || s_startupArguments.Count == 0)
            {
                s_startupArguments = GetCommandlineArgs();
            }

            if (s_startupArguments.ContainsKey(key))
            {
                return s_startupArguments[key];
            }
            string keyLower = key.ToLower();
            if (s_startupArguments.ContainsKey(keyLower))
            {
                return s_startupArguments[keyLower];
            }

            return null;
        }

        public static string DefaultArgumentsFilePath()
        {
            return System.IO.Path.Combine(UnityEngine.Application.dataPath, "..", ".args.default");
        }

        public static Dictionary<string, string>  LoadArguments(string filePath)
        {
            Dictionary<string, string> res = new();
            if (System.IO.File.Exists(filePath))
            {
                IEnumerator<string> lineItor = System.IO.File.ReadLines(filePath).GetEnumerator();
                while (lineItor.MoveNext())
                {
                    string[] kvPair = lineItor.Current.Trim().Split('=', System.StringSplitOptions.None);

                    if (kvPair.Length < 1 || string.IsNullOrEmpty(kvPair[0])) continue;

                    res[kvPair[0]] = kvPair.Length > 1 ? kvPair[1] : null;
                }
            }
            else
            {
                BonGames.Tools.Domain.LogW($"Default args does not exist at {filePath}");
            }
            return res;
        }

        private static Dictionary<string, string> LoadDefaultArguments()
        {
            return LoadArguments(DefaultArgumentsFilePath());
        }
    }
}
