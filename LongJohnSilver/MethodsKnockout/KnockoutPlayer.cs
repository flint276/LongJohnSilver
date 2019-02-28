using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Discord;
using LongJohnSilver.Database;
using LongJohnSilver.Interfaces;
using LongJohnSilver.Statics;

namespace LongJohnSilver.MethodsKnockout
{
    public class KnockoutPlayer
    {
        private string _playerId;
        private int _turnsLeft;
        private int _lastPlayed;
        private string _channel;

        private readonly IDatabase MainDataDb = Factory.GetDatabase();

        private long Id;

        public string PlayerId
        {
            get => _playerId;
            set
            {
                _playerId = value;
                Update();
            }
        }

        public int TurnsLeft
        {
            get => _turnsLeft;
            set
            {
                _turnsLeft = value;
                Update();
            }
        }

        public int LastPlayed
        {
            get => _lastPlayed;
            set
            {
                _lastPlayed = value;
                Update();
            }
        }

        public string Channel
        {
            get => _channel;
            set
            {
                _channel = value;
                Update();             
            }
        }

        private object[] UpdateParameters => new object[] {Id, PlayerId, TurnsLeft, LastPlayed, Channel};    
        private object[] InsertParameters => new object[] {PlayerId, TurnsLeft, LastPlayed, Channel};
        private object[] IdParameters => new object[] {Id};

        public KnockoutPlayer(long id, string playerId, int turnsLeft, int lastPlayed, string channel)
        {
            Id = id;
            _playerId = playerId;
            _turnsLeft = turnsLeft;
            _lastPlayed = lastPlayed;
            _channel = channel;            
        }

        public KnockoutPlayer(string playerId, int turnsLeft, int lastPlayed, string channel)
        {            
            _playerId = playerId;
            _turnsLeft = turnsLeft;
            _lastPlayed = lastPlayed;
            _channel = channel;
            Insert();
        }

        public KnockoutPlayer()
        {
            Id = -1;
        }

        public static List<KnockoutPlayer> SelectAll()
        {
            var resultList = new List<KnockoutPlayer>();
            var mainDataDb = Factory.GetDatabase();
            var dataResults = mainDataDb.GetData("SELECT * FROM kplayers;");

            foreach (var dataRow in dataResults)
            {
                var id = (long) dataRow["id"];
                var playerId = (string) dataRow["playerid"];
                var turnsLeft = (int) dataRow["turnsleft"];
                var lastPlayed = (int) dataRow["lastplayed"];
                var channel = (string) dataRow["channel"];

                resultList.Add(new KnockoutPlayer(id, playerId, turnsLeft, lastPlayed, channel));
            }

            return resultList;
        }

        private void Update()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Update An Empty Knockout Player Class");

            MainDataDb.RunQuery(
                "UPDATE kplayers SET playerid = @param2, turnsleft = @param3, lastplayed = @param4, channel = @param5 WHERE id = @param1",
                UpdateParameters);
        }

        private void Insert()
        {
            MainDataDb.RunQuery(
                "INSERT INTO kplayers (playerid, turnsleft, lastplayed, channel) VALUES (@param1, @param2, @param3, @param4)",
                InsertParameters);

            var dataResults = MainDataDb.GetData(
                "SELECT id FROM kplayers WHERE playerid = @param1 AND turnsleft = @param2 AND lastplayed = @param3 AND channel = @param4",
                InsertParameters);

            Id = (long)dataResults.First()["id"];
        }

        public void Delete()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Delete An Empty Knockout Player Class");

            MainDataDb.RunQuery("DELETE FROM kplayers WHERE id = @param1", IdParameters);
        }
    }
}
