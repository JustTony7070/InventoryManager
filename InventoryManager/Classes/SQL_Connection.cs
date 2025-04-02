namespace InventoryManager.Classes
{
    class SQL_Connection(string Server, string DbName, string UserID, string Password)
    {
        public string Server { get; set; } = Server;
        public string DbName { get; set; } = DbName;
        public string UserID { get; set; } = UserID;
        public string Password { get; set; } = Password;
    }
}
