using CsvHelper;
using InventoryManager.Classes;
using InventoryManager.Interfaces;
using InventoryManager.UtilitiesMetods;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace InventoryManager.DataManagement
{
    class LocalDataProvider : IDataProvider
    {
        private const string FileNameBase = "InventoryManagerData";
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + FileNameBase;
        public static bool IsDefaultFile = true;
        public static List<IData> ChoosenFileBackup = [];

        public List<IData>? GetAllInventoryItems(Enums.Databases database)
        {
            try
            {
                return IsDefaultFile ? LoadDataGeneric(FilePath + database + ".csv",database) : ChoosenFileBackup;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error while loading data."+ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public List<IData>? SearchInventoryItem(string Type, string Param, Enums.Databases database)
        {
            List<IData>? list = [];
            List<IData>? Data = GetAllInventoryItems(database);
            if (Data == null) return null;
            foreach (IData item in Data)
            {
                PropertyInfo? property = item.GetType().GetProperty(Type);
                if (property != null)
                {
                    string? value = property.GetValue(item)?.ToString();
                    if (!string.IsNullOrEmpty(value) && value.StartsWith(Param, StringComparison.OrdinalIgnoreCase))
                        list.Add(item);
                }
            }
            return list;
        }

        public static void DefaultSave(List<IData> list, Enums.Databases database)
        {
            // override the file
            if (!IsDefaultFile) return;
            try
            {
                if (list == null) return;
                string path = FilePath + database;
                switch (database)
                {
                    case Enums.Databases.Products_Table:
                        EasyCsv.Write(path,[.. list.Cast<Product>()]);
                        break;
                    case Enums.Databases.Orders_Table:
                        EasyCsv.Write(path, [.. list.Cast<Order>()]);
                        break;
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Error while saving data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static Enums.Databases? ChooseDataToLoad()
        {
            FileDialog dialog = new OpenFileDialog()
            {
                Filter = "CSV Files (*.csv)|*.csv",
                DefaultExt = ".csv",
                Title = "Select a CSV file",
                Multiselect = false
            };
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.FileName)) return null;
            Enums.Databases database = dialog.FileName.Contains(Enums.Databases.Products_Table.ToString()) ?
                    Enums.Databases.Products_Table : Enums.Databases.Orders_Table;
            IsDefaultFile = false;
            ChoosenFileBackup = LoadDataGeneric(dialog.FileName, database);
            return database;
        }

        private static List<IData> LoadDataGeneric(string Path,Enums.Databases database)
        {
            return database switch
            {
                Enums.Databases.Products_Table => [.. EasyCsv.Load<Product>(Path)],
                Enums.Databases.Orders_Table => [.. EasyCsv.Load<Order>(Path)],
                _ => [],
            };
        } 
    }
}
