﻿using System.Collections.Generic;

namespace LongJohnSilver.Database
{
    public interface IDatabase
    {
        void CleanRoleFromChannel(string channelId);
        void CreateDatabase();
        List<ChannelRole> GetAllChannelRoles();
        List<Dictionary<string, object>> GetData(string sql);
        List<Dictionary<string, object>> GetData(string sql, object[] parameters);
        int GetVersion();
        void RunQuery(string sql);
        void RunQuery(string sql, object[] parameters);
        void RunUnsafeQuery(string sql);     
        void SetRoleForChannel(string channelId, string role);      
    }
}