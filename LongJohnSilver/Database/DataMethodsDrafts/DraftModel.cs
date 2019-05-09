using System;
using System.Collections.Generic;
using System.Text;
using LongJohnSilver.Enums;

namespace LongJohnSilver.Database.DataMethodsDrafts
{
    public class DraftModel
    {
        private DraftData DData;
        public ulong GameChannel;

        public DraftModel(ulong channelId)
        {            
            GameChannel = channelId;
            DData = new DraftData(channelId.ToString());
        }

        public string Timezone;

        // Combined Data Handling

        public void DeleteAllData()
        {
            DData.DeleteAllData();
        }

        // Draft Game Data
        public void AddNewDraft(ulong playerId)
        {
            var playerIdString = playerId.ToString();
            DData.DeleteAllData();

            var _ = new DraftGame(
                GameChannel.ToString(),
                playerIdString,
                (int)DraftStatus.DraftInConstruction,
                "",
                "",
                2001010100,
                0
                );
        }

        public DraftStatus DraftStatus
        {
            get => DData.Game.Status == 0 ? DraftStatus.NoDraft : (DraftStatus)DData.Game.Status;
            set => DData.Game.Status = (int)value;
        }
    }
}
