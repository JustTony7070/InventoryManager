using InventoryManager.Classes;
using InventoryManager.Interfaces;
using InventoryManager.UtilitiesMetods;
using System.Windows;

namespace InventoryManager.DataManagement
{
    static class DataExporter
    {
        private const string FileNameBase = @"\InventoryManagerData";
        private static string ExportPath = string.Empty;
        public static void ExportData(List<IData> list)
        {
            if (string.IsNullOrEmpty(ExportPath))
                SelectExportPath();
            try
            {
                if (string.IsNullOrEmpty(ExportPath)) return;
                Enums.Databases database = list.First().GetType().Name == "Product" ? Enums.Databases.Products_Table
                    : Enums.Databases.Orders_Table;
                string filePath = ExportPath + FileNameBase + database.ToString() + ".csv";
                switch (database)
                {
                    case Enums.Databases.Products_Table:
                        EasyCsv.Write(filePath,[.. list.Cast<Product>()]);
                        break;
                    case Enums.Databases.Orders_Table:
                        EasyCsv.Write(filePath, [.. list.Cast<Order>()]);
                        break;
                }
                System.Windows.MessageBox.Show("Data Exported Successfully.", "Data Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                System.Windows.MessageBox.Show("Invalid Path. Error while exporting data.", "Path not Valid",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                ExportPath = string.Empty;
            }
        }
        public static void SelectExportPath()
        {
            FolderBrowserDialog folderBrowserDialog = new();
            folderBrowserDialog.ShowDialog();
            if (!string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
                ExportPath = folderBrowserDialog.SelectedPath;
        }
    }
}
