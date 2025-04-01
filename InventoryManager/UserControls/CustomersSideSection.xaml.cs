using System.Windows;

namespace InventoryManager.UserControls
{
    public partial class CustomersSideSection : System.Windows.Controls.UserControl
    {
        public CustomersSideSection()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (Window.GetWindow(this) is Window window)
                    DataContext = window.DataContext;
            };
        }
    }
}
