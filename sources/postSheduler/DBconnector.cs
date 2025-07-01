using System;
using System.Data;
using System.Data.SQLite;  // Важно: используем System.Data.SQLite, а не Microsoft.Data.Sqlite

    public class DBconnector : IDisposable
    {
        private SQLiteConnection _connection;
        private static readonly string DatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "smaik.db");


    /// <summary>
    /// Открывает соединение с базой данных.
    /// </summary>
    public void OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection($"Data Source={DatabasePath};Version=3;");
            }

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        /// <summary>
        /// Закрывает соединение с базой данных.
        /// </summary>
        public void CloseConnection()
        {
            if (_connection?.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Выполняет SQL-запрос (INSERT, UPDATE, DELETE).
        /// </summary>
        public int ExecuteNonQuery(string sql)
        {
            OpenConnection();

            
            using (var command = new SQLiteCommand("PRAGMA foreign_keys = ON;" + sql, _connection))
            {
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Выполняет запрос и возвращает DataTable.
        /// </summary>
        public DataTable ExecuteQuery(string sql)
        {
            OpenConnection();
            using (var command = new SQLiteCommand(sql, _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }

        public DataTable ExecuteQuery(string sql, SQLiteParameter[] parameters)
        {
            OpenConnection();
            using (var command = new SQLiteCommand(sql, _connection))
            {
                command.Parameters.AddRange(parameters);
                using (var reader = command.ExecuteReader())
                {
                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    return dataTable;
                }
            }
        }

        public int ExecuteNonQuery(string sql, SQLiteParameter[] parameters)
        {
            OpenConnection();
            using (var command = new SQLiteCommand(sql, _connection))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Освобождает ресурсы.
        /// </summary>
        public void Dispose()
        {
            _connection?.Dispose();
        }
}