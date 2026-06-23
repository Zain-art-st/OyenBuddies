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
    public partial class CheckoutBillingForm : Form
    {
        private Staff _currentStaff;

        private Invoice _currentInvoice = new Invoice();

        // Collection #7 — BindingList for live DataGridView binding
        private System.ComponentModel.BindingList<InvoiceItem> _lineItems
            = new System.ComponentModel.BindingList<InvoiceItem>();

        private List<Customer> _customers = new List<Customer>();
        private List<Pet> _pets = new List<Pet>();
        private List<Service> _services = new List<Service>();

        private const decimal TAX_RATE = 0.06m; // 6% SST
        private const decimal DEFAULT_DISCOUNT = 0m;

        // Delegate for payment confirmed — used to update dashboard revenue
        public delegate void PaymentProcessed(Invoice invoice);
        public event PaymentProcessed OnPaymentProcessed;

        public CheckoutBillingForm(Staff staff)
        {
            InitializeComponent();
            _currentStaff = staff;
            SetupLineItemsGrid();
            SetupMouseEvents();
            SetupKeyboardEvents();
            LoadDropdownData();
            LoadRecentInvoices();
            StartNewInvoice();
            lblDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        // ─── SETUP ────────────────────────────────────────────────

        private void SetupLineItemsGrid()
        {
            dgvLineItems.AutoGenerateColumns = false;
            dgvLineItems.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Description", DataPropertyName = "Description", Width = 200 });
            dgvLineItems.Columns.Add(new DataGridViewTextBoxColumn
            { HeaderText = "Qty", DataPropertyName = "Quantity", Width = 50 });
            dgvLineItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Unit Price",
                DataPropertyName = "UnitPrice",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });
            dgvLineItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Total",
                DataPropertyName = "LineTotal",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" },
                ReadOnly = true
            });

            // Delete button column
            var deleteCol = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Text = "🗑",
                UseColumnTextForButtonValue = true,
                Width = 35
            };
            dgvLineItems.Columns.Add(deleteCol);

            dgvLineItems.DataSource = _lineItems;
        }

        private void SetupMouseEvents()
        {
            // Delete row on button click
            dgvLineItems.CellClick += (s, e) =>
            {
                try
                {
                    if (e.ColumnIndex == dgvLineItems.Columns.Count - 1 && e.RowIndex >= 0)
                    {
                        _lineItems.RemoveAt(e.RowIndex);
                        RecalculateTotals();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing item: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Recalculate when qty cell is edited
            dgvLineItems.CellEndEdit += (s, e) =>
            {
                try
                {
                    if (e.RowIndex >= 0 && e.RowIndex < _lineItems.Count)
                    {
                        var item = _lineItems[e.RowIndex];
                        if (!item.Validate())
                        {
                            MessageBox.Show("Invalid quantity or price.",
                                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        RecalculateTotals();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating item: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Payment method panel click selection
            foreach (Panel p in new[] { panelCash, panelCard, panelQR, panelBank })
            {
                p.MouseEnter += (s, e) =>
                {
                    if (((Panel)s).BackColor != Color.FromArgb(225, 245, 238))
                        ((Panel)s).BackColor = Color.FromArgb(241, 239, 232);
                };
                p.MouseLeave += (s, e) =>
                {
                    if (((Panel)s).BackColor != Color.FromArgb(225, 245, 238))
                        ((Panel)s).BackColor = Color.White;
                };
                p.Click += PaymentMethodPanel_Click;
            }

            // Recalculate change when amount tendered changes
            txtAmountTendered.TextChanged += (s, e) => CalculateChange();

            // Reload pets when customer changes
            cmbCustomer.SelectedIndexChanged += (s, e) => LoadPetsForCustomer();

            // Sync the chosen customer/pet into the invoice being built
            cmbPet.SelectedIndexChanged += (s, e) =>
            {
                if (cmbCustomer.SelectedValue != null && cmbPet.SelectedValue != null)
                {
                    var customer = (Customer)cmbCustomer.SelectedItem;
                    var pet = (Pet)cmbPet.SelectedItem;

                    _currentInvoice.CustomerID = (int)cmbCustomer.SelectedValue;
                    _currentInvoice.PetID = (int)cmbPet.SelectedValue;
                    _currentInvoice.CustomerName = customer?.FullName;
                    _currentInvoice.PetName = pet?.Name;

                    lblCustomerName.Text = customer?.FullName ?? "—";
                    lblPetName.Text = pet?.Name ?? "—";
                    lblSource.Text = "Walk-up";
                }
            };
        }

        private void SetupKeyboardEvents()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.S) ConfirmPayment();
                if (e.Control && e.KeyCode == Keys.P) PrintReceipt();
                if (e.Control && e.KeyCode == Keys.N) StartNewInvoice();
                if (e.KeyCode == Keys.F5) LoadRecentInvoices();
            };

            // Enter on amount tendered triggers confirm
            txtAmountTendered.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ConfirmPayment();
                    e.Handled = true;
                }
            };
        }

        // ─── LOAD DATA ────────────────────────────────────────────

        private void LoadDropdownData()
        {
            try
            {
                _customers.Clear();
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = "SELECT CustomerID, FullName FROM Customers WHERE IsActive=1 ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader r = cmd.ExecuteReader())
                        while (r.Read())
                            _customers.Add(new Customer
                            {
                                ID = (int)r["CustomerID"],
                                FullName = r["FullName"].ToString()
                            });
                }
                cmbCustomer.DisplayMember = "FullName";
                cmbCustomer.ValueMember = "ID";
                cmbCustomer.DataSource = new List<Customer>(_customers);

                LoadServices();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    string sql = "SELECT PetID, Name FROM Pets WHERE CustomerID=@CID AND IsActive=1";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CID", customerID);
                        using (SqlDataReader r = cmd.ExecuteReader())
                            while (r.Read())
                                _pets.Add(new Pet
                                {
                                    ID = (int)r["PetID"],
                                    Name = r["Name"].ToString()
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
                string sql = "SELECT ServiceID, ServiceName, Price FROM Services WHERE IsActive=1 ORDER BY ServiceName";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                    while (r.Read())
                        _services.Add(new Service
                        {
                            ID = (int)r["ServiceID"],
                            ServiceName = r["ServiceName"].ToString(),
                            Price = (decimal)r["Price"]
                        });
            }
            cmbAddService.DisplayMember = "ServiceName";
            cmbAddService.ValueMember = "ID";
            cmbAddService.DataSource = new List<Service>(_services);
        }

        private void LoadRecentInvoices()
        {
            try
            {
                lstRecentInvoices.Items.Clear();

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT TOP 10 i.InvoiceID, c.FullName, p.Name AS PetName,
                                      i.TotalAmount, i.Status, i.CreatedAt
                               FROM Invoices i
                               JOIN Customers c ON i.CustomerID = c.CustomerID
                               JOIN Pets      p ON i.PetID      = p.PetID
                               ORDER BY i.CreatedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string display = $"INV-{r["InvoiceID"]:D3}  {r["FullName"]}  " +
                                             $"RM {r["TotalAmount"]:F2}  [{r["Status"]}]";
                            lstRecentInvoices.Items.Add(display);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error loading invoices: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── LOAD FROM APPOINTMENT / WALK-IN ─────────────────────

        public void LoadFromAppointment(int appointmentID)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT a.AppointmentID, a.CustomerID, a.PetID,
                                      a.ServiceType, c.FullName, p.Name AS PetName,
                                      s.Price
                               FROM Appointments a
                               JOIN Customers c ON a.CustomerID = c.CustomerID
                               JOIN Pets      p ON a.PetID      = p.PetID
                               LEFT JOIN Services s ON a.ServiceID = s.ServiceID
                               WHERE a.AppointmentID = @ID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", appointmentID);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                _currentInvoice.CustomerID = (int)r["CustomerID"];
                                _currentInvoice.PetID = (int)r["PetID"];
                                _currentInvoice.AppointmentID = appointmentID;

                                lblCustomerName.Text = r["FullName"].ToString();
                                lblPetName.Text = r["PetName"].ToString();
                                lblSource.Text = $"Appointment #{appointmentID}";

                                // Auto-add the appointment's service as a line item
                                decimal price = r["Price"] == DBNull.Value ? 0 : (decimal)r["Price"];
                                AddLineItem(new InvoiceItem
                                {
                                    Description = r["ServiceType"].ToString(),
                                    Quantity = 1,
                                    UnitPrice = price
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error loading appointment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadFromWalkIn(int queueID)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string sql = @"SELECT q.QueueID, q.CustomerID, q.PetID,
                                      c.FullName, p.Name AS PetName,
                                      sv.ServiceName, sv.Price
                               FROM WalkInQueue q
                               JOIN Customers c  ON q.CustomerID = c.CustomerID
                               JOIN Pets      p  ON q.PetID      = p.PetID
                               LEFT JOIN Services sv ON q.ServiceID = sv.ServiceID
                               WHERE q.QueueID = @ID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", queueID);
                        using (SqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                _currentInvoice.CustomerID = (int)r["CustomerID"];
                                _currentInvoice.PetID = (int)r["PetID"];
                                _currentInvoice.QueueID = queueID;

                                lblCustomerName.Text = r["FullName"].ToString();
                                lblPetName.Text = r["PetName"].ToString();
                                lblSource.Text = $"Walk-in #{queueID}";

                                decimal price = r["Price"] == DBNull.Value ? 0 : (decimal)r["Price"];
                                AddLineItem(new InvoiceItem
                                {
                                    Description = r["ServiceName"].ToString(),
                                    Quantity = 1,
                                    UnitPrice = price
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Error loading walk-in: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── LINE ITEMS ───────────────────────────────────────────

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbAddService.SelectedValue == null)
                {
                    MessageBox.Show("Please select a service to add.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtUnitPrice.Text, out decimal price) || price < 0)
                {
                    MessageBox.Show("Please enter a valid price.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!int.TryParse(nudQty.Value.ToString(), out int qty) || qty < 1)
                {
                    MessageBox.Show("Quantity must be at least 1.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var item = new InvoiceItem
                {
                    ServiceID = (int)cmbAddService.SelectedValue,
                    Description = cmbAddService.Text,
                    Quantity = qty,
                    UnitPrice = price
                };

                AddLineItem(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddLineItem(InvoiceItem item)
        {
            if (!item.Validate())
            {
                MessageBox.Show("Invalid item — check description and price.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // LINQ #20 — check if same service already exists, merge if so
            var existing = _lineItems.FirstOrDefault(i => i.ServiceID == item.ServiceID
                                                       && item.ServiceID != null);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
                dgvLineItems.Refresh();
            }
            else
            {
                _lineItems.Add(item);
            }

            RecalculateTotals();
            cmbAddService.SelectedIndex = -1;
            nudQty.Value = 1;
            txtUnitPrice.Clear();
        }

        // ─── TOTALS ───────────────────────────────────────────────

        private void RecalculateTotals()
        {
            try
            {
                // LINQ #21 — sum all line totals
                decimal subtotal = _lineItems.Sum(i => i.LineTotal);

                decimal discountPct = nudDiscount.Value / 100m;
                decimal taxPct = nudTax.Value / 100m;

                decimal discountAmt = Math.Round(subtotal * discountPct, 2);
                decimal taxAmt = Math.Round((subtotal - discountAmt) * taxPct, 2);
                decimal total = subtotal - discountAmt + taxAmt;

                _currentInvoice.SubTotal = subtotal;
                _currentInvoice.DiscountAmt = discountAmt;
                _currentInvoice.TaxAmt = taxAmt;
                _currentInvoice.TotalAmount = total;

                lblSubtotal.Text = $"RM {subtotal:F2}";
                lblDiscount.Text = $"— RM {discountAmt:F2}";
                lblTax.Text = $"+ RM {taxAmt:F2}";
                lblTotal.Text = $"RM {total:F2}";
                lblTotalDue.Text = $"RM {total:F2}";

                CalculateChange();
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Calculation error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateChange()
        {
            try
            {
                if (decimal.TryParse(txtAmountTendered.Text, out decimal tendered))
                {
                    decimal change = tendered - _currentInvoice.TotalAmount;
                    lblChange.Text = change >= 0 ? $"RM {change:F2}" : "Insufficient";
                    lblChange.ForeColor = change >= 0
                        ? Color.FromArgb(59, 109, 17)
                        : Color.FromArgb(163, 45, 45);
                }
                else
                {
                    lblChange.Text = "RM 0.00";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Change calculation error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── PAYMENT ──────────────────────────────────────────────

        private void ConfirmPayment()
        {
            try
            {
                if (_currentInvoice.CustomerID == 0 || _currentInvoice.PetID == 0)
                {
                    MessageBox.Show("Please select a Customer and Pet first.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_lineItems.Count == 0)
                {
                    MessageBox.Show("Please add at least one service item.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // LINQ #22 — validate all line items before saving
                bool allValid = _lineItems.All(i => i.Validate());
                if (!allValid)
                {
                    MessageBox.Show("One or more line items are invalid. Please check.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Keep the invoice's Items in sync with what's actually in the line-item grid —
                // Invoice.Validate() checks this collection, but nothing was ever copying into it.
                _currentInvoice.Items = new List<InvoiceItem>(_lineItems);

                if (!_currentInvoice.Validate())
                {
                    MessageBox.Show("Invoice is incomplete. Please check all fields.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Check sufficient cash tendered
                if (_currentInvoice.PaymentMethod == "Cash")
                {
                    if (!decimal.TryParse(txtAmountTendered.Text, out decimal tendered)
                        || tendered < _currentInvoice.TotalAmount)
                    {
                        MessageBox.Show("Amount tendered is less than total due.",
                            "Insufficient Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var confirm = MessageBox.Show(
                    $"Confirm payment of {_currentInvoice.TotalAmount:C} via {_currentInvoice.PaymentMethod}?",
                    "Confirm Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                int invoiceID = SaveInvoice();

                if (invoiceID > 0)
                {
                    _currentInvoice.ID = invoiceID;
                    _currentInvoice.Status = "Paid";
                    _currentInvoice.PaidAt = DateTime.Now;

                    OnPaymentProcessed?.Invoke(_currentInvoice);

                    MessageBox.Show(
                        $"Payment confirmed!\nInvoice: INV-{invoiceID:D3}\n" +
                        $"Total: RM {_currentInvoice.TotalAmount:F2}\n" +
                        $"Method: {_currentInvoice.PaymentMethod}",
                        "Payment Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    PrintReceipt();
                    LoadRecentInvoices();
                    StartNewInvoice();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Payment error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int SaveInvoice()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Insert invoice header
                        string invoiceSql = @"
                        INSERT INTO Invoices
                            (CustomerID, PetID, AppointmentID, QueueID,
                             SubTotal, DiscountAmt, TaxAmt, TotalAmount,
                             PaymentMethod, Status, Notes, PaidAt)
                        VALUES
                            (@CID, @PID, @AID, @QID,
                             @Sub, @Disc, @Tax, @Total,
                             @Method, 'Paid', @Notes, GETDATE());
                        SELECT SCOPE_IDENTITY();";

                        int invoiceID;
                        using (SqlCommand cmd = new SqlCommand(invoiceSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@CID", _currentInvoice.CustomerID);
                            cmd.Parameters.AddWithValue("@PID", _currentInvoice.PetID);
                            cmd.Parameters.AddWithValue("@AID", (object)_currentInvoice.AppointmentID ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@QID", (object)_currentInvoice.QueueID ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Sub", _currentInvoice.SubTotal);
                            cmd.Parameters.AddWithValue("@Disc", _currentInvoice.DiscountAmt);
                            cmd.Parameters.AddWithValue("@Tax", _currentInvoice.TaxAmt);
                            cmd.Parameters.AddWithValue("@Total", _currentInvoice.TotalAmount);
                            cmd.Parameters.AddWithValue("@Method", _currentInvoice.PaymentMethod);
                            cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                            invoiceID = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Insert all line items
                        string itemSql = @"INSERT INTO InvoiceItems
                        (InvoiceID, ServiceID, Description, Quantity, UnitPrice, LineTotal)
                        VALUES (@IID, @SID, @Desc, @Qty, @Price, @Total)";

                        foreach (var item in _lineItems)
                        {
                            using (SqlCommand cmd = new SqlCommand(itemSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@IID", invoiceID);
                                cmd.Parameters.AddWithValue("@SID", (object)item.ServiceID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Desc", item.Description);
                                cmd.Parameters.AddWithValue("@Qty", item.Quantity);
                                cmd.Parameters.AddWithValue("@Price", item.UnitPrice);
                                cmd.Parameters.AddWithValue("@Total", item.LineTotal);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return invoiceID;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // ─── PRINT RECEIPT ────────────────────────────────────────

        private void PrintReceipt()
        {
            try
            {
                // LINQ #23 — build receipt lines from items
                var receiptLines = _lineItems
                    .Select(i => $"  {i.Description,-30} x{i.Quantity}  RM {i.LineTotal,8:F2}")
                    .ToList();

                string receipt =
                    "╔══════════════════════════════════════╗\n" +
                    "║            OYEN'S BUDDIES            ║\n" +
                    "╚══════════════════════════════════════╝\n" +
                    $"  Date   : {DateTime.Now:dd/MM/yyyy HH:mm}\n" +
                    $"  Invoice: INV-{_currentInvoice.ID:D3}\n" +
                    $"  Customer: {_currentInvoice.CustomerName}\n" +
                    $"  Pet     : {_currentInvoice.PetName}\n" +
                    "  ─────────────────────────────────────\n" +
                    string.Join("\n", receiptLines) + "\n" +
                    "  ─────────────────────────────────────\n" +
                    $"  Subtotal : RM {_currentInvoice.SubTotal,8:F2}\n" +
                    $"  Discount : RM {_currentInvoice.DiscountAmt,8:F2}\n" +
                    $"  Tax (6%) : RM {_currentInvoice.TaxAmt,8:F2}\n" +
                    $"  TOTAL    : RM {_currentInvoice.TotalAmount,8:F2}\n" +
                    "  ─────────────────────────────────────\n" +
                    $"  Payment  : {_currentInvoice.PaymentMethod}\n" +
                    "  Thank you for visiting Oyen's Buddies!\n" +
                    "╚══════════════════════════════════════╝";

                MessageBox.Show(receipt, "Receipt Preview",
                    MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─── HELPERS ──────────────────────────────────────────────

        private void StartNewInvoice()
        {
            _currentInvoice = new Invoice();
            _lineItems.Clear();
            cmbCustomer.SelectedIndex = -1;
            cmbPet.DataSource = null;
            txtAmountTendered.Clear();
            txtNotes.Clear();
            nudDiscount.Value = 0;
            nudTax.Value = 6;
            lblCustomerName.Text = "—";
            lblPetName.Text = "—";
            lblSource.Text = "New";
            lblInvoiceNum.Text = "INV-NEW";
            lblSubtotal.Text = "RM 0.00";
            lblDiscount.Text = "— RM 0.00";
            lblTax.Text = "+ RM 0.00";
            lblTotal.Text = "RM 0.00";
            lblTotalDue.Text = "RM 0.00";
            lblChange.Text = "RM 0.00";
            SelectPaymentMethod("Cash");
        }

        private void SelectPaymentMethod(string method)
        {
            _currentInvoice.PaymentMethod = method;
            Color selected = Color.FromArgb(225, 245, 238);
            Color unselected = Color.White;

            panelCash.BackColor = method == "Cash" ? selected : unselected;
            panelCard.BackColor = method == "Card" ? selected : unselected;
            panelQR.BackColor = method == "QR/e-Wallet" ? selected : unselected;
            panelBank.BackColor = method == "Bank Transfer" ? selected : unselected;

            // Show amount tendered only for cash
            txtAmountTendered.Enabled = method == "Cash";
            if (method != "Cash") lblChange.Text = "N/A";
        }

        private void PaymentMethodPanel_Click(object sender, EventArgs e)
        {
            var panel = (Panel)sender;
            string method = panel.Tag?.ToString() ?? "Cash";
            SelectPaymentMethod(method);
        }

        private void UpdateStatusBar()
        {
            // LINQ #24 — item count and total
            int itemCount = _lineItems.Count;
            decimal total = _lineItems.Sum(i => i.LineTotal);
            lblStatusInfo.Text = $"{itemCount} item(s)  ·  RM {total:F2}";
        }

        // ─── BUTTON EVENTS ────────────────────────────────────────

        private void btnNewInvoice_Click(object sender, EventArgs e) => StartNewInvoice();
        private void btnConfirmPayment_Click(object sender, EventArgs e) => ConfirmPayment();
        private void btnPrintReceipt_Click(object sender, EventArgs e) => PrintReceipt();
        private void btnRefresh_Click(object sender, EventArgs e) => LoadRecentInvoices();
        private void btnBack_Click(object sender, EventArgs e) => this.Close();

        private void nudDiscount_ValueChanged(object sender, EventArgs e) => RecalculateTotals();
        private void nudTax_ValueChanged(object sender, EventArgs e) => RecalculateTotals();

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