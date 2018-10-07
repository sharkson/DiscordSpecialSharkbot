using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Linq;

namespace DiscordSpecialBot
{
    public class IgnoreMessageService
    {
        public bool ignoreMessage(MessageCreateEventArgs e, DiscordClient discord)
        {
            if (e.Message.MentionedUsers.Any(u => u.Username == discord.CurrentUser.Username))
            {
                return false;
            }
            if (ignoredBot(e) || e.Message.MessageType != MessageType.Default || ConfigurationService.IgnoredChannels.Any(channel => channel == e.Message.Channel.Name))
            {
                return true;
            }
            return false;
        }

        private bool ignoredBot(MessageCreateEventArgs e)
        {
            if (e.Message.Author.IsBot)
            {
                if (ConfigurationService.AllowedBots.Any(b => b.ToLower() == e.Message.Author.Username.ToLower()))
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
