using InventoryManager.Classes;

namespace InventoryManager.Interfaces
{
    interface IDataProvider
    {
        List<IData>? GetAllInventoryItems(Enums.Databases database);
        List<IData>? SearchInventoryItem(string Type, string Param, Enums.Databases database);
    }
}
