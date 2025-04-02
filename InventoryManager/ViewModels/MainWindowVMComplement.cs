using InventoryManager.Classes;
using InventoryManager.UserControls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InventoryManager.ViewModels
{
    partial class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? PropertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        // DataGrids
        public ObservableCollection<Product> B_DataGrid { get; set; } = [];
        public ObservableCollection<Order> B_CustomersDataGrid { get; set; } = [];
        // Connection Text
        private string b_ConnectionStatus = string.Empty;
        public string B_ConnectionStatus
        {
            get { return b_ConnectionStatus; }
            set
            {
                b_ConnectionStatus = value;
                OnPropertyChanged();
            }
        }
        // Connection text color
        private SolidColorBrush b_ConnectionColor = new(Colors.Red);
        public SolidColorBrush B_ConnectionColor
        {
            get { return b_ConnectionColor; }
            set
            {
                b_ConnectionColor = value;
                OnPropertyChanged();
            }
        }
        // Connection button content
        private string b_SqlButtonContent = string.Empty;
        public string B_SqlButtonContent
        {
            get { return b_SqlButtonContent; }
            set
            {
                b_SqlButtonContent = value;
                OnPropertyChanged();
            }
        }
        // Connection Button Enable
        private bool b_SqlButtonEnabled = true;
        public bool B_SqlButtonEnabled
        {
            get { return b_SqlButtonEnabled; }
            set
            {
                b_SqlButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        // Toolbar buttons Control

        // Edit Button
        private bool b_EditEnabled;
        public bool B_EditEnabled
        {
            get { return b_EditEnabled; }
            set
            {
                b_EditEnabled = value;
                OnPropertyChanged();
            }
        }
        // Delete Button
        private bool b_DelEnabled;
        public bool B_DelEnabled
        {
            get { return b_DelEnabled; }
            set
            {
                b_DelEnabled = value;
                OnPropertyChanged();
            }
        }
        //New Button
        private GridLength b_SplitterWidth;
        public GridLength B_SplitterWidth
        {
            get { return b_SplitterWidth; }
            set
            {
                b_SplitterWidth = value;
                OnPropertyChanged();
            }
        }
        // Action window title
        private string b_ActionTitle = string.Empty;
        public string B_ActionTitle
        {
            get { return b_ActionTitle; }
            set
            {
                b_ActionTitle = value;
                OnPropertyChanged();
            }
        }
        // Name
        private string b_NewName = string.Empty;
        public string B_NewName
        {
            get { return b_NewName; }
            set
            {
                b_NewName = value;
                OnPropertyChanged();
            }
        }
        // Quantity
        private string b_NewQuantity = string.Empty;
        public string B_NewQuantity
        {
            get { return b_NewQuantity; }
            set
            {
                b_NewQuantity = value;
                OnPropertyChanged();
            }
        }
        // Price
        private string b_NewPrice = string.Empty;
        public string B_NewPrice
        {
            get { return b_NewPrice; }
            set
            {
                b_NewPrice = value;
                if (B_NewPrice.Contains(',')) b_NewPrice = b_NewPrice.Replace(',', '.');
                OnPropertyChanged();
            }
        }
        // Code
        private string b_NewCode = string.Empty;
        public string B_NewCode
        {
            get { return b_NewCode; }
            set
            {
                b_NewCode = value;
                OnPropertyChanged();
            }
        }
        // Search Text Opacity
        private double b_SearchTextOpacity = .5;
        public double B_SearchTextOpacity
        {
            get { return b_SearchTextOpacity; }
            set
            {
                b_SearchTextOpacity = value;
                OnPropertyChanged();
            }
        }
        // Search Type
        private ComboBoxItem? b_SearchType;
        public ComboBoxItem? B_SearchType
        {
            get { return b_SearchType; }
            set
            {
                b_SearchType = value;
                OnPropertyChanged();
            }
        }
        // Load Data
        private bool b_LoadDataEnabled = true;
        public bool B_LoadDataEnabled
        {
            get { return b_LoadDataEnabled; }
            set
            {
                b_LoadDataEnabled = value;
                OnPropertyChanged();
            }
        }
        // Clear Table
        private bool b_ClearTableEnabled = true;
        public bool B_ClearTableEnabled
        {
            get { return b_ClearTableEnabled; }
            set
            {
                b_ClearTableEnabled = value;
                OnPropertyChanged();
            }
        }
        // SideSection Column
        private double b_SideSectionMaxWidth;
        public double B_SideSectionMaxWidth
        {
            get { return b_SideSectionMaxWidth; }
            set { b_SideSectionMaxWidth = value; 
                OnPropertyChanged();
            }
        }
        private GridLength b_SideSectionWidth;
        public GridLength B_SideSectionWidth
        {
            get { return b_SideSectionWidth; }
            set { b_SideSectionWidth = value;
                OnPropertyChanged();
            }
        }
        private double b_SideSectionMinWidth;
        public double B_SideSectionMinWidth
        {
            get { return b_SideSectionMinWidth; }
            set
            {
                b_SideSectionMinWidth = value;
                OnPropertyChanged();
            }
        }
        // Side Section user Control
        private System.Windows.Controls.UserControl b_SideSectionUserControl = new ProductsSideSection();
        public System.Windows.Controls.UserControl B_SideSectionUserControl
        {
            get { return b_SideSectionUserControl; }
            set { b_SideSectionUserControl = value; 
                OnPropertyChanged();
            }
        }
        // Costumers Side Section Fields
        public ObservableCollection<string> B_Products_QuantitiesList { get; set; } = [];
        private string? b_PQListSelectedItem;
        public string? B_PQListSelectedItem
        {
            get { return b_PQListSelectedItem; }
            set { b_PQListSelectedItem = value;
                if (value != null)
                {
                    string[] strings = value.Split(": ");
                    B_NewProductToAdd = strings[0];
                    B_NewQuantityToAdd = strings[1];
                }
                OnPropertyChanged(); 
            }
        } // Product Quantity List Selected Item
        private string b_NewProductToAdd = string.Empty;
        public string B_NewProductToAdd
        {
            get { return b_NewProductToAdd ; }
            set { b_NewProductToAdd  = value; OnPropertyChanged(); }
        }
        private string b_NewQuantityToAdd = string.Empty;
        public string B_NewQuantityToAdd
        {
            get { return b_NewQuantityToAdd; }
            set { b_NewQuantityToAdd = value; OnPropertyChanged(); }
        }
        private int b_OrderStatus = 0;
        public int B_OrderStatus
        {
            get { return b_OrderStatus; }
            set { b_OrderStatus = value; OnPropertyChanged(); }
        }
        public readonly string[] OrderStatus = [ "Pending", "Completed", "Cancelled" ];
        public ObservableCollection<ComboBoxItem> B_SearchTypeList { get; set; } = [];
    }
}
