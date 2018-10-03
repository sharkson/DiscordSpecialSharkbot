using ChatModels;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    public class UserDetailService
    {
        HttpClient client;

        public UserDetailService(HttpClient httpClient)
        {
            client = httpClient;
        }

        public async Task<bool> HasRequiredPropertyAsync(MessageCreateEventArgs e)
        {
            var userName = e.Author.Username;
            var response = await client.GetAsync(ConfigurationService.ApiUrl + "/api/user/" + userName);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<UserData>(jsonResponse);

            return ConfigurationService.RequiredProperyMatches.All(rp => userData.derivedProperties.Any(dp => dp.name == rp));
        }
    }
}
