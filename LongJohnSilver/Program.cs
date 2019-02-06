using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Reflection;
using System.IO;
using System.Data.SQLite;
using LongJohnSilver.Database;
using LongJohnSilver.Statics;

namespace LongJohnSilver
{
    class Program 
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;
        private DiscordSocketConfig _config;

        private static System.Timers.Timer _botTimer;

        

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _config = new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true
            };



            var botToken = "";

            // Validate config and data directories and get bot token

            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var currentConfig = $@"{currentDirectory}{Path.DirectorySeparatorChar}config";
            var dataDirectory = $@"{currentDirectory}{Path.DirectorySeparatorChar}Data";

            if (File.Exists(currentConfig)) {
                var lines = System.IO.File.ReadAllLines(currentConfig);
                botToken = lines.First();
            }
            else
            {
                throw new Exception($"Config File is Missing: {currentConfig}");
            }

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }

            // event subscriptions
            _client.Log += Log;
            
            // hourly timer

            var secondsTillHourIsUp = (60 - (DateTime.Now.Minute)) * 60;
            

            _botTimer = new System.Timers.Timer(secondsTillHourIsUp * 1000);

            _botTimer.Elapsed += async (sender, e) => await OnTimedEvent(sender, e, _client);
            _botTimer.Elapsed += (sender, e) => _botTimer.Interval = 3600000;

            _botTimer.AutoReset = true;
            _botTimer.Enabled = true;

            // start bot

            await RegisterCommandsAsync();

            await _client.LoginAsync(Discord.TokenType.Bot, botToken);

            await _client.SetGameAsync("type !help for... help, obv");

            await _client.StartAsync();            

            await Task.Delay(-1);
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage message = arg as SocketUserMessage;

            if (message == null || message.Author.IsBot) return;           

            var argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }

            }

        }

        private static async Task OnTimedEvent(object source, System.Timers.ElapsedEventArgs e, DiscordSocketClient _client)
        {
            Console.WriteLine("Starting!");            
            Console.WriteLine("In!");
            var channelsToNotify = KnockOutHandler.NewDayForAll(Factory.GetDatabase());

            foreach (var c in channelsToNotify)
            {
                var discordChannel = (ISocketMessageChannel)_client.GetChannel(c);
                await discordChannel.SendMessageAsync("It is a glorious new hour. Everyone's turns are reset!");
            }

            await Task.Delay(-1);
        }
    }
}
