using System.Collections.Generic;

namespace DiscordSpecialBot
{
    public static class ConfigurationService
    {
        public static string Token;
        public static string ApiUrl;
        public static string BotName;
        public static List<string> IgnoredChannels;
        public static string ChatType;
        public static List<string> ExclusiveTypes;
        public static List<string> RequiredProperyMatches;
        public static List<string> NickNames;
        public static double TargetedResponseConfidenceThreshold;
        public static string DefaultResponse;
    }
}
