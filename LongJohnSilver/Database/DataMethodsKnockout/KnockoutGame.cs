using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Database.DataMethodsKnockout
{
    public class KnockoutGame
    {
        private string _name;
        private int _status;
        private string _owner;
        private string _channel;

        private readonly IDatabase MainDataDb = Factory.GetDatabase();

        private long Id;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
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

        public string Owner
        {
            get => _owner;
            set
            {
                _owner = value;
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

        private object[] UpdateParameters => new object[] {Id, Name, Status, Owner, Channel};
        private object[] InsertParameters => new object[] {Name, Status, Owner, Channel};
        private object[] IdParameters => new object[] {Id};

        private KnockoutGame(long id, string name, int status, string owner, string channel)
        {
            Id = id;
            _name = name;
            _status = status;
            _owner = owner;
            _channel = channel;
        }

        public KnockoutGame(string name, int status, string owner, string channel)
        {            
            _name = name;
            _status = status;
            _owner = owner;
            _channel = channel;
            Insert();
        }

        public KnockoutGame()
        {
            Id = -1;
        }

        public static List<KnockoutGame> SelectAll()
        {
            var resultList = new List<KnockoutGame>();
            var mainDataDb = Factory.GetDatabase();
            var dataResults = mainDataDb.GetData("SELECT * FROM knockout");

            foreach (var dataRow in dataResults)
            {
                var id = (long) dataRow["id"];
                var name = (string) dataRow["name"];
                var status = (int) dataRow["status"];
                var owner = (string) dataRow["owner"];
                var channel = (string) dataRow["channel"];

                resultList.Add(new KnockoutGame(id, name, status, owner, channel));
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

            Id = (long) dataResults.First()["id"];
        }

        public void Delete()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Delete An Empty Knockout Game Class");

            MainDataDb.RunQuery("DELETE FROM knockout WHERE id = @param1", IdParameters);
        }
    }
}
