using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Admin
{
    public class RebuildDatabases : ModuleBase<SocketCommandContext>
    {
        [Command("rebuild")]
        public async Task RebuildDatabasesAsync()
        {
            if (!ChannelCheck.IsKnockoutChannel(Context))
            {
                return;
            }

            var currentUser = Context.User as SocketGuildUser;
            if (!(currentUser.GuildPermissions.KickMembers))
            {
                await Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
                return;
            }

            if (Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync("Please use this command in the knockout channel!");
                return;
            }

            await Context.Channel.SendMessageAsync("!!! All databases are being rebuilt and purged !!!");

            var knockouts = new KnockOutHandler(Context.Channel.Id, Factory.GetDatabase());
            knockouts.RebuildDataBase();
            
            await Context.Channel.SendMessageAsync("!!! Done !!!");

        }


    }
}

