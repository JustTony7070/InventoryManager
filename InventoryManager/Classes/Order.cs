using InventoryManager.Interfaces;
using InventoryManager.UtilitiesMetods;
using System.Collections.ObjectModel;

namespace InventoryManager.Classes
{
    class Order(int Id, string CustomerName, string Products_Quantities, string Code, string Status) : IData
    {
        public int Id { get; set; } = Id;
        public string CustomerName { get; set; } = CustomerName;
        public string Products_Quantities { get; set; } = Products_Quantities;
        public string Status { get; set; } = Status;
        public string Code { get; set; } = Code;

        public static string ToString(ObservableCollection<string> list)
        {
            string result = string.Empty;
            foreach (string item in list)
                result += $"{item}\n";
            if (result != string.Empty)
                // remove the last \n
                result = result[..^1];
            return result;
        }
        public static List<string> ToList(string str)
        {
            List<string> result = [];
            string[] strings = str.Split('\n');
            foreach (string item in strings)
                result.Add(item);
            return result;
        }
    }
}
