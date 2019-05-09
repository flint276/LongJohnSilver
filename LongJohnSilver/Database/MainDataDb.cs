using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace LongJohnSilver.Database
{
    /// <summary>
    /// Knockout SQLite Database Handler
    /// </summary>
    public class MainDataDb : IDatabase
    {
        private static readonly string CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public string DbLocation = $@"{CurrentDirectory}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}";
        public string DbPath;
        public string DbSource;
        public int CurrentVersion = 9;

        /// <summary>
        /// Constructor, creates database if not present
        /// </summary>
        public MainDataDb()
        {

            DbPath = $"{DbLocation}maindata.db";
            DbSource = $"Data Source={DbPath}";

            if (!Directory.Exists(DbLocation))
            {
                Directory.CreateDirectory(DbLocation);
            }

            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
                CreateDatabase();                
            }

            var versionData = GetData("SELECT * FROM version");
            var version = (int)versionData.First()["ver"];

            if (CurrentVersion > version)
            {
                CreateDatabase();
            }
        }

        #region SQL Command Methods
        /// <summary>
        /// Run a simple, param free statement
        /// </summary>
        /// <param name="sql"></param>
        public void RunQuery(string sql)
        {
            using (var db = new SQLiteConnection(DbSource))
            {
                db.Open();

                using (var command = new SQLiteCommand(sql, db))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RunUnsafeQuery(string sql)
        {
            using (var db = new SQLiteConnection(DbSource))
            {
                db.Open();

                using (var command = new SQLiteCommand(sql, db))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SQLiteException)
                    {
                        Console.WriteLine("SQL Error");
                    }
                    
                }
            }
        }        

        /// <summary>
        /// Run a simple statement with parameters
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public void RunQuery(string sql, object[] parameters)
        {
            // Validate Parameters
            var paramNumber = 0;
            foreach (var unused in parameters)
            {
                paramNumber += 1;
                if (!sql.Contains($"@param{paramNumber}"))
                {
                    throw new Exception("Corrupt SQL Parameters");
                }
            }

            paramNumber = 0;

            using (var db = new SQLiteConnection(DbSource))
            {
                db.Open();

                using (var command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    foreach (var s in parameters)
                    {
                        paramNumber += 1;
                        command.Parameters.Add(new SQLiteParameter($"@param{paramNumber}", s));
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Dictionary<string, object>> GetData(string sql)
        {
            var dataResults = new List<Dictionary<string, object>>();

            using (var db = new SQLiteConnection(DbSource))
            {
                db.Open();

                using (var command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dataDictionary = new Dictionary<string, object>();

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var n = reader.GetName(i);
                                var v = reader.GetValue(i);

                                dataDictionary[n] = v;
                            }

                            dataResults.Add(dataDictionary);
                        }
                    }
                }
            }

            return dataResults;
        }

        public List<Dictionary<string, object>> GetData(string sql, object[] parameters)
        {
            var dataResults = new List<Dictionary<string, object>>();
            
            // Validate Parameters
            var paramNumber = 0;
            foreach (var unused in parameters)
            {
                paramNumber += 1;
                if (!sql.Contains($"@param{paramNumber}"))
                {
                    throw new Exception("Corrupt SQL Parameters");
                }
            }

            paramNumber = 0;

            using (var db = new SQLiteConnection(DbSource))
            {
                db.Open();

                using (var command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    foreach (var s in parameters)
                    {
                        paramNumber += 1;
                        command.Parameters.Add(new SQLiteParameter($"@param{paramNumber}", s));
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dataDictionary = new Dictionary<string, object>();

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var n = reader.GetName(i);
                                var v = reader.GetValue(i);

                                dataDictionary[n] = v;
                            }

                            dataResults.Add(dataDictionary);
                        }
                    }
                }
            }

            return dataResults;
        }
        #endregion SQL Command Methods

        #region Data Download Methods

        /// <summary>
        /// Return the version number from the database
        /// </summary>
        /// <returns></returns>
        public int GetVersion()
        {
            var versionData = GetData("SELECT * FROM version");
            var versionNumber = (int)versionData.First()["ver"];

            return versionNumber;
        }

        public List<ChannelRole> GetAllChannelRoles()
        {
            var channelRoles = new List<ChannelRole>();
            
            using (var db = new SQLiteConnection(DbSource))
            {
                db.Open();

                var sql = $"SELECT * FROM channelroles";
                using (var command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var channel = (string)reader["channel"];
                            var role = (string)reader["role"];

                            channelRoles.Add(new ChannelRole{ Channel = channel, Role = role });
                        }
                    }
                }
            }

            return channelRoles;
        }



        #endregion Data Download Methods

        #region Role Methods

        /// <summary>
        /// Remove existing role for channel and add selected new one
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="role"></param>
        public void SetRoleForChannel(string channelId, string role)
        {
            object[] deleteParameters = {channelId};
            object[] insertParameters = {channelId, role};
            RunQuery("DELETE FROM channelroles WHERE channel = @param1", deleteParameters);
            RunQuery("INSERT INTO channelroles (channel, role) VALUES (@param1, @param2)", insertParameters);
        }

        /// <summary>
        /// Remove role from channel
        /// </summary>
        /// <param name="channelId"></param>
        public void CleanRoleFromChannel(string channelId)
        {
            object[] parameters = {channelId};
            RunQuery("DELETE FROM channelroles WHERE channel = @param1", parameters);
        }

        #endregion Role Methods
        
        #region Database Creation Method

        public void CreateDatabase()
        {
            // Create Tables
            RunQuery($"CREATE TABLE IF NOT EXISTS contenders(id INTEGER PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS knockout(id INTEGER PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS kplayers(id INTEGER PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS channelroles(id INTEGER PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS gamertags(id INTEGER PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS version(id INTEGER PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS draftgames(id INTEGER PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS draftslots(id INTEGER PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS draftchoices(id INTEGER PRIMARY KEY)");


            // Populate Columns, catch and ignore any SQL errors as they are just advising the column already exists
            RunUnsafeQuery($"ALTER TABLE contenders ADD name VARCHAR(200)");
            RunUnsafeQuery($"ALTER TABLE contenders ADD score INT");
            RunUnsafeQuery($"ALTER TABLE contenders ADD killer VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE contenders ADD epitaph VARCHAR(200)");
            RunUnsafeQuery($"ALTER TABLE contenders ADD channel VARCHAR(50)");

            RunUnsafeQuery($"ALTER TABLE knockout ADD name VARCHAR(200)");
            RunUnsafeQuery($"ALTER TABLE knockout ADD status INT");
            RunUnsafeQuery($"ALTER TABLE knockout ADD owner VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE knockout ADD channel VARCHAR(50)");

            RunUnsafeQuery($"ALTER TABLE kplayers ADD playerid VARCHAR(30)");
            RunUnsafeQuery($"ALTER TABLE kplayers ADD turnsleft INT");
            RunUnsafeQuery($"ALTER TABLE kplayers ADD lastplayed INT");
            RunUnsafeQuery($"ALTER TABLE kplayers ADD channel VARCHAR(50)");

            RunUnsafeQuery($"ALTER TABLE channelroles ADD channel VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE channelroles ADD role VARCHAR(20)");

            RunUnsafeQuery($"ALTER TABLE gamertags ADD guild VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE gamertags ADD user VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE gamertags ADD service VARCHAR(20)");
            RunUnsafeQuery($"ALTER TABLE gamertags ADD tag VARCHAR(200)");

            RunUnsafeQuery($"ALTER TABLE version ADD ver INT");

            RunUnsafeQuery($"ALTER TABLE draftgames ADD channel VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE draftgames ADD owner VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE draftgames ADD status INT");
            RunUnsafeQuery($"ALTER TABLE draftgames ADD title VARCHAR(200)");
            RunUnsafeQuery($"ALTER TABLE draftgames ADD description VARCHAR(1000)");
            RunUnsafeQuery($"ALTER TABLE draftgames ADD starttime INT");
            RunUnsafeQuery($"ALTER TABLE draftgames ADD days INT");

            RunUnsafeQuery($"ALTER TABLE draftslots ADD channel VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE draftslots ADD playerid VARCHAR(30)");
            RunUnsafeQuery($"ALTER TABLE draftslots ADD day INT");
            RunUnsafeQuery($"ALTER TABLE draftslots ADD slot INT");

            RunUnsafeQuery($"ALTER TABLE draftchoices ADD channel VARCHAR(50)");
            RunUnsafeQuery($"ALTER TABLE draftchoices ADD playerid VARCHAR(30)");
            RunUnsafeQuery($"ALTER TABLE draftchoices ADD name VARCHAR(200)");
            

                                                                      
            // Update Version
            RunQuery($"DELETE FROM version");
            RunQuery($"INSERT INTO version (ver) VALUES ({CurrentVersion})");
        }

#endregion Database Creation Method
    }
}
