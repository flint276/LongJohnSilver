using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using LongJohnSilver.Database;
using LongJohnSilver.Enums;
using LongJohnSilver.Statics;

namespace LongJohnSilver.Extensions
{
    public static class ISocketMessageChannelExtension
    {
        public static ChannelRoles Role(this ISocketMessageChannel channel)
        {
            var roleHandler = new RoleHandler(Factory.GetDatabase());
            switch (roleHandler.GetRoleForChannel(channel.Id))
            {
                case "knockout":
                    return ChannelRoles.Knockout;
                case "draft":
                    return ChannelRoles.Draft;
                case "gaming":
                    return ChannelRoles.Gaming;
                case "general":
                    return ChannelRoles.General;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
