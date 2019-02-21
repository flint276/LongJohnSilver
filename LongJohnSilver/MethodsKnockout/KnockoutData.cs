using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public KnockoutData(string channelId)
        {
            KnockoutChannel = channelId;            
            Contenders = GetAllContenders(KnockoutChannel);
            Players = GetAllPlayers(KnockoutChannel);
            Game = GetGame(KnockoutChannel);
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

        public static string GetGameChannelForUser(string userId)
        {
            var gameList = KnockoutGame.SelectAll();
            return gameList.Count == 0 ? null : gameList.Find(x => x.Owner == userId).Channel;
        }

        // Entire Data Commands
        public void DeleteAllData()
        {
            Contenders.ForEach(x => x.Delete());
            Contenders.Clear();

            Players.ForEach(x => x.Delete());
            Players.Clear();

            Game.Delete();
            Game = null;

            KnockoutChannel = "";
        }

        // Knockout Contender       
        public void AddContender(string input)
        {
            Contenders.Add(new KnockoutContender(input, 3, "", "", KnockoutChannel));
        }
        public List<string> AllContenderNames => 
            Contenders.Select(x => x.Name).ToList();
        public Dictionary<string, int> AllContendersWithScore => 
            Contenders.ToDictionary(x => x.Name, x => x.Score);
        public Dictionary<string, string> AllContendersWithEpitaph => 
            Contenders.ToDictionary(x => x.Name, x => x.Epitaph);
        public void DeleteContender(string input)
        {
            Contenders.FindAll(x => x.Name == input).ForEach(x => x.Delete());
            Contenders.RemoveAll(x => x.Name == input);
        }

        // Knockout Player
        public List<string> AllPlayers => 
            Players.Select(x => x.PlayerId).ToList();
        public Dictionary<string, int> AllPlayersWithTurnsLeftValue =>
            Players.ToDictionary(x => x.PlayerId, x => x.TurnsLeft);
        public Dictionary<string, int> AllPlayersWithLastPlayedValue =>
            Players.ToDictionary(x => x.PlayerId, x => x.LastPlayed);
        
        // Knockout Game
        public string GameChannel => 
            Game?.Channel;        
        public string KnockoutName
        {
            get => Game?.Name;
            set => Game.Name = value;
        }
        public string KnockoutOwner => 
            Game?.Owner;
        public int KnockoutStatus => 
            Game.Status;




    }
}
