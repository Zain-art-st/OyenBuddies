using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace OyenGrooming
{
    public partial class LoginForm : Form
    {
        // Delegate for login success — used by MainDashboard to know who logged in
        public delegate void LoginSuccessHandler(Staff loggedInStaff);
        public event LoginSuccessHandler OnLoginSuccess;

        public LoginForm()
        {
            InitializeComponent();
            LoadRoles();
            SetupMouseEvents();
            SetupKeyboardEvents();
        }

        // ─── SETUP ────────────────────────────────────────────────

        private void LoadRoles()
        {
            cmbRole.Items.Clear();
            cmbRole.Items.AddRange(new[] { "Admin", "Groomer", "Receptionist" });
            cmbRoleRegister.Items.Clear();
            cmbRoleRegister.Items.AddRange(new[] { "Admin", "Groomer", "Receptionist" });
        }

        private void SetupMouseEvents()
        {
            // Mouse hover highlight on login button
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(15, 110, 86);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(29, 158, 117);

            btnRegister.MouseEnter += (s, e) => btnRegister.BackColor = Color.FromArgb(15, 110, 86);
            btnRegister.MouseLeave += (s, e) => btnRegister.BackColor = Color.FromArgb(29, 158, 117);
        }

        private void SetupKeyboardEvents()
        {
            // Press Enter on password field to trigger login
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                    btnLogin_Click(s, e);
            };

            // Escape clears fields
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    btnClearLogin_Click(s, e);
            };
        }

        // ─── LOGIN ────────────────────────────────────────────────

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Please enter username and password.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string hashed = Staff.HashPassword(txtPassword.Text.Trim());

                // OPTIONAL DEBUG LINE: Uncomment this next line if you want to see what your code is hashing!
                // MessageBox.Show($"Checking DB for Username: {txtUsername.Text.Trim()}\nPassword Hash: {hashed}");

                Staff staff = AuthenticateUser(txtUsername.Text.Trim(), hashed);

                if (staff != null)
                {
                    statusLabel.Text = $"Welcome, {staff.FullName}!";
                    OnLoginSuccess?.Invoke(staff); // fire delegate

                    // Route based on role
                    if (staff.Role == "Admin")
                    {
                        AdminDashboardForm adminDash = new AdminDashboardForm(staff);
                        adminDash.Show();
                    }
                    else
                    {
                        MainDashboard dashboard = new MainDashboard(staff);
                        dashboard.Show();
                    }

                    this.Hide();
                }
                else
                {
                    // ─── THIS WAS MISSING ───
                    // This will now execute if the database returns no matching user
                    MessageBox.Show("Invalid username or password.\n\nNote: If you manually added rows to the database, ensure the password is pre-hashed and 'IsActive' is set to 1.",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Staff AuthenticateUser(string username, string passwordHash)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT StaffID, FullName, Username, Email, Phone, Role, CreatedAt
                             FROM Staff
                             WHERE Username = @Username
                               AND PasswordHash = @PasswordHash
                               AND IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Staff
                            {
                                ID = (int)reader["StaffID"],
                                FullName = reader["FullName"].ToString(),
                                Username = reader["Username"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString(),
                                Role = reader["Role"].ToString(),
                                CreatedAt = (DateTime)reader["CreatedAt"]
                            };
                        }
                    }
                }
            }
            return null;
        }

        // ─── REGISTER ─────────────────────────────────────────────

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate raw fields BEFORE hashing the password
                if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                    string.IsNullOrWhiteSpace(txtRegUsername.Text) ||
                    string.IsNullOrWhiteSpace(txtEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtRegPassword.Text) ||
                    cmbRoleRegister.SelectedIndex == -1)
                {
                    MessageBox.Show("Please fill in all required fields and select a role.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!txtEmail.Text.Contains("@"))
                {
                    MessageBox.Show("Please enter a valid email address.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtRegPassword.Text != txtConfirmPassword.Text)
                {
                    MessageBox.Show("Passwords do not match.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtRegPassword.Text.Length < 6)
                {
                    MessageBox.Show("Password must be at least 6 characters.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Only build the Staff object (and hash the password) after all checks pass
                Staff newStaff = new Staff
                {
                    FullName = txtFullName.Text.Trim(),
                    Username = txtRegUsername.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Role = cmbRoleRegister.SelectedItem.ToString(),
                    PasswordHash = Staff.HashPassword(txtRegPassword.Text.Trim())
                };

                RegisterStaff(newStaff);
                MessageBox.Show($"Staff registered successfully!\n{newStaff.GetSummary()}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnClearRegister_Click(sender, e);
            }
            catch (SqlException ex) when (ex.Number == 2627) // unique constraint violation
            {
                MessageBox.Show("Username or email already exists.",
                    "Duplicate Entry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegisterStaff(Staff staff)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO Staff (FullName, Username, Email, Phone, PasswordHash, Role)
                             VALUES (@FullName, @Username, @Email, @Phone, @PasswordHash, @Role)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", staff.FullName);
                    cmd.Parameters.AddWithValue("@Username", staff.Username);
                    cmd.Parameters.AddWithValue("@Email", staff.Email);
                    cmd.Parameters.AddWithValue("@Phone", staff.Phone);
                    cmd.Parameters.AddWithValue("@PasswordHash", staff.PasswordHash);
                    cmd.Parameters.AddWithValue("@Role", staff.Role);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ─── CLEAR BUTTONS ────────────────────────────────────────

        private void btnClearLogin_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;
            chkRemember.Checked = false;
            txtUsername.Focus();
        }

        private void btnClearRegister_Click(object sender, EventArgs e)
        {
            txtFullName.Clear();
            txtRegUsername.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtRegPassword.Clear();
            txtConfirmPassword.Clear();
            cmbRoleRegister.SelectedIndex = -1;
            txtFullName.Focus();
        }

        // ─── PASSWORD VISIBILITY TOGGLE ───────────────────────────

        private void btnShowPassword_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
        }

        private void btnShowRegPassword_Click(object sender, EventArgs e)
        {
            txtRegPassword.UseSystemPasswordChar = !txtRegPassword.UseSystemPasswordChar;
            txtConfirmPassword.UseSystemPasswordChar = !txtConfirmPassword.UseSystemPasswordChar;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            this.Width += 150;
            this.Height += 100;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    // ─── DATABASE HELPER ──────────────────────────────────────────
    public static class DatabaseHelper
    {
        // Storing your connection string securely in a central location
        private static readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=OyenGrooming;Integrated Security=True;Connect Timeout=30;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}