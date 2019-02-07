using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands 
{
    public class NewDay : ModuleBase<SocketCommandContext>
    {
        // ReSharper disable once StringLiteralTypo
        [Command("newday")]
        public async Task NewDayAsync()
        {
            if (!ChannelCheck.IsKnockoutChannel(Context))
            {
                return;
            }

            SocketGuildUser CurrentUser = Context.User as SocketGuildUser;
            if (!(CurrentUser.GuildPermissions.KickMembers))
            {
                await Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
                return;
            }

            if (Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync("Please use this command in the knockout channel!");
                return;
            }

            var knockouts = new KnockOutHandler(Context.Channel.Id, Factory.GetDatabase());
            knockouts.NewDay();
            await Context.Channel.SendMessageAsync("It is a glorious new day. Everyone's turns are reset!");
            return;
        }
    }
}
