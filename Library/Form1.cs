using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;



namespace Library
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string connectionString = ConfigurationManager.ConnectionStrings["LibraryDBConnection"].ConnectionString;
        private void LoadBooks()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT b.Id, b.Title, a.FullName AS Author, c.Name AS Category, b.Price, b.Quantity " +
                               "FROM Books b " +
                               "JOIN BooksAuthors ba ON b.Id = ba.BookId " +
                               "JOIN Authors a ON ba.AuthorId = a.Id " +
                               "JOIN Categories c ON b.CategoryId = c.Id";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridViewBooks.DataSource = dt;
            }
        }

        private void LoadCategories()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name FROM Categories";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                comboBoxCategories.DataSource = dt;
                comboBoxCategories.DisplayMember = "Name";
                comboBoxCategories.ValueMember = "Id";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadBooks();
            LoadCategories();
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Books (Title, CategoryId, Price, Quantity) VALUES (@Title, @CategoryId, @Price, @Quantity)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Title", textBoxTitle.Text);
                cmd.Parameters.AddWithValue("@CategoryId", comboBoxCategories.SelectedValue);
                cmd.Parameters.AddWithValue("@Price", decimal.Parse(textBoxPrice.Text));
                cmd.Parameters.AddWithValue("@Quantity", int.Parse(textBoxQuantity.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();

                LoadBooks();
            }
        }
        private void dataGridViewBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewBooks.Rows[e.RowIndex];
                textBoxTitle.Text = row.Cells["Title"].Value.ToString();
                textBoxPrice.Text = row.Cells["Price"].Value.ToString();
                textBoxQuantity.Text = row.Cells["Quantity"].Value.ToString();
                comboBoxCategories.SelectedValue = row.Cells["CategoryId"].Value;
            }
        }

        private void btnUpdateBook_Click(object sender, EventArgs e)
        {
            if (dataGridViewBooks.CurrentRow != null)
            {
                int bookId = Convert.ToInt32(dataGridViewBooks.CurrentRow.Cells["Id"].Value);
                string title = textBoxTitle.Text;
                decimal price = decimal.Parse(textBoxPrice.Text);
                int quantity = int.Parse(textBoxQuantity.Text);
                int categoryId = (int)comboBoxCategories.SelectedValue;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "update Books set Title=@Title, Price=@Price, Quantity=@Quantity, CategoryId=@CategoryId where Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    cmd.Parameters.AddWithValue("@Id", bookId);
                    cmd.ExecuteNonQuery();
                }

                LoadBooks();
                MessageBox.Show("Book updates!");
            }
        }

        private void btnDeleteBook_Click(object sender, EventArgs e)
        {
            if (dataGridViewBooks.CurrentRow != null)
            {
                int bookId = Convert.ToInt32(dataGridViewBooks.CurrentRow.Cells["Id"].Value);

                var confirm = MessageBox.Show("Ви дійсно хочете видалити цю книгу?", "Підтвердження", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "delete from Books where Id=@Id";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Id", bookId);
                        cmd.ExecuteNonQuery();
                    }

                    LoadBooks();
                    MessageBox.Show("Книгу видалено");
                }
            }
        }
    }
}
