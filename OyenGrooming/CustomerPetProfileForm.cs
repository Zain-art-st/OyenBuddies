using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OyenGrooming
{
    public partial class CustomerPetProfileForm : Form
    {
        private Staff _currentStaff;

        // Collection #3 — List<Customer>
        private List<Customer> _customers = new List<Customer>();

        // Collection #4 — List<Pet>
        private List<Pet> _pets = new List<Pet>();

        private int _selectedCustomerID = -1;
        private int _selectedPetID = -1;
        private bool _isEditing = false;

        public CustomerPetProfileForm(Staff staff)
        {
            InitializeComponent();
            _currentStaff = staff;
            SetupGrid();
            SetupMouseEvents();
            SetupKeyboardEvents();
            LoadAllData();
        }

        // ─── SETUP ────────────────────────────────────────────────

        private void SetupGrid()
        {
            // Customer grid columns
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "#", DataPropertyName = "ID", Width = 36 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Full Name", DataPropertyName = "FullName", Width = 130 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", DataPropertyName = "Phone", Width = 110 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", DataPropertyName = "Email", Width = 150 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "PetCount", HeaderText = "Pets", DataPropertyName = "PetCount", Width = 45 });
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;

            // Pet grid columns
            dgvPets.AutoGenerateColumns = false;
            dgvPets.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "#", DataPropertyName = "ID", Width = 36 });
            dgvPets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Icon", HeaderText = "Icon", DataPropertyName = "Icon", Width = 36 });
            dgvPets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Name", DataPropertyName = "Name", Width = 90 });
            dgvPets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Species", HeaderText = "Species", DataPropertyName = "Species", Width = 70 });
            dgvPets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Breed", HeaderText = "Breed", DataPropertyName = "Breed", Width = 100 });
            dgvPets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Age", HeaderText = "Age", DataPropertyName = "Age", Width = 44 });
            dgvPets.Columns.Add(new DataGridViewTextBoxColumn { Name = "Owner", HeaderText = "Owner", DataPropertyName = "Owner", Width = 110 });
            dgvPets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPets.MultiSelect = false;
            dgvPets.ReadOnly = true;

            // Status dropdown — must be populated before anything sets SelectedIndex/SelectedItem on it
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new object[] { "Active", "Inactive" });
        }

        private void SetupMouseEvents()
        {
            // Customer row selected — populate form
            dgvCustomers.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SelectCustomerRow(e.RowIndex);
            };

            // Pet row selected — populate form
            dgvPets.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    SelectPetRow(e.RowIndex);
            };

            // Double-click to enter edit mode
            dgvCustomers.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    SelectCustomerRow(e.RowIndex);
                    SetEditMode(true);
                }
            };

            dgvPets.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    SelectPetRow(e.RowIndex);
                    SetEditMode(true);
                }
            };
        }

        private void SetupKeyboardEvents()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                // Delete key on grid = delete selected record
                if (e.KeyCode == Keys.Delete)
                {
                    if (tabMain.SelectedTab == tabCustomers && _selectedCustomerID > 0)
                        DeleteCustomer();
                    else if (tabMain.SelectedTab == tabPets && _selectedPetID > 0)
                        DeletePet();
                }
                // Ctrl+S = Save
                if (e.Control && e.KeyCode == Keys.S)
                {
                    if (tabMain.SelectedTab == tabCustomers)
                        SaveCustomer();
                    else
                        SavePet();
                }
                // Ctrl+N = New record
                if (e.Control && e.KeyCode == Keys.N)
                    btnNew_Click(s, e);
            };

            // Live search as user types
            txtSearch.KeyUp += (s, e) => FilterRecords();
        }

        // ─── LOAD DATA ────────────────────────────────────────────

        private void LoadAllData()
        {
            try
            {
                _customers = LoadCustomers();
                _pets = LoadPets();
                BindCustomerGrid(_customers);
                BindPetGrid(_pets);
                LoadCustomerDropdown();
                UpdateStatusBar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Customer> LoadCustomers()
        {
            var list = new List<Customer>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT c.CustomerID, c.FullName, c.Phone, c.Email,
                       c.Address, c.IsActive, c.CreatedAt,
                       COUNT(p.PetID) AS PetCount
                FROM Customers c
                LEFT JOIN Pets p ON c.CustomerID = p.CustomerID AND p.IsActive = 1
                WHERE c.IsActive = 1
                GROUP BY c.CustomerID, c.FullName, c.Phone,
                         c.Email, c.Address, c.IsActive, c.CreatedAt
                ORDER BY c.FullName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var customer = new Customer
                        {
                            ID = (int)reader["CustomerID"],
                            FullName = reader["FullName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Email = reader["Email"].ToString(),
                            Address = reader["Address"].ToString(),
                            IsActive = (bool)reader["IsActive"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                        list.Add(customer);
                    }
                }
            }
            return list;
        }

        private List<Pet> LoadPets()
        {
            var list = new List<Pet>();
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"
                SELECT p.PetID, p.CustomerID, p.Name, p.Species,
                       p.Breed, p.Gender, p.DateOfBirth,
                       p.Weight, p.Allergies, p.MedicalNotes,
                       p.IsActive, p.CreatedAt, c.FullName AS OwnerName
                FROM Pets p
                JOIN Customers c ON p.CustomerID = c.CustomerID
                WHERE p.IsActive = 1
                ORDER BY p.Name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var pet = new Pet
                        {
                            ID = (int)reader["PetID"],
                            CustomerID = (int)reader["CustomerID"],
                            Name = reader["Name"].ToString(),
                            Species = reader["Species"].ToString(),
                            Breed = reader["Breed"].ToString(),
                            Gender = reader["Gender"].ToString(),
                            DateOfBirth = reader["DateOfBirth"] as DateTime?,
                            Weight = reader["Weight"] == DBNull.Value ? 0 : (decimal)reader["Weight"],
                            Allergies = reader["Allergies"].ToString(),
                            MedicalNotes = reader["MedicalNotes"].ToString(),
                            IsActive = (bool)reader["IsActive"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                        list.Add(pet);
                    }
                }
            }
            return list;
        }

        // ─── BIND GRIDS ───────────────────────────────────────────

        private void BindCustomerGrid(List<Customer> source)
        {
            // LINQ #6 — project customers to display-friendly anonymous type
            var display = source
                .Select(c => new
                {
                    ID = c.ID,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email,
                    PetCount = _pets.Count(p => p.CustomerID == c.ID)
                })
                .ToList();

            dgvCustomers.DataSource = display;
            tabCustomers.Text = $"👥 Customers ({display.Count})";
        }

        private void BindPetGrid(List<Pet> source)
        {
            // LINQ #7 — project pets with icon from GetSpeciesIcon()
            var display = source
                .Select(p => new
                {
                    ID = p.ID,
                    Icon = p.GetSpeciesIcon(),
                    Name = p.Name,
                    Species = p.Species,
                    Breed = p.Breed,
                    Age = p.AgeInYears + " yr",
                    Owner = _customers.FirstOrDefault(c => c.ID == p.CustomerID)?.FullName ?? "Unknown"
                })
                .ToList();

            dgvPets.DataSource = display;
            tabPets.Text = $"🐾 Pets ({display.Count})";
        }

        private void LoadCustomerDropdown()
        {
            // LINQ #8 — sorted customer name list for pet form dropdown
            var sorted = _customers
                .OrderBy(c => c.FullName)
                .Select(c => new { c.ID, c.FullName })
                .ToList();

            cmbCustomerID.DisplayMember = "FullName";
            cmbCustomerID.ValueMember = "ID";
            cmbCustomerID.DataSource = sorted;
        }

        // ─── SEARCH / FILTER ──────────────────────────────────────

        private void FilterRecords()
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            if (tabMain.SelectedTab == tabCustomers)
            {
                // LINQ #9 — filter customers by name, phone, or email
                var filtered = _customers
                    .Where(c => c.FullName.ToLower().Contains(keyword)
                             || c.Phone.Contains(keyword)
                             || (c.Email ?? "").ToLower().Contains(keyword))
                    .ToList();
                BindCustomerGrid(filtered);
            }
            else
            {
                // LINQ #10 — filter pets by name, species, or breed
                var filtered = _pets
                    .Where(p => p.Name.ToLower().Contains(keyword)
                             || p.Species.ToLower().Contains(keyword)
                             || (p.Breed ?? "").ToLower().Contains(keyword))
                    .ToList();
                BindPetGrid(filtered);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e) => FilterRecords();

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            BindCustomerGrid(_customers);
            BindPetGrid(_pets);
        }

        // ─── SELECT ROW → POPULATE FORM ───────────────────────────

        private void SelectCustomerRow(int rowIndex)
        {
            try
            {
                int id = (int)dgvCustomers.Rows[rowIndex].Cells["ID"].Value;
                var customer = _customers.FirstOrDefault(c => c.ID == id);
                if (customer == null) return;

                _selectedCustomerID = customer.ID;
                txtFullName.Text = customer.FullName;
                txtPhone.Text = customer.Phone;
                txtEmail.Text = customer.Email;
                txtAddress.Text = customer.Address;
                cmbStatus.SelectedItem = customer.IsActive ? "Active" : "Inactive";

                SetEditMode(false);
                lblStatusRecord.Text = $"Selected: {customer.FullName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting record: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectPetRow(int rowIndex)
        {
            try
            {
                int id = (int)dgvPets.Rows[rowIndex].Cells["ID"].Value;
                var pet = _pets.FirstOrDefault(p => p.ID == id);
                if (pet == null) return;

                _selectedPetID = pet.ID;
                txtPetName.Text = pet.Name;
                cmbSpecies.Text = pet.Species;
                txtBreed.Text = pet.Breed;
                cmbGender.Text = pet.Gender;
                nudWeight.Value = (decimal)pet.Weight;
                txtAllergies.Text = pet.Allergies;
                txtMedicalNotes.Text = pet.MedicalNotes;
                cmbCustomerID.SelectedValue = pet.CustomerID;

                if (pet.DateOfBirth.HasValue)
                    dtpDateOfBirth.Value = pet.DateOfBirth.Value;

                SetEditMode(false);
                lblStatusRecord.Text = $"Selected: {pet.Name} ({pet.GetSpeciesIcon()})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting pet: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── SAVE CUSTOMER ────────────────────────────────────────

        private void SaveCustomer()
        {
            try
            {
                var customer = new Customer
                {
                    ID = _selectedCustomerID,
                    FullName = txtFullName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    IsActive = cmbStatus.SelectedItem?.ToString() == "Active"
                };

                if (!customer.Validate())
                {
                    MessageBox.Show("Full Name and Phone are required.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    if (_selectedCustomerID == -1) // INSERT
                    {
                        string sql = @"INSERT INTO Customers (FullName, Phone, Email, Address, IsActive)
                                   VALUES (@FullName, @Phone, @Email, @Address, @IsActive)";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                            cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                            cmd.Parameters.AddWithValue("@Email", customer.Email);
                            cmd.Parameters.AddWithValue("@Address", customer.Address);
                            cmd.Parameters.AddWithValue("@IsActive", customer.IsActive);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Customer added successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else // UPDATE
                    {
                        string sql = @"UPDATE Customers
                                   SET FullName=@FullName, Phone=@Phone, Email=@Email,
                                       Address=@Address, IsActive=@IsActive
                                   WHERE CustomerID=@ID";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@FullName", customer.FullName);
                            cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                            cmd.Parameters.AddWithValue("@Email", customer.Email);
                            cmd.Parameters.AddWithValue("@Address", customer.Address);
                            cmd.Parameters.AddWithValue("@IsActive", customer.IsActive);
                            cmd.Parameters.AddWithValue("@ID", customer.ID);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Customer updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LoadAllData();
                SetEditMode(false);
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

        // ─── SAVE PET ─────────────────────────────────────────────

        private void SavePet()
        {
            try
            {
                var pet = new Pet
                {
                    ID = _selectedPetID,
                    CustomerID = (int)(cmbCustomerID.SelectedValue ?? 0),
                    Name = txtPetName.Text.Trim(),
                    Species = cmbSpecies.Text.Trim(),
                    Breed = txtBreed.Text.Trim(),
                    Gender = cmbGender.Text.Trim(),
                    Weight = nudWeight.Value,
                    Allergies = txtAllergies.Text.Trim(),
                    MedicalNotes = txtMedicalNotes.Text.Trim(),
                    DateOfBirth = dtpDateOfBirth.Checked ? dtpDateOfBirth.Value : (DateTime?)null
                };

                if (!pet.Validate())
                {
                    MessageBox.Show("Pet Name, Species, and Owner are required.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    if (_selectedPetID == -1) // INSERT
                    {
                        string sql = @"INSERT INTO Pets
                                   (CustomerID, Name, Species, Breed, Gender,
                                    DateOfBirth, Weight, Allergies, MedicalNotes)
                                   VALUES
                                   (@CustomerID, @Name, @Species, @Breed, @Gender,
                                    @DOB, @Weight, @Allergies, @MedicalNotes)";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", pet.CustomerID);
                            cmd.Parameters.AddWithValue("@Name", pet.Name);
                            cmd.Parameters.AddWithValue("@Species", pet.Species);
                            cmd.Parameters.AddWithValue("@Breed", pet.Breed);
                            cmd.Parameters.AddWithValue("@Gender", pet.Gender);
                            cmd.Parameters.AddWithValue("@DOB", (object)pet.DateOfBirth ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Weight", pet.Weight);
                            cmd.Parameters.AddWithValue("@Allergies", pet.Allergies);
                            cmd.Parameters.AddWithValue("@MedicalNotes", pet.MedicalNotes);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show($"Pet added! {pet.GetSummary()}", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else // UPDATE
                    {
                        string sql = @"UPDATE Pets
                                   SET CustomerID=@CustomerID, Name=@Name, Species=@Species,
                                       Breed=@Breed, Gender=@Gender, DateOfBirth=@DOB,
                                       Weight=@Weight, Allergies=@Allergies,
                                       MedicalNotes=@MedicalNotes
                                   WHERE PetID=@ID";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@CustomerID", pet.CustomerID);
                            cmd.Parameters.AddWithValue("@Name", pet.Name);
                            cmd.Parameters.AddWithValue("@Species", pet.Species);
                            cmd.Parameters.AddWithValue("@Breed", pet.Breed);
                            cmd.Parameters.AddWithValue("@Gender", pet.Gender);
                            cmd.Parameters.AddWithValue("@DOB", (object)pet.DateOfBirth ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Weight", pet.Weight);
                            cmd.Parameters.AddWithValue("@Allergies", pet.Allergies);
                            cmd.Parameters.AddWithValue("@MedicalNotes", pet.MedicalNotes);
                            cmd.Parameters.AddWithValue("@ID", pet.ID);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Pet updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                LoadAllData();
                SetEditMode(false);
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

        // ─── DELETE ───────────────────────────────────────────────

        private void DeleteCustomer()
        {
            try
            {
                if (_selectedCustomerID == -1) return;

                // LINQ #11 — check if customer has pets before deleting
                bool hasPets = _pets.Any(p => p.CustomerID == _selectedCustomerID);
                if (hasPets)
                {
                    MessageBox.Show("Cannot delete — this customer still has active pets.\nDeactivate or reassign pets first.",
                        "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var confirm = MessageBox.Show("Delete this customer? This cannot be undone.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Soft delete
                    string sql = "UPDATE Customers SET IsActive = 0 WHERE CustomerID = @ID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", _selectedCustomerID);
                        cmd.ExecuteNonQuery();
                    }
                }

                _selectedCustomerID = -1;
                LoadAllData();
                ClearCustomerForm();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeletePet()
        {
            try
            {
                if (_selectedPetID == -1) return;

                var confirm = MessageBox.Show("Delete this pet record?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm != DialogResult.Yes) return;

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "UPDATE Pets SET IsActive = 0 WHERE PetID = @ID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", _selectedPetID);
                        cmd.ExecuteNonQuery();
                    }
                }

                _selectedPetID = -1;
                LoadAllData();
                ClearPetForm();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── HELPERS ──────────────────────────────────────────────

        private void SetEditMode(bool editing)
        {
            _isEditing = editing;
            bool isCustomerTab = tabMain.SelectedTab == tabCustomers;

            if (isCustomerTab)
            {
                txtFullName.ReadOnly = !editing && _selectedCustomerID != -1;
                txtPhone.ReadOnly = !editing && _selectedCustomerID != -1;
                txtEmail.ReadOnly = !editing && _selectedCustomerID != -1;
                txtAddress.ReadOnly = !editing && _selectedCustomerID != -1;
            }
        }

        private void ClearCustomerForm()
        {
            _selectedCustomerID = -1;
            txtFullName.Clear(); txtPhone.Clear();
            txtEmail.Clear(); txtAddress.Clear();
            cmbStatus.SelectedIndex = 0;
        }

        private void ClearPetForm()
        {
            _selectedPetID = -1;
            txtPetName.Clear(); cmbSpecies.SelectedIndex = -1;
            txtBreed.Clear(); cmbGender.SelectedIndex = -1;
            txtAllergies.Clear(); txtMedicalNotes.Clear();
        }

        private void UpdateStatusBar()
        {
            lblStatusCount.Text = $"{_customers.Count} customers  |  {_pets.Count} pets loaded";
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (tabMain.SelectedTab == tabCustomers)
            {
                ClearCustomerForm();
                SetEditMode(true);
                txtFullName.Focus();
            }
            else
            {
                ClearPetForm();
                SetEditMode(true);

                // NEW LOGIC: Automatically link the pet to the currently selected customer
                if (_selectedCustomerID != -1)
                {
                    cmbCustomerID.SelectedValue = _selectedCustomerID;
                }

                txtPetName.Focus();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tabMain.SelectedTab == tabCustomers) SaveCustomer();
            else SavePet();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (tabMain.SelectedTab == tabCustomers) DeleteCustomer();
            else DeletePet();
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadAllData();

        private void btnBack_Click(object sender, EventArgs e) => this.Close();

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
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

        private void btnAddPetToCustomer_Click(object sender, EventArgs e)
        {
            if (_selectedCustomerID == -1)
            {
                MessageBox.Show("Please select a customer from the grid first.", "No Customer Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 1. Switch the view to the Pets tab
            tabMain.SelectedTab = tabPets;

            // 2. Prepare the pet form for a new entry
            ClearPetForm();
            SetEditMode(true);

            // 3. Crucial step: Auto-assign the selected customer ID to the dropdown
            cmbCustomerID.SelectedValue = _selectedCustomerID;

            // 4. Set focus to start typing immediately
            txtPetName.Focus();
        }
    }
}
