using CsvHelper;
using System.Globalization;
using System.IO;
using System.Windows;

namespace InventoryManager.UtilitiesMetods
{
    static class EasyCsv
    {
        public static void Write<T>(string FilePath, List<T> Records)
        {
            try
            {
                using StreamWriter writer = new(FilePath + ".csv");
                using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(Records);
            }
            catch
            {
                System.Windows.MessageBox.Show("Error While Saving Data", "Saving Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public static List<T> Load<T>(string Path, bool CreateIfNotExists = true)
        {
            try
            {
                if (!File.Exists(Path) && CreateIfNotExists)
                {
                    File.Create(Path).Close();
                    return [];
                }
                using StreamReader reader = new(Path);
                using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
                return [.. csv.GetRecords<T>()];
            }
            catch
            {
                System.Windows.MessageBox.Show("Error While Loading Data","Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return [];
        }
    }
}
