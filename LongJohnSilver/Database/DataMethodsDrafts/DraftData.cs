using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LongJohnSilver.Database.DataMethodsKnockout;

namespace LongJohnSilver.Database.DataMethodsDrafts
{
    public class DraftData
    {
        public string DraftChannel { get; set; }

        public DraftData(string channelId)
        {
            DraftChannel = channelId;
        }

        public DraftGame Game => DraftGame.SelectAll().Find(x => x.Channel == DraftChannel) ?? new DraftGame();

        public List<DraftChoice> DraftChoices => 
            DraftChoice.SelectAll().FindAll(x => x.Channel == DraftChannel) ?? new List<DraftChoice>();

        public List<DraftSlot> DraftSlots =>
            DraftSlot.SelectAll().FindAll(x => x.Channel == DraftChannel) ?? new List<DraftSlot>();

        public void DeleteAllData()
        {
            DraftChoices.ForEach(x => x.Delete());
            DraftSlots.ForEach(x => x.Delete());
            if (Game.Channel != null) Game.Delete();
        }

        public static List<ulong> GetAllDraftChannels()
        {
            return DraftGame.SelectAll()?.Select(x => Convert.ToUInt64(x.Channel)).ToList();
        }

    }
}
