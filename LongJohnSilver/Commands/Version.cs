using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongJohnSilver.Commands
{
    public class Version : ModuleBase<SocketCommandContext>
    {
        [Command("version"), Alias("ver"), Summary("Version Number of Bot")]
        public async Task VersionAsync()
        {
            await Context.Channel.SendMessageAsync("Version 0.82h");
        }

    }
}
