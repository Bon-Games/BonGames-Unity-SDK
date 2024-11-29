
namespace BonGames.Tools
{
    using System.Collections.Generic;

    public static class EnvironmentArguments
    {
        public static class Key
        {
            public const string ApplicationMode = "-app-mode";
            public const string NetworkMode = "-net-mode";
            /// <summary>
            /// The port at which the specific server/client instance is accessible.
            /// </summary>
            public const string Port = "-port";
            /// <summary>
            /// The IP address of the server instance.
            /// </summary>
            public const string Ipv4 = "-ipv4";
            /// <summary>
            /// The IPv6 address of the server instance, if available.
            /// </summary>
            public const string Ipv6 = "-ipv6";

            // Additional for Unity Game Server Hosting (Multiplay)
            /// <summary>
            /// The unique ID of the allocation
            /// </summary>
            public const string AllocatedID = "-allocatedID";
            /// <summary>
            /// The IP address of the server instance. (Ipv4)
            /// </summary>
            public const string Ip = "-ip";
            /// <summary>
            /// The unique identifier of the server instance.
            /// </summary>
            public const string ServerID = "-serverID";
            /// <summary>
            /// The query protocol the server instance uses.
            /// </summary>
            public const string QueryType = "-queryType";
            /// <summary>
            /// The port at which you can access the query protocol data.
            /// </summary>
            public const string QueryPort = "-queryPort";
        }

        private static Dictionary<string, string> s_startupArguments { get; set; }

        static EnvironmentArguments()
        {
            Load();
        }

        public static void Load()
        {
            s_startupArguments = GetCommandlineArgs();
            Print();
        }

        public static void Print()
        {
            if (s_startupArguments != null)
            {
                Domain.LogI($"Environment Arguments");
                foreach (var item in s_startupArguments)
                {
                    Domain.LogI($"{item.Key}:{item.Value}");
                }
            }
        }

        public static Dictionary<string, string> GetCommandlineArgs()
        {
            Dictionary<string, string> argDictionary = new();

            string[] args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].ToLower();
                if (arg.StartsWith("-"))
                {
                    string value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    if (!string.IsNullOrEmpty(value))
                    {
                        value = value.StartsWith("-") ? null : value;
                    }
                    argDictionary.Add(arg, value);
                }
            }

            // Fill out default arguments if they do not exist
            Dictionary<string, string> defaultArgs = LoadDefaultArguments();
            foreach (KeyValuePair<string, string> it in defaultArgs)
            {
                if (argDictionary.ContainsKey(it.Key)) continue;

                argDictionary.Add(it.Key, it.Value);
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

        private static string DefaultArgumentsFilePath()
        {
            return System.IO.Path.Combine(UnityEngine.Application.dataPath, "../default.conf");
        }

        private static Dictionary<string, string> LoadDefaultArguments()
        {
            string filePath = DefaultArgumentsFilePath();
            Dictionary<string, string> res = new();
            if (System.IO.File.Exists(filePath))
            {
                IEnumerator<string> lineItor = System.IO.File.ReadLines(filePath).GetEnumerator();
                while (lineItor.MoveNext())
                {
                    string[] kvPair = lineItor.Current.Trim().Split(' ', System.StringSplitOptions.None);

                    if (kvPair.Length < 1 || string.IsNullOrEmpty(kvPair[0])) continue;

                    res[kvPair[0]] = kvPair.Length > 1 ? kvPair[1] : null;
                }
            }
            return res;
        }
    }
}
