using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands
{
    public class Version : ModuleBase<SocketCommandContext>
    {
        [Command("version"), Alias("ver"), Summary("Version Number of Bot")]
        public async Task VersionAsync()
        {
            var db = Factory.GetDatabase();
            await Context.Channel.SendMessageAsync($"0.9h - DB: {db.GetVersion()}");
        }

    }
}
