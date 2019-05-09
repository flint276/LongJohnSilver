using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace LongJohnSilver.Database
{
    public class RoleHandler
    {
        private List<ChannelRole> ChannelRoles = new List<ChannelRole>();
        private readonly IDatabase MainDb;

        private readonly string[] Roles = {"gaming", "knockout", "draft"};

        public RoleHandler(IDatabase db)
        {
            MainDb = db;
            GetDataFromDb();
        }

        private void GetDataFromDb()
        {
            ChannelRoles = MainDb.GetAllChannelRoles();
        }

        public string GetRoleForChannel(ulong channelIdUlong)
        {
            var role = ChannelRoles.Find(x => x.Channel == channelIdUlong.ToString());

            return role == null ? "general" : role.Role;
        }

        public bool SetRoleForChannel(ulong channelIdUlong, string role)
        {
            if (!IsValidRole(role)) return false;

            MainDb.SetRoleForChannel(channelIdUlong.ToString(), role.ToLower());
            GetDataFromDb();
            return true;
        }

        public void ClearRoleForChannel(ulong channelIdUlong)
        {
            MainDb.CleanRoleFromChannel(channelIdUlong.ToString());
            GetDataFromDb();            
        }

        public Dictionary<ulong, string> GetChannelRoles(List<ulong> channels)
        {
            var channelDict = new Dictionary<ulong, string>();

            foreach (var channel in channels)
            {
                var role = GetRoleForChannel(channel);
                channelDict.Add(channel, role);
            }

            return channelDict;
        }

        public bool IsValidRole(string role)
        {
            return Roles.Contains(role.ToLower());
        }
    }
}
