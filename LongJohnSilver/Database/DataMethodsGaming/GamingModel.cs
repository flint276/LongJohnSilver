using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace LongJohnSilver.Database.DataMethodsGaming
{
    public class GamingModel
    {
        private GamingData GData;
        public ulong Guild;
        string[] ValidServices = { "steam", "psn", "twitch", "xbox", "switch", "eso", "blizzard", "origin", "epic", "ubisoft" };

        public static GamingModel ForGuild(ulong guildId)
        {
            var guildString = guildId.ToString();
            return new GamingModel(guildString);
        }

        private GamingModel(string guildId)
        {
            Guild = Convert.ToUInt64(guildId);
            GData = new GamingData(guildId);
        }

        public void SetGamerTag(ulong userId, string service, string tagContent)
        {
            var userIdString = userId.ToString();
            var currentTag = GData.GamerTags.Find(x => x.Service == service && x.User == userIdString);

            if (currentTag != null)
            {
                currentTag.Tag = tagContent;
            }
            else
            {
                var newTag = new GamerTag(GData.GuildId, userIdString, service.ToLower(), tagContent);
            }
        }

        public void RemoveGamerTag(ulong userId, string service)
        {
            var userIdString = userId.ToString();
            var currentTags = GData.GamerTags.FindAll(x => x.User == userIdString && x.Service == service.ToLower());

            foreach (var t in currentTags)
            {
                t.Delete();
            }
        }

        public Dictionary<string, string> GetTagsForUser(ulong userId)
        {
            var userIdString = userId.ToString();
            var currentTags = GData.GamerTags.FindAll(x => x.User == userIdString)
                .ToDictionary(x => x.Service, x => x.Tag);
            return currentTags;
        }

        public Dictionary<ulong, string> GetTagsForService(string service)
        {
            var currentTags = GData.GamerTags.FindAll(x => x.Service == service.ToLower())
                .ToDictionary(x => Convert.ToUInt64(x.User), x => x.Tag);
            return currentTags;
        }

        public bool IsValidService(string service)
        {            
            return ValidServices.Contains(service.ToLower());
        }

        public string[] ShowValidServices()
        {
            return ValidServices;
        }
    }
}
