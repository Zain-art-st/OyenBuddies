namespace OyenGrooming
{
    partial class MainDashboard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDashboard));
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.dashboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.appoinmentReportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblClock = new System.Windows.Forms.Label();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.lblGreeting = new System.Windows.Forms.Label();
            this.btnTileServices = new System.Windows.Forms.Button();
            this.lblDate = new System.Windows.Forms.Label();
            this.btnTileQueue = new System.Windows.Forms.Button();
            this.btnTileAppointments = new System.Windows.Forms.Button();
            this.btnTileAnalytics = new System.Windows.Forms.Button();
            this.lblStats = new System.Windows.Forms.Label();
            this.dgvTodayAppts = new System.Windows.Forms.DataGridView();
            this.btnTileCheckout = new System.Windows.Forms.Button();
            this.panelStat1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.picStatIcon1 = new System.Windows.Forms.PictureBox();
            this.lblStatAppointments = new System.Windows.Forms.Label();
            this.panelStat2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.picStatIcon2 = new System.Windows.Forms.PictureBox();
            this.lblStatConfirmed = new System.Windows.Forms.Label();
            this.btnTileCustomers = new System.Windows.Forms.Button();
            this.panelStat3 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.lblStatCustomers = new System.Windows.Forms.Label();
            this.picStatIcon3 = new System.Windows.Forms.PictureBox();
            this.panelStat4 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblStatRevenue = new System.Windows.Forms.Label();
            this.picStatIcon4 = new System.Windows.Forms.PictureBox();
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblStatusText = new System.Windows.Forms.StatusStrip();
            this.lblStatPending = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatQueue = new System.Windows.Forms.ToolStripStatusLabel();
            this.button3 = new System.Windows.Forms.Button();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.menuMain.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTodayAppts)).BeginInit();
            this.panelStat1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatIcon1)).BeginInit();
            this.panelStat2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatIcon2)).BeginInit();
            this.panelStat3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatIcon3)).BeginInit();
            this.panelStat4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatIcon4)).BeginInit();
            this.lblStatusText.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuMain
            // 
            this.menuMain.BackColor = System.Drawing.Color.Gainsboro;
            this.menuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dashboardToolStripMenuItem,
            this.customersToolStripMenuItem,
            this.appoinmentReportsToolStripMenuItem,
            this.reportsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuMain.Size = new System.Drawing.Size(950, 28);
            this.menuMain.TabIndex = 0;
            this.menuMain.Text = "menuStrip1";
            // 
            // dashboardToolStripMenuItem
            // 
            this.dashboardToolStripMenuItem.Name = "dashboardToolStripMenuItem";
            this.dashboardToolStripMenuItem.Size = new System.Drawing.Size(96, 24);
            this.dashboardToolStripMenuItem.Text = "Dashboard";
            // 
            // customersToolStripMenuItem
            // 
            this.customersToolStripMenuItem.Name = "customersToolStripMenuItem";
            this.customersToolStripMenuItem.Size = new System.Drawing.Size(92, 24);
            this.customersToolStripMenuItem.Text = "Customers";
            this.customersToolStripMenuItem.Click += new System.EventHandler(this.menuCustomers_Click);
            // 
            // appoinmentReportsToolStripMenuItem
            // 
            this.appoinmentReportsToolStripMenuItem.Name = "appoinmentReportsToolStripMenuItem";
            this.appoinmentReportsToolStripMenuItem.Size = new System.Drawing.Size(106, 24);
            this.appoinmentReportsToolStripMenuItem.Text = "Appoinment";
            this.appoinmentReportsToolStripMenuItem.Click += new System.EventHandler(this.menuAppointments_Click);
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.reportsToolStripMenuItem.Text = "Reports";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.menuLogout_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripButton5});
            this.toolStrip1.Location = new System.Drawing.Point(0, 28);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(950, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(62, 24);
            this.toolStripButton1.Text = "Refresh";
            this.toolStripButton1.Click += new System.EventHandler(this.toolBtnRefresh_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(135, 24);
            this.toolStripButton2.Text = "New Appointment";
            this.toolStripButton2.Click += new System.EventHandler(this.toolBtnNewAppointment_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(110, 24);
            this.toolStripButton3.Text = "New Customer";
            this.toolStripButton3.Click += new System.EventHandler(this.toolBtnNewCustomer_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(92, 24);
            this.toolStripButton4.Text = "Print Report";
            this.toolStripButton4.Click += new System.EventHandler(this.toolBtnPrint_Click);
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton5.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton5.Image")));
            this.toolStripButton5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton5.Name = "toolStripButton5";
            this.toolStripButton5.Size = new System.Drawing.Size(57, 24);
            this.toolStripButton5.Text = "Search";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.lblClock);
            this.panelMain.Controls.Add(this.lblUserInfo);
            this.panelMain.Controls.Add(this.lblGreeting);
            this.panelMain.Controls.Add(this.btnTileServices);
            this.panelMain.Controls.Add(this.lblDate);
            this.panelMain.Controls.Add(this.btnTileQueue);
            this.panelMain.Controls.Add(this.btnTileAppointments);
            this.panelMain.Controls.Add(this.btnTileAnalytics);
            this.panelMain.Controls.Add(this.lblStats);
            this.panelMain.Controls.Add(this.dgvTodayAppts);
            this.panelMain.Controls.Add(this.btnTileCheckout);
            this.panelMain.Controls.Add(this.panelStat1);
            this.panelMain.Controls.Add(this.panelStat2);
            this.panelMain.Controls.Add(this.btnTileCustomers);
            this.panelMain.Controls.Add(this.panelStat3);
            this.panelMain.Controls.Add(this.panelStat4);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 55);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(950, 595);
            this.panelMain.TabIndex = 3;
            this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMain_Paint);
            // 
            // lblClock
            // 
            this.lblClock.AutoSize = true;
            this.lblClock.Location = new System.Drawing.Point(90, 579);
            this.lblClock.Name = "lblClock";
            this.lblClock.Size = new System.Drawing.Size(44, 16);
            this.lblClock.TabIndex = 21;
            this.lblClock.Text = "label1";
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.AutoSize = true;
            this.lblUserInfo.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserInfo.Location = new System.Drawing.Point(85, 59);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(43, 17);
            this.lblUserInfo.TabIndex = 20;
            this.lblUserInfo.Text = "label1";
            // 
            // lblGreeting
            // 
            this.lblGreeting.AutoSize = true;
            this.lblGreeting.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGreeting.Location = new System.Drawing.Point(417, 13);
            this.lblGreeting.Name = "lblGreeting";
            this.lblGreeting.Size = new System.Drawing.Size(79, 31);
            this.lblGreeting.TabIndex = 10;
            this.lblGreeting.Text = "label2";
            // 
            // btnTileServices
            // 
            this.btnTileServices.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileServices.Location = new System.Drawing.Point(577, 300);
            this.btnTileServices.Name = "btnTileServices";
            this.btnTileServices.Size = new System.Drawing.Size(230, 70);
            this.btnTileServices.TabIndex = 19;
            this.btnTileServices.Text = "✂️ Services";
            this.btnTileServices.UseVisualStyleBackColor = true;
            this.btnTileServices.Click += new System.EventHandler(this.btnTileServices_Click);
            // 
            // lblDate
            // 
            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblDate.Location = new System.Drawing.Point(78, 196);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(55, 23);
            this.lblDate.TabIndex = 11;
            this.lblDate.Text = "label2";
            // 
            // btnTileQueue
            // 
            this.btnTileQueue.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileQueue.Location = new System.Drawing.Point(577, 222);
            this.btnTileQueue.Name = "btnTileQueue";
            this.btnTileQueue.Size = new System.Drawing.Size(230, 70);
            this.btnTileQueue.TabIndex = 17;
            this.btnTileQueue.Text = "🚶 Walk-in Queue";
            this.btnTileQueue.UseVisualStyleBackColor = true;
            this.btnTileQueue.Click += new System.EventHandler(this.btnTileQueue_Click);
            // 
            // btnTileAppointments
            // 
            this.btnTileAppointments.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileAppointments.Location = new System.Drawing.Point(341, 222);
            this.btnTileAppointments.Name = "btnTileAppointments";
            this.btnTileAppointments.Size = new System.Drawing.Size(230, 70);
            this.btnTileAppointments.TabIndex = 18;
            this.btnTileAppointments.Text = "📅 Appointments";
            this.btnTileAppointments.UseVisualStyleBackColor = true;
            this.btnTileAppointments.Click += new System.EventHandler(this.btnTileAppointments_Click);
            // 
            // btnTileAnalytics
            // 
            this.btnTileAnalytics.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileAnalytics.Location = new System.Drawing.Point(341, 300);
            this.btnTileAnalytics.Name = "btnTileAnalytics";
            this.btnTileAnalytics.Size = new System.Drawing.Size(230, 70);
            this.btnTileAnalytics.TabIndex = 15;
            this.btnTileAnalytics.Text = "📈 Analytics";
            this.btnTileAnalytics.UseVisualStyleBackColor = true;
            this.btnTileAnalytics.Click += new System.EventHandler(this.btnTileAnalytics_Click);
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStats.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.lblStats.Location = new System.Drawing.Point(89, 384);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(171, 23);
            this.lblStats.TabIndex = 12;
            this.lblStats.Text = "Today\'s Appointment";
            // 
            // dgvTodayAppts
            // 
            this.dgvTodayAppts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvTodayAppts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTodayAppts.Location = new System.Drawing.Point(92, 410);
            this.dgvTodayAppts.Name = "dgvTodayAppts";
            this.dgvTodayAppts.RowHeadersWidth = 51;
            this.dgvTodayAppts.RowTemplate.Height = 24;
            this.dgvTodayAppts.Size = new System.Drawing.Size(715, 150);
            this.dgvTodayAppts.TabIndex = 13;
            this.dgvTodayAppts.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTodayAppts_CellContentClick);
            // 
            // btnTileCheckout
            // 
            this.btnTileCheckout.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileCheckout.Location = new System.Drawing.Point(92, 300);
            this.btnTileCheckout.Name = "btnTileCheckout";
            this.btnTileCheckout.Size = new System.Drawing.Size(230, 70);
            this.btnTileCheckout.TabIndex = 16;
            this.btnTileCheckout.Text = "💰 Checkout";
            this.btnTileCheckout.UseVisualStyleBackColor = true;
            this.btnTileCheckout.Click += new System.EventHandler(this.btnTileCheckout_Click);
            // 
            // panelStat1
            // 
            this.panelStat1.BackColor = System.Drawing.Color.White;
            this.panelStat1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStat1.Controls.Add(this.label1);
            this.panelStat1.Controls.Add(this.picStatIcon1);
            this.panelStat1.Controls.Add(this.lblStatAppointments);
            this.panelStat1.Location = new System.Drawing.Point(84, 78);
            this.panelStat1.Name = "panelStat1";
            this.panelStat1.Size = new System.Drawing.Size(154, 100);
            this.panelStat1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 17);
            this.label1.TabIndex = 22;
            this.label1.Text = "Today\'s appointment";
            // 
            // picStatIcon1
            // 
            this.picStatIcon1.Image = global::OyenGrooming.Properties.Resources._1067374;
            this.picStatIcon1.Location = new System.Drawing.Point(7, 9);
            this.picStatIcon1.Name = "picStatIcon1";
            this.picStatIcon1.Size = new System.Drawing.Size(52, 50);
            this.picStatIcon1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStatIcon1.TabIndex = 21;
            this.picStatIcon1.TabStop = false;
            // 
            // lblStatAppointments
            // 
            this.lblStatAppointments.AutoSize = true;
            this.lblStatAppointments.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatAppointments.Location = new System.Drawing.Point(5, 62);
            this.lblStatAppointments.Name = "lblStatAppointments";
            this.lblStatAppointments.Size = new System.Drawing.Size(43, 17);
            this.lblStatAppointments.TabIndex = 13;
            this.lblStatAppointments.Text = "label1";
            // 
            // panelStat2
            // 
            this.panelStat2.BackColor = System.Drawing.Color.White;
            this.panelStat2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStat2.Controls.Add(this.label2);
            this.panelStat2.Controls.Add(this.picStatIcon2);
            this.panelStat2.Controls.Add(this.lblStatConfirmed);
            this.panelStat2.Location = new System.Drawing.Point(262, 78);
            this.panelStat2.Name = "panelStat2";
            this.panelStat2.Size = new System.Drawing.Size(165, 100);
            this.panelStat2.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 17);
            this.label2.TabIndex = 23;
            this.label2.Text = "Total Customers";
            // 
            // picStatIcon2
            // 
            this.picStatIcon2.Image = global::OyenGrooming.Properties.Resources.images;
            this.picStatIcon2.Location = new System.Drawing.Point(9, 9);
            this.picStatIcon2.Name = "picStatIcon2";
            this.picStatIcon2.Size = new System.Drawing.Size(50, 50);
            this.picStatIcon2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStatIcon2.TabIndex = 22;
            this.picStatIcon2.TabStop = false;
            // 
            // lblStatConfirmed
            // 
            this.lblStatConfirmed.AutoSize = true;
            this.lblStatConfirmed.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatConfirmed.Location = new System.Drawing.Point(6, 62);
            this.lblStatConfirmed.Name = "lblStatConfirmed";
            this.lblStatConfirmed.Size = new System.Drawing.Size(43, 17);
            this.lblStatConfirmed.TabIndex = 14;
            this.lblStatConfirmed.Text = "label2";
            // 
            // btnTileCustomers
            // 
            this.btnTileCustomers.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTileCustomers.Location = new System.Drawing.Point(92, 222);
            this.btnTileCustomers.Name = "btnTileCustomers";
            this.btnTileCustomers.Size = new System.Drawing.Size(230, 70);
            this.btnTileCustomers.TabIndex = 14;
            this.btnTileCustomers.Text = "👥 CUSTOMER AND PETS";
            this.btnTileCustomers.UseVisualStyleBackColor = true;
            this.btnTileCustomers.Click += new System.EventHandler(this.btnTileCustomers_Click);
            // 
            // panelStat3
            // 
            this.panelStat3.BackColor = System.Drawing.Color.White;
            this.panelStat3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStat3.Controls.Add(this.label3);
            this.panelStat3.Controls.Add(this.lblStatCustomers);
            this.panelStat3.Controls.Add(this.picStatIcon3);
            this.panelStat3.Location = new System.Drawing.Point(448, 78);
            this.panelStat3.Name = "panelStat3";
            this.panelStat3.Size = new System.Drawing.Size(150, 100);
            this.panelStat3.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(5, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 17);
            this.label3.TabIndex = 24;
            this.label3.Text = "Walk-in Queue";
            // 
            // lblStatCustomers
            // 
            this.lblStatCustomers.AutoSize = true;
            this.lblStatCustomers.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatCustomers.Location = new System.Drawing.Point(5, 62);
            this.lblStatCustomers.Name = "lblStatCustomers";
            this.lblStatCustomers.Size = new System.Drawing.Size(43, 17);
            this.lblStatCustomers.TabIndex = 16;
            this.lblStatCustomers.Text = "label4";
            // 
            // picStatIcon3
            // 
            this.picStatIcon3.Image = global::OyenGrooming.Properties.Resources.Man_walking_icon_1410105361_svg;
            this.picStatIcon3.Location = new System.Drawing.Point(8, 18);
            this.picStatIcon3.Name = "picStatIcon3";
            this.picStatIcon3.Size = new System.Drawing.Size(39, 41);
            this.picStatIcon3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStatIcon3.TabIndex = 23;
            this.picStatIcon3.TabStop = false;
            // 
            // panelStat4
            // 
            this.panelStat4.BackColor = System.Drawing.Color.White;
            this.panelStat4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelStat4.Controls.Add(this.label4);
            this.panelStat4.Controls.Add(this.lblStatRevenue);
            this.panelStat4.Controls.Add(this.picStatIcon4);
            this.panelStat4.Location = new System.Drawing.Point(621, 78);
            this.panelStat4.Name = "panelStat4";
            this.panelStat4.Size = new System.Drawing.Size(152, 100);
            this.panelStat4.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 17);
            this.label4.TabIndex = 25;
            this.label4.Text = "Revenue";
            // 
            // lblStatRevenue
            // 
            this.lblStatRevenue.AutoSize = true;
            this.lblStatRevenue.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatRevenue.Location = new System.Drawing.Point(3, 62);
            this.lblStatRevenue.Name = "lblStatRevenue";
            this.lblStatRevenue.Size = new System.Drawing.Size(43, 17);
            this.lblStatRevenue.TabIndex = 15;
            this.lblStatRevenue.Text = "label3";
            // 
            // picStatIcon4
            // 
            this.picStatIcon4.Image = global::OyenGrooming.Properties.Resources.istockphoto_1330591104_612x612;
            this.picStatIcon4.Location = new System.Drawing.Point(3, 9);
            this.picStatIcon4.Name = "picStatIcon4";
            this.picStatIcon4.Size = new System.Drawing.Size(73, 50);
            this.picStatIcon4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picStatIcon4.TabIndex = 24;
            this.picStatIcon4.TabStop = false;
            // 
            // timerClock
            // 
            this.timerClock.Interval = 1000;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // lblStatusText
            // 
            this.lblStatusText.BackColor = System.Drawing.Color.White;
            this.lblStatusText.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.lblStatusText.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatPending,
            this.lblStatQueue});
            this.lblStatusText.Location = new System.Drawing.Point(0, 624);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(950, 26);
            this.lblStatusText.TabIndex = 20;
            this.lblStatusText.Text = "statusStrip1";
            // 
            // lblStatPending
            // 
            this.lblStatPending.Name = "lblStatPending";
            this.lblStatPending.Size = new System.Drawing.Size(151, 20);
            this.lblStatPending.Text = "toolStripStatusLabel1";
            // 
            // lblStatQueue
            // 
            this.lblStatQueue.Name = "lblStatQueue";
            this.lblStatQueue.Size = new System.Drawing.Size(151, 20);
            this.lblStatQueue.Text = "toolStripStatusLabel2";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.Green;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(919, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(12, 12);
            this.button3.TabIndex = 23;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Yellow;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Location = new System.Drawing.Point(901, 12);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(12, 12);
            this.btnMinimize.TabIndex = 22;
            this.btnMinimize.Text = "button2";
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Red;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(883, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(12, 12);
            this.btnClose.TabIndex = 21;
            this.btnClose.Text = "button1";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // MainDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            this.ClientSize = new System.Drawing.Size(950, 650);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblStatusText);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuMain;
            this.Name = "MainDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MainDashboard";
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTodayAppts)).EndInit();
            this.panelStat1.ResumeLayout(false);
            this.panelStat1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatIcon1)).EndInit();
            this.panelStat2.ResumeLayout(false);
            this.panelStat2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatIcon2)).EndInit();
            this.panelStat3.ResumeLayout(false);
            this.panelStat3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatIcon3)).EndInit();
            this.panelStat4.ResumeLayout(false);
            this.panelStat4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picStatIcon4)).EndInit();
            this.lblStatusText.ResumeLayout(false);
            this.lblStatusText.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripMenuItem dashboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem appoinmentReportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripButton toolStripButton5;
        private System.Windows.Forms.Panel panelStat1;
        private System.Windows.Forms.Panel panelStat2;
        private System.Windows.Forms.Panel panelStat3;
        private System.Windows.Forms.Panel panelStat4;
        private System.Windows.Forms.Label lblGreeting;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.Timer timerClock;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridView dgvTodayAppts;
        private System.Windows.Forms.Button btnTileCustomers;
        private System.Windows.Forms.Button btnTileAnalytics;
        private System.Windows.Forms.Button btnTileCheckout;
        private System.Windows.Forms.Button btnTileQueue;
        private System.Windows.Forms.Button btnTileAppointments;
        private System.Windows.Forms.Button btnTileServices;
        private System.Windows.Forms.StatusStrip lblStatusText;
        private System.Windows.Forms.PictureBox picStatIcon1;
        private System.Windows.Forms.PictureBox picStatIcon2;
        private System.Windows.Forms.PictureBox picStatIcon3;
        private System.Windows.Forms.PictureBox picStatIcon4;
        private System.Windows.Forms.Label lblStatAppointments;
        private System.Windows.Forms.Label lblStatConfirmed;
        private System.Windows.Forms.Label lblStatCustomers;
        private System.Windows.Forms.Label lblStatRevenue;
        private System.Windows.Forms.Label lblUserInfo;
        private System.Windows.Forms.Label lblClock;
        private System.Windows.Forms.ToolStripStatusLabel lblStatPending;
        private System.Windows.Forms.ToolStripStatusLabel lblStatQueue;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}