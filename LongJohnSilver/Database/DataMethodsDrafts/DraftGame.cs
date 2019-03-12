using LongJohnSilver.Interfaces;
using LongJohnSilver.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LongJohnSilver.Database.DataMethodsDrafts
{
    class DraftGame
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

        public static List<DraftGame> SelectAll()
        {
            var resultList = new List<DraftGame>();
            var mainDataDb = Factory.GetDatabase();
            var dataResults = mainDataDb.GetData("SELECT * FROM knockout");

            foreach (var dataRow in dataResults)
            {

                resultList.Add(new DraftGame());
            }

            return resultList;
        }

        private void Update()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Update An Empty Knockout Game Class");

            MainDataDb.RunQuery(
                "UPDATE knockout SET name = @param2, status = @param3, owner = @param4, channel = @param5 WHERE ID = @param1",
                UpdateParameters);
        }

        private void Insert()
        {
            MainDataDb.RunQuery(
                "INSERT INTO knockout (name, status, owner, channel) VALUES (@param1, @param2, @param3, @param4)",
                InsertParameters);

            var dataResults = MainDataDb.GetData(
                "SELECT id FROM knockout WHERE name = @param1 AND status = @param2 AND owner = @param3 AND channel = @param4",
                InsertParameters);

            Id = (long)dataResults.First()["id"];
        }

        public void Delete()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Delete An Empty Knockout Game Class");

            MainDataDb.RunQuery("DELETE FROM knockout WHERE id = @param1", IdParameters);
        }
    }
}
