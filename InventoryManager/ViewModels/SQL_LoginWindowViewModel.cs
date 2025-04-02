using InventoryManager.Classes;
using InventoryManager.DataManagement;
using InventoryManager.UtilitiesMetods;
using System.Windows;

namespace InventoryManager.ViewModels
{
    partial class SQL_LoginWindowViewModel
    {
        #region RelayCommands
        public RelayCommand ConfirmCommand => new(ExecutionContext => ConfirmAction());
        public RelayCommand DeleteSelectedSqlConnCommand => new(ExecutionContext => DeleteSelectedConnectionAction());
        #endregion

        private readonly Window window;

        public SQL_LoginWindowViewModel(Window _window)
        {
            window = _window;
            AutoFillOnLastConnection();
            PopulateConnectionList();
        }
        private void ConfirmAction()
        {
            string[] fields = [B_tbDbName, B_tbPassword, B_tbUserId, B_tbPassword];
            if (fields.All(string.IsNullOrEmpty)) return;
            SQL_Connection connection = new(B_tbServerIp, B_tbDbName, B_tbUserId, B_tbPassword);
            SQL_Manager.Connect(connection);
            if (B_SaveConnCheck && !SQL_Manager.SavedConnections.Any(
                conn => conn.Server == connection.Server && conn.DbName == connection.DbName && 
                conn.UserID == connection.UserID))
            {
                SQL_Manager.SavedConnections.Add(connection);
                SQL_Manager.UpdateSavedConnectionsFile();
            }
            window.Close();
        }
        private void AutoFillOnLastConnection()
        {
            if (SQL_Manager.LastConnection != null)
            {
                B_tbServerIp = SQL_Manager.LastConnection.Server;
                B_tbDbName = SQL_Manager.LastConnection.DbName;
                B_tbUserId = SQL_Manager.LastConnection.UserID;
                B_tbPassword = SQL_Manager.LastConnection.Password;
            }
        }
        private void PopulateConnectionList()
        {
            B_SqlConnectionsList.Clear();
            if (SQL_Manager.SavedConnections.Count > 0)
            {
                foreach (SQL_Connection conn in SQL_Manager.SavedConnections)
                {
                    B_SqlConnectionsList.Add($"{conn.Server}, {conn.DbName}, {conn.UserID}");
                }
            }
        }
        private void OnConnectionSelected()
        {
            if (B_SqlConnListSelected < 0) return;
            SQL_Connection conn = SQL_Manager.SavedConnections[B_SqlConnListSelected];
            B_tbServerIp = conn.Server;
            B_tbDbName = conn.DbName;
            B_tbUserId = conn.UserID;
            B_tbPassword = conn.Password;
            B_SaveConnCheck = false;
        }
        private void DeleteSelectedConnectionAction()
        {
            if (B_SqlConnListSelected >= 0)
            {
                SQL_Manager.SavedConnections.RemoveAt(B_SqlConnListSelected);
                PopulateConnectionList();
                SQL_Manager.UpdateSavedConnectionsFile();
                B_SaveConnCheck = false;
                B_SqlConnListSelected = -1;
                B_tbServerIp = string.Empty;
                B_tbDbName = string.Empty;
                B_tbUserId = string.Empty;
                B_tbPassword = string.Empty;
            }
        }
    }
}
