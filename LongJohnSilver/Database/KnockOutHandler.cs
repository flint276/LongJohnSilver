using System;
using System.Collections.Generic;
using System.Linq;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Database
{
    public class KnockOutHandler
    {
        private List<Contender> Contenders = new List<Contender>();
        private Knockout KnockoutInfo = new Knockout();        
        private List<KPlayer> Players = new List<KPlayer>();
        private readonly IDatabase MainDb;


        /// <summary>
        ///     Constructor when called from channel
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="db"></param>
        public KnockOutHandler(ulong channelId, IDatabase db)
        {
            KnockoutChannel = channelId.ToString();
            MainDb = db;
            GetDataFromDb();
        }

        public string KnockoutChannel { get; set; }

        public ulong KnockoutChannelUlong => Convert.ToUInt64(KnockoutChannel);

        public int KnockoutStatus => KnockoutInfo.Status == 0 ? 1 : KnockoutInfo.Status;

        public ulong KnockoutCreatorUlong => Convert.ToUInt64(KnockoutInfo.Owner);

        public string KnockoutTitle => KnockoutInfo.Name;

        public int ContendersCount => Contenders.Count;

        public int LivingContendersCount
        {
            get { return Contenders.FindAll(o => o.Score > 0).Count; }
        }

        public List<ulong> AllPlayerIds
        {
            get
            {
                var listToReturn = new List<ulong>();
                foreach (var player in Players) listToReturn.Add(Convert.ToUInt64(player.PlayerId));

                return listToReturn;
            }
        }

        public static ulong ChannelForUser(ulong userId, IDatabase db)
        {
            var userIdString = userId.ToString();
            var allKnockouts = db.GetAllKnockouts();
            var channelIdString = allKnockouts.Find(o => o.Owner == userIdString).Channel;

            if (channelIdString == null) return 0;

            var channelIdUlong = Convert.ToUInt64(channelIdString);
            return channelIdUlong;
        }

        public static List<ulong> NewDayForAll(IDatabase db)
        {
            var mainDb = db;

            mainDb.NewDayForAll();

            var allKnockouts = mainDb.GetAllKnockouts();
            var allActiveKnockouts = allKnockouts.FindAll(o => o.Status == 2);
            var allChannelsToReturn = new List<ulong>();

            foreach (var k in allActiveKnockouts) allChannelsToReturn.Add(Convert.ToUInt64(k.Channel));

            return allChannelsToReturn;
        }

        /// <summary>
        ///     Populate all properties from DB
        /// </summary>
        public void GetDataFromDb()
        {
            GetContendersFromDb();
            GetKnockoutFromDb();
            GetPlayersFromDb();
        }

        /// <summary>
        ///     Populate Contenders property from DB
        /// </summary>
        public void GetContendersFromDb()
        {
            Contenders = MainDb.GetAllContenders(KnockoutChannel);
        }

        /// <summary>
        ///     Populate KnockoutInfo property from DB
        /// </summary>
        public void GetKnockoutFromDb()
        {
            if (MainDb.GetAllKnockouts(KnockoutChannel).Count != 0)
                KnockoutInfo = MainDb.GetAllKnockouts(KnockoutChannel).First();
        }

        /// <summary>
        ///     Populate Players property from DB
        /// </summary>
        public void GetPlayersFromDb()
        {
            Players = MainDb.GetAllPlayers(KnockoutChannel);
        }

        /// <summary>
        ///     Return the turns left for a particular user id, if null (new player) return 3
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int TurnsLeftForPlayer(ulong userId)
        {
            var userIdString = userId.ToString();
            var currentPlayer = Players.Find(o => o.PlayerId == userIdString);

            return currentPlayer?.TurnsLeft ?? 3;
        }

        /// <summary>
        ///     Halve the scores of contenders and clear/repopulate handler
        /// </summary>
        public void HalveScores()
        {
            var livingContenders = Contenders.FindAll(o => o.Score > 1);
            foreach (var c in livingContenders)
            {
                var newScore = c.Score / 2 + (c.Score % 2 > 0 ? 1 : 0);
                MainDb.SetScoreForContender(newScore, c.Name, KnockoutChannel);
            }

            GetContendersFromDb();
        }

        /// <summary>
        ///     Clear the existing database, create new knockout entry and clear/repopulate handler
        /// </summary>
        /// <param name="userId"></param>
        public void CreateNewKnockout(ulong userId)
        {
            var userIdString = userId.ToString();

            MainDb.EmptyKnockoutDatabase(KnockoutChannel);
            MainDb.CreateNewKnockout(userIdString, KnockoutChannel);

            GetDataFromDb();
        }

        /// <summary>
        ///     Alter the knockout title in the database and repopulate the KnockoutInfo in handler
        /// </summary>
        /// <param name="title"></param>
        public void ChangeKnockoutTitle(string title)
        {
            MainDb.AddKnockoutTitle(title, KnockoutChannel);
            GetKnockoutFromDb();
        }

        /// <summary>
        ///     Add a new Contender into the database and repopulate the Contenders in the handler
        /// </summary>
        /// <param name="title"></param>
        public void AddNewContender(string title)
        {
            MainDb.AddContender(title, KnockoutChannel);
            GetContendersFromDb();
        }

        /// <summary>
        ///     Try to find parameter in Contender list, remove from DB and repopulate if there and TRUE, else FALSE
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool DeleteContender(string title)
        {
            if (Contenders.Find(o => o.Name == title) == null) return false;

            MainDb.RemoveContender(title, KnockoutChannel);
            GetContendersFromDb();
            return true;
        }

        /// <summary>
        ///     Clear the whole database, populate with defaults and then repopulate instance
        /// </summary>
        public void EmptyDatabase()
        {
            MainDb.ResetAllTables(KnockoutChannel);
            GetDataFromDb();
        }

        /// <summary>
        ///     Resets players table to daily defaults, repopulates instances
        /// </summary>
        public void NewDay()
        {
            MainDb.NewDay(KnockoutChannel);
            GetPlayersFromDb();
        }

        /// <summary>
        ///     Admin only function to rebuild database from scratch. Clear entries from UserChannel too. Used for new versions
        /// </summary>
        public void RebuildDataBase()
        {
            MainDb.EmptyKnockoutDatabase(KnockoutChannel);
            GetDataFromDb();
        }

        /// <summary>
        ///     Start a knockout, change the status of the table and repopulate
        ///     instance
        /// </summary>
        public void SetKnockoutToActive()
        {
            MainDb.SetKnockoutToActive(KnockoutChannel);
            GetDataFromDb();
        }

        /// <summary>
        ///     Perform a find on the players table, to see if that userid is listed as last played entry in database to 1
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool PlayerWentLastTime(ulong userId)
        {
            var userIdString = userId.ToString();

            var playerTurnCheck = Players.Find(o => o.PlayerId == userIdString);

            return playerTurnCheck != null && playerTurnCheck.LastPlayed != 0;
        }

        /// <summary>
        ///     Fairly hefty method that looks for best match using string starts, case insensitive string starts and Levenshtein
        ///     distance
        /// </summary>
        /// <param name="choiceToValidate"></param>
        /// <returns></returns>
        public string FindNearestMatch(string choiceToValidate)
        {
            var livingContenderList = Contenders.FindAll(o => o.Score > 0);

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

                Console.WriteLine($"Best LScore {bestLScore} for {bestMatch}");

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

                Console.WriteLine($"Best LScore {bestLScore} for {bestMatch}");

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

                Console.WriteLine($"Best LScore {bestLScore} for {bestMatch}");

                if (bestLScore > 2) bestMatch = "ERROR";
            }

            return bestMatch;
        }

        /// <summary>
        ///     Apply the scoring, player registering and contender elimination functions of a voting round
        /// </summary>
        /// <param name="choiceToAdd"></param>
        /// <param name="choiceToSub"></param>
        /// <param name="userId"></param>
        public void ApplyVoteChanges(string choiceToAdd, string choiceToSub, ulong userId)
        {
            var userIdString = userId.ToString();

            ApplyScoreChangesForVotingRound(choiceToAdd, choiceToSub);
            ApplyPlayerTurnChangesForVotingRound(userIdString);
            ApplyFallenChangesForVotingRound(userIdString);
        }

        /// <summary>
        ///     Add one to a choice in the contender table and deduct one. All choices must be pre-verified.
        /// </summary>
        /// <param name="choiceToAdd"></param>
        /// <param name="choiceToSub"></param>
        public void ApplyScoreChangesForVotingRound(string choiceToAdd, string choiceToSub)
        {
            MainDb.ChangeScore(choiceToAdd, 1, KnockoutChannel);
            MainDb.ChangeScore(choiceToSub, -1, KnockoutChannel);
            GetContendersFromDb();
        }

        /// <summary>
        ///     Check if player has played yet, add them if not and adjust their status
        /// </summary>
        /// <param name="userIdString"></param>
        public void ApplyPlayerTurnChangesForVotingRound(string userIdString)
        {
            // Has Player played before in this knockout? If not, add them.
            if (Players.Find(o => o.PlayerId == userIdString) == null)
                MainDb.AddPlayerToKnockout(userIdString, KnockoutChannel);

            MainDb.ResetAllPlayersLastPlayedStatus(KnockoutChannel);
            MainDb.RegisterPlayersTurn(userIdString, KnockoutChannel);
            GetPlayersFromDb();
        }

        /// <summary>
        ///     Find if any of the contenders has a score of 0 and give them a negative score, making them 'fallen'
        /// </summary>
        public void ApplyFallenChangesForVotingRound(string userIdString)
        {
            // Find first contender that has a score of 0. There should only be one!
            var newlyFallenContender = Contenders.Find(o => o.Score == 0);

            if (newlyFallenContender != null)
            {
                // Get count of all contenders left alive and negative it. This is the final score ranking for this contender.                
                var fallenScore = Contenders.FindAll(o => o.Score > 0).Count * -1;
                MainDb.SetScoreForContender(fallenScore, newlyFallenContender.Name, KnockoutChannel);
                MainDb.SetKillerForContender(userIdString, newlyFallenContender.Name, KnockoutChannel);
                GetContendersFromDb();
            }
        }

        /// <summary>
        ///     Set knockout info status to 3 (ended) and repopulate table.
        /// </summary>
        public void EndKnockout()
        {
            MainDb.SetKnockoutToEnded(KnockoutChannel);
            GetKnockoutFromDb();
        }

        /// <summary>
        ///     Check if a contender has a blank epitaph but a killer, if so, fill in placeholder epitaph and send id of killer,
        ///     else return 0
        /// </summary>
        /// <returns></returns>
        public ulong PlayerHasJustKilled()
        {
            var recentlyKilledContender = Contenders.Find(o => o.Epitaph == "" && o.Killer != "");
            if (recentlyKilledContender == null) return 0;

            MainDb.SetEpitaphForContender(recentlyKilledContender.Name, "inscription pending", KnockoutChannel);
            return Convert.ToUInt64(recentlyKilledContender.Killer);
        }

        /// <summary>
        ///     Validate if a user is in the killers list of the Contenders section and has a pending inscription
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CanWriteAnEpitaph(ulong userId)
        {
            var userIdString = userId.ToString();
            var killerSearch = Contenders.Find(o => o.Killer == userIdString);
            return killerSearch != null && killerSearch.Epitaph == "inscription pending";
        }

        public void WriteEpitaphFromUser(ulong userId, string epitaph)
        {
            var userIdString = userId.ToString();
            var contender = Contenders.Find(o => o.Killer == userIdString).Name;
            MainDb.SetEpitaphForContender(contender, epitaph, KnockoutChannel);
            GetContendersFromDb();
        }

        /// <summary>
        ///     String List Generation for Embed. List of Contender names sorted by score order. For Final Tally.
        /// </summary>
        /// <returns></returns>
        public List<string> AllContendersByScoreOrderList()
        {
            var sortedList = Contenders.OrderByDescending(o => o.Score).ToList();

            var outputList = new List<string>();

            foreach (var contender in sortedList) outputList.Add($"{contender.Name}");

            return outputList;
        }

        public bool ContendersScoredEqually(string contenderOne, string contenderTwo)
        {
            var score1 = Contenders.Find(m => m.Name == contenderOne).Score;
            var score2 = Contenders.Find(m => m.Name == contenderTwo).Score;

            return score1 == score2;
        }

        /// <summary>
        ///     String Generation for Embed. List of Contenders and Scores where Score > 0
        /// </summary>
        /// <returns></returns>
        public List<string> AllLivingContendersByScoreOrderList()
        {
            var livingContenderList = Contenders.FindAll(o => o.Score > 0);
            var sortedList = livingContenderList.OrderByDescending(o => o.Score).ToList();

            var outputList = new List<string>();

            foreach (var contender in sortedList) outputList.Add($"{contender.Name} : {contender.Score}");

            return outputList;
        }


        public List<string> AllFallenContendersByScoreOrderListWithEpitaph()
        {
            var fallenContenderList = Contenders.FindAll(o => o.Score < 1);
            var sortedList = fallenContenderList.OrderByDescending(o => o.Score).ToList();

            var outputList = new List<string>();

            foreach (var contender in sortedList) outputList.Add($"{contender.Name}\n\t- _{contender.Epitaph}_");

            return outputList;
        }
    }
}