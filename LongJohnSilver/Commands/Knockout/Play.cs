using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LongJohnSilver.Embeds;
using LongJohnSilver.Enums;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.Extensions;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class Play : ModuleBase<SocketCommandContext>
    {        

        [Command("vote")]
        public async Task PlayAsync([Remainder]string input = "")
        {
            var kModel = KnockoutModel.ForChannel(Context.Channel.Id);            

            if (!Context.IsKnockoutChannel() || Context.IsPrivate)
            {
                return;
            }

            if (input == "")
            {
                await Context.Channel.SendMessageAsync(":x: Please enter your options in this format: *!vote choice to add/choice to delete*");
                return;
            }

            if (!input.Contains("/") || input.Count(c => c == '/') > 1)
            {
                await Context.Channel.SendMessageAsync(":x: Please enter your options in this format: *!vote choice to add/choice to delete*");
                return;
            }

            var choices = input.Split('/');

            var choiceToAdd = choices.First();
            var choiceToSub = choices.Last();

            switch (kModel.KnockoutStatus)
            {
                case KnockoutStatus.NoKnockout:
                    await Context.Channel.SendMessageAsync(":x: No Knockout ongoing. Feel free to start a new one!");
                    return;
                case KnockoutStatus.KnockoutInProgress:
                    break;
                case KnockoutStatus.KnockoutFinished:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished. Feel free to start a new one!");
                    return;
                case KnockoutStatus.KnockoutUnderConstruction:
                    await Context.Channel.SendMessageAsync(":x: This knockout is still under construction! Patience!");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!kModel.PlayerCanGo(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync(":x: You either just went or are out of turns. Either way, you can't go.");
                return;
            }            

            var original = choiceToAdd;
            choiceToAdd = kModel.FindNearestMatch(choiceToAdd);
            if (choiceToAdd == "ERROR")
            {
                await Context.Channel.SendMessageAsync($":x: I'm sorry, I could not find a close match for **{original}**, please try again");
                return;
            }

            original = choiceToSub;
            choiceToSub = kModel.FindNearestMatch(choiceToSub);
            if (choiceToSub == "ERROR")
            {
                await Context.Channel.SendMessageAsync($":x: I'm sorry, I could not find a close match for **{original}**, please try again");
                return;
            }

            if (choiceToAdd == choiceToSub)
            {
                await Context.Channel.SendMessageAsync(":x: Choices are the same, try again.");
                return;
            }

            kModel.ApplyContenderScoreChanges(choiceToAdd, choiceToSub);
            var recentKiller = kModel.ApplyKilledSettings(Context.User.Id);
            kModel.ApplyPostVotePlayerChanges(Context.User.Id);

            if (kModel.AllLivingContendersCount <= 3)
            {
                kModel.KnockoutStatus = KnockoutStatus.KnockoutFinished;

                var embedData = KnockoutIsOverDataBuilder.BuildData(Context, kModel);
                await KnockoutEmbeds.KnockoutIsOver(embedData);

            }
            else
            {                                
                var embedDataA = KnockoutVotingReportDataBuilder.BuildData(Context, kModel, choiceToAdd, choiceToSub);
                await KnockoutEmbeds.VotingReport(embedDataA);                

                if (recentKiller)
                {                    
                    await Context.User.SendMessageAsync("You have killed a contender, type !epitaph _<message>_ in the main channel to leave a mark on their grave!");
                    await Context.User.SendMessageAsync("(please note, if you eliminate another contender, you will lose the opportunity to engrave an epitaph for this one)");                                    
                }

                var embedDataB = ShowKnockoutDataBuilder.BuildData(Context, kModel);
                await KnockoutEmbeds.ShowKnockout(embedDataB);
            }
        }
    }
}
