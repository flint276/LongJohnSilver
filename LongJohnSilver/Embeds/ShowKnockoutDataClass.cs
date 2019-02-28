using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;
using LongJohnSilver.Enums;
using LongJohnSilver.Database.DataMethodsKnockout;

namespace LongJohnSilver.Embeds
{
    public struct ShowKnockoutDataStruct
    {
        public string Title;
        public string Username;
        public string UserAvatar;
        public string PlayersReadyString;
        public List<string> TheLivingFields;
        public List<string> TheFallenFields;
        public IMessageChannel ChannelToSendTo;
    }

    public static class ShowKnockoutDataBuilder
    {
        public static ShowKnockoutDataStruct BuildData(SocketCommandContext context, KnockoutModel kModel)
        {
            var dataStruct = new ShowKnockoutDataStruct
            {
                Title = kModel.KnockoutName,
                Username = context.Client.GetUser(kModel.KnockoutOwner).Username,
                UserAvatar = context.Client.GetUser(kModel.KnockoutOwner).GetAvatarUrl()
            };

            var allContendersWithScore = kModel.AllContendersWithScore;
            var allContendersWithEpitaph = kModel.AllContendersWithEpitaph;

            if (context.IsPrivate && kModel.KnockoutStatus == KnockoutStatus.KnockoutInProgress)
            {
                dataStruct.ChannelToSendTo = context.Client.GetChannel(kModel.GameChannel) as IMessageChannel;
            }
            else
            {
                dataStruct.ChannelToSendTo = context.Channel;
            }

            dataStruct.PlayersReadyString = "";

            foreach (var (playerId, canGoBool) in kModel.AllPlayersWithCanGoStatus())
            {
                if (context.Guild.GetUser(playerId) == null) continue;

                var username = context.Guild.GetUser(playerId).Username;

                if (canGoBool)
                {
                    dataStruct.PlayersReadyString += $"{username}, ";
                }
                else
                {
                    dataStruct.PlayersReadyString += $"~~{username}~~, ";
                }                    
            }

            var ranking = 0;

            var sortedContenderList =
                allContendersWithScore.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();

            dataStruct.TheLivingFields = new List<string>();
            dataStruct.TheFallenFields = new List<string>();

            foreach (var contender in sortedContenderList)
            {
                ranking += 1;
                var contenderName = contender;
                var contenderScore = allContendersWithScore[contender];

                if (contenderScore > 0)
                {
                    dataStruct.TheLivingFields.Add($"**{ranking})** {contenderName} : {contenderScore}\n");
                }
                else
                {
                    var epitaph = allContendersWithEpitaph[contender];
                    dataStruct.TheFallenFields.Add($"**{ranking})** {contenderName}\n- *{epitaph}*\n");
                }
            }

            return dataStruct;
        }
    }
}
