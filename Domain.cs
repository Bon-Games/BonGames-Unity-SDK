using System.Diagnostics;

namespace BonGames.Tools
{
    public static class Domain
    {
        [Conditional("ENABLE_LOG")]
        public static void LogI(string message)
        {
            UnityEngine.Debug.Log(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogW(string message)
        {
            UnityEngine.Debug.LogWarning(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

        [Conditional("ENABLE_LOG")]
        public static void LogE(string message)
        {
            UnityEngine.Debug.LogError(message);
            System.Diagnostics.Debug.Fail(message);
        }

        public static void ThrowIf(bool condition, string message)
        {
            if (condition)
            {
                LogE(message);
                throw new System.Exception(message);
            }
        }
    }
}
