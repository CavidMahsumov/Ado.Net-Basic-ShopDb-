using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ado.Net_Task1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable table;
        SqlConnection conn;
        string cs = "";
        SqlDataReader reader;
        DataSet dataSet;
        SqlDataAdapter dataadapter;
        DataRowView DataRowView;
        string ProductName;
        string ProductId;
        string price;
        string Description;
        int count;
        int count1;
        string customerid;

        public MainWindow()
        {
            InitializeComponent();
            conn = new SqlConnection();
            cs = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            TableCombobox.Items.Add("Products");
            TableCombobox.Items.Add("Orders");
            TableCombobox.Items.Add("Customers");
            TableCombobox.Items.Add("OrderDetailes");
        }
        public void RefreshTable()
        {

            using (conn = new SqlConnection())
            {

                MyGrid.ItemsSource = null;
                SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(dataadapter);
                dataadapter.Fill(dataSet);

                MyGrid.ItemsSource = dataSet.Tables[0].DefaultView;

                sqlCommandBuilder.GetUpdateCommand();


            }
        }

        public void RefreshAfterRemove()
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = cs;
                conn.Open();
                SqlCommand command = new SqlCommand();
                command.CommandText = $"select*from  {TableCombobox.SelectedItem.ToString()} ";
                command.Connection = conn;
                table = new DataTable();

                bool hasColumnAdded = false;
                using (reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        if (!hasColumnAdded)
                        {

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                table.Columns.Add(reader.GetName(i));
                            }
                            hasColumnAdded = true;
                        }
                        DataRow row = table.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader[i];
                        }
                        table.Rows.Add(row);
                    }
                    MyGrid.ItemsSource = table.DefaultView;

                }



            }
        }

        private void TableCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = cs;

                dataSet = new DataSet();
                dataadapter = new SqlDataAdapter($@"select *from {TableCombobox.SelectedItem.ToString()}", conn);


                RefreshTable();

            }
        }

        private void MyGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView = MyGrid.SelectedItem as DataRowView;

            try
            {
                if (TableCombobox.SelectedItem.ToString() == "Products")
                {

                    if (DataRowView != null)
                    {
                        ProductId = DataRowView["Id"].ToString();
                        ProductName = DataRowView["Name"].ToString();
                        price = DataRowView["Price"].ToString();
                        Description = DataRowView["Description"].ToString();

                    }
                }
                else if (TableCombobox.SelectedItem.ToString() == "Customers")
                {

                    if (DataRowView != null)
                    {
                        customerid = DataRowView["Id"].ToString();
                      

                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = cs;
                conn.Open();

                ++count;
                SqlCommand command = new SqlCommand("sp_InsertOrder", conn);
                command.CommandType = CommandType.StoredProcedure;
                var param1 = new SqlParameter();
                param1.Value = count;
                param1.ParameterName = "@OrderId";
                param1.SqlDbType = SqlDbType.Int;
                command.Parameters.Add(param1);


                var param2 = new SqlParameter();
                param2.Value = int.Parse(ProductId);
                param2.ParameterName = "@ProductiD";
                param2.SqlDbType = SqlDbType.Int;
                command.Parameters.Add(param2);


                var param3 = new SqlParameter();
                param3.Value = 1;
                param3.ParameterName = "@CustomerId";
                param3.SqlDbType = SqlDbType.Int;
                command.Parameters.Add(param3);


                command.ExecuteNonQuery();
                ++count1;

                SqlCommand command1 = new SqlCommand("sp_InsertOrderDetails", conn);
                command1.CommandType = CommandType.StoredProcedure;





                var c3p = new SqlParameter();
                c3p.Value = ProductName;
                c3p.ParameterName = @"ProductName";
                c3p.SqlDbType = SqlDbType.NVarChar;
                command1.Parameters.Add(c3p);

                var c4p = new SqlParameter();
                c4p.Value = price;
                c4p.ParameterName = "@ProductPrice";
                c4p.SqlDbType = SqlDbType.NVarChar;
                command1.Parameters.Add(c4p);

                var c5p = new SqlParameter();
                c5p.Value = Description;
                c5p.ParameterName = "@ProductDescription";
                c5p.SqlDbType = SqlDbType.NVarChar;
                command1.Parameters.Add(c5p);

                command1.ExecuteNonQuery();

            }

            // RefreshTable();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = cs;
                conn.Open();

                if (TableCombobox.SelectedItem.ToString() == "Products")
                {


                    SqlCommand command = new SqlCommand("sp_DeleteProduct", conn);
                    command.CommandType = CommandType.StoredProcedure;

                    var param1 = new SqlParameter();
                    param1.Value = ProductId;
                    param1.ParameterName = "@ProductId";
                    param1.SqlDbType = SqlDbType.Int;
                    command.Parameters.Add(param1);

                    command.ExecuteNonQuery();


                    RefreshAfterRemove();
                }
                else if (TableCombobox.SelectedItem.ToString() == "Customers")
                {
                    SqlCommand command = new SqlCommand("sp_DeleteCustomer", conn);
                    command.CommandType = CommandType.StoredProcedure;

                    var param1 = new SqlParameter();
                    param1.Value = customerid;
                    param1.ParameterName = "@id";
                    param1.SqlDbType = SqlDbType.Int;
                    command.Parameters.Add(param1);

                    command.ExecuteNonQuery();


                    RefreshAfterRemove();
                }
                else
                {
                    MessageBox.Show("Only Delete Customer And Products");
                }
            }

        }
    }
}

