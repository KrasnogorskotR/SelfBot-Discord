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

namespace SelfBot.Modules
{
    [Name("Main")]
    class MainModule : ModuleBase
    {
        [Command("guilds")]
        [Summary("Shows all the guilds User is in.")]
        public async Task Guilds()
        {
            string toReturn = "```\n";
            int fields = 0;
            foreach (SocketGuild g in (Context.Client as DiscordSocketClient).Guilds)
            {
                if (!(fields >= 8))
                {
                    try
                    {
                        toReturn += $"{g.Name.Substring(0, 15)}... | Owner: {g.GetUser(g.OwnerId).Username}#{g.GetUser(g.OwnerId).Discriminator}\n";
                    }
                    catch
                    {
                        toReturn += $"{g.Name}{SpaceForUser(g.Name, 18)} | Owner: {g.GetUser(g.OwnerId).Username}#{g.GetUser(g.OwnerId).Discriminator}\n";
                    }
                    fields++;
                }
            }
            await Context.Message.ModifyAsync(x =>
            {
                x.Content = toReturn + "```";
            });
        }

        [Command("assignrole")]
        public async Task AssignRole(IUser user, [Remainder] string role)
        {
            List<IRole> roles = new List<IRole>();

            foreach (IRole ir in Context.Guild.Roles)
            {
                if (role == ir.Name || role == ir.Id.ToString())
                {
                    roles.Add(ir);
                }
            }

            await (user as SocketGuildUser).AddRolesAsync(roles);
        }

        [Command("quote")]
        public async Task Quote(ulong id)
        {
            var message = Context.Channel.GetMessageAsync(id).Result;

            EmbedBuilder embed = new EmbedBuilder();

            //embed.Author = EmbedHelper.Author(message.Author);
            //embed.AddField(EmbedHelper.Field("­", message.Content, false));
            //embed.Timestamp = message.Timestamp;

            await Context.Message.ModifyAsync(x =>
            {
                x.Content = $"{message.Content}";
                x.Embed = embed;
            });
        }

        [Command("ping")]
        [Summary("Pong!")]
        public async Task Ping()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await Context.Channel.TriggerTypingAsync();
            stopwatch.Stop();
            long elapsed = stopwatch.ElapsedMilliseconds;
            await Context.Message.ModifyAsync(x =>
            {
                x.Content = $"Elapsed time: {elapsed}ms.";
            });
            stopwatch.Reset();
        }

        [Command("react")]
        public async Task React(ulong messageId, [Remainder] string emojis)
        {
            foreach (string s in emojis.Split(' '))
            {
                await (Context.Channel.GetMessageAsync(messageId).Result as SocketUserMessage).AddReactionAsync(s);
            }
        }

        [Command("status")]
        public async Task Status([Remainder] string status)
        {
            string toReply = "";
            string statu = status.ToLower();
            UserStatus userS = UserStatus.Unknown;

            switch (statu)
            {
                case "dnd":
                    toReply = "Do Not Disturb";
                    userS = UserStatus.DoNotDisturb;
                    break;
                case "online":
                    toReply = "Online";
                    userS = UserStatus.Online;
                    break;
                case "inv":
                    toReply = "Invisible";
                    userS = UserStatus.Invisible;
                    break;
                case "invisible":
                    toReply = "Invisible";
                    userS = UserStatus.Invisible;
                    break;
                case "do not disturb":
                    toReply = "Do Not Disturb";
                    userS = UserStatus.DoNotDisturb;
                    break;
                case "idle":
                    toReply = "Idle";
                    userS = UserStatus.Idle;
                    break;
            }

            await (Context.Client as DiscordSocketClient).SetStatus(userS);

            await Context.Message.ModifyAsync(x =>
            {
                x.Content = $"Changed status to {toReply}.";
            });
        }

        [Command("user")]
        [Summary("Get the user information.")]
        public async Task User(IUser user = null)
        {
            SocketGuildUser gUser = user as SocketGuildUser ?? Context.User as SocketGuildUser;

            var sharedGuilds_ = Context.Client.GetGuildsAsync().Result;
            var sharedGuilds = sharedGuilds_.Where(x => x.GetUsersAsync().Result.Select(d => d.Id).Contains(gUser.Id));

            EmbedBuilder embed = new EmbedBuilder();
            embed.AddField(EmbedHelper.Field("Username", gUser.Username));
            embed.AddField(EmbedHelper.Field("ID", gUser.Id.ToString()));
            embed.AddField(EmbedHelper.Field("Discriminator", gUser.Discriminator));
            embed.AddField(EmbedHelper.Field("Member since", gUser.JoinedAt.Value.ToString()));
            embed.AddField(EmbedHelper.Field("Creation", gUser.CreatedAt.ToString()));
            embed.AddField(EmbedHelper.Field("Shared guilds", sharedGuilds.Count().ToString()));
            embed.AddField(EmbedHelper.Field("Roles", (gUser.RoleIds.Count() - 1).ToString()));
            embed.AddField(EmbedHelper.Field("Status", gUser.Status.ToString()));
            embed.ThumbnailUrl = gUser.AvatarUrl ?? "https://discordapp.com/assets/322c936a8c8be1b803cd94861bdfa868.png";
            embed.Color = new Color(20, 100, 180);

            await Context.Message.ModifyAsync(x =>
            {
                x.Content = $"";
                x.Embed = embed;
            });
        }

        [Command("ban")]
        [Summary("Bans a user.")]
        public async Task Ban(IUser user, int days = 0)
        {
            await Context.Guild.AddBanAsync(user, days);
        }

        [Command("kick")]
        [Summary("Kicks a user.")]
        public async Task Kick(IUser user)
        {
            await (user as SocketGuildUser).KickAsync();
        }

        [Command("findchannel")]
        public async Task FindChannel(string channel)
        {
            string toReply = "";
            foreach (IGuild guild in Context.Client.GetGuildsAsync().Result)
            {
                foreach (SocketTextChannel t in guild.GetTextChannelsAsync().Result)
                {
                    if (t.Name == channel)
                    {
                        toReply += $"{t.Mention} - {t.Guild.Name}\n";
                    }
                }
            }

            await Context.Message.ModifyAsync(x =>
            {
                x.Content = toReply;
            });
        }

        [Command("color")]
        [Summary("Make a color!")]
        public async Task Color(byte r, byte g, byte b)
        {
            EmbedBuilder embed = new EmbedBuilder();
            System.Drawing.Color c = System.Drawing.Color.FromArgb(r, g, b);
            embed.AddField(EmbedHelper.Field("Color Maker", $"R: {r}\nG: {g}\nB: {b}\n\u00ad\nHex:{ToHexString(c)}", false));
            embed.Color = new Color(r, g, b);
            await Context.Message.ModifyAsync(x =>
            {
                x.Content = $"";
                x.Embed = embed;
            });
        }

        [Command("guildowner")]
        public async Task GuildOwner()
        {
            var owner = Context.Client.GetUserAsync(Context.Guild.OwnerId).Result;
            await Context.Message.ModifyAsync(x =>
            {
                x.Content = $"`{owner.Username}#{owner.Discriminator}` owns `{Context.Guild.Name}`.";
            });
        }

        //[Command("eval")]
        //public async Task Eval([Remainder] string code)
        //{
        //    using (Noesis.Javascript.JavascriptContext jscontext = new Noesis.Javascript.JavascriptContext())
        //    {
        //        jscontext.SetParameter("Context", Context);
        //        jscontext.SetParameter("Client", Context.Client);
        //        jscontext.SetParameter("CurrentUser", Context.Client.CurrentUser);
        //        jscontext.SetParameter("Channel", Context.Channel);
        //        jscontext.SetParameter("Message", Context.Message);
        //        jscontext.SetParameter("Author", Context.Message.Author);
        //        jscontext.SetParameter("Console", new SystemConsole());
        //        jscontext.Run(code);
        //        //ReplyAsync(jscontext.GetParameter);
                
        //    }
        //}

        [Group("base64")]
        class Base64 : ModuleBase
        {
            [Command("decode")]
            [Alias("d")]
            [Remarks("base64")]
            public async Task Decode([Remainder] string text)
            {
                var base64EncodedBytes = Convert.FromBase64String(text);
                var s = Encoding.UTF8.GetString(base64EncodedBytes);
                await Context.Message.ModifyAsync(x =>
                {
                    x.Content = $"```\n{s}```";
                });
            }

            [Command("encode")]
            [Alias("e")]
            [Remarks("base64")]
            public async Task Encode([Remainder] string text)
            {
                var plainTextBytes = Encoding.UTF8.GetBytes(text);
                var s = Convert.ToBase64String(plainTextBytes);
                await Context.Message.ModifyAsync(x =>
                {
                    x.Content = $"```\n{s}```";
                });
            }
        }

        [Group("binary")]
        class Binary : ModuleBase
        {
            [Command("en")]
            [Remarks("binary")]
            public async Task Encode([Remainder] string text)
            {
                await Context.Message.ModifyAsync(x =>
                {
                    x.Content = $"```\n{ToBinary(ConvertToByteArray(text, Encoding.ASCII))}```";
                });
            }

            [Command("de")]
            [Remarks("binary")]
            public async Task Decode([Remainder] string text)
            {
                var data = GetBytesFromBinaryString(text);
                await Context.Message.ModifyAsync(x =>
                {
                    //x.Content = $"```\n{Encoding.ASCII.GetString(data)}```";
                    x.Content = $"```\n{Convert.ToBase64String(data)}```";
                });
            }

            public static byte[] GetBytesFromBinaryString(string binary)
            {
                var list = new List<byte>();

                for (int i = 0; i < binary.Length; i += 8)
                {
                    string t = binary.Substring(i, 8);

                    list.Add(Convert.ToByte(t, 2));
                }

                return list.ToArray();
            }

            public static string ToBinary(Byte[] data)
            {
                return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
            }

            public static byte[] ConvertToByteArray(string str, Encoding encoding)
            {
                return encoding.GetBytes(str);
            }
        }

        [Command("reset")]
        public async Task Reset()
        {
            await Context.Message.ModifyAsync(x =>
            {
                x.Content = $"```\n"+
"        \\          SORRY            /\n"+
"         \\                         /\n"+
"          \\    This reset does    /\n"+
"           ]   not exist yet.    [    ,'|\n"+
"           ]                     [   /  |\n"+
"           ]___               ___[ ,'   |\n"+
"           ]  ]\\             /[  [ |:   |\n"+
"           ]  ] \\           / [  [ |:   |\n"+
"           ]  ]  ]         [  [  [ |:   |\n"+
"           ]  ]  ]__     __[  [  [ |:   |\n"+
"           ]  ]  ] ]\\ _ /[ [  [  [ |:   |\n"+
"           ]  ]  ] ] (#) [ [  [  [ :===='\n"+
"           ]  ]  ]_].nHn.[_[  [  [\n"+
"           ]  ]  ]  HHHHH. [  [  [\n"+
"           ]  ] /   `HH(\"N  \\ [  [\n"+
"           ]__]/     HHH  \"  \\[__[\n"+
"           ]         NNN         [\n"+
"           ]         N/\"         [\n"+
"           ]         N H         [\n"+
"          /          N            \\\n"+
"         /           q,            \\\n"+
"        /                           \\\n"
                +"```";
            });
        }

        [Command("stats")]
        [Summary("Returns User bot stats.")]
        public async Task Stats()
        {
            //var application = await Context.Client.GetApplicationInfoAsync();
            Process proc = Process.GetCurrentProcess();

            EmbedBuilder embed = new EmbedBuilder();

            embed.Author = EmbedHelper.Author(Context.Client.CurrentUser, $"User Bot Stats | {Context.Client.CurrentUser.Username}#{Context.Client.CurrentUser.Discriminator}");
            embed.Description = $"**ID**: {Context.Client.CurrentUser.Id}\n\u00ad";
            embed.Color = new Color(59, 191, 232);
            embed.AddField(EmbedHelper.Field("Uptime:", GetUptime(), false));
            embed.AddField(EmbedHelper.Field("RAM:", proc.PrivateMemorySize64.ToString().Substring(0, proc.PrivateMemorySize64.ToString().Length - 6) + " MBs"));
            embed.AddField(EmbedHelper.Field("Discord.NET Version:", DiscordConfig.Version));
            embed.AddField(EmbedHelper.Field("Guilds:", (Context.Client as DiscordSocketClient).Guilds.Count.ToString()));
            embed.AddField(EmbedHelper.Field("Users:", (Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count()).ToString()));
            embed.AddField(EmbedHelper.Field("Text Channels:", (Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count(c => c is ITextChannel)).ToString()));
            embed.AddField(EmbedHelper.Field("Voice Channels", (Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count(c => c is IVoiceChannel)).ToString() + "\n\u00ad"));
            embed.Footer = EmbedHelper.Footer($"Requested by {Context.User.Username}", Context.User);

            //await Context.Message.DeleteAsync();
            //await ReplyAsync("", false, embed);
            await Context.Message.ModifyAsync(x =>
            {
                x.Content = $"";
                x.Embed = embed;
            });
        }

        private string GetCpuUsage()
        {
            PerformanceCounter cpuCounter;

            cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            return cpuCounter.NextValue() + "%";
        }
        private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetTimeUS()
            => (DateTime.Now).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();

        public string SpaceForUser(string s, int defaultNumber)
        {
            string toReturn = "";
            for (int i = s.Length; i < defaultNumber; i++)
            {
                toReturn += " ";
            }
            return toReturn;
        }
        public static string ToHexString(System.Drawing.Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }

    public class SystemConsole
    {
        public SystemConsole() { }

        public void Print(string iString)
        {
            Console.WriteLine(iString);
        }
    }
}
