using InventoryManager.Classes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace InventoryManager.ViewModels
{
    partial class SQL_LoginWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? PropertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        private string b_tbServerIp = string.Empty;
        public string B_tbServerIp
        {
            get { return b_tbServerIp; }
            set { b_tbServerIp = value; OnPropertyChanged(); }
        }
        private string b_tbDbName = string.Empty;
        public string B_tbDbName
        {
            get { return b_tbDbName; }
            set { b_tbDbName = value; OnPropertyChanged(); }
        }
        private string b_tbUserId = string.Empty;
        public string B_tbUserId
        {
            get { return b_tbUserId; }
            set { b_tbUserId = value; OnPropertyChanged(); }
        }
        private string b_tbPassword = string.Empty;
        public string B_tbPassword
        {
            get { return b_tbPassword; }
            set { b_tbPassword = value; OnPropertyChanged(); }
        }
        private bool b_SaveConnCheck;
        public bool B_SaveConnCheck
        {
            get { return b_SaveConnCheck; }
            set { b_SaveConnCheck = value; OnPropertyChanged(); }
        }
        private int b_SqlConnListSelected;
        public int B_SqlConnListSelected
        {
            get { return b_SqlConnListSelected; }
            set { b_SqlConnListSelected = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> B_SqlConnectionsList { get; set; } = [];
    }
}
