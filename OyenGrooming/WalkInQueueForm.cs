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
    public partial class WalkInQueueForm : Form
    {
        private Staff _currentStaff;

        
        private Queue<QueueEntry> _liveQueue = new Queue<QueueEntry>();

     
        private List<QueueEntry> _allEntries = new List<QueueEntry>();

        private List<Customer> _customers = new List<Customer>();
        private List<Pet> _pets = new List<Pet>();
        private List<Service> _services = new List<Service>();
        private List<Staff> _groomers = new List<Staff>();

        // Delegate "Serving"
        public delegate void QueueEntryServing(QueueEntry entry);
        public event QueueEntryServing OnEntryServing;

        public WalkInQueueForm(Staff staff)
        {
            InitializeComponent();
            _currentStaff = staff;
            SetupMouseEvents();
            SetupKeyboardEvents();
            LoadDropdownData();
            LoadTodayQueue();
            timerRefresh.Interval = 30000; // 30s
            timerRefresh.Start();
        }
        private void SetupMouseEvents()
        {
            // Hover effect 
            btnCheckIn.MouseEnter += (s, e) =>
                btnCheckIn.BackColor = Color.FromArgb(15, 110, 86);
            btnCheckIn.MouseLeave += (s, e) =>
                btnCheckIn.BackColor = Color.FromArgb(29, 158, 117);

            // When customer changes reload their pets
            cmbCustomer.SelectedIndexChanged += (s, e) => LoadPetsForCustomer();
        }

        private void SetupKeyboardEvents()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                // Enter on customer combo 
                if (e.KeyCode == Keys.Enter && cmbCustomer.Focused)
                {
                    cmbPet.Focus();
                    e.Handled = true;
                }if (e.KeyCode == Keys.F2) CallNext();
                if (e.KeyCode == Keys.F5) LoadTodayQueue();
                if (e.KeyCode == Keys.Escape) ClearForm();
            };
        }



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
                    while (r.Read())
                        _customers.Add(new Customer
                        {
                            ID = (int)r["CustomerID"],
                            FullName = r["FullName"].ToString(),
                            Phone = r["Phone"].ToString()
                        });
            }
            cmbCustomer.DisplayMember = "FullName";
            cmbCustomer.ValueMember = "ID";
            cmbCustomer.DataSource = new List<Customer>(_customers);
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
                            while (r.Read())
                                _pets.Add(new Pet
                                {
                                    ID = (int)r["PetID"],
                                    Name = r["Name"].ToString(),
                                    Species = r["Species"].ToString()
                                });
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
                string sql = "SELECT ServiceID, ServiceName FROM Services WHERE IsActive=1 ORDER BY ServiceName";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                        _services.Add(new Service
                        {
                            ID = (int)r["ServiceID"],
                            ServiceName = r["ServiceName"].ToString()
                        });
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
                    while (r.Read())
                        _groomers.Add(new Staff
                        {
                            ID = (int)r["StaffID"],
                            FullName = r["FullName"].ToString()
                        });
            }

            // "Auto-assign" option 
            var groomerList = new List<Staff>
            { new Staff { ID = -1, FullName = "Auto-assign" } };
            groomerList.AddRange(_groomers);

            cmbGroomer.DisplayMember = "FullName";
            cmbGroomer.ValueMember = "ID";
            cmbGroomer.DataSource = groomerList;
        }


        private void LoadTodayQueue()
        {
            try
            {
                _allEntries = GetTodayQueueFromDB();
                RebuildLiveQueue();
                RenderQueueCards();
                UpdateStats();
                UpdateNextQueueNumber();
                lblLastUpdated.Text = "Last updated: " + DateTime.Now.ToString("HH:mm:ss");
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

        private List<QueueEntry> GetTodayQueueFromDB()
        {
            var list = new List<QueueEntry>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"
                SELECT q.QueueID, q.CustomerID, q.PetID, q.ServiceID,
                       q.GroomerID, q.QueueNumber, q.Status,
                       q.CheckInTime, q.ServedTime, q.Notes, q.CreatedAt,
                       c.FullName  AS CustomerName,
                       p.Name      AS PetName,
                       sv.ServiceName,
                       st.FullName AS GroomerName
                FROM WalkInQueue q
                JOIN Customers c  ON q.CustomerID = c.CustomerID
                JOIN Pets      p  ON q.PetID      = p.PetID
                LEFT JOIN Services sv ON q.ServiceID  = sv.ServiceID
                LEFT JOIN Staff    st ON q.GroomerID  = st.StaffID
                WHERE q.QueueDate = CAST(GETDATE() AS DATE)
                ORDER BY q.QueueNumber";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new QueueEntry
                        {
                            ID = (int)r["QueueID"],
                            CustomerID = (int)r["CustomerID"],
                            PetID = (int)r["PetID"],
                            QueueNumber = (int)r["QueueNumber"],
                            Status = r["Status"].ToString(),
                            CheckInTime = (DateTime)r["CheckInTime"],
                            ServedTime = r["ServedTime"] as DateTime?,
                            Notes = r["Notes"].ToString(),
                            CustomerName = r["CustomerName"].ToString(),
                            PetName = r["PetName"].ToString(),
                            ServiceName = r["ServiceName"].ToString(),
                            GroomerName = r["GroomerName"].ToString(),
                            CreatedAt = (DateTime)r["CreatedAt"]
                        });
                    }
                }
            }
            return list;
        }

        private void RebuildLiveQueue()
        {
            _liveQueue.Clear();

            // LINQ filter only Waiting entries
            var waiting = _allEntries
                .Where(e => e.Status == "Waiting")
                .OrderBy(e => e.QueueNumber)
                .ToList();

            foreach (var entry in waiting)
                _liveQueue.Enqueue(entry);
        }


        private void RenderQueueCards()
        {
            flowQueue.Controls.Clear();

            foreach (var entry in _allEntries)
            {
                Panel card = BuildQueueCard(entry);
                flowQueue.Controls.Add(card);
            }
        }

        private Panel BuildQueueCard(QueueEntry entry)
        {
            bool isServing = entry.Status == "Serving";
            bool isDone = entry.Status == "Done" || entry.Status == "Left";

            Panel card = new Panel
            {
                Width = flowQueue.Width - 20,
                Height = 54,
                BackColor = isServing ? Color.FromArgb(225, 245, 238)
                          : isDone ? Color.FromArgb(248, 248, 246)
                          : Color.White,
                Margin = new Padding(0, 0, 0, 5)
            };
            card.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics,
                    card.ClientRectangle,
                    isServing ? Color.FromArgb(29, 158, 117) : Color.FromArgb(211, 209, 199),
                    ButtonBorderStyle.Solid);
            };

            // Queue number badge
            Label numLbl = new Label
            {
                Text = isDone ? "✓" : $"#{entry.QueueNumber}",
                Left = 8,
                Top = 9,
                Width = 36,
                Height = 36,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = isServing ? Color.White
                          : isDone ? Color.FromArgb(59, 109, 17)
                          : Color.FromArgb(29, 158, 117),
                BackColor = isServing ? Color.FromArgb(29, 158, 117)
                          : isDone ? Color.FromArgb(234, 243, 222)
                          : Color.FromArgb(241, 239, 232)
            };

            // Pet name & service
            Label infoLbl = new Label
            {
                Text = $"🐾 {entry.PetName}  —  {entry.ServiceName}",
                Left = 52,
                Top = 8,
                Width = 260,
                Height = 18,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = isDone ? Color.FromArgb(170, 168, 160) : Color.FromArgb(44, 44, 42),
                AutoSize = false
            };

            // Owner & groomer
            Label subLbl = new Label
            {
                Text = $"{entry.CustomerName}  ·  " +
                            (string.IsNullOrWhiteSpace(entry.GroomerName)
                                ? "Unassigned" : "Groomer: " + entry.GroomerName),
                Left = 52,
                Top = 27,
                Width = 260,
                Height = 16,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(136, 135, 128),
                AutoSize = false
            };

            // Wait time
            Label waitLbl = new Label
            {
                Text = isDone
                    ? $"Done {entry.ServedTime:HH:mm}"
                    : entry.WaitDuration + " waiting",
                Left = 315,
                Top = 18,
                Width = 80,
                Height = 18,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(136, 135, 128),
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false
            };

            card.Controls.AddRange(new Control[] { numLbl, infoLbl, subLbl, waitLbl });

            // Action buttons
            if (!isDone)
            {
                if (isServing)
                {
                    Button doneBtn = CreateCardButton("✅ Done", 400, entry, "Done");
                    card.Controls.Add(doneBtn);
                }
                else
                {
                    Button serveBtn = CreateCardButton("▶ Serve", 395, entry, "Serving");
                    Button removeBtn = CreateCardButton("✖", 440, entry, "Left");
                    removeBtn.ForeColor = Color.FromArgb(163, 45, 45);
                    removeBtn.Width = 30;
                    card.Controls.Add(serveBtn);
                    card.Controls.Add(removeBtn);
                }
            }

            // Mouse hover 
            card.MouseEnter += (s, e) =>
            {
                if (!isServing && !isDone)
                    card.BackColor = Color.FromArgb(248, 252, 250);
            };
            card.MouseLeave += (s, e) =>
            {
                if (!isServing && !isDone)
                    card.BackColor = Color.White;
            };

            return card;
        }

        private Button CreateCardButton(string text, int left, QueueEntry entry, string targetStatus)
        {
            Button btn = new Button
            {
                Text = text,
                Left = left,
                Top = 14,
                Width = 60,
                Height = 26,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(95, 94, 90),
                BackColor = Color.FromArgb(241, 239, 232),
                Tag = entry
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(211, 209, 199);
            btn.Click += (s, e) => UpdateQueueStatus(entry, targetStatus);
            return btn;
        }


        private void btnCheckIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbCustomer.SelectedValue == null || cmbPet.SelectedValue == null)
                {
                    MessageBox.Show("Please select a Customer and Pet.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var entry = new QueueEntry
                {
                    CustomerID = (int)cmbCustomer.SelectedValue,
                    PetID = (int)cmbPet.SelectedValue,
                    ServiceID = cmbService.SelectedValue as int?,
                    GroomerID = cmbGroomer.SelectedValue is int gid && gid > 0
                                    ? (int?)gid : null,
                    Notes = txtNotes.Text.Trim(),
                    CheckInTime = DateTime.Now
                };

                if (!entry.Validate())
                {
                    MessageBox.Show("Invalid entry. Please check your selections.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int newQueueNumber = GetNextQueueNumber();
                entry.QueueNumber = newQueueNumber;

                InsertQueueEntry(entry);
                _liveQueue.Enqueue(entry);

                MessageBox.Show($"Checked in! Queue number: #{newQueueNumber}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearForm();
                LoadTodayQueue();
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

        private void CallNext()
        {
            try
            {
                if (_liveQueue.Count == 0)
                {
                    MessageBox.Show("No one in the queue right now.",
                        "Empty Queue", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Dequeue
                QueueEntry next = _liveQueue.Dequeue();
                UpdateQueueStatus(next, "Serving");
                OnEntryServing?.Invoke(next);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calling next: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateQueueStatus(QueueEntry entry, string newStatus)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"UPDATE WalkInQueue
                               SET Status    = @Status,
                                   ServedTime = CASE WHEN @Status IN ('Done','Serving') THEN GETDATE() ELSE ServedTime END
                               WHERE QueueID = @ID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@ID", entry.ID);
                        cmd.ExecuteNonQuery();
                    }
                }

                LoadTodayQueue();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertQueueEntry(QueueEntry entry)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO WalkInQueue
                (CustomerID, PetID, ServiceID, GroomerID, QueueNumber, Status, CheckInTime, Notes)
                VALUES (@CID, @PID, @SID, @GID, @QNum, 'Waiting', @CheckIn, @Notes)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CID", entry.CustomerID);
                    cmd.Parameters.AddWithValue("@PID", entry.PetID);
                    cmd.Parameters.AddWithValue("@SID", (object)entry.ServiceID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GID", (object)entry.GroomerID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@QNum", entry.QueueNumber);
                    cmd.Parameters.AddWithValue("@CheckIn", entry.CheckInTime);
                    cmd.Parameters.AddWithValue("@Notes", entry.Notes ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
        }



        private void UpdateStats()
        {
            // LINQ count by status
            int waiting = _allEntries.Count(e => e.Status == "Waiting");
            int serving = _allEntries.Count(e => e.Status == "Serving");
            int done = _allEntries.Count(e => e.Status == "Done");

            lblWaiting.Text = waiting.ToString();
            lblServing.Text = serving.ToString();
            lblDone.Text = done.ToString();

            // LINQ average wait time 
            var avgWait = _allEntries
                .Where(e => e.Status == "Done" && e.ServedTime.HasValue)
                .Select(e => (e.ServedTime.Value - e.CheckInTime).TotalMinutes)
                .DefaultIfEmpty(0)
                .Average();

            lblAvgWait.Text = $"~{(int)avgWait}m";

            lblQueueCount.Text = $"{_liveQueue.Count} in queue";
        }

        private void UpdateNextQueueNumber()
        {
            int next = GetNextQueueNumber();
            txtNotes.Text = $"#{next:D3}";
            lblNextNumber.Text = $"#{next:D3}";
        }

        private int GetNextQueueNumber()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT ISNULL(MAX(QueueNumber), 0) + 1
                           FROM WalkInQueue
                           WHERE QueueDate = CAST(GETDATE() AS DATE)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                    return (int)cmd.ExecuteScalar();
            }
        }


        private void UpdateGroomerPanel()
        {
            try
            {
                // LINQ find groomers
                var busyGroomerIDs = _allEntries
                    .Where(e => e.Status == "Serving" && e.GroomerID.HasValue)
                    .Select(e => e.GroomerID.Value)
                    .ToHashSet();

                flowGroomers.Controls.Clear();

                foreach (var groomer in _groomers)
                {
                    bool isBusy = busyGroomerIDs.Contains(groomer.ID);

                    Panel row = new Panel { Width = 150, Height = 24, Margin = new Padding(0, 0, 0, 3) };

                    Panel dot = new Panel
                    {
                        Width = 8,
                        Height = 8,
                        Left = 0,
                        Top = 8,
                        BackColor = isBusy ? Color.FromArgb(239, 159, 39)
                                           : Color.FromArgb(29, 158, 117)
                    };
                    Label lbl = new Label
                    {
                        Text = groomer.FullName + (isBusy ? " — Busy" : " — Free"),
                        Left = 14,
                        Top = 4,
                        Width = 130,
                        Height = 18,
                        Font = new Font("Segoe UI", 8.5f),
                        ForeColor = Color.FromArgb(95, 94, 90)
                    };

                    row.Controls.Add(dot);
                    row.Controls.Add(lbl);
                    flowGroomers.Controls.Add(row);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating groomer panel: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void ClearForm()
        {
            cmbCustomer.SelectedIndex = -1;
            cmbPet.DataSource = null;
            cmbService.SelectedIndex = -1;
            cmbGroomer.SelectedIndex = 0;
            txtNotes.Clear();
        }


        private void btnCallNext_Click(object sender, EventArgs e) => CallNext();
        private void btnRefresh_Click(object sender, EventArgs e) => LoadTodayQueue();
        private void btnBack_Click(object sender, EventArgs e) => this.Close();
        private void btnClear_Click(object sender, EventArgs e) => ClearForm();

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadTodayQueue();
            }
            catch (Exception ex)
            {
                lblLastUpdated.Text = $"Refresh error: {ex.Message}";
            }
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
            private static readonly string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=OyenGrooming;Integrated Security=True;Connect Timeout=30;";

            public static SqlConnection GetConnection()
            {
                return new SqlConnection(_connectionString);
            }
        }
    }
}