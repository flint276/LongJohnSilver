﻿using Discord.Commands;
using LongJohnSilver.Database.DataMethodsGaming;
using LongJohnSilver.Statics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LongJohnSilver.Extensions;

namespace LongJohnSilver.Commands.Gaming
{
    public class RemoveGamerTag : ModuleBase<SocketCommandContext>
    {
        [Command("removegamertag")]
        public async Task RemoveGamerTagAsync([Remainder] string input = "")
        {
            var gModel = GamingModel.ForGuild(Context.Guild.Id);

            if (!Context.IsGamingChannel() || Context.IsPrivate)
            {
                return;
            }

            if (input == "")
            {
                await Context.Channel.SendMessageAsync(
                    ":x: Please remove a tag using the format !removegamertag <gameservice>");
                return;
            }

            if (!gModel.IsValidService(input))
            {
                var servicelist = "";

                foreach (var s in gModel.ShowValidServices())
                {
                    servicelist += "**" + s + "**, ";
                }

                await Context.Channel.SendMessageAsync(
                    $":x: {input} is not a valid service, please choose from {servicelist}");

                return;
            }

            gModel.RemoveGamerTag(Context.User.Id, input);
            await Context.Channel.SendMessageAsync($"Removed!");
        }

    }
}