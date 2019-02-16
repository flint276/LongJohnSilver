using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.General
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
