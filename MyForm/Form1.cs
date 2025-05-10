using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace StoreManagement
{
    public partial class MainForm : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["LibraryDB"].ConnectionString;

        public MainForm()
        {
            InitializeComponent();
            LoadProducts(); // Завантажуємо продукти при відкритті форми
        }

        private void LoadProducts(string filter = "", string filterValue = "")
        {
            string query = "SELECT ProductID, ProductName, Price, Quantity, CategoryName, SupplierName " +
                           "FROM Products " +
                           "JOIN Categories ON Products.CategoryID = Categories.CategoryID " +
                           "JOIN Suppliers ON Products.SupplierID = Suppliers.SupplierID";

            // Додавання фільтрації, якщо це необхідно
            if (!string.IsNullOrEmpty(filter) && !string.IsNullOrEmpty(filterValue))
            {
                if (filter == "Category")
                    query += " WHERE CategoryName LIKE @FilterValue";
                else if (filter == "Supplier")
                    query += " WHERE SupplierName LIKE @FilterValue";
                else if (filter == "Price")
                    query += " WHERE Price <= @FilterValue";
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    if (!string.IsNullOrEmpty(filterValue))
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@FilterValue", "%" + filterValue + "%");

                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    dataGridViewProducts.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private void buttonApplyFilter_Click(object sender, EventArgs e)
        {
            string filter = comboBoxFilter.SelectedItem.ToString();
            string filterValue = textBoxFilterValue.Text;
            LoadProducts(filter, filterValue);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Реалізуйте код для додавання нового товару (можна використовувати форму для введення даних)
            AddProductForm addProductForm = new AddProductForm();
            addProductForm.ShowDialog();
            LoadProducts(); // Завантажуємо продукти після додавання
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                int productId = Convert.ToInt32(dataGridViewProducts.SelectedRows[0].Cells[0].Value);
                EditProductForm editProductForm = new EditProductForm(productId);
                editProductForm.ShowDialog();
                LoadProducts(); // Завантажуємо продукти після редагування
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0)
            {
                int productId = Convert.ToInt32(dataGridViewProducts.SelectedRows[0].Cells[0].Value);

                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Products WHERE ProductID = @ProductID";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@ProductID", productId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    LoadProducts(); // Завантажуємо продукти після видалення
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting product: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.");
            }
        }
    }
}
