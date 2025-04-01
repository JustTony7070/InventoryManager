namespace InventoryManager.Classes
{
    class SQL_Connection(string server,string dbName, string UserID, string password)
    {
        public string Server { get; set; } = server;
        public string DbName { get; set; } = dbName;
        public string UserID { get; set; } = UserID;
        public string Password { get; set; } = password;
    }
}
