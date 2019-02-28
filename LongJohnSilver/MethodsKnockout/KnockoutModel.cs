using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Discord;
using Discord.Commands;
using LongJohnSilver.Enums;
using LongJohnSilver.Statics;
using NUnit.Framework.Internal;

namespace LongJohnSilver.MethodsKnockout
{
    public class KnockoutModel
    {
        private KnockoutData KData;        
        public ulong GameChannel;

        public static KnockoutModel ForUser(ulong userId)
        {
            var channelString = KnockoutData.GetGameChannelForUser(userId.ToString());
            return new KnockoutModel(channelString);
        }

        public static KnockoutModel ForChannel(ulong channelId)
        {
            return new KnockoutModel(channelId.ToString());
        }
        
        private KnockoutModel(string channelId)
        {
            if (string.IsNullOrEmpty(channelId)) channelId = "0";

            GameChannel = Convert.ToUInt64(channelId);
            KData = new KnockoutData(channelId);
        }

        // Combined Data Handling
        public void DeleteAllData()
        {
            KData.DeleteAllData();
        }

        // Knockout Data Handling   
        public void AddNewKnockout(ulong playerId)
        {
            var playerIdString = playerId.ToString();
            KData.DeleteAllData();

            var _ = new KnockoutGame(
                "Knockout Under Construction", 
                (int)KnockoutStatus.KnockoutUnderConstruction,
                playerIdString, 
                GameChannel.ToString()
                );
        }
        public string KnockoutName
        {
            get => KData.Game.Name;
            set => KData.Game.Name = value;            
        }
        public ulong KnockoutOwner
        {
            get => Convert.ToUInt64(KData.Game.Owner);
            set => KData.Game.Owner = value.ToString();            
        }
        public KnockoutStatus KnockoutStatus
        {
            get => KData.Game.Status == 0 ? KnockoutStatus.NoKnockout : (KnockoutStatus)KData.Game.Status;
            set => KData.Game.Status = (int)value;            
        }

        // Contender Data Handling           
        public Dictionary<string, int> AllContendersWithScore => KData.Contenders.ToDictionary(x => x.Name, x => x.Score);
        public Dictionary<string, string> AllContendersWithEpitaph => KData.Contenders.ToDictionary(x => x.Name, x => x.Epitaph);
        public List<string> AllContenderNames => KData.Contenders.Select(x => x.Name).ToList();        
        public int AllLivingContendersCount => KData.Contenders.FindAll(x => x.Score > 0).Count;

        public void AddNewContender(string contenderName)
        {          
            KData.Contenders.Add(new KnockoutContender(contenderName, 3, "", "", KData.KnockoutChannel));            
        }

        public void ApplyContenderScoreChanges(string choiceToAdd, string choiceToSub)
        {
            KData.Contenders.Find(x => x.Name == choiceToAdd).Score += 1;
            KData.Contenders.Find(x => x.Name == choiceToSub).Score -= 1;
        }

        public bool ApplyKilledSettings(ulong userId)
        {
            var newlyFallenContender = KData.Contenders.Find(x => x.Score == 0);

            if (newlyFallenContender == null) return false;
            
            var fallenScore = KData.Contenders.FindAll(x => x.Score > 0).Count * -1;
            newlyFallenContender.Score = fallenScore;
            newlyFallenContender.Killer = userId.ToString();
            newlyFallenContender.Epitaph = "inscription pending";
            return true;            
        }

        public void ApplyVoteChanges(string choiceToAdd, string choiceToSub, ulong userId)
        {
            var userIdString = userId.ToString();

            KData.Contenders.Find(x => x.Name == choiceToAdd).Score += 1;
            KData.Contenders.Find(x => x.Name == choiceToSub).Score -= 1;
        }

        public bool DoesContenderExist(string contenderName)
        {
            return KData.Contenders.FindAll(x => x.Name == contenderName).Count != 0;
        }

        public void DeleteContender(string contenderName)
        {
            KData.Contenders.FindAll(x => x.Name == contenderName).ForEach(x => x.Delete());            
        }

        public string FindNearestMatch(string choiceToValidate)
        {
            var livingContenderList = KData.Contenders.FindAll(x => x.Score > 0);

            var beginsWithMatches = livingContenderList.FindAll(o => o.Name.StartsWith(choiceToValidate));
            var beginsWithMatchesCaseInsensitive =
                livingContenderList.FindAll(o => o.Name.ToLower().StartsWith(choiceToValidate.ToLower()));

            var containsMatches = livingContenderList.FindAll(o => o.Name.Contains(choiceToValidate));
            var containsMatchesCaseInsensitive =
                livingContenderList.FindAll(o => o.Name.ToLower().Contains(choiceToValidate.ToLower()));

            var bestMatch = "ERROR";

            // look for matches at the start of the word
            if (beginsWithMatches.Count == 1) return beginsWithMatches.First().Name;

            // look for case insensitive matches at the start of the word
            if (beginsWithMatchesCaseInsensitive.Count == 1) return beginsWithMatchesCaseInsensitive.First().Name;

            if (containsMatches.Count == 1) return containsMatches.First().Name;

            if (containsMatchesCaseInsensitive.Count == 1) return containsMatchesCaseInsensitive.First().Name;


            // fuzzy matching
            if (choiceToValidate.Length > 15)
            {
                var bestLScore = 1000;
                foreach (var c in livingContenderList)
                {
                    var lScore = LevenshteinDistance.Compute(choiceToValidate, c.Name);
                    if (lScore < bestLScore)
                    {
                        bestLScore = lScore;
                        bestMatch = c.Name;
                    }
                }

                Console.WriteLine($"Best LScore is {bestLScore} for {choiceToValidate} and {bestMatch}");

                if (bestLScore > 10) bestMatch = "ERROR";
            }

            if (choiceToValidate.Length > 8)
            {
                var bestLScore = 1000;
                foreach (var c in livingContenderList)
                {
                    var lScore = LevenshteinDistance.Compute(choiceToValidate, c.Name);
                    if (lScore >= bestLScore) continue;
                    bestLScore = lScore;
                    bestMatch = c.Name;
                }

                Console.WriteLine($"Best LScore is {bestLScore} for {choiceToValidate} and {bestMatch}");

                if (bestLScore > 6) bestMatch = "ERROR";
            }

            if (choiceToValidate.Length <= 8)
            {
                var bestLScore = 1000;
                foreach (var c in livingContenderList)
                {
                    var lScore = LevenshteinDistance.Compute(choiceToValidate, c.Name);
                    if (lScore < bestLScore)
                    {
                        bestLScore = lScore;
                        bestMatch = c.Name;
                    }
                }

                Console.WriteLine($"Best LScore is {bestLScore} for {choiceToValidate} and {bestMatch}");

                if (bestLScore > 2) bestMatch = "ERROR";
            }

            return bestMatch;
        }

        public void HalveScores()
        {
            KData.Contenders.FindAll(x => x.Score > 1).ForEach(x => x.Score = x.Score / 2 + (x.Score % 2 > 0 ? 1 :0));            
        }

        public bool WriteEpitaph(ulong playerId, string epitaph)
        {
            var playerIdString = playerId.ToString();
            var contenderToUpdate = KData.Contenders.Find(o => o.Killer == playerIdString && o.Epitaph == "inscription pending");
            if (contenderToUpdate == null) return false;
            contenderToUpdate.Epitaph = epitaph;
            return true;
        }

        // Player Data Handling

        public List<ulong> AllPlayerIds => KData.Players.Select(x => Convert.ToUInt64(x.PlayerId)).ToList();
        public Dictionary<ulong, bool> AllPlayersWithCanGoStatus()
        {
            var returnDictionary = new Dictionary<ulong, bool>();

            foreach (var p in KData.Players)
            {
                returnDictionary[Convert.ToUInt64(p.PlayerId)] = p.LastPlayed == 0 && p.TurnsLeft > 0;
            }

            return returnDictionary;
        }

        public void ApplyPostVotePlayerChanges(ulong playerId)
        {
            var playerIdString = playerId.ToString();

            KData.Players.FindAll(x => x.LastPlayed > 0).ForEach(x => x.LastPlayed -= 1);
            KData.Players.Find(x => x.PlayerId == playerIdString).LastPlayed = 2;
            KData.Players.Find(x => x.PlayerId == playerIdString).TurnsLeft -= 1;
        }

        public bool PlayerCanGo(ulong playerId)
        {
            var playerIdString = playerId.ToString();
            if (KData.Players.Find(x => x.PlayerId == playerIdString) == null)
            {
                KData.Players.Add(new KnockoutPlayer(playerIdString, 3, 0, KData.KnockoutChannel));
            }
            return KData.Players.Find(x => x.PlayerId == playerIdString && x.LastPlayed == 0 && x.TurnsLeft > 0) != null;
        }

        public void NewDay()
        {
            KData.Players.ForEach(x =>
            {
                x.LastPlayed = 0;
                x.TurnsLeft = 3;                
            });            
        }


    }
}
