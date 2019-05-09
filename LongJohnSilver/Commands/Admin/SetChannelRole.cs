using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Extensions;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Admin
{
    public class SetChannelRole : ModuleBase<SocketCommandContext>
    {
        // ReSharper disable once StringLiteralTypo
        [Command("setchannelrole")]
        public async Task SetChannelRoleAsync([Remainder]string input = "")
        {            
            if (!Context.IsGuildAdmin())
            {
                await Context.Channel.SendMessageAsync(":x: You are not guild admin!");
                return;
            }
            
            if (Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync(
                    ":x: Please use this command in the channel you wish to set the role for");
            }

            var roleHandler = new RoleHandler(Factory.GetDatabase());

            if (!roleHandler.IsValidRole(input))
            {
                await Context.Channel.SendMessageAsync($":x: *{input}* is not a valid channel role");
                return;
            }

            roleHandler.SetRoleForChannel(Context.Channel.Id, input);
            await Context.Channel.SendMessageAsync(
                $"This channel has now been set to the *{input.ToLower()}* role!");
        }
    }
}
