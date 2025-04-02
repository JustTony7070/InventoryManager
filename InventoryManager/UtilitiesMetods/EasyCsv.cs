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
            string path = PathFixer(FilePath);
            try
            {
                using StreamWriter writer = new(path);
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
            string path = PathFixer(Path);
            try
            {
                if (!File.Exists(path) && CreateIfNotExists)
                {
                    File.Create(path).Close();
                    return [];
                }
                using StreamReader reader = new(path);
                using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
                return [.. csv.GetRecords<T>()];
            }
            catch
            {
                System.Windows.MessageBox.Show("Error While Loading Data","Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return [];
        }
        private static string PathFixer(string Path) => Path.EndsWith(".csv") ? Path : Path + ".csv";
    }
}
