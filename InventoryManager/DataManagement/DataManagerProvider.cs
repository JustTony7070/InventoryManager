using InventoryManager.Classes;
using InventoryManager.Interfaces;

namespace InventoryManager.DataManagement
{
    static class DataManagerProvider
    {
        public static IDataProvider GetDataManager()
        {
            if (SQL_Manager.GetConnectionStatus())
                return new SQL_DataProvider();
            else
                return new LocalDataProvider();
        }
    }
}
