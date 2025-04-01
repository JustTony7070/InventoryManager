using System.Windows;
using InventoryManager.DataManagement;

namespace InventoryManager.Windows
{
    public partial class SQL_LoginWindow : Window
    {
        public SQL_LoginWindow()
        {
            InitializeComponent();
            Owner = System.Windows.Application.Current.MainWindow;
            if (SQL_Manager.LastConnection != null)
            {
                tb_Server.Text = SQL_Manager.LastConnection.Server;
                tb_DbName.Text = SQL_Manager.LastConnection.DbName;
                tb_UserID.Text = SQL_Manager.LastConnection.UserID;
                Password.Password = SQL_Manager.LastConnection.Password;
            }
            else tb_Server.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_Server.Text) && !string.IsNullOrEmpty(tb_DbName.Text) &&
                !string.IsNullOrEmpty(tb_UserID.Text) && !string.IsNullOrEmpty(Password.Password))
            {
                SQL_Manager.Connect(new(tb_Server.Text, tb_DbName.Text, tb_UserID.Text, Password.Password));
                Close();
            }
        }
    }
}