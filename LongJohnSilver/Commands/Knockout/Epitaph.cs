﻿using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class Epitaph : ModuleBase<SocketCommandContext>
    {
        [Command("epitaph")]
        public async Task EpitaphAsync([Remainder]string input = "")
        {
            if (!StateChecker.IsKnockoutChannel(Context) || StateChecker.IsPrivateMessage(Context))
            {
                return;
            }

            if (input == "")
            {
                await Context.Channel.SendMessageAsync(":x: Please enter a valid epitaph.");
                return;
            }

            if (input.Count() > 199)
            {
                await Context.Channel.SendMessageAsync(":x: Epitaph too long!");
            }

            var knockouts = new KnockOutHandler(Context.Channel.Id, Factory.GetDatabase());

            switch (knockouts.KnockoutStatus)
            {
                case 1:
                    await Context.Channel.SendMessageAsync(":x: No Knockout ongoing. Feel free to start a new one!");
                    return;
                case 2:
                    break;
                case 3:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished.");
                    return;
                case 4:
                    await Context.Channel.SendMessageAsync(":x: This knockout is still under construction! Patience!");
                    return;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }

            if (!knockouts.CanWriteAnEpitaph(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync(":x: You are not eligible to write an epitaph for a contender.");
                return;
            }

            knockouts.WriteEpitaphFromUser(Context.User.Id, input);
            await Context.Channel.SendMessageAsync(":skull: Engraved!");
        }
    }
}
