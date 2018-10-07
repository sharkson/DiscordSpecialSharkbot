using ChatModels;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    public class ChatApiService
    {
        ApiUtilities apiUtilities;
        public ChatApiService()
        {
            apiUtilities = new ApiUtilities();
        }

        public async Task<ChatResponse> GetResponseAsync(MessageCreateEventArgs e, string chatType, List<string> exclusiveTypes, List<string> excludedTypes, List<string> requiredProperyMatches)
        {
            var chat = apiUtilities.GetChat(e);
            var conversationName = apiUtilities.GetConversationName(e, chatType);
            var metadata = apiUtilities.GetMetadata(e);
            var chatRequest = new ResponseRequest { type = chatType, conversationName = conversationName, metadata = metadata, requestTime = DateTime.Now, exclusiveTypes = exclusiveTypes, excludedTypes = excludedTypes, requiredProperyMatches = requiredProperyMatches };

            var httpContent = apiUtilities.GetHttpContent(chatRequest);
            var response = await ApiService.client.PutAsync(ConfigurationService.ApiUrl + "/api/response", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonConvert.DeserializeObject<ChatResponse>(jsonResponse);

            return chatResponse;
        }

        public async Task<bool> UpdateChatAsync(MessageCreateEventArgs e, string type)
        {
            var chat = apiUtilities.GetChat(e);
            var conversationName = apiUtilities.GetConversationName(e, type);
            var metadata = apiUtilities.GetMetadata(e);
            var chatRequest = new ChatRequest { chat = chat, type = type, conversationName = conversationName, metadata = metadata };

            var httpContent = apiUtilities.GetHttpContent(chatRequest);
            var response = await ApiService.client.PutAsync(ConfigurationService.ApiUrl + "/api/chatupdate", httpContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var success = JsonConvert.DeserializeObject<bool>(jsonResponse);

            return success;
        }
    }
}
