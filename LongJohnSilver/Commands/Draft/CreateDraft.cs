using Discord.Commands;
using LongJohnSilver.Database.DataMethodsDrafts;
using LongJohnSilver.Statics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LongJohnSilver.Enums;
using LongJohnSilver.Extensions;

namespace LongJohnSilver.Commands.Draft
{
    public class CreateDraft : ModuleBase<SocketCommandContext>
    {
        [Command("createdraft")]
        public async Task CreateDraftAsync()
        {
            var dModel = new DraftModel(Context.Channel.Id);

            if (!Context.IsDraftChannel() || Context.IsPrivate)
            {
                return;
            }

            switch (dModel.DraftStatus)
            {
                case DraftStatus.NoDraft:
                    break;
                case DraftStatus.DraftInConstruction:
                case DraftStatus.DraftInProgress:
                case DraftStatus.DraftVoting:
                    await Context.Channel.SendMessageAsync(":x: A draft is already in progress!");
                    return;                                    
                case DraftStatus.DraftFinished:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Context.Channel.SendMessageAsync(
                "Commands to create your own Draft (all commands in this window please):\n\n" +
                "**!title** _The Name Of Your Draft_\n" +
                "**!quitdraft** _Abandon and Delete your Draft_\n"
            );

            dModel.AddNewDraft(Context.User.Id);
        }
    }
}
