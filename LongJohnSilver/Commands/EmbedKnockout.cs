using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LongJohnSilver.Database;
using LongJohnSilver.Embeds;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands
{
    public class EmbedKnockout : ModuleBase<SocketCommandContext>
    {
        // ReSharper disable once StringLiteralTypo
        [Command("showknockout")]
        public async Task EmbedKnockoutAsync()
        {

            if (Context.IsPrivate)
            {
                await Context.Channel.SendMessageAsync("Please use this command in the knockout channel!");
                return;
            }

            var knockouts = new KnockOutHandler(Context.Channel.Id, Factory.GetDatabase());

            switch (knockouts.KnockoutStatus)
            {
                case 1:
                    await Context.Channel.SendMessageAsync(":x: No Knockout has been created or is being created. Go for it! Type !createknockout to begin.");
                    return;
                case 2:
                    await BotEmbeds.ShowKnockout(Context, knockouts);
                    return;
                case 3:
                    await BotEmbeds.KnockoutIsOver(Context, knockouts);
                    break;
                case 4:
                    var userId = knockouts.KnockoutCreatorUlong;
                    var username = Context.Client.GetUser(userId).Username;
                    await Context.Channel.SendMessageAsync($"This Knockout is currently under construction by **{username}**! Feel free to advise!");
                    await BotEmbeds.ShowKnockout(Context, knockouts);
                    return;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }
        }
    }
}
