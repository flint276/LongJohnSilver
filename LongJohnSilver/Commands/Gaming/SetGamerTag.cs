using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongJohnSilver.Database.DataMethodsGaming;
using LongJohnSilver.Extensions;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Gaming
{
    public class SetGamerTag : ModuleBase<SocketCommandContext>
    {
        [Command("setgamertag")]
        public async Task SetGamerTagAsync([Remainder] string input = "")
        {
            var gModel = GamingModel.ForGuild(Context.Guild.Id);

            if (!Context.IsGamingChannel() || Context.IsPrivate)
            {
                return;
            }

            if (input == "")
            {
                await Context.Channel.SendMessageAsync(":x: Please enter a tag using the format !setgamertag <gameservice> <nick>");
                return;
            }

            string[] splitInput = input.Split(new[] {' '}, 2);

            if (splitInput.Length != 2)
            {
                await Context.Channel.SendMessageAsync(":x: Please enter a tag using the format !setgamertag <gameservice> <nick>");
                return;
            }

            var service = splitInput[0];
            var gamerTag = splitInput[1];

            if (!gModel.IsValidService(service))
            {
                var servicelist = "";

                foreach (var s in gModel.ShowValidServices())
                {
                    servicelist += "**" + s + "**, ";
                }
                await Context.Channel.SendMessageAsync(
                    $":x: {service} is not a valid service, please choose from {servicelist}");

                return;
            }

            gModel.SetGamerTag(Context.User.Id, service, gamerTag);

            await Context.Channel.SendMessageAsync($"Set!");
        }
    }
}
