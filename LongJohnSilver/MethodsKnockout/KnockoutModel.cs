using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Discord.Commands;
using NUnit.Framework.Internal;

namespace LongJohnSilver.MethodsKnockout
{
    public class KnockoutModel
    {
        private KnockoutData KData;

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
            KData = new KnockoutData(channelId);
        }

        // Universal Data Handling
        public void DeleteAllData()
        {
            KData.DeleteAllData();
        }

        // Knockout Data Handling
        public ulong GameChannel => KData.GameChannel == null ? 0 : Convert.ToUInt64(KData.GameChannel);
        public string KnockoutName => KData.GameChannel == null ? "" : KData.KnockoutName;
        public ulong KnockoutOwner => KData.KnockoutOwner == null ? 0 : Convert.ToUInt64(KData.KnockoutOwner);
        public KnockoutStatus KnockoutStatus => (KnockoutStatus)KData.KnockoutStatus;
        public ModificationResult SetKnockoutName(string input)
        {
            if (input == "") return ModificationResult.ValueIsEmpty;

            KData.KnockoutName = input;
            return ModificationResult.Success;
        }

        // Contender Data Handling
        public ModificationResult AddNewContender(string input)
        {
            if (input == "") return ModificationResult.ValueIsEmpty;            
            if (KData.AllContenderNames.Contains(input)) return ModificationResult.Duplicate;
            if (input.Contains("/")) return ModificationResult.IllegalCharacter;
            
            KData.AddContender(input);            
            return ModificationResult.Success;
        }

        public Dictionary<string, int> AllContendersWithScore => KData.AllContendersWithScore;
        public Dictionary<string, string> AllContendersWithEpitaph => KData.AllContendersWithEpitaph;        

        public ModificationResult DeleteContender(string input)
        {
            if (input == "") return ModificationResult.ValueIsEmpty;
            if (!KData.AllContenderNames.Contains(input)) return ModificationResult.Missing;
            
            KData.DeleteContender(input);
            return ModificationResult.Success;
        }

        // Player Data Handling

        public List<ulong> AllPlayerIds => KData.AllPlayers.Select(x => Convert.ToUInt64(x)).ToList();

        public Dictionary<ulong, bool> GetAllPlayerIdsWithCanGoBool()
        {
            var returnDictionary = new Dictionary<ulong, bool>();
            var turnsLeftDictionary = KData.AllPlayersWithTurnsLeftValue;
            var lastPlayedDictionary = KData.AllPlayersWithLastPlayedValue;

            var playerList = KData.AllPlayers;

            foreach (var p in playerList)
            {
                var hasTurnsLeft = turnsLeftDictionary[p] > 0;
                var hasNotGoneRecently = lastPlayedDictionary[p] == 0;

                returnDictionary[Convert.ToUInt64(p)] = hasNotGoneRecently && hasTurnsLeft;
            }

            return returnDictionary;
        }


    }
}
