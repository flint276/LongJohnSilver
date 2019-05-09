using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using LongJohnSilver.Database.DataMethodsDrafts;
using LongJohnSilver.Enums;
using LongJohnSilver.Extensions;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Commands.Draft
{
    public class QuitDraft : ModuleBase<SocketCommandContext>
    {
        [Command("quitdraft")]
        public async Task QuitDraftAsync()
        {
            var dModel = new DraftModel(Context.Channel.Id);

            if (!Context.IsDraftChannel() || Context.IsPrivate)
            {
                return;
            }

            switch (dModel.DraftStatus)
            {
                case DraftStatus.DraftInConstruction:
                    break;
                case DraftStatus.DraftInProgress:
                case DraftStatus.DraftVoting:
                    await Context.Channel.SendMessageAsync(":x: A draft is already in progress!");
                    return;
                case DraftStatus.NoDraft:
                case DraftStatus.DraftFinished:
                    await Context.Channel.SendMessageAsync(":x: No draft is under construction");
                    return;
                default:
                    throw new ArgumentOutOfRangeException();                    
            }

            await Context.Channel.SendMessageAsync("You have aborted creation of this draft");

            dModel.DeleteAllData();
        }
    }
}
