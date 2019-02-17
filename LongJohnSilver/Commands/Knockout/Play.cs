using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Embeds;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class Play : ModuleBase<SocketCommandContext>
    {        

        [Command("vote")]
        public async Task PlayAsync([Remainder]string input = "")
        {
            if (!StateChecker.IsKnockoutChannel(Context) || StateChecker.IsPrivateMessage(Context))
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

            var knockouts = new KnockOutHandler(Context.Channel.Id, Factory.GetDatabase());

            switch (knockouts.KnockoutStatus)
            {
                case 1:
                    await Context.Channel.SendMessageAsync(":x: No Knockout ongoing. Feel free to start a new one!");
                    return;
                case 2:
                    break;
                case 3:
                    await Context.Channel.SendMessageAsync(":x: This knockout is finished. Feel free to start a new one!");
                    return;
                case 4:
                    await Context.Channel.SendMessageAsync(":x: This knockout is still under construction! Patience!");
                    return;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }

            if (knockouts.PlayerWentLastTime(Context.User.Id))
            {
                await Context.Channel.SendMessageAsync(":x: You just went! Give a few other people a chance!");
                return;
            }

            if (knockouts.TurnsLeftForPlayer(Context.User.Id) <= 0)
            {
                await Context.Channel.SendMessageAsync(":x: You are out of turns, please wait until the turns are reset");
                return;
            }

            var original = choiceToAdd;
            choiceToAdd = knockouts.FindNearestMatch(choiceToAdd);
            if (choiceToAdd == "ERROR")
            {
                await Context.Channel.SendMessageAsync($":x: I'm sorry, I could not find a close match for **{original}**, please try again");
                return;
            }

            original = choiceToSub;
            choiceToSub = knockouts.FindNearestMatch(choiceToSub);
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

            knockouts.ApplyVoteChanges(choiceToAdd, choiceToSub, Context.User.Id);

            if (knockouts.LivingContendersCount <= 3)
            {
                knockouts.EndKnockout();

                await BotEmbeds.KnockoutIsOver(Context, knockouts);

            }
            else
            {                
                var userName = Context.User.Username;
                var avatarUrl = Context.User.GetAvatarUrl();

                await Context.Channel.SendMessageAsync("", false, BotEmbeds.PlayerVotingReportEmbed(userName, avatarUrl, choiceToAdd, choiceToSub));

                var recentKiller = knockouts.PlayerHasJustKilled();

                if (recentKiller != 0)
                {
                    var killer = Context.Client.GetUser(recentKiller);

                    if (killer != null)
                    {
                        await killer.SendMessageAsync("You have killed a contender, type !epitaph _<message>_ in the main channel to leave a mark on their grave!");
                        await killer.SendMessageAsync("(please note, if you eliminate another contender, you will lose the opportunity to engrave an epitaph for this one)");
                    }
                    
                    knockouts.GetContendersFromDb();
                }

                await BotEmbeds.ShowKnockout(Context, knockouts);

            }
        }
    }
}
