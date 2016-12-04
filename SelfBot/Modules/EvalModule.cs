using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SelfBot.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace SelfBot.Modules
{
    [Name("eval")]
    public class EvalModule : ModuleBase
    {
        private DiscordSocketClient _client;

        public EvalModule(DiscordSocketClient client)
        {
            _client = client;
        }

        public class RoslynGlobals
        {
            public DiscordSocketClient _client { get; set; }
            //public DataContext db { get; set; } = new DataContext();

            public RoslynGlobals(DiscordSocketClient c)
            {
                _client = c;
            }
        }

        [Command("evaluate"), Alias("eval")]
        [Summary("Execute some c# code.")]
        public async Task Evaluate([Remainder]string expression)
        {
            
        }
    }
}
