using OyenGrooming;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace OyenGrooming
{
    public partial class AdminDashboardForm : Form
    {
        private Staff _currentAdmin;

        public AdminDashboardForm(Staff admin)
        {
            InitializeComponent();
            _currentAdmin = admin;

            // Setup UI
            lblAdminName.Text = $"Admin Access: {_currentAdmin.FullName}";
            SetupComboBox();
            LoadRevenueStat();
        }

        private void SetupComboBox()
        {
            cmbDataView.Items.AddRange(new string[] {
            "Staff Directory",
            "All Invoices (Financials)",
            "All Appointments",
            "Customer Registry"
        });

            // Trigger the data load when selection changes
            cmbDataView.SelectedIndexChanged += (s, e) =>
            {
                LoadData(cmbDataView.SelectedItem.ToString());
            };

            // Select the first item by default
            cmbDataView.SelectedIndex = 0;
        }

        private void LoadData(string viewType)
        {
            string query = "";

            // Map the dropdown selection to specific SQL queries
            switch (viewType)
            {
                case "Staff Directory":
                    query = "SELECT StaffID, FullName, Username, Role, IsActive, CreatedAt FROM Staff";
                    break;
                case "All Invoices (Financials)":
                    query = "SELECT InvoiceID, TotalAmount, TaxAmt, PaymentMethod, Status, PaidAt FROM Invoices";
                    break;
                case "All Appointments":
                    query = "SELECT AppointmentID, CustomerID, PetID, AppointmentDate, Status, ServiceType FROM Appointments";
                    break;
                case "Customer Registry":
                    query = "SELECT CustomerID, FullName, Phone, Email, CreatedAt FROM Customers";
                    break;
            }

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    // SqlDataAdapter automatically handles opening and closing the connection
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvAdminData.DataSource = dt;
                    dgvAdminData.ReadOnly = true;
                    dgvAdminData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Admin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRevenueStat()
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT ISNULL(SUM(TotalAmount), 0) FROM Invoices WHERE Status = 'Paid'";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        decimal totalRev = (decimal)cmd.ExecuteScalar();
                        lblTotalRevenue.Text = $"Total System Revenue: RM {totalRev:F2}";
                    }
                }
            }
            catch { /* Handle silently for UI */ }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.Show();
            this.Close();
        }
    }
}