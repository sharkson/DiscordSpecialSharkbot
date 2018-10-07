using DSharpPlus;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    class Program
    {
        static DiscordClient discord;
        static ResponseService responseService;

        static void Main(string[] args)
        {
            ApiService.client = new HttpClient();

            Startup startup = new Startup();
            startup.Configure();

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = ConfigurationService.Token,
                TokenType = TokenType.Bot
            });

            responseService = new ResponseService(discord);
            
            discord.MessageCreated += async e =>
            {
                await responseService.ReadMessage(e);
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
