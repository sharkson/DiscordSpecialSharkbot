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
    public class ResponseService
    {
        DiscordClient discord;
        public ResponseService(DiscordClient discordClient)
        {
            discord = discordClient;
        }

        public async void hasRequiredPropertyResponse(MessageCreateEventArgs e, ChatResponse chatResponse)
        {
            if (alwaysRespond(e) || chatResponse.confidence > ConfigurationService.TargetedResponseConfidenceThreshold)
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
            }
        }

        public async void defaultResponse(MessageCreateEventArgs e)
        {
            await e.Channel.TriggerTypingAsync();
            var response = formatResponse(e, ConfigurationService.DefaultResponse);
            var typeTime = getTypeTime(response);
            await Task.Delay(typeTime).ContinueWith((task) => { e.Message.RespondAsync(response); });
        }

        public string formatResponse(MessageCreateEventArgs e, string chat)
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

        public bool ignoreMessage(MessageCreateEventArgs e)
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
            if(e.Message.Author.IsBot)
            {
                if(ConfigurationService.AllowedBots.Any(b => b.ToLower() == e.Message.Author.Username.ToLower()))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool alwaysRespond(MessageCreateEventArgs e)
        {
            if (e.Message.Channel.Type == ChannelType.Private || e.MentionedUsers.Contains(discord.CurrentUser) || e.Message.Content.ToLower().Contains(discord.CurrentUser.Username.ToLower()) || e.Message.Content.ToLower().Contains(ConfigurationService.BotName.ToLower()) || ConfigurationService.NickNames.Any(nickName => e.Message.Content.ToLower().Contains(nickName.ToLower())))
            {
                return true;
            }
            return false;
        }

        public int getTypeTime(string message)
        {
            return message.Length * 80;
        }
    }
}
