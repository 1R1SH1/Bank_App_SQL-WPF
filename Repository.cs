using System.Data;
using System.Data.SqlClient;

namespace Bank_App_SQL_WPF
{
    internal class Repository
    {
        private SqlConnection con = new SqlConnection(
            @"Data Source = (localdb)\MSSQLLocalDB; 
              Initial Catalog = BankA; 
              Integrated Security = True;");

        public void LoadClientData(DataTable dt)
        {
            SqlCommand cmd = new("Select * From Client", con);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
        }

        public void LoadDepositData(DataTable dt, string client)
        {
            SqlCommand cmd = new("Select d.ClientId, d.DepositNumber, d.AmountFunds, d.DepositType " +
                                         "FROM Deposit AS d " +
                                         "JOIN Client AS c ON d.ClientId = c.Id " +
                                         $"WHERE c.Id = '{client}';", con);
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();
        }
    }
}
