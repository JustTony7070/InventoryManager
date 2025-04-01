using CsvHelper;
using InventoryManager.Classes;
using InventoryManager.Interfaces;
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
                using StreamWriter writer = new(FilePath + database + ".csv");
                using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
                switch (database)
                {
                    case Enums.Databases.Products_Table:
                        csv.WriteRecords(list.Cast<Product>());
                        break;
                    case Enums.Databases.Orders_Table:
                        csv.WriteRecords(list.Cast<Order>());
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
            try
            {
                Enums.Databases database = dialog.FileName.Contains(Enums.Databases.Products_Table.ToString()) ?
                    Enums.Databases.Products_Table : Enums.Databases.Orders_Table;
                IsDefaultFile = false;
                ChoosenFileBackup = LoadDataGeneric(dialog.FileName, database);
                return database;
            }
            catch
            {
                System.Windows.MessageBox.Show("Error while loading data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IsDefaultFile = true;
            }
            return null;
        }

        private static List<IData> LoadDataGeneric(string Path,Enums.Databases database)
        {
            List<IData> list = [];
            if (!File.Exists(Path) && IsDefaultFile)
                File.Create(Path).Close();
            using StreamReader reader = new(Path);
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
            switch (database)
            {
                case Enums.Databases.Products_Table:
                    var IIRecord = csv.GetRecords<Product>();
                    foreach (var record in IIRecord)
                        list.Add(record);
                    break;
                case Enums.Databases.Orders_Table:
                    var OC_Record = csv.GetRecords<Order>();
                    foreach (var record in OC_Record)
                        list.Add(record);
                    break;
            }
            return list;
        } 
    }
}
