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
                    dgvAdminData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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

        private void btnToggleStatus_Click(object sender, EventArgs e)
        {
            // 1. Ensure we are looking at the Staff Directory
            if (cmbDataView.SelectedItem?.ToString() != "Staff Directory")
            {
                MessageBox.Show("Please select the 'Staff Directory' view from the dropdown first.",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. Ensure a staff member is selected
            if (dgvAdminData.CurrentRow == null)
            {
                MessageBox.Show("Please select a staff member from the list.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Get data from the selected row
                DataGridViewRow row = dgvAdminData.CurrentRow;
                int staffId = Convert.ToInt32(row.Cells["StaffID"].Value);
                string fullName = row.Cells["FullName"].Value.ToString();
                bool isActive = Convert.ToBoolean(row.Cells["IsActive"].Value);

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // 3. THE CONSTRAINT: If trying to DEACTIVATE, check for active jobs first
                    if (isActive)
                    {
                        // This SQL counts active Appointments AND active Walk-in Queue jobs
                        string checkSql = @"
                    SELECT 
                        (SELECT COUNT(*) FROM Appointments WHERE GroomerID = @ID AND Status IN ('Pending', 'Confirmed')) +
                        (SELECT COUNT(*) FROM WalkInQueue WHERE GroomerID = @ID AND Status IN ('Waiting', 'Serving'))";

                        using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@ID", staffId);
                            int activeJobs = (int)checkCmd.ExecuteScalar();

                            if (activeJobs > 0)
                            {
                                MessageBox.Show($"{fullName} still has {activeJobs} active appointment(s) or walk-in(s).\n\nPlease find a replacement or complete their jobs first.",
                                    "Cannot Deactivate", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return; // Stop the deactivation!
                            }
                        }
                    }

                    // 4. If safe (or if we are activating them), toggle the status
                    string updateSql = "UPDATE Staff SET IsActive = @NewStatus WHERE StaffID = @ID";
                    using (SqlCommand cmd = new SqlCommand(updateSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewStatus", !isActive); // Flip the boolean
                        cmd.Parameters.AddWithValue("@ID", staffId);
                        cmd.ExecuteNonQuery();
                    }

                    string statusText = !isActive ? "Activated" : "Deactivated";
                    MessageBox.Show($"{fullName} has been successfully {statusText}.",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 5. Refresh the grid to show the new status
                    LoadData("Staff Directory");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating staff status: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}