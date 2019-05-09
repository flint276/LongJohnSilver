using Discord.Commands;
using LongJohnSilver.Database.DataMethodsGaming;
using LongJohnSilver.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LongJohnSilver.Embeds;
using LongJohnSilver.Enums;
using LongJohnSilver.Extensions;

namespace LongJohnSilver.Commands.Gaming
{
    public class ShowGamerTags : ModuleBase<SocketCommandContext>
    {
        [Command("showgamertags")]
        public async Task ShowGamerTagsAsync([Remainder] string input = "")
        {
            var gModel = GamingModel.ForGuild(Context.Guild.Id);

            if (Context.Channel.Role() != ChannelRoles.Gaming || Context.IsPrivate)
            {
                return;
            }

            if (input == "")
            {
                await Context.Channel.SendMessageAsync(":x: Please search either by username or service in the format !showgamertags <user/service>");
                return;
            }

            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();                                   

            if (mentionedUser != null)
            {
                var userId = mentionedUser.Id;

                var serviceArray = gModel.GetTagsForUser(userId);

                var body = "";

                foreach (var kvp in serviceArray)
                {
                    if (kvp.Value.Contains("__"))
                    {
                        var output = kvp.Value.Replace("__", @"\__");
                        body += $"**{kvp.Key}** : {output}\n";
                    }
                    else
                    {
                        body += $"**{kvp.Key}** : {kvp.Value}\n";
                    }
                }

                var username = Context.Client.GetUser(userId)?.Username;
                username = Context.Guild.GetUser(userId)?.Nickname ?? username;

                var embed = GamerTagEmbeds.GamerTagEmbed($"Gamertags for {username}", body);
                await Context.Channel.SendMessageAsync("", false, embed.Build());

                return;
            }
                        
            if (gModel.IsValidService(input))
            {
                var userArray = gModel.GetTagsForService(input);
                var body = "";

                foreach (var kvp in userArray)
                {
                    var username = Context.Client.GetUser(kvp.Key)?.Username;

                    if (username == null)
                    {
                        continue;
                    }

                    username = Context.Guild.GetUser(kvp.Key)?.Nickname ?? username;

                    if (kvp.Value.Contains("__"))
                    {
                        var output = kvp.Value.Replace("__", @"\__");
                        body += $"**{username}** : {output}\n";
                    }
                    else
                    {
                        body += $"**{username}** : {kvp.Value}\n";
                    }                   
                }

                var embed = GamerTagEmbeds.GamerTagEmbed($"Gamertags for {input}", body);
                await Context.Channel.SendMessageAsync("", false, embed.Build());

                return;
            }

            var userlist = Context.Guild.Users;

            foreach (var u in userlist)
            {
                var userNick = u.Nickname?.ToLower() ?? "";
                var userName = u.Username?.ToLower() ?? "";

                if (userNick.Contains(input.ToLower()) || userName.Contains(input.ToLower()))
                {
                    var nameToDisplay = userNick == "" ? u.Username : u.Nickname;
                    var title = $"Gamertags for {nameToDisplay}";

                    var userId = u.Id;

                    var serviceArray = gModel.GetTagsForUser(userId);

                    var body = "";

                    foreach (var kvp in serviceArray)
                    {
                        if (kvp.Value.Contains("__"))
                        {
                            var output = kvp.Value.Replace("__", @"\__");
                            body += $"**{kvp.Key}** : {output}\n";
                        }
                        else
                        {
                            body += $"**{kvp.Key}** : {kvp.Value}\n";
                        }
                        
                    }

                    var embed = GamerTagEmbeds.GamerTagEmbed(title, body);
                    await Context.Channel.SendMessageAsync("", false, embed.Build());


                    return;
                }
            }

            await Context.Channel.SendMessageAsync(":x: Please search either by username or service in the format !showgamertags <user/service>");


        }
    }
}
