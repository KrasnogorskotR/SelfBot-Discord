using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SelfBot.Common.Helpers;
using SelfBot.Common.Types;
using SelfBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfBot
{
    class CommandHandler
    {
        public CommandService commands;
        private DiscordSocketClient client;
        private IDependencyMap map;
        private Configuration config;

        public async Task Install(IDependencyMap _map, Configuration _config)
        {
            client = _map.Get<DiscordSocketClient>();
            CommandServiceConfig commandConfig = new CommandServiceConfig();
            commandConfig.CaseSensitiveCommands = true;
            commands = new CommandService(commandConfig);
            _map.Add(commands);
            map = _map;
            config = _config;

            await commands.AddModuleAsync<MainModule>();
            await commands.AddModuleAsync<EvalModule>();
            //await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            client.MessageReceived += HandleCommand;
        }

        public async Task HandleCommand(SocketMessage parameterMessage)
        {

            var message = parameterMessage as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;
            string prefix = "dev.";
            if (!(message.HasStringPrefix(prefix, ref argPos))) return;

            var context = new CommandContext(client, message);
            try
            {
                if (context.Message.Author.Id == context.Client.CurrentUser.Id)
                {
                    if (message.Content.Substring(prefix.Length).StartsWith("help"))
                    {
                        await Help(context, prefix);
                        return;
                    }

                    var result = await commands.ExecuteAsync(context, argPos, map);
                }
            }
            catch (Exception e)
            {
                await context.Channel.SendMessageAsync($"Error: `{e.Message}`, StackTrace: {e.StackTrace}");
            }
        }

        private async Task Help(CommandContext context, string prefix)
        {
            int numCommands = 0;

            EmbedBuilder embed = new EmbedBuilder();
            
            List<string> Titles = new List<string>();
            List<string> Descriptions = new List<string>();
            int maxItems = 9;
            int pageN = 0;
            if (context.Message.Content.Length > prefix.Length+4) pageN = Convert.ToInt32(context.Message.Content.Substring(prefix.Length + 5)) - 1;

            embed.Description = "\u00ad";
            //embed.Footer = EmbedHelper.Footer($"Requested by {context.User.Username}", context.User);
            embed.Color = new Color(59, 191, 232);
            foreach (CommandInfo ci in commands.Commands)
            {
                string sets = "";

                if (!(ci.Remarks == "" || ci.Remarks == null))
                {
                    sets += $"{ci.Remarks} ";
                }

                if (ci.Summary == "" || ci.Summary == null)
                {
                    Titles.Add(sets + ci.Name);
                    Descriptions.Add("Unknown Description.");
                }
                else
                {
                    Titles.Add(sets + ci.Name);
                    Descriptions.Add(ci.Summary);
                }
            }

            Pages pages = new Pages(Titles, Descriptions, maxItems);
            embed.Title = $"Commands - Page {pageN + 1}/{pages.NumberOfPages}";
            if (pages.NumberOfPages < pageN + 1)
            {
                await context.Message.ModifyAsync(x =>
                {
                    x.Content = $"Page {pageN + 1} dosen't exist!";
                });
                return;
            }
            Page page = pages[pageN];

            for (int i = 0; i < page.Title.Count; i++)
            {
                embed.AddField(EmbedHelper.Field(page.Title[i], page.Description[i]));
                numCommands++;
            }

            for (int i = numCommands; i < maxItems; i++)
            {
                if (i == maxItems - 1)
                {
                    embed.AddField(EmbedHelper.Field("\u00ad", "\u00ad\n\u00ad"));
                }
                else
                {
                    embed.AddField(EmbedHelper.Field("\u00ad", "\u00ad"));
                }
                numCommands++;
            }

            await context.Message.ModifyAsync(x =>
            {
                x.Content = $"";
                x.Embed = embed;
            });
        }
    }
}
