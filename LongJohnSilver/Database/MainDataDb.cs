using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

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
        public int CurrentVersion = 3;

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

            if (CurrentVersion > GetVersion())
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
        #endregion SQL Comman Methods

        #region Data Download Methods

        /// <summary>
        /// Get all Contenders from Database and return as a List of Contender Type
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public List<Contender> GetAllContenders(string channelId)
        {
            var contenderList = new List<Contender>();

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                var sql = $"SELECT * FROM contenders WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rName = (string)reader["name"];
                            var rScore = (int)reader["score"];
                            var killer = (string)reader["killer"];
                            var epitaph = (string)reader["epitaph"];
                            contenderList.Add(new Contender { Name = rName, Score = rScore, Killer = killer, Epitaph = epitaph });
                        }
                    }
                }
            }

            return contenderList;
        }

        /// <summary>
        /// Get all Knockouts from Database and return as a List of Knockout Type (note this should be a single entry for now)
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public List<Knockout> GetAllKnockouts(string channelId)
        {
            List<Knockout> knockoutList = new List<Knockout>();

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                var sql = $"SELECT * FROM knockout WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string rname = (string)reader["name"];
                            int rstatus = (int)reader["status"];
                            string rowner = (string)reader["owner"];
                            string rchannel = (string)reader["channel"];

                            knockoutList.Add(new Knockout { Name = rname, Status = rstatus, Owner = rowner, Channel = rchannel });
                        }
                    }
                }
            }

            return knockoutList;
        }

        public List<Knockout> GetAllKnockouts()
        {
            List<Knockout> knockoutList = new List<Knockout>();

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                var sql = $"SELECT * FROM knockout";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string rname = (string)reader["name"];
                            int rstatus = (int)reader["status"];
                            string rowner = (string)reader["owner"];
                            string rchannel = (string)reader["channel"];

                            knockoutList.Add(new Knockout { Name = rname, Status = rstatus, Owner = rowner, Channel = rchannel });
                        }
                    }
                }
            }

            return knockoutList;
        }

        /// <summary>
        /// Get all Players from Database and return as a List of KPlayer type
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public List<KPlayer> GetAllPlayers(string channelId)
        {
            List<KPlayer> playerList = new List<KPlayer>();

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                var sql = $"SELECT * FROM kplayers WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string rid = (string)reader["playerid"];
                            int rturnsleft = (int)reader["turnsleft"];
                            int rlastplayed = (int)reader["lastplayed"];

                            playerList.Add(new KPlayer { PlayerId = rid, TurnsLeft = rturnsleft, LastPlayed = rlastplayed });
                        }
                    }
                }
            }

            return playerList;
        }

        /// <summary>
        /// Return the version number from the database
        /// </summary>
        /// <returns></returns>
        public int GetVersion()
        {
            var versionNumber = 0;

            using (var db = new SQLiteConnection(DbSource))
            {
                db.Open();

                var sql = $"SELECT * FROM version";
                using (var command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            versionNumber = (int)reader["ver"];
                        }
                    }
                }
            }

            return versionNumber;
        }



        #endregion Data Download Methods

        #region Knockout Methods    
        
        /// <summary>
        /// Delete the contents of the main database associated with a channel id
        /// </summary>
        /// <param name="channelId"></param>
        public void EmptyKnockoutDatabase(string channelId)
        {
            object[] parameters = { channelId };

            RunQuery("DELETE FROM knockout where channel = @param1", parameters);
            RunQuery("DELETE FROM contenders where channel = @param1", parameters);
            RunQuery("DELETE FROM kplayers where channel = @param1", parameters);
        }

        /// <summary>
        /// Create a new knockout entry 'Under Construction' and set status. Assign creating user and channel ids.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="channelId"></param>
        public void CreateNewKnockout(string userId, string channelId)
        {
            object[] deleteParameters = {channelId};
            object[] insertParameters = {userId, channelId};

            RunQuery("DELETE FROM knockout WHERE channel = @param1", deleteParameters);
            RunQuery("INSERT INTO knockout (name, status, owner, channel) VALUES ('Knockout Under Construction', 4, @param1, @param2)", insertParameters);
        }

        /// <summary>
        /// Update the name of Knockout to parameter
        /// </summary>
        /// <param name="knockoutTitle"></param>
        /// <param name="channelId"></param>
        public void AddKnockoutTitle(string knockoutTitle, string channelId)
        {
            object[] parameters = {knockoutTitle, channelId};
            RunQuery("UPDATE knockout SET name = @param1 where channel = @param2", parameters);
        }

        /// <summary>
        /// Insert a new Contender into the Contender table titled from parameter and assign score of 3
        /// </summary>
        /// <param name="contenderName"></param>
        /// /// <param name="channelId"></param>
        public void AddContender(string contenderName, string channelId)
        {
            object[] parameters = { contenderName, channelId };
            RunQuery("INSERT INTO contenders (name, score, killer, epitaph, channel) VALUES (@param1, 3, '', '', @param2)", parameters);
        }

        /// <summary>
        /// Delete all rows in contender where name equals parameter
        /// </summary>
        /// <param name="contenderName"></param>
        /// <param name="channelId"></param>
        public void RemoveContender(string contenderName, string channelId)
        {
            object[] parameters = {contenderName, channelId};
            RunQuery("DELETE FROM contenders WHERE name = @param1 AND channel = @param2", parameters);
        }

        //
        // DATA CLEANUP
        //

        /// <summary>
        /// Clear all tables of data and rebuild knockoutinfo to default
        /// </summary>
        public void ResetAllTables(string channelId)
        {
            ResetKnockoutTable(channelId);
            ResetContenderTable(channelId);
            ResetPlayersTable(channelId);
        }

        /// <summary>
        /// Clear knockout table of data and rebuild to default data
        /// </summary>
        public void ResetKnockoutTable(string channelId)
        {
            object[] parameters = {channelId};
            RunQuery("DELETE FROM knockout WHERE channel = @param1", parameters);
            RunQuery("INSERT INTO knockout(name, status, owner, channel) values ('No Knockout In Progress', 1, '0', @param1)", parameters);
        }

        /// <summary>
        /// Clear contender table of data
        /// </summary>
        public void ResetContenderTable(string channelId)
        {
            object[] parameters = {channelId};
            RunQuery("DELETE FROM contenders WHERE channel = @param1", parameters);
        }

        /// <summary>
        /// Clear kplayers table of data
        /// </summary>
        public void ResetPlayersTable(string channelId)
        {
            object[] parameters = {channelId};
            RunQuery("DELETE FROM kplayers WHERE channel = @param1", parameters);
        }

        /// <summary>
        /// Resets all the player turns and lastplayed values to daily start values
        /// </summary>
        public void NewDay(string channelId)
        {
            object[] parameters = {channelId};
            RunQuery("UPDATE kplayers SET lastplayed = 0, turnsleft = 3 WHERE channel = @param1", parameters);
        }

        /// <summary>
        /// Resets all player turns and lastplayed values to daily start values
        /// </summary>
        public void NewDayForAll()
        {
            RunQuery("UPDATE kplayers SET lastplayed = 0, turnsleft = 3");
        }

        /// <summary>
        /// Change the knockout status to 2, meaning an active knockout
        /// </summary>
        public void SetKnockoutToActive(string channelId)
        {
            object[] parameters = { channelId };
            RunQuery("UPDATE knockout SET status = 2 WHERE channel = @param1", parameters);            
        }

        /// <summary>
        /// Update the contender table by adding the modifier to a selected row's score
        /// </summary>
        /// <param name="contenderName"></param>
        /// <param name="modifier"></param>
        /// <param name="channelId"></param>
        public void ChangeScore(string contenderName, int modifier, string channelId)
        {
            object[] parameters = {modifier, contenderName, channelId};
            RunQuery("UPDATE contenders SET score = score + @param1 WHERE name = @param2 AND channel = @param3", parameters);
        }

        /// <summary>
        /// Add a new player to the knockout. Even though they've most likely just gone their turns are still 3 and lastplayed 0. These values should be set by the calling method.
        /// </summary>
        /// <param name="userIdString"></param>
        /// <param name="channelId"></param>
        public void AddPlayerToKnockout(string userIdString, string channelId)
        {
            object[] parameters = {userIdString, channelId};
            RunQuery("INSERT INTO kplayers (playerid, turnsleft, lastplayed, channel) VALUES (@param1, 3, 0, @param2)", parameters);
        }

        /// <summary>
        /// Clear kplayers table so all players are set to lastplayed of 0.
        /// </summary>
        public void ResetAllPlayersLastPlayedStatus(string channelId)
        {
            object[] parameters = {channelId};
            RunQuery("UPDATE kplayers SET lastplayed = 0 WHERE lastplayed = 1 AND channel = @param1", parameters);
            RunQuery("UPDATE kplayers SET lastplayed = 1 WHERE lastplayed = 2 AND channel = @param1", parameters);
        }

        /// <summary>
        /// Deduct nominated kplayer row turnplayed by 1 and set lastplayed to 1
        /// </summary>
        /// <param name="userIdString"></param>
        /// <param name="channelId"></param>
        public void RegisterPlayersTurn(string userIdString, string channelId)
        {
            object[] parameters = {userIdString, channelId};
            RunQuery("UPDATE kplayers SET turnsleft = turnsleft - 1 WHERE playerid = @param1 AND channel = @param2", parameters);
            RunQuery("UPDATE kplayers SET lastplayed = 2 WHERE playerid = @param1 AND channel = @param2", parameters);
        }

        /// <summary>
        /// Set a contenders score outright
        /// </summary>
        /// <param name="value"></param>
        /// <param name="contenderName"></param>
        /// <param name="channelId"></param>
        public void SetScoreForContender(int value, string contenderName, string channelId)
        {
            object[] parameters = {value, contenderName, channelId};
            RunQuery("UPDATE contenders SET score = @param1 WHERE name = @param2 AND channel = @param3", parameters);            
        }

        /// <summary>
        /// set the killer of a contender
        /// </summary>
        /// <param name="userIdString"></param>
        /// <param name="contenderName"></param>
        /// <param name="channelId"></param>
        public void SetKillerForContender(string userIdString, string contenderName, string channelId)
        {
            object[] clearParameters = {userIdString, channelId};
            object[] setParameters = {userIdString, contenderName, channelId};

            RunQuery("UPDATE contenders SET killer = '' WHERE killer = @param1 AND channel = @param2", clearParameters);
            RunQuery("UPDATE contenders SET killer = @param1 WHERE name = @param2 AND channel = @param3", setParameters);            
        }

        /// <summary>
        /// Set the Epitaph of a Contender
        /// </summary>
        /// <param name="contenderName"></param>
        /// <param name="epitaph"></param>
        /// <param name="channelId"></param>
        public void SetEpitaphForContender(string contenderName, string epitaph, string channelId)
        {
            object[] parameters = {epitaph, contenderName, channelId};
            RunQuery("UPDATE contenders SET epitaph = @param1 WHERE name = @param2 AND channel = @param3", parameters);
        }

        /// <summary>
        /// Set the status of knockoutinfo to 3 (ended)
        /// </summary>
        public void SetKnockoutToEnded(string channelId)
        {
            object[] parameters = {channelId};
            RunQuery("UPDATE knockout SET status = 3 WHERE channel = @param1", parameters);
        }

        #endregion Knockout Methods

        #region Database Creation Method

        public void CreateDatabase()
        {
            // Create Tables
            RunQuery($"CREATE TABLE IF NOT EXISTS contenders(id INT PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS knockout(id INT PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS kplayers(id INT PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS channelroles(id INT PRIMARY KEY)");
            RunQuery($"CREATE TABLE IF NOT EXISTS version(ver INT)");

            // Populate Columns, catch and ignore any SQL errors as they are just advising the column already exists
            try
            {
                RunQuery($"ALTER TABLE contenders ADD name VARCHAR(200)");
                RunQuery($"ALTER TABLE contenders ADD score INT");
                RunQuery($"ALTER TABLE contenders ADD killer VARCHAR(50)");
                RunQuery($"ALTER TABLE contenders ADD epitaph VARCHAR(200)");
                RunQuery($"ALTER TABLE contenders ADD channel VARCHAR(50)");

                RunQuery($"ALTER TABLE knockout ADD name VARCHAR(200)");
                RunQuery($"ALTER TABLE knockout ADD status INT");
                RunQuery($"ALTER TABLE knockout ADD owner VARCHAR(50)");
                RunQuery($"ALTER TABLE knockout ADD channel VARCHAR(50)");

                RunQuery($"ALTER TABLE kplayers ADD playerid VARCHAR(30)");
                RunQuery($"ALTER TABLE kplayers ADD turnsleft INT");
                RunQuery($"ALTER TABLE kplayers ADD lastplayed INT");
                RunQuery($"ALTER TABLE kplayers ADD channel VARCHAR(50)");

                RunQuery($"ALTER TABLE channelroles ADD channel VARCHAR(50)");
                RunQuery($"ALTER TABLE channelroles ADD role VARCHAR(20)");
            }
            catch (SQLiteException)
            {                
            }
                        
            // Update Version
            RunQuery($"DELETE FROM version");
            RunQuery($"INSERT INTO version (ver) VALUES ({CurrentVersion})");
        }

#endregion Database Creation Method
    }
}
