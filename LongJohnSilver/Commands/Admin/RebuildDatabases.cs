using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Admin
{
    public class RebuildDatabases : ModuleBase<SocketCommandContext>
    {
        [Command("rebuild")]
        public async Task RebuildDatabasesAsync()
        {
            var kModel = KnockoutModel.ForChannel(Context.Channel.Id);

            if (!StateChecker.IsKnockoutChannel(Context) || StateChecker.IsPrivateMessage(Context))
            {
                return;
            }

            if (!StateChecker.IsChannelOp(Context))
            {
                await Context.Channel.SendMessageAsync(":x: You are not a channel op!");
                return;
            }

            await Context.Channel.SendMessageAsync("!!! All databases are being rebuilt and purged !!!");
            
            kModel.DeleteAllData();
            
            await Context.Channel.SendMessageAsync("!!! Done !!!");

        }


    }
}

