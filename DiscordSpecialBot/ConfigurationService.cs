using System.Collections.Generic;

namespace DiscordSpecialBot
{
    public static class ConfigurationService
    {
        public static string Token;
        public static string ApiUrl;
        public static string BotName;
        public static bool MentionOnly;
        public static List<string> IgnoredChannels;
        public static string ChatType;
        public static string NsfwChatType;
        public static List<string> ExclusiveTypes;
        public static List<string> NsfwExclusiveTypes;
        public static List<string> ExcludedTypes;
        public static List<string> NsfwExcludedTypes;
        public static List<string> RequiredPropertyMatches;
        public static List<string> RequiredNsfwPropertyMatches;
        public static List<string> NickNames;
        public static List<string> AllowedBots;
        public static double TargetedResponseConfidenceThreshold;
        public static double TargetedNsfwResponseConfidenceThreshold;
        public static double NsfwAlternateResponseThreshold;
        public static string DefaultResponse;
        public static string DefaultNsfwResponse;
    }
}
