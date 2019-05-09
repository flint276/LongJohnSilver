using LongJohnSilver.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LongJohnSilver.Database.DataMethodsDrafts
{
    public class DraftGame
    {
        private string _channel;
        private string _owner;
        private int _status;
        private string _title;
        private string _description;
        private int _startTime;
        private int _days;

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

        public string Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                Update();
            }
        }

        public int Status
        {
            get => _status;
            set
            {
                _status = value;
                Update();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Update();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                Update();
            }
        }

        public int StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                Update();
            }
        }

        public int Days
        {
            get => _days;
            set
            {
                _days = value;
                Update();
            }
        }

        private object[] UpdateParameters => new object[] { Id, Channel, Owner, Status, Title, Description, StartTime, Days };
        private object[] InsertParameters => new object[] { Channel, Owner, Status, Title, Description, StartTime, Days };
        private object[] IdParameters => new object[] { Id };

        private DraftGame(long id, string channel, string owner, int status, string title, string description,
            int startTime, int days)
        {
            Id = id;
            _channel = channel;
            _owner = owner;
            _status = status;
            _title = title;
            _description = description;
            _startTime = startTime;
            _days = days;
        }

        public DraftGame(string channel, string owner, int status, string title, string description,
            int startTime, int days)
        {            
            _channel = channel;
            _owner = owner;
            _status = status;
            _title = title;
            _description = description;
            _startTime = startTime;
            _days = days;
            Insert();
        }

        public DraftGame()
        {
            Id = -1;
        }

        public static List<DraftGame> SelectAll()
        {
            var resultList = new List<DraftGame>();
            var mainDataDb = Factory.GetDatabase();
            var dataResults = mainDataDb.GetData("SELECT * FROM draftgames");

            foreach (var dataRow in dataResults)
            {
                var id = (long)dataRow["id"];
                var channel = (string)dataRow["channel"];
                var owner = (string) dataRow["owner"];
                var status = (int) dataRow["status"];
                var title = (string) dataRow["title"];
                var description = (string) dataRow["description"];
                var startTime = (int) dataRow["starttime"];
                var days = (int) dataRow["days"];

                resultList.Add(new DraftGame(id, channel, owner, status, title, description, startTime, days));
            }

            return resultList;
        }

        private void Update()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Update An Empty Draft Game Class");

            MainDataDb.RunQuery(
                "UPDATE draftgames SET channel = @param2, owner = @param3, status = @param4, title = @param5, description = @param6, starttime = @param7, days = @param8 WHERE ID = @param1",
                UpdateParameters);
        }

        private void Insert()
        {
            MainDataDb.RunQuery(
                "INSERT INTO draftgames (channel, owner, status, title, description, starttime, days) VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7)",
                InsertParameters);

            var dataResults = MainDataDb.GetData(
                "SELECT id FROM draftgames WHERE channel = @param1 AND owner = @param2 AND status = @param3 AND title = @param4 AND description = @param5 AND starttime = @param6 AND days = @param7",
                InsertParameters);

            Id = (long)dataResults.First()["id"];
        }

        public void Delete()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Delete An Empty Draft Game Class");

            MainDataDb.RunQuery("DELETE FROM draftgames WHERE id = @param1", IdParameters);
        }
    }
}
