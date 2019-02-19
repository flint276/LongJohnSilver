using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord.Audio.Streams;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver.MethodsKnockout
{
    public class KnockoutData
    {
        private List<KnockoutContender> Contenders;
        private List<KnockoutPlayer> Players;
        private KnockoutGame Game;

        public string KnockoutChannel { get; set; }

        public KnockoutData(ulong channelId)
        {
            KnockoutChannel = channelId.ToString();

            var channelIdString = channelId.ToString();
            Contenders = GetAllContenders(channelIdString);
            Players = GetAllPlayers(channelIdString);
            Game = GetGame(channelIdString);
        }        

        private static List<KnockoutContender> GetAllContenders(string channelId)
        {
            var contenderList = KnockoutContender.SelectAll();
            return contenderList?.FindAll(x => x.Channel == channelId);
        }

        private static List<KnockoutPlayer> GetAllPlayers(string channelId)
        {
            var playerList = KnockoutPlayer.SelectAll();
            return playerList?.FindAll(x => x.Channel == channelId);        
        }

        private static KnockoutGame GetGame(string channelId)
        {
            var gameList = KnockoutGame.SelectAll();
            return gameList?.Find(x => x.Channel == channelId);
        }

        public static ulong GetGameChannelForUser(ulong userId)
        {
            var userIdString = userId.ToString();
            var gameList = KnockoutGame.SelectAll();
            return gameList.Count == 0 ? 0 : Convert.ToUInt64(gameList.Find(x => x.Owner == userIdString).Channel);
        }

        public ulong GameChannel => Game?.Channel == null ? 0 : Convert.ToUInt64(Game.Channel);

        public int KnockoutStatus => Game.Status;

        public string NewContender
        {
            set => Contenders.Add(new KnockoutContender(value, 3, "", "", KnockoutChannel));
        }
    }
}
