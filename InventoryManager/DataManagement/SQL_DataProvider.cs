using InventoryManager.Classes;
using InventoryManager.Interfaces;
using InventoryManager.UtilitiesMetods;
using MySql.Data.MySqlClient;

namespace InventoryManager.DataManagement
{
    class SQL_DataProvider : IDataProvider
    {

        public List<IData>? GetAllInventoryItems(Enums.Databases database)
        {
            List<IData> list = [];
            using MySqlDataReader? reader = SQL_Manager.ExecuteQuery(SQL_QueryHelper.GetAll(database), true);
            if (reader == null) return null;
            while (reader.Read())
            {
                IData? DataMapped = DataMapping(reader, database);
                if (DataMapped != null) list.Add(DataMapped);
            }
            return list;
        }
        public List<IData>? SearchInventoryItem(string Type, string Param, Enums.Databases database)
        {
            List<IData> list = [];
            using MySqlDataReader? reader = SQL_Manager.ExecuteQuery(SQL_QueryHelper.GetFiltered(Type, Param, database), true);
            if (reader == null) return null;
            while (reader.Read())
            {
                IData? DataMapped = DataMapping(reader,database);
                if (DataMapped != null) list.Add(DataMapped);
            }
            return list;
        }
        private static IData? DataMapping(MySqlDataReader reader,Enums.Databases database)
        {
            switch (database)
            {
                case Enums.Databases.Products_Table:
                    IData InventoryItem = new Product
                    (reader.GetInt32("ID"),
                    reader.GetString("Name"),
                    reader.GetInt32("Quantity"),
                    reader.GetDecimal("Price"),
                    reader.GetString("Code"));
                    return InventoryItem;
                case Enums.Databases.Orders_Table:
                    return new Order
                        (reader.GetInt32("ID"),
                        reader.GetString("CustomerName"),
                        reader.GetString("Products_Quantities"),
                        reader.GetString("Code"),
                        reader.GetString("Status"));
                default:
                    return null;
            }
        }
    }
}
