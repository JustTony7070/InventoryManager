using InventoryManager.Classes;
using InventoryManager.DataManagement;
using InventoryManager.Interfaces;
using InventoryManager.UserControls;
using InventoryManager.UtilitiesMetods;
using InventoryManager.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InventoryManager.ViewModels
{
    internal class MainWindowViewModel : MainWindowVMComplement
    {
        #region RelayCommands
        public RelayCommand ConnectCommand => new(execute => ConnectToSQLAction());
        public RelayCommand NewCommand => new(execute => NewButtonAction());
        public RelayCommand CancelCommand => new(execute => CancelButtonAction());
        public RelayCommand ConfirmCommand => new(execute => ConfirmButtonAction());
        public RelayCommand EditCommand => new(execute => EditButtonAction());
        public RelayCommand DeleteCommand => new(execute => DeleteButtonAction());
        public RelayCommand SearchCommand => new(execute => SearchAction());
        public RelayCommand AutoGenerateCodeCommand => new(execute => B_NewCode = Tools.GenerateRandomString(10));
        public RelayCommand ExportDataCommand => new(ExecutionContext => ExportDataAction());
        public static RelayCommand SelectPathCommand => new(ExecutionContext => DataExporter.SelectExportPath());
        public RelayCommand LoadDataCommand => new(ExecutionContext => LoadDataAction());
        public RelayCommand ClearTableCommand => new(ExecutionContext => ClearTableAction());
        public RelayCommand AddNewProductCommand => new(ExecutionContext => AddNewProductAction());
        public RelayCommand RemoveSelectedProductCommand => new(ExecutionContext => RemoveSelectedProductAction());
        #endregion

        #region AutoSync Params
        private const int AutoSyncInterval = 3000; //milliseconds
        private bool CanAutoSync = true;
        #endregion

        #region Other Params
        private IDataProvider dataProvider = DataManagerProvider.GetDataManager();
        private string? CurrentAction = null;

        // TabControl Table Selector
        private int b_TableSelectorIndex;
        public int B_TableSelectorIndex
        {
            get { return b_TableSelectorIndex; }
            set
            {
                b_TableSelectorIndex = value;
                OnTableChanged();
                OnPropertyChanged();
            }
        }

        // Selected Items
        private Product? b_SelectedItemProductTable;
        public Product? B_SelectedItemProductTable
        {
            get { return b_SelectedItemProductTable; }
            set
            {
                b_SelectedItemProductTable = value;
                OnPropertyChanged();
                ButtonsBehavior();
            }
        }
        private Order? b_SelectedItemCustomerTable;
        public Order? B_SelectedItemCustomerTable
        {
            get { return b_SelectedItemCustomerTable; }
            set { b_SelectedItemCustomerTable = value; 
                OnPropertyChanged();
                ButtonsBehavior();
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            UpdateConnectionText();
            OnTableChanged();
        }

        #region Connection Manager
        private void UpdateConnectionText()
        {
            if (SQL_Manager.GetConnectionStatus())
            {
                B_ConnectionColor = new SolidColorBrush(Colors.Green);
                B_ConnectionStatus = "Connection Status: Connected";
                B_SqlButtonContent = "Disconnect";
            }
            else
            {
                B_ConnectionColor = new SolidColorBrush(Colors.Red);
                B_ConnectionStatus = "Connection Status: Not Connected";
                B_SqlButtonContent = "Connect to MySQL";
            }
        }
        private async void ConnectToSQLAction()
        {
            if (SQL_Manager.GetConnectionStatus())
            {
                SQL_Manager.Disconnect();
                dataProvider = DataManagerProvider.GetDataManager();
                SyncData();
            }
            else
            {
                // try to connect
                SQL_LoginWindow sQL_LoginWindow = new();
                sQL_LoginWindow.ShowDialog();
                dataProvider = DataManagerProvider.GetDataManager();
                AutoSyncData();
            }
            ;
            UpdateConnectionText();
            B_LoadDataEnabled = !SQL_Manager.GetConnectionStatus();
            B_ClearTableEnabled = !SQL_Manager.GetConnectionStatus();
            B_SqlButtonEnabled = false;
            await Task.Delay(1000);
            B_SqlButtonEnabled = true;
        }
        #endregion

        #region Functions
        private void SyncData(List<IData>? list = null)
        {
            if (dataProvider is SQL_DataProvider && !SQL_Manager.GetConnectionStatus()) return;
            try
            {
                List<IData>? items = list ?? dataProvider.GetAllInventoryItems(GetCurrentDatabase());
                Product? currentProductSelected = B_SelectedItemProductTable;
                Order? currentOrderSelected = B_SelectedItemCustomerTable;
                B_DataGrid.Clear();
                B_CustomersDataGrid.Clear();
                if (items == null || items.Count == 0) return;
                switch (items.First().GetType().Name)
                {
                    case "Product":
                        foreach (Product item in items.Cast<Product>())
                            B_DataGrid.Add(item);
                        break;
                    case "Order":
                        foreach (Order item in items.Cast<Order>())
                            B_CustomersDataGrid.Add(item);
                        break;
                }
                // Reselect the item
                if (currentProductSelected != null)
                {
                    foreach (Product item in B_DataGrid)
                    {
                        if (item.Id == currentProductSelected.Id)
                        {
                            B_SelectedItemProductTable = item;
                            break;
                        }
                    }
                }
                if (currentOrderSelected != null)
                {
                    foreach (Order item in B_CustomersDataGrid)
                    {
                        if (item.Id == currentOrderSelected.Id)
                        {
                            B_SelectedItemCustomerTable = item;
                            break;
                        }
                    }
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Unable to Sync Data.", "Data Sync Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Open_SideSection(bool OnOff)
        {
            B_SideSectionMaxWidth = OnOff ? 500 : 0;
            B_SideSectionWidth = OnOff ? new(400) : new(0);
            B_SideSectionMinWidth = OnOff ? 300 : 0;
            B_SplitterWidth = OnOff ? new(7) : new(0);
            ClearSideSectionFields();
        }
        private void ClearSideSectionFields()
        {
            B_NewName = string.Empty;
            B_NewQuantity = string.Empty;
            B_NewPrice = string.Empty;
            B_NewCode = string.Empty;
            B_Products_QuantitiesList.Clear();
            B_NewQuantityToAdd = string.Empty;
            B_OrderStatus = 0;
            B_NewProductToAdd = string.Empty;
        }
        private bool CheckFieldsValidity()
        {
            // General checks
            if (string.IsNullOrEmpty(B_NewName) || string.IsNullOrEmpty(B_NewCode))
            {
                System.Windows.MessageBox.Show("Please fill all fields.", "Empty Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (CurrentAction == "New")
            {
                string[] messageBoxText = ["Code must be unique.", "Invalid Code"];
                if (dataProvider is SQL_DataProvider)
                {
                    if (!SQL_QueryHelper.CheckIfTheCodeIsUnique(B_NewCode))
                    {
                        System.Windows.MessageBox.Show(messageBoxText[0], messageBoxText[1], MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                else
                {
                    List<IData>? items = dataProvider.GetAllInventoryItems(GetCurrentDatabase());
                    if (items == null) return false;
                    if (items.ToList().Exists(item => item.Code == B_NewCode))
                    {
                        System.Windows.MessageBox.Show(messageBoxText[0], messageBoxText[1], MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
            } // Check if Code is unique only when adding new item
            if (!B_NewCode.All(char.IsLetterOrDigit))
            {
                System.Windows.MessageBox.Show("Code must contain only letters and numbers.", "Invalid Code", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            } // check if code has no special characters
            if (B_NewCode.Length != 10)
            {
                System.Windows.MessageBox.Show("Code must be 10 characters long.", "Invalid Code", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            } // Check if Code is 10 characters long
            if (B_NewPrice.Contains('.'))
            {
                string[] priceParts = B_NewPrice.Split('.');
                if (priceParts[1].Length > 2)
                {
                    System.Windows.MessageBox.Show("Price must have only 2 decimal numbers.", "Invalid Price", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            } // Decimal must have only 2 decimal numbers
            switch (GetCurrentDatabase())
            {
                case Enums.Databases.Products_Table:
                    if (string.IsNullOrEmpty(B_NewQuantity) || string.IsNullOrEmpty(B_NewPrice))
                    {
                        System.Windows.MessageBox.Show("Please fill all fields.", "Empty Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (!int.TryParse(B_NewQuantity, out int _))
                    {
                        System.Windows.MessageBox.Show("Quantity is not a valid integer.", "Invalid Quantity", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (!decimal.TryParse(B_NewPrice, out decimal _))
                    {
                        System.Windows.MessageBox.Show("Price must be a number.", "Invalid Price", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    break;
                case Enums.Databases.Orders_Table:
                    if (B_Products_QuantitiesList.Count == 0)
                    {
                        System.Windows.MessageBox.Show("Please add at least one product.", "Empty Products", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    break;
            }
            return true;
        }
        private async void AutoSyncData()
        {
            while (SQL_Manager.GetConnectionStatus() && CanAutoSync)
            {
                SyncData();
                await Task.Delay(AutoSyncInterval);
            }
            if (CanAutoSync) // if CanAutoSync is true then the connection was lost
                OnConnectionLost();
        }
        private void OnConnectionLost()
        {
            SyncData(); // AutoSync once to sync to the local data
            UpdateConnectionText();
            ButtonsBehavior();
        }
        private Enums.Databases GetCurrentDatabase()
        {
            return B_TableSelectorIndex switch
            {
                0 => Enums.Databases.Products_Table,
                1 => Enums.Databases.Orders_Table,
                _ => Enums.Databases.Products_Table,
            };
        }
        private void ButtonsBehavior()
        {
            switch (GetCurrentDatabase())
            {
                case Enums.Databases.Products_Table:
                    B_EditEnabled = B_SelectedItemProductTable != null;
                    B_DelEnabled = B_SelectedItemProductTable != null;
                    break;
                case Enums.Databases.Orders_Table:
                    B_EditEnabled = B_SelectedItemCustomerTable != null;
                    B_DelEnabled = B_SelectedItemCustomerTable != null;
                    break;
            }
        }
        private void OnTableChanged()
        {
            switch (B_TableSelectorIndex)
            {
                case 0:
                    B_SideSectionUserControl = new ProductsSideSection();
                    break;
                case 1:
                    B_SideSectionUserControl = new CustomersSideSection();
                    break;
            } // Change the SideSection User Control
            SyncData();
            ButtonsBehavior();
            Open_SideSection(false);
            string[][] types =
            [
                ["Id", "Name", "Quantity", "Price", "Code"],
                ["Id", "CustomerName", "Products_Quantities", "Code", "Status"],
            ];
            B_SearchTypeList.Clear();
            foreach (string type in types[B_TableSelectorIndex])
                B_SearchTypeList.Add(new ComboBoxItem() {Content = type });
        }
        #endregion

        #region ButtonsActions
        private void NewButtonAction()
        {
            B_ActionTitle = "Add New Item";
            Open_SideSection(true);
            CurrentAction = "New";
        }
        private void CancelButtonAction()
        {
            Open_SideSection(false);
            CurrentAction = null;
        }
        private void EditButtonAction()
        {
            CurrentAction = "Edit";
            B_ActionTitle = "Edit Item";
            Open_SideSection(true);
            //Rewrite data in fields
            switch (GetCurrentDatabase())
            {
                case Enums.Databases.Products_Table:
                    if (B_SelectedItemProductTable != null)
                    {
                        Product product = B_SelectedItemProductTable; //copy the selected item
                        B_NewName = product.Name;
                        B_NewQuantity = product.Quantity.ToString();
                        B_NewPrice = product.Price.ToString();
                        B_NewCode = product.Code;
                    }
                    break;
                case Enums.Databases.Orders_Table:
                    if (B_SelectedItemCustomerTable != null)
                    {
                        Order order = B_SelectedItemCustomerTable; //copy the selected item
                        B_NewName = order.CustomerName;
                        B_NewCode = order.Code;
                        B_OrderStatus = Array.IndexOf(OrderStatus,order.Status);
                        foreach (string x in Order.ToList(order.Products_Quantities))
                            B_Products_QuantitiesList.Add(x);
                    }
                    break;
            }
        }
        private void DeleteButtonAction()
        {
            switch (GetCurrentDatabase())
            {
                case Enums.Databases.Products_Table:
                    if (B_SelectedItemProductTable == null) return;
                    break;
                case Enums.Databases.Orders_Table:
                    if (B_SelectedItemCustomerTable == null) return;
                    break;
            }
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to delete this item?",
                "Delete Item", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (dataProvider is SQL_DataProvider)
                {
                    try
                    {
                        int selectedId = GetCurrentDatabase() == Enums.Databases.Products_Table ? 
                            B_SelectedItemProductTable!.Id : B_SelectedItemCustomerTable!.Id;
                        SQL_QueryHelper.DeleteItem(selectedId, GetCurrentDatabase());
                    }
                    catch
                    {
                        System.Windows.MessageBox.Show("An error occurred while deleting the item.", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    SyncData();
                }
                else
                {
                    IData dataToRemove = GetCurrentDatabase() == Enums.Databases.Products_Table ?
                        B_SelectedItemProductTable! : B_SelectedItemCustomerTable!;
                    if (LocalDataProvider.IsDefaultFile)
                    {
                        switch (GetCurrentDatabase())
                        {
                            case Enums.Databases.Products_Table:
                                B_DataGrid.Remove((Product)dataToRemove);
                                break;
                            case Enums.Databases.Orders_Table:
                                B_CustomersDataGrid.Remove((Order)dataToRemove);
                                break;
                        }
                        LocalDataProvider.DefaultSave(GetCurrentDatabase() == Enums.Databases.Products_Table ? 
                            [.. B_DataGrid] : [.. B_CustomersDataGrid],GetCurrentDatabase());
                    }
                    else
                        LocalDataProvider.ChoosenFileBackup.Remove(dataToRemove);
                    SyncData();
                }
                Open_SideSection(false);
                CurrentAction = null;
            }
        }
        private static decimal AdjustPrice(string price) => decimal.Parse(price.Contains('.') ? price.Replace('.', ',') : price + ",00");
        private void ConfirmButtonAction()
        {
            if (!CheckFieldsValidity()) return;
            if (dataProvider is SQL_DataProvider)
            {
                switch (CurrentAction)
                {
                    case "New":
                        switch (GetCurrentDatabase())
                        {
                            case Enums.Databases.Products_Table:
                                SQL_QueryHelper.AddItem(new QH_Product(B_NewName, B_NewQuantity, B_NewPrice, B_NewCode), 
                                    Enums.Databases.Products_Table);
                                break;
                            case Enums.Databases.Orders_Table:
                                SQL_QueryHelper.AddItem(new Order(0, B_NewName, Order.ToString(B_Products_QuantitiesList),
                                    B_NewCode, OrderStatus[B_OrderStatus]),
                                    Enums.Databases.Orders_Table);
                                break;
                        }
                        break;
                    case "Edit":
                        switch (GetCurrentDatabase())
                        {
                            case Enums.Databases.Products_Table:
                                if (B_SelectedItemProductTable == null) return;
                                SQL_QueryHelper.UpdateItem(B_SelectedItemProductTable.Id, new QH_Product(B_NewName, B_NewQuantity, B_NewPrice, B_NewCode),
                                    Enums.Databases.Products_Table);
                                break;
                            case Enums.Databases.Orders_Table:
                                if (B_SelectedItemCustomerTable == null) return;
                                SQL_QueryHelper.UpdateItem(B_SelectedItemCustomerTable.Id, new Order(0, B_NewName, Order.ToString(B_Products_QuantitiesList), 
                                    B_NewCode, OrderStatus[B_OrderStatus]), Enums.Databases.Orders_Table);
                                break;
                        }
                        break;
                    default:
                        CurrentAction = null;
                        Open_SideSection(false);
                        break;
                }
            }
            else
            {
                
                List<IData>? list = dataProvider.GetAllInventoryItems(GetCurrentDatabase());

                if (list == null) return;
                switch (CurrentAction)
                {
                    case "New":
                        int LastId = list.ToList().LastOrDefault()?.Id ?? 0;
                        switch (GetCurrentDatabase())
                        {
                            case Enums.Databases.Products_Table:
                                B_DataGrid.Add(new Product(LastId + 1, B_NewName, int.Parse(B_NewQuantity), AdjustPrice(B_NewPrice), B_NewCode));
                                break;
                            case Enums.Databases.Orders_Table:
                                B_CustomersDataGrid.Add(new Order(LastId + 1, B_NewName, Order.ToString(B_Products_QuantitiesList), 
                                    B_NewCode, OrderStatus[B_OrderStatus]));
                                break;
                        }
                        break;
                    case "Edit":
                        switch (GetCurrentDatabase())
                        {
                            case Enums.Databases.Products_Table:
                                if (B_SelectedItemProductTable == null) return;
                                B_SelectedItemProductTable.Name = B_NewName;
                                B_SelectedItemProductTable.Quantity = int.Parse(B_NewQuantity);
                                B_SelectedItemProductTable.Price = AdjustPrice(B_NewPrice);
                                B_SelectedItemProductTable.Code = B_NewCode;
                                break;
                            case Enums.Databases.Orders_Table:
                                if (B_SelectedItemCustomerTable == null) return;
                                B_SelectedItemCustomerTable.CustomerName = B_NewName;
                                B_SelectedItemCustomerTable.Products_Quantities = Order.ToString(B_Products_QuantitiesList);
                                B_SelectedItemCustomerTable.Code = B_NewCode;
                                B_SelectedItemCustomerTable.Status = OrderStatus[B_OrderStatus];
                                break;
                        }
                        break;
                    default:
                        CurrentAction = null;
                        Open_SideSection(false);
                        break;
                }
                List<IData> currentList = GetCurrentDatabase() == Enums.Databases.Products_Table ? [.. B_DataGrid] : [.. B_CustomersDataGrid];
                if (LocalDataProvider.IsDefaultFile)
                    LocalDataProvider.DefaultSave(currentList,GetCurrentDatabase());
                else
                    LocalDataProvider.ChoosenFileBackup = currentList;
            }
            SyncData();
            Open_SideSection(false);
        }
        private void ClearTableAction()
        {
            switch (GetCurrentDatabase())
            {
                case Enums.Databases.Products_Table:
                    if (B_DataGrid.Count == 0) return;
                    break;
                case Enums.Databases.Orders_Table:
                    if (B_CustomersDataGrid.Count == 0) return;
                    break;
            }
            MessageBoxResult result = System.Windows.MessageBox.Show("Are you sure you want to clear the table?",
                "Clear Table", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (LocalDataProvider.IsDefaultFile)
                    LocalDataProvider.DefaultSave([], GetCurrentDatabase());
                else
                    LocalDataProvider.IsDefaultFile = true;
                SyncData();
            }
        }
        private void AddNewProductAction()
        {
            if (B_NewProductToAdd == string.Empty || B_NewQuantityToAdd == string.Empty)
            {
                System.Windows.MessageBox.Show("Please fill all fields.", "Empty Fields", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(B_NewQuantityToAdd, out int _))
            {
                System.Windows.MessageBox.Show("Quantity is not valid.", "Invalid Quantity", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string? value = B_Products_QuantitiesList.FirstOrDefault(item => item.StartsWith(B_NewProductToAdd));
            if (value == null)
            {
                if (int.Parse(B_NewQuantityToAdd) > 0)
                    B_Products_QuantitiesList.Add($"{B_NewProductToAdd}: {B_NewQuantityToAdd}");
            }
            else
            {
                // modify the quantity
                string[] strings = value.Split(": ");
                if (int.Parse(B_NewQuantityToAdd) < 0 && int.Parse(strings[1]) + int.Parse(B_NewQuantityToAdd) > 0||
                    int.Parse(B_NewQuantityToAdd) > 0)
                {
                    string modifiedValue = $"{strings[0]}: {int.Parse(strings[1]) + int.Parse(B_NewQuantityToAdd)}";
                    int index = B_Products_QuantitiesList.IndexOf(value);
                    B_Products_QuantitiesList[index] = modifiedValue;
                }
            }
            B_NewProductToAdd = string.Empty;
            B_NewQuantityToAdd = string.Empty;
            B_PQListSelectedItem = null;
        }
        private void RemoveSelectedProductAction()
        {
            if (B_PQListSelectedItem == null) return;
            B_Products_QuantitiesList.Remove(B_PQListSelectedItem);
            B_PQListSelectedItem = null;
            B_NewQuantityToAdd = string.Empty;
            B_NewProductToAdd = string.Empty;
        }
        #endregion

        #region SearchBar
        private string b_SearchBarText = string.Empty;
        public string B_SearchBarText
        {
            get { return b_SearchBarText; }
            set
            {
                b_SearchBarText = value;
                OnPropertyChanged();
                B_SearchTextOpacity = B_SearchBarText == string.Empty ? .5 : 0;
                if (B_SearchBarText == string.Empty)
                {
                    if (!CanAutoSync)
                        CanAutoSync = true;
                    if (dataProvider is LocalDataProvider)
                        SyncData();
                    else
                        AutoSyncData();
                }
            }
        }

        private void SearchAction()
        {
            if (B_SearchBarText == string.Empty || B_SearchType == null) return;
            try
            {
                if (dataProvider is SQL_DataProvider)
                    CanAutoSync = false;
                List<IData>? items = [];
                items = dataProvider.SearchInventoryItem(B_SearchType.Content.ToString()!, B_SearchBarText, GetCurrentDatabase());
                SyncData(items);
            }
            catch
            {
                System.Windows.MessageBox.Show("An error occurred while searching.", "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
                B_SearchBarText = string.Empty;
            }
        }
        #endregion

        #region Load/Export Data
        private void ExportDataAction()
        {
            string[] MessageBoxText = ["No data to export.", "No Data"];
            switch (GetCurrentDatabase())
            {
                case Enums.Databases.Products_Table:
                    if (B_DataGrid.Count == 0)
                    {
                        System.Windows.MessageBox.Show(MessageBoxText[0], MessageBoxText[1], MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    break;
                case Enums.Databases.Orders_Table:
                    if (B_CustomersDataGrid.Count == 0)
                    {
                        System.Windows.MessageBox.Show(MessageBoxText[0], MessageBoxText[1], MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    break;
            }
            DataExporter.ExportData(GetCurrentDatabase() == Enums.Databases.Products_Table ? [.. B_DataGrid] : [.. B_CustomersDataGrid]);
        }
        private void LoadDataAction()
        {
            Enums.Databases? LoadDataDb = LocalDataProvider.ChooseDataToLoad();
            if (LoadDataDb == null) return;
            B_TableSelectorIndex = LoadDataDb == Enums.Databases.Products_Table ? 0 : 1;
            SyncData();
        }
        #endregion
    }
}