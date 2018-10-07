using ChatModels;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace DiscordSpecialBot
{
    public class ApiUtilities
    {
        static long launchTime;
        
        public ApiUtilities()
        {
            launchTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public StringContent GetHttpContent(Object chatRequest)
        {
            var jsonString = JsonConvert.SerializeObject(chatRequest);
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        public Metadata GetMetadata(MessageCreateEventArgs e)
        {
            return new Metadata { channelId = e.Message.ChannelId, guildId = e.Message.Channel.GuildId };
        }

        public Chat GetChat(MessageCreateEventArgs e)
        {
            var message = e.Message.Content;
            foreach (var mention in e.MentionedUsers)
            {
                message = message.Replace(mention.Mention.Replace("!", ""), "@" + mention.Username);
                message = message.Replace(mention.Mention, "@" + mention.Username);
            }
            return new Chat { botName = ConfigurationService.BotName, message = message, user = e.Message.Author.Username, time = DateTimeOffset.Now.ToUnixTimeMilliseconds() };
        }

        public string GetConversationName(MessageCreateEventArgs e, string type)
        {
            return type + "-discord-" + e.Message.Channel.GuildId + "-" + e.Message.ChannelId + "-" + launchTime;
        }
    }
}
