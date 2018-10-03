using System;

namespace DiscordSpecialBot
{
    [Serializable]
    public class Metadata
    {
        public ulong guildId { get; set; }
        public ulong channelId { get; set; }
    }
}
