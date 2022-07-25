using Saving_InfoLog_ClassLibrary;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bank_App_SQL_WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private InfoLog _log = new();
        private SavingMethod _savingInfoLogs = new();
        private Repository _repository = new();
        DataTable dt;

        public event Action<string> Transaction;

        private SqlConnection con = new SqlConnection(
            @"Data Source = (localdb)\MSSQLLocalDB; 
              Initial Catalog = BankA; 
              Integrated Security = True;");

        public MainWindow()
        {
            InitializeComponent();
            Transaction += LogRepository_Transaction;
            LoadClientDataToCB();
            infoList.ItemsSource = _log.log;
            clientList.Items.Refresh();
        }

        private void LogRepository_Transaction(string message)
        {
            _log.AddToLog(message);
            infoList.Items.Refresh();
            _savingInfoLogs.SaveInfoLog(_log.log);
        }

        private void LoadClientDataToCB()
        {
            dt = new();
            dt.Clear();
            _repository.LoadClientData(dt);
            clientList.Items.Refresh();
            clientList.ItemsSource = dt.DefaultView;
        }

        private void Button_AddFunds_Clients_Click(object sender, RoutedEventArgs e)
        {
            string deposit = ((DataRowView)depositList.SelectedItem).Row["DepositNumber"].ToString();
            con.Open();
            SqlCommand cmd = new("Update Deposit " +
                                 "set AmountFunds += '" + amountFundsTextBox.Text + "' " +
                                 $"Where DepositNumber = '{deposit}'", con);
            try
            {
                cmd.ExecuteNonQuery();

                con.Close();
                Transaction?.Invoke($"Счёт №{deposit} пополнен на $'{amountFundsTextBox.Text}' ");
                CleareTextBox();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void Button_Transfer_Clients_Click(object sender, RoutedEventArgs e)
        {
            string senders = ((DataRowView)depositList.SelectedItem).Row["DepositNumber"].ToString();
            con.Open();
            SqlCommand cmdS = new("Update Deposit " +
                                 "set AmountFunds -= '" + amountTransferTextBox.Text + "' " +
                                 $"Where DepositNumber = '{senders}'", con);

            SqlCommand cmdR = new("Update Deposit " +
                                 "set AmountFunds += '" + amountTransferTextBox.Text + "' " +
                                 $"Where DepositNumber = '" + transferToTextBox.Text + "' ", con);
            try
            {
                cmdS.ExecuteNonQuery();
                cmdR.ExecuteNonQuery();
                con.Close();
                Transaction?.Invoke($"Перевод со счёта №{senders} на счёт №{transferToTextBox.Text} на сумму $'{amountTransferTextBox.Text}' ");
                CleareTextBox();

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void ClientInfo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string client = ((DataRowView)clientList.SelectedItem).Row["Id"].ToString();
            try
            {
                dt = new();
                dt.Clear();
                _repository.LoadDepositData(dt, client);
                depositList.Items.Refresh();
                depositList.ItemsSource = dt.DefaultView;
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemAddFunds_OnClick(object sender, RoutedEventArgs e)
        {
            pAddFunds.IsOpen = true;
        }

        private void MenuItemTransfer_OnClick(object sender, RoutedEventArgs e)
        {
            pTransfer.IsOpen = true;
        }

        private void MenuItem_Click_About(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Банк_А_Версия_3.0_SQL", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItem_Click_Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_Refresh(object sender, RoutedEventArgs e)
        {
            clientList.Items.Refresh();
        }

        private void DepositList_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                ContextMenu cm = this.FindResource("CmButton") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.IsOpen = true;
            }
        }

        private void Button_Ok_AddClient(object sender, RoutedEventArgs e)
        {
            try
            {
                dt = new();
                dt.Clear();
                SqlCommand cmd = new("INSERT INTO Client VALUES(@Name, @SurName, @Patronymic) SET @Id = @@IDENTITY;", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@Id", SqlDbType.Int, 4, "Id").Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@Name", Name_txt.Text);
                cmd.Parameters.AddWithValue("@SurName", SurName_txt.Text);
                cmd.Parameters.AddWithValue("@Patronymic", Patronymic_txt.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                Transaction?.Invoke($"Добавлен новый клиент {Name_txt.Text} {SurName_txt.Text} {Patronymic_txt.Text}");
                dt.Clear();
                LoadClientDataToCB();
                CleareTextBox();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void ClientList_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                ContextMenu cm = this.FindResource("CmButton") as ContextMenu;
                cm.PlacementTarget = sender as Button;
                cm.IsOpen = true;
            }
        }

        private void MenuItem_Add_Client_Click(object sender, RoutedEventArgs e)
        {
            pAddClient.IsOpen = true;
        }

        private void MenuItem_Add_Deposit_Click(object sender, RoutedEventArgs e)
        {
            pAddDeposit.IsOpen = true;
        }

        private void Button_Ok_AddDeposit(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlCommand cmd = new("INSERT INTO Deposit VALUES(@ClientId, @DepositNumber, @AmountFunds, @DepositType);", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ClientId", ClientId_txt.Text);
                cmd.Parameters.AddWithValue("@DepositNumber", DepositNumber_txt.Text);
                cmd.Parameters.AddWithValue("@AmountFunds", AmountFunds_txt.Text);
                cmd.Parameters.AddWithValue("@DepositType", DepositType_txt.Text);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                Transaction?.Invoke($"Добавлен новый счёт: №{DepositNumber_txt.Text} {DepositType_txt.Text}");
                CleareTextBox();

            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void MenuItem_Delete_Client_Click(object sender, RoutedEventArgs e)
        {
            pDeleteClient.IsOpen = true;
        }

        private void MenuItem_Close_Deposit_Click(object sender, RoutedEventArgs e)
        {
            pCloseDeposit.IsOpen = true;
        }

        private void Button_Ok_DeleteClient(object sender, RoutedEventArgs e)
        {
            dt = new();
            dt.Clear();
            con.Open();
            SqlCommand cmd = new("Delete From Client Where Id = " + Id_Delete_txt.Text + " ", con);
            try
            {
                cmd.ExecuteNonQuery();
                con.Close();
                Transaction?.Invoke($"Клиент {Id_Delete_txt.Text} удалён");
                dt.Clear();
                LoadClientDataToCB();
                CleareTextBox();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void Button_Ok_CloseDeposit(object sender, RoutedEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new("Delete From Deposit Where DepositNumber = " + DepositNumber_Close_txt.Text + " ", con);
            try
            {
                cmd.ExecuteNonQuery();
                con.Close();
                Transaction?.Invoke($"Счёт {DepositNumber_Close_txt.Text} закрыт");
                CleareTextBox();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private void CleareTextBox()
        {
            try
            {
                Name_txt.Clear();
                SurName_txt.Clear();
                Patronymic_txt.Clear();
                ClientId_txt.Clear();
                DepositNumber_txt.Clear();
                AmountFunds_txt.Clear();
                DepositType_txt.Clear();
                Id_Delete_txt.Clear();
                DepositNumber_Close_txt.Clear();
                amountFundsTextBox.Clear();
                amountTransferTextBox.Clear();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
