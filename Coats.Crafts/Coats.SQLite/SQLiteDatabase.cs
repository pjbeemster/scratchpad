using System;
using System.Data.SQLite;

namespace Coats.SQLite
{
    public class SQLiteDatabase
    {
        private string _databaseDefault = "PublishedTcmIdList.s3db";
        private string _database = null;
        private string _tableName = "tcm";
        private string _createTableSQL = "CREATE TABLE [tcm] ([id] VARCHAR(20) NULL)";

        public string Database
        {
            get
            {
                if (string.IsNullOrEmpty(_database))
                {
                    if (string.IsNullOrEmpty(Properties.Settings.Default.DatabaseName))
                    {
                        _database = _databaseDefault;
                    }
                    else
                    {
                        _database = Properties.Settings.Default.DatabaseName;
                    }
                }
                return _database;
            }
        }

        private string DbConnection
        {
            //string dbConnectionString = @"Data Source=c:\\tmp\\TmpTester.s3db";
            get { return @"Data Source=" + Database; }
        }

        /// <summary>
        /// Default Constructor for SQLiteDatabase Class.
        /// </summary>
        public SQLiteDatabase()
        {
        }

        /// <summary>
        /// Contructor to specify a database name.
        /// </summary>
        /// <param name="database">
        /// The name of the database to use, e.g. "PublishedTcmIdList.s3db", 
        /// or specify a path, e.g. "c:\bill\beaumont\PublishedTcmIdList.s3db"
        /// </param>
        public SQLiteDatabase(string database)
        {
            _database = database;
        }

        #region Public methods

        /// <summary>
        /// Works out what the operation should really be by checking if the supplied tcm id exists in the database.
        /// SmartTarget always provides "add" and "update" data as simply "add". We need to change this so we
        /// don't overwrite any fields such as "rating" or "commentcount", etc.
        /// N.B. - THIS IS NO LONGER THE CASE, BUT COMMENT LEFT IN TO SHOW WHAT IT USED TO DO...
        /// "...If the tcm id doesn't exist, then it will be added to the SQLite database. 
        /// The only provisio is if the supplied operation is "delete" - in which case the tcm id is removed 
        /// from the database..."
        /// </summary>
        /// <param name="tcmId">
        /// The tcm id (or any id string for that matter) to check for existance in the database
        /// </param>
        /// <param name="operation">
        /// The current operation, usually created by SmartTarget
        /// </param>
        /// <returns>
        /// Operation "add": If the tcm id already exists in the database, return "update".
        /// Operation "add": If the tcm id doesn't exists in the database, add tcm id to database and return "add".
        /// Operation "delete": The tcm id is removed from the database, and "delete" is returned.
        /// </returns>
        public string GetOperation(string tcmId, string operation)
        {
            // ASH: Do I REALLY need this???
            //SQLiteDatabase sqlite = new SQLiteDatabase();

            if (operation.ToLower() == "delete")
            {
                //sqlite.DeleteTcmId(tcmId);
                return operation;
            }

            //if (sqlite.TcmIdExists(tcmId))
            if (TcmIdExists(tcmId))
            {
                return "update";
            }
            else
            {
                //sqlite.InsertTcmId(tcmId);
                return "add";
            }
        }

        /// <summary>
        /// Works out what the operation should really be by checking if the supplied tcm id exists in the database.
        /// SmartTarget always provides "add" and "update" data as simply "add". We need to change this so we
        /// don't overwrite any fields such as "rating" or "commentcount", etc.
        /// If the tcm id doesn't exist, then it will be added to the SQLite database. 
        /// The only provisio is if the supplied operation is "delete" - in which case the tcm id is removed 
        /// from the database
        /// </summary>
        /// <param name="tcmId">
        /// The tcm id (or any id string for that matter) to check for existance in the database
        /// </param>
        /// <param name="operation">
        /// The current operation, usually created by SmartTarget
        /// </param>
        /// <returns>
        /// true - on success.
        /// false - if not.
        /// </returns>
        public bool UpdateOpertaion(string tcmId, string operation)
        {
            // ASH: Do I REALLY need this???
            //SQLiteDatabase sqlite = new SQLiteDatabase();
            try
            {
                if (operation.ToLower() == "delete")
                {
                    //return sqlite.DeleteTcmId(tcmId);
                    return DeleteTcmId(tcmId);
                }

                //if (sqlite.TcmIdExists(tcmId))
                if (TcmIdExists(tcmId))
                {
                    // Nothing to do, tcmif already exists
                    return true;
                }
                else
                {
                    //return sqlite.InsertTcmId(tcmId);
                    return InsertTcmId(tcmId);
                }
            }
            catch (Exception)
            {
                //return false;
                throw;
            }
        }

        /// <summary>
        /// WARNING!!! This will delete ALL data currently held in the tcm table!!!
        /// Use extreme caution!!! (That's drama right there!)
        /// </summary>
        public void ClearDownTable()
        {
            string deleteSQL = "DELETE FROM " + _tableName;
            ExecuteNonQuery(deleteSQL);
        }

        /// <summary>
        /// Check if the tcm id exists in the SQLite database
        /// </summary>
        /// <param name="tcmId">
        /// The tcm id to check the existance thereof!
        /// </param>
        /// <returns>
        /// true - if tcm id is in the SQLite database, false if not.
        /// </returns>
        public bool TcmIdExists(string tcmId)
        {
            string selectSQL = "SELECT [id] FROM " + _tableName + " WHERE [id] = '" + tcmId + "'";
            return !string.IsNullOrEmpty(ExecuteScalar(selectSQL));
        }

        /// <summary>
        /// Insert the tcm id in to the SQLite database
        /// </summary>
        /// <param name="tcmId">
        /// The tcm id to be insterted
        /// </param>
        /// <returns>
        /// true - if more than one field was affected, flase if not.
        /// </returns>
        public bool InsertTcmId(string tcmId)
        {
            string insertSQL = "INSERT INTO " + _tableName + " ([id]) VALUES ('" + tcmId + "')";
            return (ExecuteNonQuery(insertSQL) > 0);
        }

        /// <summary>
        /// Removes the tcm id from the SQLite database
        /// </summary>
        /// <param name="tcmId">
        /// The tcm id to be removed
        /// </param>
        /// <returns>
        /// true - if more than one field was affected, flase if not.
        /// </returns>
        public bool DeleteTcmId(string tcmId)
        {
            string deleteSQL = "DELETE FROM " + _tableName + " WHERE [id] = '" + tcmId + "'";
            return (ExecuteNonQuery(deleteSQL) > 0);
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Ensure table exists
        /// </summary>
        private void EnsureTableExists()
        {
            // Open connection to database
            using (SQLiteConnection sqliteCon = new SQLiteConnection(DbConnection))
            {
                SQLiteCommand cmd = null;
                try
                {
                    sqliteCon.Open();

                    // Define the SQL Create table statement
                    using (SQLiteTransaction sqlTransaction = sqliteCon.BeginTransaction())
                    {
                        // Create the table    
                        using (cmd = new SQLiteCommand(_createTableSQL, sqliteCon))
                        {
                            cmd.ExecuteNonQuery();
                            // Commit the changes into the database    
                            sqlTransaction.Commit();
                            // Dispose
                            sqlTransaction.Dispose(); // Should be handled by the "using", but just to make sure
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    // Close the database connection
                    if (sqliteCon != null)
                    {
                        if (sqliteCon.State == System.Data.ConnectionState.Open)
                        {
                            sqliteCon.Close();
                        }
                        sqliteCon.Dispose(); // Should be handled by the "using", but just to make sure
                    }
                    if (cmd != null)
                    {
                        cmd.Dispose(); // Should be handled by the "using", but just to make sure
                    }
                    GC.Collect();
                }
            }

        }


        /// <summary>
        /// Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            EnsureTableExists();

            // Open connection to database
            using (SQLiteConnection sqliteCon = new SQLiteConnection(DbConnection))
            {
                SQLiteCommand cmd = null;
                try
                {
                    sqliteCon.Open();

                    using (cmd = new SQLiteCommand(sqliteCon))
                    {
                        cmd.CommandText = sql;
                        object value = cmd.ExecuteScalar();
                        if (value != null)
                        {
                            return value.ToString();
                        }
                        return "";
                    }
                }
                catch (Exception)
                {
                    return "";
                }
                finally
                {
                    // Close the database connection
                    if (sqliteCon != null)
                    {
                        if (sqliteCon.State == System.Data.ConnectionState.Open)
                        {
                            sqliteCon.Close();
                        }
                        sqliteCon.Dispose(); // Should be handled by the "using", but just to make sure
                    }
                    if (cmd != null)
                    {
                        cmd.Dispose(); // Should be handled by the "using", but just to make sure
                    }
                    GC.Collect();
                }
            }

        }

        /// <summary>
        /// Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            EnsureTableExists();

            // Open connection to database
            using (SQLiteConnection sqliteCon = new SQLiteConnection(DbConnection))
            {
                SQLiteCommand cmd = null;
                try
                {
                    sqliteCon.Open();

                    using (cmd = new SQLiteCommand(sqliteCon))
                    {
                        cmd.CommandText = sql;
                        int ret = cmd.ExecuteNonQuery();
                        return ret;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }
                finally
                {
                    // Close the database connection
                    if (sqliteCon != null)
                    {
                        if (sqliteCon.State == System.Data.ConnectionState.Open)
                        {
                            sqliteCon.Close();
                        }
                        sqliteCon.Dispose(); // Should be handled by the "using", but just to make sure
                    }
                    if (cmd != null)
                    {
                        cmd.Dispose(); // Should be handled by the "using", but just to make sure
                    }
                    GC.Collect();
                }
            }
        }

        #endregion
    }
}
