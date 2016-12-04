using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SelfBot.Common.Helpers
{
    public static class EmbedHelper
    {
        public static EmbedAuthorBuilder Author(IUser u, string name = null)
        {
            EmbedAuthorBuilder aub = new EmbedAuthorBuilder();

            aub.Name = name;
            if (name == null) aub.Name = u.Username;
            aub.IconUrl = u.AvatarUrl;
            aub.Url = u.AvatarUrl;

            return aub;
        }

        public static EmbedFooterBuilder Footer(string text = null, string iconUrl = null)
        {
            EmbedFooterBuilder aub = new EmbedFooterBuilder();

            aub.Text = text;
            aub.IconUrl = iconUrl;

            return aub;
        }

        public static EmbedFooterBuilder Footer(string text = null, IUser user = null)
        {
            EmbedFooterBuilder aub = new EmbedFooterBuilder();

            aub.Text = text;
            aub.IconUrl = user.AvatarUrl;

            return aub;
        }

        //public static EmbedImage Image(string url)
        //{
        //    EmbedImageBuilder aub = new EmbedImageBuilder();

        //    aub.Url = url;

        //    return aub;
        //}

        //public static EmbedImageBuilder Image(IUser user)
        //{
        //    EmbedImageBuilder aub = new EmbedImageBuilder();

        //    aub.Url = user.AvatarUrl;

        //    return aub;
        //}

        //public static EmbedThumbnailBuilder Thumbnail(string url)
        //{
        //    EmbedThumbnailBuilder aub = new EmbedThumbnailBuilder();

        //    aub.Url = url;

        //    return aub;
        //}

        //public static EmbedThumbnailBuilder Thumbnail(IUser user)
        //{
        //    EmbedThumbnailBuilder aub = new EmbedThumbnailBuilder();

        //    aub.Url = user.AvatarUrl;

        //    return aub;
        //}

        public static Action<EmbedFieldBuilder> Field(string name, string value, bool isInline = true)
        {
            return x =>
            {
                x.Name = name;
                x.Value = value;
                x.IsInline = isInline;
            };
        }

        #region Geral Creation
        public static EmbedBuilder Build(IUser author, Color color, string footerText,
            IUser footerUser, IUser imageUser, IUser thumbnailUser, string title, IUser avatarUser, string description,
            Action<EmbedFieldBuilder>[] fields)
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.Author = Author(author);
            embed.Color = color;
            embed.Footer = Footer(footerText, footerUser);
            //embed.Image = Image(imageUser);
            //embed.Thumbnail = Thumbnail(thumbnailUser);
            embed.Title = title;
            embed.Url = avatarUser.AvatarUrl;
            embed.Description = description;
            foreach (Action<EmbedFieldBuilder> emb in fields)
            {
                embed.AddField(emb);
            }

            return embed;
        }

        public static EmbedBuilder Build(CommandContext Context, Color color, string footerText, string title,
            Action<EmbedFieldBuilder>[] fields)
        {
            EmbedBuilder embed = new EmbedBuilder();

            embed.Author = Author(Context.User);
            embed.Color = color;
            embed.Footer = Footer(footerText, Context.User);
            embed.Title = title;
            foreach (Action<EmbedFieldBuilder> emb in fields)
            {
                embed.AddField(emb);
            }

            return embed;
        }

        public static EmbedBuilder Build(CommandContext Context)
        {
            EmbedBuilder embed = new EmbedBuilder();



            return embed;
        }
        #endregion
    }
}
