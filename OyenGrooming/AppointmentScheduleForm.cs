using OyenGrooming;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OyenGrooming
{
    public partial class AppointmentScheduleForm : Form
    {
        private Staff _currentStaff;
        private List<Appointment> _appointments = new List<Appointment>();
        private List<Customer> _customers = new List<Customer>();
        private List<Pet> _pets = new List<Pet>();
        private List<Service> _services = new List<Service>();
        private List<Staff> _groomers = new List<Staff>();

        // Collection #5 — SortedDictionary maps time slots to appointments for the day
        private SortedDictionary<string, Appointment> _slotMap
            = new SortedDictionary<string, Appointment>();

        private DateTime _selectedDate = DateTime.Today;
        private int _selectedApptID = -1;

        // Delegate for appointment status change
        public delegate void AppointmentStatusChanged(int appointmentID, string newStatus);
        public event AppointmentStatusChanged OnStatusChanged;

        // Fixed time slots for the day
        private readonly List<string> _timeSlots = new List<string>
    {
        "09:00","09:30","10:00","10:30","11:00","11:30",
        "12:00","12:30","13:00","13:30","14:00","14:30",
        "15:00","15:30","16:00","16:30","17:00"
    };

        private readonly HashSet<string> _blockedSlots = new HashSet<string>
    {
        "12:00", "12:30" // lunch break
    };

        public AppointmentScheduleForm(Staff staff)
        {
            InitializeComponent();
            _currentStaff = staff;
            SetupMouseEvents();
            SetupKeyboardEvents();
            SetupCalendar();
            LoadDropdownData();
            LoadAppointmentsForDate(_selectedDate);
        }

        // ─── SETUP ────────────────────────────────────────────────

        private void SetupCalendar()
        {
            calAppointments.MaxSelectionCount = 1;
            calAppointments.DateSelected += (s, e) =>
            {
                _selectedDate = calAppointments.SelectionStart;
                LoadAppointmentsForDate(_selectedDate);
            };
        }

        private void SetupMouseEvents()
        {
            // Clicking a slot panel auto-fills the time combobox
            flowSlots.MouseClick += (s, e) => { };
        }

        private void SetupKeyboardEvents()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.S) SaveAppointment();
                if (e.Control && e.KeyCode == Keys.N) btnNew_Click(s, e);
                if (e.KeyCode == Keys.F5) LoadAppointmentsForDate(_selectedDate);
                if (e.KeyCode == Keys.Escape) ClearForm();
            };

            // When customer changes, reload their pets
            cmbCustomer.SelectedIndexChanged += (s, e) => LoadPetsForCustomer();
        }

        // ─── LOAD DROPDOWN DATA ───────────────────────────────────

        private void LoadDropdownData()
        {
            try
            {
                LoadCustomers();
                LoadServices();
                LoadGroomers();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCustomers()
        {
            _customers.Clear();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT CustomerID, FullName, Phone FROM Customers WHERE IsActive=1 ORDER BY FullName";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        _customers.Add(new Customer
                        {
                            ID = (int)r["CustomerID"],
                            FullName = r["FullName"].ToString(),
                            Phone = r["Phone"].ToString()
                        });
                    }
                }
            }

            cmbCustomer.DisplayMember = "FullName";
            cmbCustomer.ValueMember = "ID";
            cmbCustomer.DataSource = new List<Customer>(_customers);

            // Also populate filter dropdown
            var filterList = new List<Customer> { new Customer { ID = -1, FullName = "All Customers" } };
            filterList.AddRange(_customers);
            cmbFilterGroomer.DisplayMember = "FullName";
            cmbFilterGroomer.ValueMember = "ID";
        }

        private void LoadPetsForCustomer()
        {
            try
            {
                if (cmbCustomer.SelectedValue == null) return;
                int customerID = (int)cmbCustomer.SelectedValue;

                _pets.Clear();
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT PetID, Name, Species FROM Pets WHERE CustomerID=@CID AND IsActive=1";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CID", customerID);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                _pets.Add(new Pet
                                {
                                    ID = (int)r["PetID"],
                                    Name = r["Name"].ToString(),
                                    Species = r["Species"].ToString()
                                });
                            }
                        }
                    }
                }

                cmbPet.DisplayMember = "Name";
                cmbPet.ValueMember = "ID";
                cmbPet.DataSource = new List<Pet>(_pets);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error loading pets: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadServices()
        {
            _services.Clear();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT ServiceID, ServiceName, Price FROM Services WHERE IsActive=1 ORDER BY ServiceName";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        _services.Add(new Service
                        {
                            ID = (int)r["ServiceID"],
                            ServiceName = r["ServiceName"].ToString(),
                            Price = (decimal)r["Price"]
                        });
                    }
                }
            }

            cmbService.DisplayMember = "ServiceName";
            cmbService.ValueMember = "ID";
            cmbService.DataSource = new List<Service>(_services);
        }

        private void LoadGroomers()
        {
            _groomers.Clear();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "SELECT StaffID, FullName FROM Staff WHERE Role='Groomer' AND IsActive=1 ORDER BY FullName";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        _groomers.Add(new Staff
                        {
                            ID = (int)r["StaffID"],
                            FullName = r["FullName"].ToString()
                        });
                    }
                }
            }

            cmbGroomer.DisplayMember = "FullName";
            cmbGroomer.ValueMember = "ID";
            cmbGroomer.DataSource = new List<Staff>(_groomers);
        }

        // ─── LOAD APPOINTMENTS FOR SELECTED DATE ──────────────────

        private void LoadAppointmentsForDate(DateTime date)
        {
            try
            {
                _appointments = GetAppointmentsByDate(date);
                BuildSlotMap();
                RenderSlots();
                UpdateStatusBar();
                lblDateHeader.Text = date.ToString("dddd, dd MMMM yyyy");
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Appointment> GetAppointmentsByDate(DateTime date)
        {
            var list = new List<Appointment>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                SELECT a.AppointmentID, a.CustomerID, a.PetID, a.GroomerID,
                       a.AppointmentDate, a.TimeSlot, a.Status,
                       a.ServiceType, a.Notes, a.CreatedAt,
                       c.FullName AS CustomerName,
                       p.Name     AS PetName,
                       p.Species  AS PetSpecies,
                       s.FullName AS GroomerName
                FROM Appointments a
                JOIN Customers c ON a.CustomerID = c.CustomerID
                JOIN Pets      p ON a.PetID      = p.PetID
                LEFT JOIN Staff s ON a.GroomerID = s.StaffID
                WHERE a.AppointmentDate = @Date
                ORDER BY a.TimeSlot";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Date", date.Date);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new Appointment
                            {
                                ID = (int)r["AppointmentID"],
                                CustomerID = (int)r["CustomerID"],
                                PetID = (int)r["PetID"],
                                AppointmentDate = (DateTime)r["AppointmentDate"],
                                TimeSlot = r["TimeSlot"].ToString(),
                                Status = r["Status"].ToString(),
                                ServiceType = r["ServiceType"].ToString(),
                                CustomerName = r["CustomerName"].ToString(),
                                PetName = r["PetName"].ToString(),
                                CreatedAt = (DateTime)r["CreatedAt"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        private void BuildSlotMap()
        {
            _slotMap.Clear();

            // LINQ #12 — group appointments by time slot into dictionary
            var grouped = _appointments
                .GroupBy(a => a.TimeSlot)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var pair in grouped)
                _slotMap[pair.Key] = pair.Value;
        }

        // ─── RENDER TIME SLOTS ────────────────────────────────────

        private void RenderSlots()
        {
            flowSlots.Controls.Clear();

            foreach (string slot in _timeSlots)
            {
                Panel row = new Panel
                {
                    Width = flowSlots.Width - 20,
                    Height = 36,
                    Margin = new Padding(0, 0, 0, 3)
                };

                Label timeLabel = new Label
                {
                    Text = slot,
                    Width = 50,
                    Height = 36,
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.FromArgb(170, 168, 160),
                    Font = new Font("Segoe UI", 9)
                };

                Button slotBtn = new Button
                {
                    Left = 58,
                    Width = row.Width - 60,
                    Height = 34,
                    Top = 1,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9),
                    Tag = slot
                };
                slotBtn.FlatAppearance.BorderSize = 1;

                if (_blockedSlots.Contains(slot))
                {
                    slotBtn.Text = "⛔ Lunch Break";
                    slotBtn.BackColor = Color.FromArgb(241, 239, 232);
                    slotBtn.ForeColor = Color.FromArgb(180, 178, 170);
                    slotBtn.Enabled = false;
                }
                else if (_slotMap.ContainsKey(slot))
                {
                    Appointment appt = _slotMap[slot];
                    slotBtn.Text = $"🐾 {appt.PetName}  —  {appt.ServiceType}     {appt.CustomerName}     [{appt.Status}]";

                    // AFTER — works on any C# version
                    if (appt.Status == "Confirmed")
                        slotBtn.BackColor = Color.FromArgb(225, 245, 238);
                    else if (appt.Status == "Pending")
                        slotBtn.BackColor = Color.FromArgb(250, 238, 218);
                    else if (appt.Status == "Walk-in")
                        slotBtn.BackColor = Color.FromArgb(230, 241, 251);
                    else if (appt.Status == "Completed")
                        slotBtn.BackColor = Color.FromArgb(234, 243, 222);
                    else
                        slotBtn.BackColor = Color.WhiteSmoke;

                    // Click to populate form
                    slotBtn.Click += (s, e) => PopulateFormFromAppointment(appt);

                    // Double-click to go straight to edit mode
                    slotBtn.DoubleClick += (s, e) =>
                    {
                        PopulateFormFromAppointment(appt);
                        SetEditMode(true);
                    };

                    // Hover effect
                    slotBtn.MouseEnter += (s, e) =>
                        ((Button)s).FlatAppearance.BorderColor = Color.FromArgb(29, 158, 117);
                    slotBtn.MouseLeave += (s, e) =>
                        ((Button)s).FlatAppearance.BorderColor = Color.FromArgb(211, 209, 199);
                }
                else
                {
                    slotBtn.Text = "Available — click to book";
                    slotBtn.BackColor = Color.FromArgb(248, 248, 246);
                    slotBtn.ForeColor = Color.FromArgb(180, 178, 170);
                    slotBtn.Font = new Font("Segoe UI", 9, FontStyle.Italic);

                    // Click empty slot to pre-fill time in form
                    slotBtn.Click += (s, e) =>
                    {
                        cmbTimeSlot.SelectedItem = slot;
                        btnNew_Click(s, e);
                    };

                    slotBtn.MouseEnter += (s, e) =>
                    {
                        ((Button)s).BackColor = Color.FromArgb(225, 245, 238);
                        ((Button)s).ForeColor = Color.FromArgb(15, 110, 86);
                    };
                    slotBtn.MouseLeave += (s, e) =>
                    {
                        ((Button)s).BackColor = Color.FromArgb(248, 248, 246);
                        ((Button)s).ForeColor = Color.FromArgb(180, 178, 170);
                    };
                }

                row.Controls.Add(timeLabel);
                row.Controls.Add(slotBtn);
                flowSlots.Controls.Add(row);
            }
        }

        // ─── SAVE APPOINTMENT ─────────────────────────────────────

        private void SaveAppointment()
        {
            try
            {
                if (cmbCustomer.SelectedValue == null ||
                    cmbPet.SelectedValue == null ||
                    cmbService.SelectedValue == null ||
                    cmbTimeSlot.SelectedItem == null)
                {
                    MessageBox.Show("Please fill in Customer, Pet, Service, and Time Slot.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // LINQ #13 — check if selected slot is already taken
                string chosenSlot = cmbTimeSlot.SelectedItem.ToString();
                bool slotTaken = _appointments
                    .Any(a => a.TimeSlot == chosenSlot && a.ID != _selectedApptID
                           && a.Status != "Cancelled");

                if (slotTaken)
                {
                    MessageBox.Show($"Time slot {chosenSlot} is already booked for this date.",
                        "Slot Taken", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var appt = new Appointment
                {
                    ID = _selectedApptID,
                    CustomerID = (int)cmbCustomer.SelectedValue,
                    PetID = (int)cmbPet.SelectedValue,
                    AppointmentDate = dtpAppointmentDate.Value.Date,
                    TimeSlot = chosenSlot,
                    ServiceType = cmbService.Text,
                    Status = cmbStatus.SelectedItem?.ToString() ?? "Pending"
                };

                if (!appt.Validate())
                {
                    MessageBox.Show("Please check that the date is today or in the future.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    if (_selectedApptID == -1) // INSERT
                    {
                        string sql = @"INSERT INTO Appointments
                        (CustomerID, PetID, GroomerID, AppointmentDate, TimeSlot, ServiceType, Status, Notes)
                        VALUES (@CID, @PID, @GID, @Date, @Slot, @Service, @Status, @Notes)";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@CID", appt.CustomerID);
                            cmd.Parameters.AddWithValue("@PID", appt.PetID);
                            cmd.Parameters.AddWithValue("@GID", cmbGroomer.SelectedValue ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Date", appt.AppointmentDate);
                            cmd.Parameters.AddWithValue("@Slot", appt.TimeSlot);
                            cmd.Parameters.AddWithValue("@Service", appt.ServiceType);
                            cmd.Parameters.AddWithValue("@Status", appt.Status);
                            cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Appointment booked successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else // UPDATE
                    {
                        string sql = @"UPDATE Appointments
                        SET CustomerID=@CID, PetID=@PID, GroomerID=@GID,
                            AppointmentDate=@Date, TimeSlot=@Slot,
                            ServiceType=@Service, Status=@Status, Notes=@Notes
                        WHERE AppointmentID=@ID";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@CID", appt.CustomerID);
                            cmd.Parameters.AddWithValue("@PID", appt.PetID);
                            cmd.Parameters.AddWithValue("@GID", cmbGroomer.SelectedValue ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@Date", appt.AppointmentDate);
                            cmd.Parameters.AddWithValue("@Slot", appt.TimeSlot);
                            cmd.Parameters.AddWithValue("@Service", appt.ServiceType);
                            cmd.Parameters.AddWithValue("@Status", appt.Status);
                            cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                            cmd.Parameters.AddWithValue("@ID", appt.ID);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Appointment updated!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                // Fire delegate to notify dashboard of status change
                OnStatusChanged?.Invoke(_selectedApptID, appt.Status);

                LoadAppointmentsForDate(_selectedDate);
                ClearForm();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── STATUS ACTIONS ───────────────────────────────────────

        private void MarkComplete()
        {
            try
            {
                if (_selectedApptID == -1)
                {
                    MessageBox.Show("Please select an appointment first.",
                        "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show("Mark this appointment as Completed?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                UpdateAppointmentStatus(_selectedApptID, "Completed");
                OnStatusChanged?.Invoke(_selectedApptID, "Completed");
                LoadAppointmentsForDate(_selectedDate);
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelAppointment()
        {
            try
            {
                if (_selectedApptID == -1)
                {
                    MessageBox.Show("Please select an appointment first.",
                        "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show("Cancel this appointment?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                UpdateAppointmentStatus(_selectedApptID, "Cancelled");
                OnStatusChanged?.Invoke(_selectedApptID, "Cancelled");
                LoadAppointmentsForDate(_selectedDate);
                ClearForm();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateAppointmentStatus(int apptID, string status)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Appointments SET Status=@Status WHERE AppointmentID=@ID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ID", apptID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ─── HELPERS ──────────────────────────────────────────────

        private void PopulateFormFromAppointment(Appointment appt)
        {
            _selectedApptID = appt.ID;
            cmbCustomer.SelectedValue = appt.CustomerID;
            LoadPetsForCustomer();
            cmbPet.SelectedValue = appt.PetID;
            cmbTimeSlot.SelectedItem = appt.TimeSlot;
            cmbStatus.SelectedItem = appt.Status;
            dtpAppointmentDate.Value = appt.AppointmentDate;
            lblStatusRecord.Text = $"Selected: {appt.PetName} at {appt.TimeSlot}";
        }

        private void ClearForm()
        {
            _selectedApptID = -1;
            cmbCustomer.SelectedIndex = -1;
            cmbPet.DataSource = null;
            cmbService.SelectedIndex = -1;
            cmbGroomer.SelectedIndex = -1;
            cmbTimeSlot.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0;
            dtpAppointmentDate.Value = DateTime.Today;
            txtNotes.Clear();
        }

        private void SetEditMode(bool editing) { /* enable/disable controls */ }

        private void UpdateStatusBar()
        {
            // LINQ #14 — count available slots
            int booked = _slotMap.Count;
            int available = _timeSlots.Count(t => !_slotMap.ContainsKey(t) && !_blockedSlots.Contains(t));

            lblStatusAppts.Text = $"{booked} appointment(s)  |  {available} slot(s) available";
        }

        // ─── TOOLBAR BUTTON EVENTS ────────────────────────────────

        private void btnNew_Click(object sender, EventArgs e) { ClearForm(); }
        private void btnSave_Click(object sender, EventArgs e) { SaveAppointment(); }
        private void btnComplete_Click(object sender, EventArgs e) { MarkComplete(); }
        private void btnCancel_Click(object sender, EventArgs e) { CancelAppointment(); }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadAppointmentsForDate(_selectedDate); }
        private void btnBack_Click(object sender, EventArgs e) { this.Close(); }

        private void cmbFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string filter = cmbFilterStatus.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(filter) || filter == "All Status")
                {
                    RenderSlots();
                    return;
                }

                // LINQ #15 — filter displayed slots by status
                var filtered = _appointments
                    .Where(a => a.Status == filter)
                    .ToList();

                _appointments = filtered;
                BuildSlotMap();
                RenderSlots();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filter error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Toolbar: New | Edit | Cancel | Complete | Refresh | Print | Back
            switch (e.ClickedItem.Text)
            {
                case "New":      btnNew_Click(sender, e);      break;
                case "Cancel":   btnCancel_Click(sender, e);   break;
                case "Complete": btnComplete_Click(sender, e); break;
                case "Refresh":  btnRefresh_Click(sender, e);  break;
                case "Back":     btnBack_Click(sender, e);     break;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
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
    }
}