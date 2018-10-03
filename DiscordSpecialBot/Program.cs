using DSharpPlus;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    class Program
    {
        static DiscordClient discord;
        static HttpClient client;
        static ChatResponder chatResponder;
        static UserDetailService userDetailService;
        static ResponseService responseService;

        static void Main(string[] args)
        {
            client = new HttpClient();

            Startup startup = new Startup();
            startup.Configure();

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            chatResponder = new ChatResponder(client);
            userDetailService = new UserDetailService(client);

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigurationService.Token,
                TokenType = TokenType.Bot
            });

            responseService = new ResponseService(discord);

            discord.MessageCreated += async e =>
            {
                if (e.Message.Author.Username == discord.CurrentUser.Username)
                {
                    await chatResponder.UpdateChatAsync(e);
                }
                else if(!responseService.ignoreMessage(e))
                {
                    var hasRequiredProperty = await userDetailService.HasRequiredPropertyAsync(e);
                    if(hasRequiredProperty)
                    {
                        var chatResponse = await chatResponder.GetChatResponseAsync(e);
                        responseService.hasRequiredPropertyResponse(e, chatResponse);
                    }
                    else
                    {
                        var chatResponse = await chatResponder.GetChatResponseAsync(e);
                        hasRequiredProperty = await userDetailService.HasRequiredPropertyAsync(e);
                        if(!hasRequiredProperty && responseService.alwaysRespond(e))
                        {
                            responseService.defaultResponse(e);
                        }
                        else
                        {
                            responseService.hasRequiredPropertyResponse(e, chatResponse);
                        }
                    }
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
