using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace LibreryApp
{
    public partial class Form1 : Form
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["LibraryDBConnectionString"].ConnectionString;

        public Form1()
        {
            InitializeComponent();
        }

        // Завантажуємо книги з бази даних з фільтрацією
        private void LoadBooks(string filterCategory = "", decimal? minPrice = null, decimal? maxPrice = null)
        {
            string query = "SELECT b.Id AS BookId, b.Title AS BookTitle, a.FullName AS Author, b.Price, b.Quantity, c.Name AS Category " +
                           "FROM Books b " +
                           "INNER JOIN BooksAuthors ba ON b.Id = ba.BookId " +
                           "INNER JOIN Authors a ON ba.AuthorId = a.Id " +
                           "INNER JOIN Categories c ON b.CategoryId = c.Id " +
                           "WHERE 1 = 1";

            if (!string.IsNullOrEmpty(filterCategory))
            {
                query += " AND c.Name = @category";
            }

            if (minPrice.HasValue)
            {
                query += " AND b.Price >= @minPrice";
            }

            if (maxPrice.HasValue)
            {
                query += " AND b.Price <= @maxPrice";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                da.SelectCommand.Parameters.AddWithValue("@category", filterCategory ?? (object)DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@minPrice", minPrice ?? (object)DBNull.Value);
                da.SelectCommand.Parameters.AddWithValue("@maxPrice", maxPrice ?? (object)DBNull.Value);

                DataTable dt = new DataTable();
                da.Fill(dt);

                // Прив'язка даних до DataGridView
                dataGridViewBooks.DataSource = dt;
            }
        }

        // Завантажуємо категорії для фільтрації
        private void LoadCategories()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Name FROM Categories", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                comboBoxFilter.DataSource = dt;
                comboBoxFilter.DisplayMember = "Name";
            }
        }

        // Завантажуємо книги при старті форми
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadBooks();
        }

        // Фільтрація за категорією
        private void comboBoxFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCategory = comboBoxFilter.SelectedItem?.ToString();
            LoadBooks(filterCategory: selectedCategory);
        }

        // Редагування книги
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridViewBooks.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a book to edit.");
                return;
            }

            int bookId = (int)dataGridViewBooks.SelectedRows[0].Cells["BookId"].Value;
            string title = textBoxTitle.Text;
            decimal price = numericUpDownPrice.Value;
            int quantity = (int)numericUpDownQuantity.Value;
            string category = comboBoxFilter.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(title) || category == null)
            {
                MessageBox.Show("Please provide all fields.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Books SET Title = @Title, Price = @Price, Quantity = @Quantity, " +
                               "CategoryId = (SELECT Id FROM Categories WHERE Name = @Category) " +
                               "WHERE Id = @BookId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@BookId", bookId);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Quantity", quantity);
                cmd.Parameters.AddWithValue("@Category", category);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            LoadBooks(); // Перезавантажити список книг
        }

        // Оновлення налаштувань DataGridView
        private void CustomizeGrid()
        {
            dataGridViewBooks.Columns["BookId"].HeaderText = "Book ID";
            dataGridViewBooks.Columns["BookTitle"].HeaderText = "Book Title";
            dataGridViewBooks.Columns["Author"].HeaderText = "Author";
            dataGridViewBooks.Columns["Price"].HeaderText = "Price";
            dataGridViewBooks.Columns["Quantity"].HeaderText = "Quantity";
            dataGridViewBooks.Columns["Category"].HeaderText = "Category";

            // Наприклад, зміна формату для ціни
            dataGridViewBooks.Columns["Price"].DefaultCellStyle.Format = "C2"; // Формат як валюту
            dataGridViewBooks.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Вирівнювання по центру
        }
    }
}
