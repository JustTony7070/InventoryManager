using System.Windows;

namespace InventoryManager.UtilitiesMetods
{
    public static class Tools
    {
        public static void Print(string message) => System.Windows.MessageBox.Show(message);

        private static readonly Random random = new();
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            return new([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)])]);
        }
    }
}
