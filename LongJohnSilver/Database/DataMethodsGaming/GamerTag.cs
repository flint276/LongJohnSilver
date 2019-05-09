using LongJohnSilver.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LongJohnSilver.Database.DataMethodsGaming
{
    public class GamerTag
    {
        private string _guild;
        private string _user;
        private string _service;
        private string _tag;

        private readonly IDatabase MainDataDb = Factory.GetDatabase();

        private long Id;

        public string Guild
        {
            get => _guild;
            set
            {
                _guild = value;
                Update();
            }
        }

        public string User
        {
            get => _user;
            set
            {
                _user = value;
                Update();
            }
        }

        public string Service
        {
            get => _service;
            set
            {
                _service = value;
                Update();
            }
        }

        public string Tag
        {
            get => _tag;
            set
            {
                _tag = value;
                Update();
            }
        }

        private object[] UpdateParameters => new object[] { Id, Guild, User, Service, Tag };
        private object[] InsertParameters => new object[] { Guild, User, Service, Tag };
        private object[] IdParameters => new object[] { Id };

        private GamerTag(long id, string guild, string user, string service, string tag)
        {
            Id = id;
            _guild = guild;
            _user = user;
            _tag = tag;
            _service = service;
        }

        public GamerTag(string guild, string user, string service, string tag)
        {
            _guild = guild;
            _user = user;
            _tag = tag;
            _service = service;
            Insert();
        }

        public static List<GamerTag> SelectAll()
        {
            var resultList = new List<GamerTag>();
            var mainDataDb = Factory.GetDatabase();
            var dataResults = mainDataDb.GetData("SELECT * FROM gamertags");

            foreach (var dataRow in dataResults)
            {
                var id = (long) dataRow["id"];
                var guild = (string)dataRow["guild"];
                var user = (string)dataRow["user"];
                var service = (string)dataRow["service"];
                var tag = (string)dataRow["tag"];

                resultList.Add(new GamerTag(id, guild, user, service, tag));
            }

            return resultList;
        }

        private void Update()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Update An Empty GamerTag Game Class");

            MainDataDb.RunQuery(
                "UPDATE gamertags SET guild = @param2, user = @param3, service = @param4, tag = @param5 WHERE ID = @param1",
                UpdateParameters);
        }

        private void Insert()
        {
            MainDataDb.RunQuery(
                "INSERT INTO gamertags (guild, user, service, tag) VALUES (@param1, @param2, @param3, @param4)",
                InsertParameters);

            var dataResults = MainDataDb.GetData(
                "SELECT id FROM gamertags WHERE guild = @param1 AND user = @param2 AND service = @param3 AND tag = @param4",
                InsertParameters);

            Id = (long)dataResults.First()["id"];
        }

        public void Delete()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Delete An Empty GamerTag Game Class");

            MainDataDb.RunQuery("DELETE FROM gamertags WHERE id = @param1", IdParameters);
        }
    }
}
