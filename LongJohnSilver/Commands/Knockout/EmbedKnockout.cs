using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Embeds;
using LongJohnSilver.Enums;
using LongJohnSilver.MethodsKnockout;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Knockout
{
    public class EmbedKnockout : ModuleBase<SocketCommandContext>
    {
        // ReSharper disable once StringLiteralTypo
        [Command("showknockout")]
        public async Task EmbedKnockoutAsync()
        {
            var kModel = KnockoutModel.ForChannel(Context.Channel.Id);

            if (!StateChecker.IsKnockoutChannel(Context) || StateChecker.IsPrivateMessage(Context))
            {
                return;
            }

            switch (kModel.KnockoutStatus)
            {
                case KnockoutStatus.NoKnockout:
                    await Context.Channel.SendMessageAsync(":x: No Knockout has been created or is being created. Go for it! Type !createknockout to begin.");
                    return;
                case KnockoutStatus.KnockoutInProgress:
                case KnockoutStatus.KnockoutFinished:                    
                    break;
                case KnockoutStatus.KnockoutUnderConstruction:
                    var userId = kModel.KnockoutOwner;
                    var username = Context.Client.GetUser(userId).Username;
                    await Context.Channel.SendMessageAsync($"This Knockout is currently under construction by **{username}**! Feel free to advise!");                    
                    return;
                default:
                    await Context.Channel.SendMessageAsync(":x: Right. This shouldn't have happened. Someone call RedFlint.");
                    return;
            }

            var embedData = ShowKnockoutDataBuilder.BuildData(Context, kModel);
            await KnockoutEmbeds.ShowKnockout(embedData);
        }
    }
}
