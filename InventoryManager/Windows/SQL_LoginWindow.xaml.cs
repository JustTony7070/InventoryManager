using System.Windows;
using InventoryManager.ViewModels;

namespace InventoryManager.Windows
{
    public partial class SQL_LoginWindow : Window
    {
        public SQL_LoginWindow()
        {
            InitializeComponent();
            Owner = System.Windows.Application.Current.MainWindow;
            DataContext = new SQL_LoginWindowViewModel(this);
            tb_Server.Focus();
        }
    }
}