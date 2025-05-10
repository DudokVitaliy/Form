using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StoreManagement
{
    public partial class EditProductForm : Form
    {
        string connectionString = "your_connection_string_here";
        int productId;

        public EditProductForm(int id)
        {
            InitializeComponent();
            productId = id;
            LoadProductData();
        }

        private void LoadProductData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductName, Price, Quantity, CategoryName, SupplierName " +
                               "FROM Products " +
                               "JOIN Categories ON Products.CategoryID = Categories.CategoryID " +
                               "JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID " +
                               "WHERE ProductID = @ProductID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", productId);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    textBoxProductName.Text = reader["ProductName"].ToString();
                    numericUpDownPrice.Value = Convert.ToDecimal(reader["Price"]);
                    numericUpDownQuantity.Value = Convert.ToInt32(reader["Quantity"]);
                    comboBoxCategory.SelectedItem = reader["CategoryName"].ToString();
                    comboBoxSupplier.SelectedItem = reader["SupplierName"].ToString();
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            string productName = textBoxProductName.Text;
            decimal price = numericUpDownPrice.Value;
            int quantity = (int)numericUpDownQuantity.Value;
            string category = comboBoxCategory.SelectedItem.ToString();
            string supplier = comboBoxSupplier.SelectedItem.ToString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Products SET ProductName = @ProductName, Price = @Price, " +
                               "Quantity = @Quantity, CategoryID = (SELECT CategoryID FROM Categories WHERE CategoryName = @Category), " +
                               "SupplierID = (SELECT SupplierID FROM Suppliers WHERE SupplierName = @Supplier) " +
                               "WHERE ProductID = @ProductID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Category", category);
                command.Parameters.AddWithValue("@Supplier", supplier);
                command.Parameters.AddWithValue("@ProductID", productId);

                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Product updated successfully!");
                this.Close(); // Закриваємо форму після оновлення
            }
        }
    }
}
