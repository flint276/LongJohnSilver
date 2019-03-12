using LongJohnSilver.Interfaces;
using LongJohnSilver.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LongJohnSilver.Database.DataMethodsDrafts
{
    class DraftSlot
    {
        private string _channel;
        private string _playerId;
        private int _day;
        private int _slot;

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

        public int Day
        {
            get => _day;
            set
            {
                _day = value;
                Update();
            }
        }

        public int Slot
        {
            get => _slot;
            set
            {
                _slot = value;
                Update();
            }
        }

        private object[] UpdateParameters => new object[] { Id, Channel, PlayerId, Day, Slot };
        private object[] InsertParameters => new object[] { Channel, PlayerId, Day, Slot };
        private object[] IdParameters => new object[] { Id };

        private DraftSlot(long id, string channel, string playerId, int day, int slot)
        {
            Id = id;
            _channel = channel;
            _playerId = playerId;
            _day = day;
            _slot = slot;
        }

        public DraftSlot(string channel, string playerId, int day, int slot)
        {
            _channel = channel;
            _playerId = playerId;
            _day = day;
            _slot = slot;
            Insert();
        }

        public DraftSlot()
        {
            Id = -1;
        }

        public static List<DraftSlot> SelectAll()
        {
            var resultList = new List<DraftSlot>();
            var mainDataDb = Factory.GetDatabase();
            var dataResults = mainDataDb.GetData("SELECT * FROM draftslots");

            foreach (var dataRow in dataResults)
            {
                var id = (long)dataRow["id"];
                var channel = (string)dataRow["channel"];
                var playerId = (string)dataRow["playerid"];
                var day = (int) dataRow["day"];
                var slot = (int) dataRow["slot"];

                resultList.Add(new DraftSlot(id, channel, playerId, day, slot));
            }

            return resultList;
        }

        private void Update()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Update An Empty Draft Slot Class");

            MainDataDb.RunQuery(
                "UPDATE draftslots SET channel = @param2, playerid = @param3, day = @param4, slot = @param5 WHERE id = @param1",
                UpdateParameters);
        }

        private void Insert()
        {
            MainDataDb.RunQuery(
                "INSERT INTO draftslots (channel, playerid, day, slot) VALUES (@param1, @param2, @param3, @param4)",
                InsertParameters);

            var dataResults = MainDataDb.GetData(
                "SELECT id FROM draftslots WHERE channel = @param1 AND playerid = @param2 AND day = @param3 AND slot = @param4",
                InsertParameters);

            Id = (long)dataResults.First()["id"];
        }

        public void Delete()
        {
            if (Id == -1) throw new InvalidOperationException("Attempted to Delete An Empty Draft Slot Class");

            MainDataDb.RunQuery("DELETE FROM draftslots WHERE id = @param1", IdParameters);
        }
    }
}
