using Discord.WebSocket;
using LongJohnSilver.Database.DataMethodsKnockout;
using LongJohnSilver.FlintTimeToolkit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LongJohnSilver.Enums;

namespace LongJohnSilver.Statics
{
    public static class BotTimers
    {
        private static DiscordSocketClient Client;
        private static System.Timers.Timer KnockoutNewDayTimer;

        public static void InitializeTimers(DiscordSocketClient _client)
        {
            Client = _client;

            var channelsToNotify = KnockoutData.GetAllKnockoutChannels();
            if (channelsToNotify != null)
            {
                KnockoutNewDayTimerInit();
            }
        }

        public static void KnockoutNewDayTimerInit()
        {                        
            KnockoutNewDayTimer = new System.Timers.Timer(SecondsTill.HourIsUp * 1000);            

            KnockoutNewDayTimer.Elapsed += async (sender, e) => await KnockoutNewDayEvent(sender, e);
            KnockoutNewDayTimer.Elapsed += (sender, e) => KnockoutNewDayTimer.Interval = SecondsTill.HourIsUp * 1000;

            KnockoutNewDayTimer.AutoReset = true;
            KnockoutNewDayTimer.Enabled = true;
        }

        private static async Task KnockoutNewDayEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Starting!");
            Console.WriteLine("In!");
            var channelsToNotify = KnockoutData.GetAllKnockoutChannels();

            foreach (var c in channelsToNotify)
            {
                var discordChannel = (ISocketMessageChannel)Client.GetChannel(c);                
                var kModel = KnockoutModel.ForChannel(c);
                if (kModel.KnockoutStatus == KnockoutStatus.KnockoutInProgress)
                {
                    await discordChannel.SendMessageAsync("It is a glorious new hour. Everyone's turns are reset!");
                    kModel.NewDay();
                }                
            }

            await Task.Delay(-1);
        }
    }
}
