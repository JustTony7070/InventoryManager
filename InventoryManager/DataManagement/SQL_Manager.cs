using MySql.Data.MySqlClient;
using InventoryManager.Classes;
using InventoryManager.UtilitiesMetods;

namespace InventoryManager.DataManagement
{
    static class SQL_Manager
    {
        private static MySqlConnection? Connection;
        public static SQL_Connection? LastConnection;
        private static readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "SQL_SavedConnections";
        public static List<SQL_Connection> SavedConnections = EasyCsv.Load<SQL_Connection>(Path);
        public static void Connect(SQL_Connection sql_Connection)
        {
            string ConnectionString = $"Server={sql_Connection.Server};Database={sql_Connection.DbName};" +
                $"User ID={sql_Connection.UserID};Password={sql_Connection.Password};Pooling=false;";
            try
            {
                Connection = new MySqlConnection(ConnectionString);
                Connection.Open();
                ExecuteQuery($"create table if not exists {Enums.Databases.Products_Table}" +
                    "(ID int primary key auto_increment," +
                    "Name varchar(255) not null," +
                    "Quantity int not null," +
                    "Price decimal(10,2) not null," +
                    "Code varchar(10) unique not null)");
                ExecuteQuery($"create table if not exists {Enums.Databases.Orders_Table}" +
                    "(ID int primary key auto_increment," +
                    "CustomerName varchar(255) not null," +
                    "Products_Quantities text not null," +
                    "Code varchar(10) unique not null," +
                    "Status varchar(255) not null)");
                LastConnection = sql_Connection;
            }
            catch
            {
                MessageBox.Show("Unable to establish connection.", "Connection Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        public static MySqlDataReader? ExecuteQuery(string Query,bool HasResult = false)
        {
            try
            {
                MySqlCommand cmd = new(Query,Connection);
                if (HasResult)
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    return reader;
                } else cmd.ExecuteNonQuery();
            }
            catch
            {
                if (GetConnectionStatus()) // If the connection is still open
                    MessageBox.Show("Database request Error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }
        public static bool GetConnectionStatus()
        {
            if (Connection?.State.ToString() != "Open")
            {
                Connection = null;
                return false;
            }
            else return true;
        }
        public static void Disconnect()
        {
            Connection?.Close();
            Connection = null;
        }
        public static void UpdateSavedConnectionsFile() => EasyCsv.Write(Path,SavedConnections);
    }
}
