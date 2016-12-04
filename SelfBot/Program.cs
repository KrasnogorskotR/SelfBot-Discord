using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using SelfBot.Common.Types;
using Discord.Commands;
using System.Threading;
using System.Globalization;

namespace SelfBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Run().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;
        private Configuration _config;

        public async Task Run()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            const string configFile = @"configuration.json";

            try
            {
                _config = Configuration.LoadFile(configFile);
            }
            catch
            {
                _config = new Configuration();

                Console.WriteLine("The User bot's configuration file has been created. Please enter a valid token.");
                Console.Write("Token: ");

                _config.Token = Console.ReadLine();
                _config.Owners = new ulong[] { 197440710431997952 };
                _config.SaveFile(configFile);
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info
            });

            _client.Log += (e) => Log(e);

            await _client.LoginAsync(TokenType.User, _config.Token);
            await _client.ConnectAsync();

            var map = new DependencyMap();
            map.Add(_client);

            _handler = new CommandHandler();
            await _handler.Install(map, _config);

            await Task.Delay(-1);
        }

        private Task _client_UserJoined(SocketGuildUser arg)
        {
            throw new NotImplementedException();
        }

        #region Logging
        public async Task Log(LogMessage e)
        {
            await Task.Run(() => Console.WriteLine($"{e.ToString()}"));
        }

        public async Task Log(string source, string message)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Info, source, message);
            await Log(logMessage);
        }

        public async Task Log(LogSeverity logSeverity, string source, string message)
        {
            LogMessage logMessage = new LogMessage(logSeverity, source, message);
            await Log(logMessage);
        }

        public async Task Log(string source, string message, Exception exception)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Info, source, message, exception);
            await Log(logMessage);
        }

        public async Task Log(LogSeverity logSeverity, string source, string message, Exception exception)
        {
            LogMessage logMessage = new LogMessage(logSeverity, source, message, exception);
            await Log(logMessage);
        }

        public async Task Log(string message)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Info, "Debug", message);
            await Log(logMessage);
        }

        public async Task Log(string message, Type type)
        {
            LogMessage logMessage = new LogMessage(LogSeverity.Info, type.Name, message);
            await Log(logMessage);
        }
        #endregion
    }
}
