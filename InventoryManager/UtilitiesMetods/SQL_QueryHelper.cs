using InventoryManager.Classes;
using InventoryManager.DataManagement;
using MySql.Data.MySqlClient;
using InventoryManager.Interfaces;

namespace InventoryManager.UtilitiesMetods
{
    static class SQL_QueryHelper
    {
        public static void DeleteItem(int Id,Enums.Databases database) => 
            SQL_Manager.ExecuteQuery($"delete from {database} where ID = {Id}");
        public static void AddItem(IData Item, Enums.Databases database)
        {
            switch (database)
            {
                case Enums.Databases.Products_Table:
                    QH_Product PItem = (QH_Product)Item;
                    SQL_Manager.ExecuteQuery($"insert into {database} " +
                    $"(Name,Quantity,Price,Code) values ('{PItem.Name}',{PItem.Quantity},{PItem.Price},'{PItem.Code}')");
                    break;
                case Enums.Databases.Orders_Table:
                    Order OItem = (Order)Item;
                    SQL_Manager.ExecuteQuery($"insert into {database} " +
                    $"(CustomerName,Products_Quantities,Code,Status) values ('{OItem.CustomerName}','{OItem.Products_Quantities}'," +
                    $"'{OItem.Code}','{OItem.Status}')");
                    break;
            }
        }
        public static void UpdateItem(int Id, IData Item, Enums.Databases database)
        {
            switch (database)
            {
                case Enums.Databases.Products_Table:
                    QH_Product PItem = (QH_Product)Item;
                    SQL_Manager.ExecuteQuery($"update {database} set Name = '{PItem.Name}', Quantity = {PItem.Quantity}," +
                    $"Price = {PItem.Price}, Code = '{PItem.Code}' where ID = {Id}");
                    break;
                case Enums.Databases.Orders_Table:
                    Order OItem = (Order)Item;
                    SQL_Manager.ExecuteQuery($"update {database} set CustomerName = '{OItem.CustomerName}', Products_Quantities = '{OItem.Products_Quantities}'," +
                    $"Code = '{OItem.Code}', Status = '{OItem.Status}' where ID = {Id}");
                    break;
            }
        }
        public static bool CheckIfTheCodeIsUnique(string code)
        {
            using MySqlDataReader? reader = SQL_Manager.ExecuteQuery(
                $"SELECT Code FROM {Enums.Databases.Products_Table} WHERE Code = '{code}' " +
                $"UNION " +
                $"SELECT Code FROM {Enums.Databases.Orders_Table} WHERE Code = '{code}'",
            true);
            return reader == null || !reader.HasRows;
        }
        public static string GetAll(Enums.Databases database) => 
            $"select * from {database}";
        public static string GetFiltered(string Type, string Param, Enums.Databases database) => 
            $"select * from {database} where {Type} like '%{Param}%'";
    }

    class QH_Product(string Name, string Quantity, string Price, string Code) : IData
    {
        public string Name { get; set; } = Name;
        public string Quantity { get; set; } = Quantity;
        public string Price { get; set; } = Price;
        public string Code { get; set; } = Code;
        public int Id { get; set; } = 0;
    }
}
