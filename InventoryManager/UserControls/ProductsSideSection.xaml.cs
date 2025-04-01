using System.Windows;

namespace InventoryManager.UserControls
{
    public partial class ProductsSideSection : System.Windows.Controls.UserControl
    {
        public ProductsSideSection()
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
