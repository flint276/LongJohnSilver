using System.Collections.Generic;

namespace LongJohnSilver.Database
{
    public interface IDatabase
    {
        void AddContender(string contenderName, string channelId);
        void AddKnockoutTitle(string knockoutTitle, string channelId);
        void AddPlayerToKnockout(string userIdString, string channelId);
        void ChangeScore(string contenderName, int modifier, string channelId);
        void CreateDatabase();
        void CreateNewKnockout(string userId, string channelId);
        void EmptyKnockoutDatabase(string channelId);
        List<Contender> GetAllContenders(string channelId);
        List<Knockout> GetAllKnockouts();
        List<Knockout> GetAllKnockouts(string channelId);
        List<KPlayer> GetAllPlayers(string channelId);
        List<ChannelRole> GetAllChannelRoles();
        int GetVersion();
        void NewDay(string channelId);
        void NewDayForAll();
        void RegisterPlayersTurn(string userIdString, string channelId);
        void RemoveContender(string contenderName, string channelId);
        void ResetAllPlayersLastPlayedStatus(string channelId);
        void ResetAllTables(string channelId);
        void ResetContenderTable(string channelId);
        void ResetKnockoutTable(string channelId);
        void ResetPlayersTable(string channelId);
        void RunQuery(string sql);
        void RunQuery(string sql, object[] parameters);
        void SetEpitaphForContender(string contenderName, string epitaph, string channelId);
        void SetKillerForContender(string userIdString, string contenderName, string channelId);
        void SetKnockoutToActive(string channelId);
        void SetKnockoutToEnded(string channelId);
        void SetScoreForContender(int value, string contenderName, string channelId);
    }
}