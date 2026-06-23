using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OyenGrooming
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;

    public partial class MainDashboard : Form
    {
        private Staff _currentStaff;

        // Collection #1 — List<Appointment> holds today's appointments
        private List<Appointment> _todayAppointments = new List<Appointment>();

        // Collection #2 — Dictionary maps form names to their Form objects (lazy-loaded)
        private Dictionary<string, Form> _openForms = new Dictionary<string, Form>();

        public MainDashboard(Staff staff)
        {
            InitializeComponent();
            _currentStaff = staff;
            SetupUI();
            SetupMouseEvents();
            SetupKeyboardShortcuts();
            LoadDashboardData();
            timerClock.Start();
        }

        // ─── SETUP ────────────────────────────────────────────────

        private void SetupUI()
        {
            lblGreeting.Text = GetGreeting() + ", " + _currentStaff.FullName + "!";
            lblDate.Text = DateTime.Now.ToString("dddd, dd MMM yyyy");
            lblUserInfo.Text = $"{_currentStaff.FullName} ({_currentStaff.Role})";
        }

        private string GetGreeting()
        {
            int hour = DateTime.Now.Hour;
            if (hour < 12) return "Good morning";
            if (hour < 17) return "Good afternoon";
            return "Good evening";
        }

        private void SetupMouseEvents()
        {
            // Highlight nav tiles on hover using lambda
            Button[] tiles = { btnTileCustomers, btnTileAppointments,
                           btnTileQueue, btnTileCheckout,
                           btnTileAnalytics };

            foreach (var tile in tiles)
            {
                tile.MouseEnter += (s, e) =>
                {
                    ((Button)s).FlatAppearance.BorderSize = 2;
                };
                tile.MouseLeave += (s, e) =>
                {
                    ((Button)s).FlatAppearance.BorderSize = 1;
                };
            }

            // Mouse click on DataGridView row
            dgvTodayAppts.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < _todayAppointments.Count)
                {
                    var appt = _todayAppointments[e.RowIndex];
                    lblStatusText.Text = $"Selected: {appt.GetSummary()}";
                }
            };
        }

        private void SetupKeyboardShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                // Ctrl+R = Refresh
                if (e.Control && e.KeyCode == Keys.R)
                {
                    LoadDashboardData();
                    e.Handled = true;
                }
                // Ctrl+N = New Appointment
                if (e.Control && e.KeyCode == Keys.N)
                {
                    btnTileAppointments_Click(s, e);
                    e.Handled = true;
                }
                // F5 = Refresh (alternate)
                if (e.KeyCode == Keys.F5)
                {
                    LoadDashboardData();
                    e.Handled = true;
                }
            };
        }

        // ─── DATA LOADING ─────────────────────────────────────────

        private void LoadDashboardData()
        {
            try
            {
                _todayAppointments = GetTodayAppointments();
                LoadStats();
                BindAppointmentsGrid();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error loading dashboard: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Appointment> GetTodayAppointments()
        {
            var list = new List<Appointment>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT a.AppointmentID, a.CustomerID, a.PetID, a.AppointmentDate,
                       a.TimeSlot, a.Status, a.ServiceType,
                       c.FullName AS CustomerName, p.Name AS PetName
                FROM Appointments a
                JOIN Customers c ON a.CustomerID = c.CustomerID
                JOIN Pets      p ON a.PetID      = p.PetID
                WHERE a.AppointmentDate = CAST(GETDATE() AS DATE)
                ORDER BY a.TimeSlot";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Appointment
                        {
                            ID = (int)reader["AppointmentID"],
                            CustomerID = (int)reader["CustomerID"],
                            PetID = (int)reader["PetID"],
                            AppointmentDate = (DateTime)reader["AppointmentDate"],
                            TimeSlot = reader["TimeSlot"].ToString(),
                            Status = reader["Status"].ToString(),
                            ServiceType = reader["ServiceType"].ToString(),
                            CustomerName = reader["CustomerName"].ToString(),
                            PetName = reader["PetName"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        private void LoadStats()
        {
            // LINQ #1 — count today's appointments
            int todayCount = _todayAppointments.Count;

            // LINQ #2 — count only Confirmed appointments today
            int confirmedCount = _todayAppointments
                .Where(a => a.Status == "Confirmed")
                .Count();

            // LINQ #3 — count Pending appointments
            int pendingCount = _todayAppointments
                .Where(a => a.Status == "Pending")
                .Count();

            lblStatAppointments.Text = todayCount.ToString();
            lblStatConfirmed.Text = confirmedCount.ToString();
            lblStatPending.Text = pendingCount.ToString();

            // Customer count and revenue come from DB
            lblStatCustomers.Text = GetTotalCustomerCount().ToString();
            lblStatRevenue.Text = "RM " + GetTodayRevenue().ToString("F2");
            lblStatQueue.Text = GetCurrentQueueCount().ToString();
        }

        private int GetTotalCustomerCount()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Customers WHERE IsActive = 1", conn);
                return (int)cmd.ExecuteScalar();
            }
        }

        private decimal GetTodayRevenue()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT ISNULL(SUM(TotalAmount), 0)
                             FROM Invoices
                             WHERE CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE)
                               AND Status = 'Paid'";
                var cmd = new SqlCommand(query, conn);
                return (decimal)cmd.ExecuteScalar();
            }
        }

        private int GetCurrentQueueCount()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"SELECT COUNT(*) FROM WalkInQueue
                             WHERE CAST(QueueDate AS DATE) = CAST(GETDATE() AS DATE)
                               AND Status = 'Waiting'";
                var cmd = new SqlCommand(query, conn);
                return (int)cmd.ExecuteScalar();
            }
        }

        private void BindAppointmentsGrid()
        {
            // LINQ #4 — project to anonymous type for display
            var display = _todayAppointments
                .Select(a => new
                {
                    Time = a.TimeSlot,
                    Pet = a.PetName,
                    Customer = a.CustomerName,
                    Service = a.ServiceType,
                    Status = a.Status
                })
                .ToList();

            dgvTodayAppts.DataSource = display;

            // Style the Status column with colors
            foreach (DataGridViewRow row in dgvTodayAppts.Rows)
            {
                string status = row.Cells["Status"].Value?.ToString();
                row.Cells["Status"].Style.ForeColor = status switch
                {
                    "Confirmed" => System.Drawing.Color.FromArgb(59, 109, 17),
                    "Pending" => System.Drawing.Color.FromArgb(133, 79, 11),
                    "Walk-in" => System.Drawing.Color.FromArgb(24, 95, 165),
                    _ => System.Drawing.Color.Gray
                };
            }
        }

        // ─── NAVIGATION ───────────────────────────────────────────

        // Lambda-style navigation helper — opens a form once, reuses if already open
        private void OpenForm<T>(string key, Func<T> factory) where T : Form
        {
            if (_openForms.ContainsKey(key) && !_openForms[key].IsDisposed)
            {
                _openForms[key].BringToFront();
            }
            else
            {
                T form = factory();
                _openForms[key] = form;
                form.FormClosed += (s, e) => _openForms.Remove(key);
                form.Show();
            }
        }

        private void btnTileCustomers_Click(object sender, EventArgs e)
            => OpenForm("customers", () => new CustomerPetProfileForm(_currentStaff));

        private void btnTileAppointments_Click(object sender, EventArgs e)
            => OpenForm("appointments", () => new AppointmentScheduleForm(_currentStaff));

        private void btnTileQueue_Click(object sender, EventArgs e)
            => OpenForm("queue", () => new WalkInQueueForm(_currentStaff));

        private void btnTileCheckout_Click(object sender, EventArgs e)
            => OpenForm("checkout", () => new CheckoutBillingForm(_currentStaff));

        private void btnTileAnalytics_Click(object sender, EventArgs e)
            => OpenForm("checkout", () => new AdminDashboardForm(_currentStaff));

        private void btnTileServices_Click(object sender, EventArgs e)
            => OpenForm("checkout", () => new CheckoutBillingForm(_currentStaff));

       
       

        // MenuStrip items mirror the tile buttons
        private void menuCustomers_Click(object sender, EventArgs e)
            => btnTileCustomers_Click(sender, e);

        private void menuAppointments_Click(object sender, EventArgs e)
            => btnTileAppointments_Click(sender, e);

      

     

        private void menuLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to logout?",
                "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Close all child forms
                foreach (var form in _openForms.Values)
                    if (!form.IsDisposed) form.Close();

                LoginForm login = new LoginForm();
                login.Show();
                this.Close();
            }
        }

        // ─── TOOLBAR ──────────────────────────────────────────────

        private void toolBtnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
            lblStatusText.Text = "Dashboard refreshed — " + DateTime.Now.ToString("HH:mm:ss");
        }

        private void toolBtnNewAppointment_Click(object sender, EventArgs e)
            => btnTileAppointments_Click(sender, e);

        private void toolBtnNewCustomer_Click(object sender, EventArgs e)
            => btnTileCustomers_Click(sender, e);

        private void toolBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                // LINQ #5 — get confirmed appointments for the print summary
                var confirmed = _todayAppointments
                    .Where(a => a.Status == "Confirmed")
                    .OrderBy(a => a.TimeSlot)
                    .Select(a => $"{a.TimeSlot}  {a.PetName}  ({a.CustomerName})")
                    .ToList();

                string report = "Today's Confirmed Appointments\n"
                              + "================================\n"
                              + string.Join("\n", confirmed);

                MessageBox.Show(report, "Print Preview",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── TIMER ────────────────────────────────────────────────

        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        public static class DatabaseHelper
        {
            // Storing your connection string securely in a central location
            private static readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=OyenGrooming;Integrated Security=True;Connect Timeout=30;";

            public static SqlConnection GetConnection()
            {
                return new SqlConnection(_connectionString);
            }
        }

        private void dgvTodayAppts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvTodayAppts.AutoGenerateColumns = false;
            dgvTodayAppts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Time", DataPropertyName = "Time", Width = 80 });
            dgvTodayAppts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Pet", DataPropertyName = "Pet", Width = 100 });
            dgvTodayAppts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Customer", DataPropertyName = "Customer", Width = 130 });
            dgvTodayAppts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Service", DataPropertyName = "Service", Width = 100 });
            dgvTodayAppts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 90 });
        }
    }
}
