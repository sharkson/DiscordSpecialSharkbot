using ChatModels;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordSpecialBot
{
    public class UserDetailService
    {
        public async Task<bool> HasRequiredPropertyAsync(MessageCreateEventArgs e, List<string> propertyMatches)
        {
            var userData = await GetUserData(e.Author.Username);

            return propertyMatches.All(rp => userData.derivedProperties.Any(dp => dp.name == rp));
        }

        public async Task<List<string>> GetNickNames(string userName)
        {
            var userData = await GetUserData(userName);

            return userData.nickNames;
        }

        private async Task<UserData> GetUserData(string userName)
        {
            var response = await ApiService.client.GetAsync(ConfigurationService.ApiUrl + "/api/user/" + userName);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserData>(jsonResponse);
        }
    }
}
