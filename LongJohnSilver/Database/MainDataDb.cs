using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
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

        /// <summary>
        /// Constructor, creates database if not present
        /// </summary>
        /// <param name="channelId"> Discord Channel Id which forms name of DB</param>
        public MainDataDb()
        {

            DbPath = $"{DbLocation}maindata.db";
            DbSource = $"Data Source={DbPath}";

            if (!File.Exists(DbPath))
            {
                throw new InvalidOperationException("Database File is missing!");
            }
        }

        /// <summary>
        /// Delete the contents of the main database associated with a channel id
        /// </summary>
        /// <param name="channelId"></param>
        public void EmptyKnockoutDatabase(string channelId)
        {
            var sql = "";
          
            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"DELETE FROM knockout where channel = @param1";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }

                sql = $"DELETE FROM contenders where channel = @param1";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }

                sql = $"DELETE FROM kplayers where channel = @param1";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }
            }               
        }

        /// <summary>
        /// Get all Contenders from Database and return as a List of Contender Type
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public List<Contender> GetAllContenders(string channelId)
        {
            string sql = "";
            var contenderList = new List<Contender>();

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"SELECT * FROM contenders WHERE channel = @param1";
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
            string sql = "";
            List<Knockout> knockoutList = new List<Knockout>();

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"SELECT * FROM knockout WHERE channel = @param1";
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
            string sql = "";
            List<Knockout> knockoutList = new List<Knockout>();

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"SELECT * FROM knockout";
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
            string sql = "";
            List<KPlayer> playerList = new List<KPlayer>();

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"SELECT * FROM kplayers WHERE channel = @param1";
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

        //
        //  KNOCKOUT CREATION METHODS          
        //

        /// <summary>
        /// Create a new knockout entry 'Under Construction' and set status. Assign creating user and channel ids.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="channelId"></param>
        public void CreateNewKnockout(string userId, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"DELETE FROM knockout WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }

                sql = $"INSERT INTO knockout (name, status, owner, channel) VALUES ('Knockout Under Construction', 4, @param1, @param2)";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", userId));
                    command.Parameters.Add(new SQLiteParameter("@param2", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Update the name of Knockout to parameter
        /// </summary>
        /// <param name="knockoutTitle"></param>
        public void AddKnockoutTitle(string knockoutTitle, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE knockout SET name = @param1 where channel = @param2";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", knockoutTitle));
                    command.Parameters.Add(new SQLiteParameter("@param2", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Insert a new Contender into the Contender table titled from parameter and assign score of 3
        /// </summary>
        /// <param name="contenderName"></param>
        public void AddContender(string contenderName, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"INSERT INTO contenders (name, score, killer, epitaph, channel) VALUES (@param1, 3, '', '', @param2)";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", contenderName));
                    command.Parameters.Add(new SQLiteParameter("@param2", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Delete all rows in contender where name equals parameter
        /// </summary>
        /// <param name="contenderName"></param>
        /// <param name="channelId"></param>
        public void RemoveContender(string contenderName, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"DELETE FROM contenders WHERE name = @param1 AND channel = @param2";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", contenderName));
                    command.Parameters.Add(new SQLiteParameter("@param2", channelId));
                    command.ExecuteNonQuery();
                }
            }
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
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"DELETE FROM knockout WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }

                sql = "INSERT INTO knockout(name, status, owner, channel) values ('No Knockout In Progress', 1, '0', @param1)";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }


            }
        }

        /// <summary>
        /// Clear contender table of data
        /// </summary>
        public void ResetContenderTable(string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"DELETE FROM contenders WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Clear kplayers table of data
        /// </summary>
        public void ResetPlayersTable(string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"DELETE FROM kplayers WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Resets all the player turns and lastplayed values to daily start values
        /// </summary>
        public void NewDay(string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE kplayers SET lastplayed = 0, turnsleft = 3 WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }


            }
        }

        /// <summary>
        /// Resets all player turns and lastplayed values to daily start values
        /// </summary>
        public void NewDayForAll()
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE kplayers SET lastplayed = 0, turnsleft = 3";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.ExecuteNonQuery();
                }


            }
        }

        /// <summary>
        /// Change the knockout status to 2, meaning an active knockout
        /// </summary>
        public void SetKnockoutToActive(string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE knockout SET status = 2 WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }


            }
        }

        /// <summary>
        /// Update the contender table by adding the modifier to a selected row's score
        /// </summary>
        /// <param name="contenderName"></param>
        /// <param name="modifier"></param>
        public void ChangeScore(string contenderName, int modifier, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE contenders SET score = score + @param1 WHERE name = @param2 AND channel = @param3";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", modifier));
                    command.Parameters.Add(new SQLiteParameter("@param2", contenderName));                    
                    command.Parameters.Add(new SQLiteParameter("@param3", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Add a new player to the knockout. Even though they've most likely just gone their turns are still 3 and lastplayed 0. These values should be set by the calling method.
        /// </summary>
        /// <param name="userIdString"></param>
        public void AddPlayerToKnockout(string userIdString, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"INSERT INTO kplayers (playerid, turnsleft, lastplayed, channel) VALUES (@param1, 3, 0, @param2)";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", userIdString));                    
                    command.Parameters.Add(new SQLiteParameter("@param2", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Clear kplayers table so all players are set to lastplayed of 0.
        /// </summary>
        public void ResetAllPlayersLastPlayedStatus(string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE kplayers SET lastplayed = 0 WHERE lastplayed = 1 AND channel = @param1";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }

                sql = $"UPDATE kplayers SET lastplayed = 1 WHERE lastplayed = 2 AND channel = @param1";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Deduct nominated kplayer row turnplayed by 1 and set lastplayed to 1
        /// </summary>
        /// <param name="userIdString"></param>
        public void RegisterPlayersTurn(string userIdString, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE kplayers SET turnsleft = turnsleft - 1 WHERE playerid = @param1 AND channel = @param2";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", userIdString));                    
                    command.Parameters.Add(new SQLiteParameter("@param2", channelId));
                    command.ExecuteNonQuery();
                }

                sql = $"UPDATE kplayers SET lastplayed = 2 WHERE playerid = @param1 AND channel = @param2";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", userIdString));                    
                    command.Parameters.Add(new SQLiteParameter("@param2", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Set a contenders score outright
        /// </summary>
        /// <param name="value"></param>
        /// <param name="contenderName"></param>
        public void SetScoreForContender(int value, string contenderName, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE contenders SET score = @param1 WHERE name = @param2 AND channel = @param3";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", value));
                    command.Parameters.Add(new SQLiteParameter("@param2", contenderName));
                    command.Parameters.Add(new SQLiteParameter("@param3", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// set the killer of a contender
        /// </summary>
        /// <param name="userIdString"></param>
        /// <param name="contenderName"></param>
        public void SetKillerForContender(string userIdString, string contenderName, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE contenders SET killer = '' WHERE killer = @param1 AND channel = @param2";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", userIdString));                    
                    command.Parameters.Add(new SQLiteParameter("@param2", channelId));
                    command.ExecuteNonQuery();
                }

                sql = $"UPDATE contenders SET killer = @param1 WHERE name = @param2 AND channel = @param3";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", userIdString));
                    command.Parameters.Add(new SQLiteParameter("@param2", contenderName));                    
                    command.Parameters.Add(new SQLiteParameter("@param3", channelId));
                    command.ExecuteNonQuery();
                }

            }
        }

        public void SetEpitaphForContender(string contenderName, string epitaph, string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE contenders SET epitaph = @param1 WHERE name = @param2 AND channel = @param3";

                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", epitaph));
                    command.Parameters.Add(new SQLiteParameter("@param2", contenderName));                    
                    command.Parameters.Add(new SQLiteParameter("@param3", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Set the status of knockoutinfo to 3 (ended)
        /// </summary>
        public void SetKnockoutToEnded(string channelId)
        {
            string sql = "";

            using (SQLiteConnection db = new SQLiteConnection(DbSource))
            {
                db.Open();

                sql = $"UPDATE knockout SET status = 3 WHERE channel = @param1";
                using (SQLiteCommand command = new SQLiteCommand(sql, db))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add(new SQLiteParameter("@param1", channelId));
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
