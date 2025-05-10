using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StoreManagement
{
    public partial class AddProductForm : Form
    {
        string connectionString = "your_connection_string_here";

        public AddProductForm()
        {
            InitializeComponent();
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
                string query = "INSERT INTO Products (ProductName, Price, Quantity, CategoryID, SupplierID) " +
                               "VALUES (@ProductName, @Price, @Quantity, " +
                               "(SELECT CategoryID FROM Categories WHERE CategoryName = @Category), " +
                               "(SELECT SupplierID FROM Suppliers WHERE SupplierName = @Supplier))";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@ProductName", productName);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Category", category);
                command.Parameters.AddWithValue("@Supplier", supplier);

                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Product added successfully!");
                this.Close(); // Закриваємо форму після додавання
            }
        }
    }
}
