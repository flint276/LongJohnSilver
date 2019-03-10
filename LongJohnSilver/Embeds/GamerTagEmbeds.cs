using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Discord;

namespace LongJohnSilver.Embeds
{
    public static class GamerTagEmbeds
    {
        public static EmbedBuilder GamerTagEmbed(string title, string body)
        {
            var embed = new EmbedBuilder();

            embed.WithAuthor(title);
            embed.WithColor(40, 200, 150);
            embed.WithDescription(body);

            return embed;
        }

    }
}
