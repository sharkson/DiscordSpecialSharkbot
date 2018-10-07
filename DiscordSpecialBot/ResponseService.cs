using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    public class ResponseService
    {
        UserDetailService userDetailService;
        ChatApiService chatApiService;
        IgnoreMessageService ignoreMessageService;
        UseResponseService useResponseService;

        DiscordClient discord;
        public ResponseService(DiscordClient discordClient)
        {
            discord = discordClient;
            userDetailService = new UserDetailService();
            chatApiService = new ChatApiService();
            ignoreMessageService = new IgnoreMessageService();
            useResponseService = new UseResponseService(discord);
        }

        public async Task<bool> ReadMessage(MessageCreateEventArgs e)
        {
            var responded = false;
            if (e.Message.Author.Username == discord.CurrentUser.Username)
            {
                if(NsfwAllowed(e))
                {
                    await chatApiService.UpdateChatAsync(e, ConfigurationService.NsfwChatType);
                }
                else
                {
                    await chatApiService.UpdateChatAsync(e, ConfigurationService.ChatType);
                }
                
                return responded;
            }

            if (!ignoreMessageService.ignoreMessage(e, discord))
            {
                if (NsfwAllowed(e))
                {
                    await chatApiService.UpdateChatAsync(e, ConfigurationService.NsfwChatType);
                    responded = await ReadMessage(e, ConfigurationService.RequiredNsfwPropertyMatches, ConfigurationService.DefaultNsfwResponse, ConfigurationService.TargetedNsfwResponseConfidenceThreshold, ConfigurationService.NsfwChatType, ConfigurationService.NsfwExclusiveTypes, ConfigurationService.NsfwExcludedTypes, ConfigurationService.RequiredNsfwPropertyMatches);
                    if(!responded && ConfigurationService.NsfwAlternateResponseThreshold > 0)
                    {
                        responded = await ReadMessage(e, ConfigurationService.RequiredPropertyMatches, ConfigurationService.DefaultResponse, ConfigurationService.NsfwAlternateResponseThreshold, ConfigurationService.NsfwChatType, ConfigurationService.ExclusiveTypes, ConfigurationService.ExcludedTypes, ConfigurationService.RequiredPropertyMatches);
                    }
                }
                else
                {
                    await chatApiService.UpdateChatAsync(e, ConfigurationService.ChatType);
                    responded = await ReadMessage(e, ConfigurationService.RequiredPropertyMatches, ConfigurationService.DefaultResponse, ConfigurationService.TargetedResponseConfidenceThreshold, ConfigurationService.ChatType, ConfigurationService.ExclusiveTypes, ConfigurationService.ExcludedTypes, ConfigurationService.RequiredPropertyMatches);
                }
            }

            return responded;
        }

        private async Task<bool> ReadMessage(MessageCreateEventArgs e, List<string> requiredProperyMatches, string defaultResponse, double confidenceThreshold, string chatType, List<string> exclusiveTypes, List<string> excludedTypes, List<string> requiredPropertyMatches)
        {
            bool responded = false;
            var hasRequiredProperty = await userDetailService.HasRequiredPropertyAsync(e, requiredProperyMatches);
            if (hasRequiredProperty)
            {
                var chatResponse = await chatApiService.GetResponseAsync(e, chatType, exclusiveTypes, excludedTypes, requiredPropertyMatches);
                responded = await useResponseService.useChatResponse(e, chatResponse, confidenceThreshold);
            }
            else if(!string.IsNullOrEmpty(defaultResponse))
            {
                useResponseService.useDefaultResponse(e, defaultResponse);
                return true;
            }
            return responded;
        }

        private bool NsfwAllowed(MessageCreateEventArgs e)
        {
            if(e.Channel.IsNSFW || e.Message.Channel.Type == ChannelType.Private || e.Message.Channel.Type == ChannelType.Group)
            {
                return true;
            }

            return false;
        }
    }
}
