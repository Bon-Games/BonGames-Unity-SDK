using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BonGames.CommandLine
{
    public static class StartupArguments
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
    }
}
