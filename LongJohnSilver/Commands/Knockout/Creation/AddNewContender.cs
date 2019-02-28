﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Enums;
using LongJohnSilver.MethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout.Creation
{
    public class AddNewContender : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        public async Task AddKnockoutAsync([Remainder]string input = "")
        {
            var kModel = KnockoutModel.ForUser(Context.User.Id);

            if (!StateChecker.IsPrivateMessage(Context))
            {
                return;
            }

            if (input == "")
            {
                await Context.Channel.SendMessageAsync(":x: No Value Entered!");
                return;
            }

            if (input.Contains('/'))
            {
                await Context.Channel.SendMessageAsync(":x: Sorry, you can't put '/' characters in your contenders name. (it confuses the voting command!)");
                return;
            }

            if (kModel.GameChannel == 0)
            {
                await Context.Channel.SendMessageAsync(":x: You are not making a knockout at the moment!");
                return;
            }

            switch (kModel.KnockoutStatus)
            {
                case KnockoutStatus.NoKnockout:
                    await Context.Channel.SendMessageAsync(":x: No Knockout is being created at the moment!");
                    return;
                case KnockoutStatus.KnockoutInProgress:
                    await Context.Channel.SendMessageAsync(":x: This knockout has already started! No more changes!");
                    return;
                case KnockoutStatus.KnockoutFinished:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished, please feel free to create a new one!");
                    return;
                case KnockoutStatus.KnockoutUnderConstruction:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (kModel.DoesContenderExist(input))
            {
                await Context.Channel.SendMessageAsync($":x: The Contender: **{input}** already exists in your list!");
                return;
            }

            kModel.AddNewContender(input);
            await Context.Channel.SendMessageAsync($"You have added the contender **{input}**");
        }
    }
}
