using System;
using System.Collections.Generic;
using System.Text;

namespace LongJohnSilver.Database.DataMethodsGaming
{
    public class GamingData
    {
        public string GuildId { get; set; }

        public GamingData(string guildId)
        {
            GuildId = guildId;
        }

        public List<GamerTag> GamerTags =>
            GamerTag.SelectAll()?.FindAll(x => x.Guild == GuildId) ?? new List<GamerTag>();
    }
}
