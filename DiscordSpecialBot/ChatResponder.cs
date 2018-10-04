using ChatModels;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    public class ChatResponder
    {
        HttpClient client;
        static long launchTime;

        public ChatResponder(HttpClient httpClient)
        {
            client = httpClient;
            launchTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public async Task<ChatResponse> GetChatResponseAsync(MessageCreateEventArgs e)
        {
            var chat = GetChat(e);
            var conversationName = GetConversationName(e);
            var metadata = GetMetadata(e);
            var chatRequest = new ChatRequest { chat = chat, type = ConfigurationService.ChatType, conversationName = conversationName, metadata = metadata, requestTime = DateTime.Now, exclusiveTypes = ConfigurationService.ExclusiveTypes, excludedTypes = ConfigurationService.ExcludedTypes, requiredProperyMatches = ConfigurationService.RequiredProperyMatches };

            var httpContent = GetHttpContent(chatRequest);
            var response = await client.PutAsync(ConfigurationService.ApiUrl + "/api/chat", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(jsonResponse);

            return chatResponse;
        }

        public async Task<bool> UpdateChatAsync(MessageCreateEventArgs e)
        {
            var chat = GetChat(e);
            var conversationName = GetConversationName(e);
            var metadata = GetMetadata(e);
            var chatRequest = new ChatRequest { chat = chat, type = ConfigurationService.ChatType, conversationName = conversationName, metadata = metadata };

            var httpContent = GetHttpContent(chatRequest);
            var response = await client.PutAsync(ConfigurationService.ApiUrl + "/api/chatupdate", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var success = JsonConvert.DeserializeObject<bool>(jsonResponse);

            return success;
        }

        private StringContent GetHttpContent(ChatRequest chatRequest)
        {
            var jsonString = JsonConvert.SerializeObject(chatRequest);
            return new StringContent(jsonString, Encoding.UTF8, "application/json");
        }

        private Metadata GetMetadata(MessageCreateEventArgs e)
        {
            return new Metadata { channelId = e.Message.ChannelId, guildId = e.Message.Channel.GuildId };
        }

        private Chat GetChat(MessageCreateEventArgs e)
        {
            var message = e.Message.Content;
            foreach(var mention in e.MentionedUsers)
            {
                message = message.Replace(mention.Mention.Replace("!",""), "@" + mention.Username);
                message = message.Replace(mention.Mention, "@" + mention.Username);
            }
            return new Chat { botName = ConfigurationService.BotName, message = message, user = e.Message.Author.Username, time = DateTimeOffset.Now.ToUnixTimeMilliseconds() };
        }

        private string GetConversationName(MessageCreateEventArgs e)
        {
            return ConfigurationService.ChatType + "-discord-" + e.Message.Channel.GuildId + "-" + e.Message.ChannelId + "-" + launchTime;
        }
    }
}
