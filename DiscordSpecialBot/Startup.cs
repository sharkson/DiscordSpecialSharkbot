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
        }

        private void LoadOptionalSettings()
        {
            ConfigurationService.ChatType = Configuration.GetSection("ChatType").Value;
            if(ConfigurationService.ChatType == null)
            {
                ConfigurationService.ChatType = "discord";
            }

            ConfigurationService.NsfwChatType = Configuration.GetSection("NsfwChatType").Value;
            if (ConfigurationService.NsfwChatType == null)
            {
                ConfigurationService.NsfwChatType = "nsfw";
            }

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

            ConfigurationService.NsfwExclusiveTypes = Configuration.GetSection("NsfwExclusiveTypes").Get<List<string>>();
            if (ConfigurationService.NsfwExclusiveTypes == null)
            {
                ConfigurationService.NsfwExclusiveTypes = new List<string>();
            }

            ConfigurationService.ExcludedTypes = Configuration.GetSection("ExcludedTypes").Get<List<string>>();
            if (ConfigurationService.ExcludedTypes == null)
            {
                ConfigurationService.ExcludedTypes = new List<string>();
            }

            ConfigurationService.NsfwExcludedTypes = Configuration.GetSection("NsfwExcludedTypes").Get<List<string>>();
            if (ConfigurationService.NsfwExcludedTypes == null)
            {
                ConfigurationService.NsfwExcludedTypes = new List<string>();
            }

            ConfigurationService.RequiredPropertyMatches = Configuration.GetSection("RequiredPropertyMatches").Get<List<string>>();
            if (ConfigurationService.RequiredPropertyMatches == null)
            {
                ConfigurationService.RequiredPropertyMatches = new List<string>();
            }

            ConfigurationService.RequiredNsfwPropertyMatches = Configuration.GetSection("RequiredNsfwPropertyMatches").Get<List<string>>();
            if (ConfigurationService.RequiredNsfwPropertyMatches == null)
            {
                ConfigurationService.RequiredNsfwPropertyMatches = new List<string>();
            }

            ConfigurationService.NickNames = Configuration.GetSection("NickNames").Get<List<string>>();
            if (ConfigurationService.NickNames == null)
            {
                ConfigurationService.NickNames = new List<string>();
            }

            ConfigurationService.AllowedBots = Configuration.GetSection("AllowedBots").Get<List<string>>();
            if (ConfigurationService.AllowedBots == null)
            {
                ConfigurationService.AllowedBots = new List<string>();
            }

            if (Configuration.GetSection("TargetedResponseConfidenceThreshold").Value != null)
            {
                ConfigurationService.TargetedResponseConfidenceThreshold = double.Parse(Configuration.GetSection("TargetedResponseConfidenceThreshold").Value);
            }
            else
            {
                ConfigurationService.TargetedResponseConfidenceThreshold = .5;
            }

            if (Configuration.GetSection("TargetedNsfwResponseConfidenceThreshold").Value != null)
            {
                ConfigurationService.TargetedNsfwResponseConfidenceThreshold = double.Parse(Configuration.GetSection("TargetedNsfwResponseConfidenceThreshold").Value);
            }
            else
            {
                ConfigurationService.TargetedNsfwResponseConfidenceThreshold = .5;
            }

            if (Configuration.GetSection("NsfwAlternateResponseThreshold").Value != null)
            {
                ConfigurationService.NsfwAlternateResponseThreshold = double.Parse(Configuration.GetSection("NsfwAlternateResponseThreshold").Value);
            }
            else
            {
                ConfigurationService.NsfwAlternateResponseThreshold = 0;
            }

            ConfigurationService.DefaultResponse = Configuration.GetSection("DefaultResponse").Value;
            ConfigurationService.DefaultNsfwResponse = Configuration.GetSection("DefaultNsfwResponse").Value;
        }
    }
}
