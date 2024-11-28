namespace BonGames.Tools
{
    public static class Domain
    {
        public static void LogI(string message)
        {
            UnityEngine.Debug.Log(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

        public static void LogW(string message)
        {
            UnityEngine.Debug.LogWarning(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

        public static void LogE(string message)
        {
            UnityEngine.Debug.LogError(message);
            System.Diagnostics.Debug.Fail(message);
        }
    }
}
