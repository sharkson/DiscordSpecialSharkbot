using ChatModels;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    public class UseResponseService
    {
        private DiscordClient discord;
        private UserDetailService userDetailService;

        public UseResponseService(DiscordClient discordClient)
        {
            discord = discordClient;
            userDetailService = new UserDetailService();
        }

        public async Task<bool> useChatResponse(MessageCreateEventArgs e, ChatResponse chatResponse, double confidenceThreshold)
        {
            var respond = await alwaysRespond(e);
            if (respond || (chatResponse.confidence > confidenceThreshold && !ConfigurationService.MentionOnly))
            {
                var typeTime = 0;
                foreach (var chat in chatResponse.response)
                {
                    DiscordEmoji emoji = null;
                    try
                    {
                        emoji = DiscordEmoji.FromName(discord, chat.Trim());
                    }
                    catch (Exception)
                    {

                    }

                    if (emoji != null)
                    {
                        await e.Message.CreateReactionAsync(emoji);
                    }
                    else
                    {
                        await Task.Delay(typeTime).ContinueWith((task) => { e.Channel.TriggerTypingAsync(); });

                        var response = chat.Trim();

                        response = formatResponse(e, chat);

                        typeTime += getTypeTime(chat);
                        await Task.Delay(typeTime).ContinueWith((task) => { e.Message.RespondAsync(response); });
                    }
                }
                return true;
            }
            return false;
        }

        public async void useDefaultResponse(MessageCreateEventArgs e, string defaultResponse)
        {
            await e.Channel.TriggerTypingAsync();
            var response = formatResponse(e, defaultResponse);
            var typeTime = getTypeTime(response);
            await Task.Delay(typeTime).ContinueWith((task) => { e.Message.RespondAsync(response); });
        }

        private string formatResponse(MessageCreateEventArgs e, string chat)
        {
            var response = chat.Trim();

            if (response.StartsWith("/me "))
            {
                response = Formatter.Italic(response.Replace("/me ", string.Empty));
            }

            var regex = new Regex("@(?<name>[^\\s]+)");
            var results = regex.Matches(response)
                .Cast<Match>()
                .Select(m => m.Groups["name"].Value)
                .ToArray();

            foreach (var userName in results)
            {
                var user = e.Guild.Members.Where(m => m.Username == userName).FirstOrDefault();
                if (user != null)
                {
                    var mention = Formatter.Mention(user);
                    response = response.Replace("@" + userName, mention);
                }
            }

            return response;
        }
        private async Task<bool> alwaysRespond(MessageCreateEventArgs e)
        {
            var nickNames = await userDetailService.GetNickNames(discord.CurrentUser.Username);
            if (e.Message.Channel.Type == ChannelType.Private || e.MentionedUsers.Contains(discord.CurrentUser) || 
                e.Message.Content.ToLower().Contains(discord.CurrentUser.Username.ToLower()) || e.Message.Content.ToLower().Contains(ConfigurationService.BotName.ToLower()) || 
                ConfigurationService.NickNames.Any(nickName => e.Message.Content.ToLower().Contains(nickName.ToLower())) ||
                nickNames.Any(nickName => e.Message.Content.ToLower().Contains(nickName.ToLower())))
            {
                return true;
            }
            return false;
        }

        private int getTypeTime(string message)
        {
            return message.Length * 80;
        }
    }
}
