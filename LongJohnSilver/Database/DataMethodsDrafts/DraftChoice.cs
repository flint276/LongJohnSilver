using LongJohnSilver.Interfaces;
using LongJohnSilver.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LongJohnSilver.Database.DataMethodsDrafts
{
    public class DraftChoice
    {
        private string _channel;
        private string _playerId;
        private string _name;

        private readonly IDatabase MainDataDb = Factory.GetDatabase();

        private long Id;

        public string Channel
        {
            get => _channel;
            set
            {
                _channel = value;
                Update();
            }
        }

        public string PlayerId
        {
            get => _playerId;
            set
            {
                _playerId = value;
                Update();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Update();
            }
        }

        private object[] UpdateParameters => new object[] {Id, Channel, PlayerId, Name};
        private object[] InsertParameters => new object[] {Channel, PlayerId, Name};
        private object[] IdParameters => new object[] {Id};

        private DraftChoice(long id, string channel, string playerId, string name)
        {
            Id = id;            
            _channel = channel;
            _playerId = playerId;
            _name = name;
        }

        public DraftChoice(string channel, string playerId, string name)
        {
            _channel = channel;
            _playerId = playerId;
            _name = name;
            Insert();
        }

        public DraftChoice()
        {
            Id = -1;
        }

        public static List<DraftChoice> SelectAll()
        {
            var resultList = new List<DraftChoice>();
            var mainDataDb = Factory.GetDatabase();
            var dataResults = mainDataDb.GetData("SELECT * FROM draftchoices");

            foreach (var dataRow in dataResults)
            {
                var id = (long) dataRow["id"];
                var channel = (string) dataRow["channel"];
                var playerId = (string) dataRow["playerid"];
                var name = (string) dataRow["name"];

                resultList.Add(new DraftChoice(id, channel, playerId, name));
            }

            return resultList;
        }

        private void Update()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Update An Empty Draft Choice Class");

            MainDataDb.RunQuery(
                "UPDATE draftchoices SET channel = @param2, playerid = @param3, name = @param4 WHERE id = @param1",
                UpdateParameters);
        }

        private void Insert()
        {
            MainDataDb.RunQuery(
                "INSERT INTO draftchoices (channel, playerid, name) VALUES (@param1, @param2, @param3)",
                InsertParameters);

            var dataResults = MainDataDb.GetData(
                "SELECT id FROM draftchoices WHERE channel = @param1 AND playerid = @param2 AND name = @param3",
                InsertParameters);

            Id = (long)dataResults.First()["id"];
        }

        public void Delete()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Delete An Empty Draft Choice Class");

            MainDataDb.RunQuery("DELETE FROM draftchoices WHERE id = @param1", IdParameters);
        }

    }
}
