using System;
using System.Collections.Generic;
using System.Linq;
using LongJohnSilver.Interfaces;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Database.DataMethodsKnockout
{
    public class KnockoutContender
    {
        private string _name;
        private int _score;
        private string _killer;
        private string _epitaph;
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

        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                Update();
            }
        }

        public string Killer
        {
            get => _killer;
            set
            {
                _killer = value;
                Update();
            }
        }

        public string Epitaph
        {
            get => _epitaph;
            set
            {
                _epitaph = value;
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

        private object[] UpdateParameters => new object[] {Id, Name, Score, Killer, Epitaph, Channel};
        private object[] InsertParameters => new object[] {Name, Score, Killer, Epitaph, Channel};
        private object[] IdParameters => new object[] {Id};

        public KnockoutContender(long id, string name, int score, string killer, string epitaph, string channel)
        {
            Id = id;
            _name = name;
            _score = score;
            _killer = killer;
            _epitaph = epitaph;
            _channel = channel;
        }

        public KnockoutContender(string name, int score, string killer, string epitaph, string channel)
        {            
            _name = name;
            _score = score;
            _killer = killer;
            _epitaph = epitaph;
            _channel = channel;
            Insert();
        }

        public KnockoutContender()
        {
            Id = -1;
        }

        public static List<KnockoutContender> SelectAll()
        {
            var resultList = new List<KnockoutContender>();
            var mainDataDb = Factory.GetDatabase();
            var dataResults = mainDataDb.GetData("SELECT * FROM contenders;");

            foreach (var dataRow in dataResults)
            {
                var id = (long) dataRow["id"];
                var name = (string) dataRow["name"];
                var score = (int) dataRow["score"];
                var killer = (string) dataRow["killer"];
                var epitaph = (string) dataRow["epitaph"];
                var channel = (string) dataRow["channel"];

                resultList.Add(new KnockoutContender(id, name, score, killer, epitaph, channel));
            }

            return resultList;
        }

        private void Update()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Update An Empty Knockout Contender Class");

            MainDataDb.RunQuery(
                "UPDATE contenders SET name = @param2, score = @param3, killer = @param4, epitaph = @param5, channel = @param6 WHERE id = @param1", 
                UpdateParameters);
        }

        private void Insert()
        {
            MainDataDb.RunQuery(
                "INSERT INTO contenders (name, score, killer, epitaph, channel) VALUES (@param1, @param2, @param3, @param4, @param5)", 
                InsertParameters);

            var dataResults = MainDataDb.GetData(
                "SELECT id FROM contenders WHERE name = @param1 AND score = @param2 AND killer = @param3 AND epitaph = @param4 AND channel = @param5",
                InsertParameters);

            Id = (long) dataResults.First()["id"];
        }

        public void Delete()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Delete An Empty Knockout Contender Class");

            MainDataDb.RunQuery("DELETE FROM contenders WHERE id = @param1", IdParameters);
        }
    }
}
