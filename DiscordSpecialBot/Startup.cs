using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DiscordSpecialBot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        public void Configure()
        {
            LoadRequiredSettings();
            LoadOptionalSettings();
        }

        private void LoadRequiredSettings()
        {
            ConfigurationService.Token = Configuration.GetSection("Token").Value;
            ConfigurationService.ApiUrl = Configuration.GetSection("ApiUrl").Value;
            ConfigurationService.BotName = Configuration.GetSection("BotName").Value;
            ConfigurationService.ChatType = Configuration.GetSection("ChatType").Value;
        }

        private void LoadOptionalSettings()
        {
            ConfigurationService.IgnoredChannels = Configuration.GetSection("IgnoredChannels").Get<List<string>>();
            if (ConfigurationService.IgnoredChannels == null)
            {
                ConfigurationService.IgnoredChannels = new List<string>();
            }

            ConfigurationService.ExclusiveTypes = Configuration.GetSection("ExclusiveTypes").Get<List<string>>();
            if (ConfigurationService.ExclusiveTypes == null)
            {
                ConfigurationService.ExclusiveTypes = new List<string>();
            }

            ConfigurationService.RequiredProperyMatches = Configuration.GetSection("RequiredProperyMatches").Get<List<string>>();
            if (ConfigurationService.RequiredProperyMatches == null)
            {
                ConfigurationService.RequiredProperyMatches = new List<string>();
            }

            ConfigurationService.NickNames = Configuration.GetSection("NickNames").Get<List<string>>();
            if (ConfigurationService.NickNames == null)
            {
                ConfigurationService.NickNames = new List<string>();
            }

            ConfigurationService.TargetedResponseConfidenceThreshold = double.Parse(Configuration.GetSection("TargetedResponseConfidenceThreshold").Value);

            ConfigurationService.DefaultResponse = Configuration.GetSection("DefaultResponse").Value;
        }
    }
}
