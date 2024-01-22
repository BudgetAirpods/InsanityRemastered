using BepInEx.Logging;

namespace InsanityRemasteredMod
{
    internal static class InsanityRemasteredLogger
    {
        internal static ManualLogSource logSource;

        public static void Initialize(string modGUID)
        {
            logSource = Logger.CreateLogSource(modGUID);
        }

        public static void Log(object message)
        {
            logSource.LogMessage(message);
        }

        public static void LogError(object message)
        {
            logSource.LogError(message);
        }

        public static void LogWarning(object message)
        {
            logSource.LogWarning(message);
        }
    }
}
