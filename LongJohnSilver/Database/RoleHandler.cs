using System;
using System.Collections.Generic;
using System.Text;

namespace LongJohnSilver.Database
{
    public class RoleHandler
    {
        private List<ChannelRole> ChannelRoles = new List<ChannelRole>();
        private readonly IDatabase MainDb;

        public RoleHandler(IDatabase db)
        {
            MainDb = db;
            GetDataFromDB();
        }

        private void GetDataFromDB()
        {
            ChannelRoles = MainDb.GetAllChannelRoles();
        }
    }
}
