using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Discord.Audio.Streams;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;
using LongJohnSilver.Interfaces;

namespace LongJohnSilver.Database.DataMethodsKnockout
{
    public class KnockoutData
    {
        public string KnockoutChannel { get; set; }

        public KnockoutData(string channelId)
        {
            KnockoutChannel = channelId;            
        }

        public List<KnockoutContender> Contenders =>
            KnockoutContender.SelectAll()?.FindAll(x => x.Channel == KnockoutChannel) ?? new List<KnockoutContender>();

        public List<KnockoutPlayer> Players =>
            KnockoutPlayer.SelectAll()?.FindAll(x => x.Channel == KnockoutChannel) ?? new List<KnockoutPlayer>();

        public KnockoutGame Game => KnockoutGame.SelectAll()?.Find(x => x.Channel == KnockoutChannel) ?? new KnockoutGame();


        public static string GetGameChannelForUser(string userId)
        {
            var gameList = KnockoutGame.SelectAll();
            return gameList.Find(x => x.Owner == userId).Channel;
        }

        // Deletion Commands
        public void DeleteAllData()
        {
            Contenders.ForEach(x => x.Delete());            
            Players.ForEach(x => x.Delete());            
            if (Game.Channel != null) Game.Delete();                        
        }

        public static List<ulong> GetAllKnockoutChannels()
        {
            return KnockoutGame.SelectAll()?.Select(x => Convert.ToUInt64(x.Channel)).ToList();
        }
    }
}
