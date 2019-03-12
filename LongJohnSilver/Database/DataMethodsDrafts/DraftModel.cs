using System;
using System.Collections.Generic;
using System.Text;

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

        // Combined Data Handling

        public void DeleteAllData()
        {
            DData.DeleteAllData();
        }
    }
}
