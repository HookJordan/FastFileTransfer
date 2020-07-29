namespace FFT
{
    partial class frmPreferences
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbLZMA = new System.Windows.Forms.RadioButton();
            this.cbDisableCompression = new System.Windows.Forms.RadioButton();
            this.cbGZIP = new System.Windows.Forms.RadioButton();
            this.cbLZO = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbBlowFish = new System.Windows.Forms.RadioButton();
            this.cbDisabledCrypto = new System.Windows.Forms.RadioButton();
            this.cbAES = new System.Windows.Forms.RadioButton();
            this.cbRC4 = new System.Windows.Forms.RadioButton();
            this.cbXOR = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cbDebug = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numBufferSize = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cbStartup = new System.Windows.Forms.CheckBox();
            this.txtPersonalPass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbUnattended = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lstProtected = new System.Windows.Forms.ListBox();
            this.mnuForbidden = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBufferSize)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.mnuForbidden.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(54, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 75);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Server";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(8, 41);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(296, 23);
            this.numPort.TabIndex = 13;
            this.numPort.Value = new decimal(new int[] {
            15056,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 16);
            this.label2.TabIndex = 12;
            this.label2.Text = "Listening Port:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbLZMA);
            this.groupBox5.Controls.Add(this.cbDisableCompression);
            this.groupBox5.Controls.Add(this.cbGZIP);
            this.groupBox5.Controls.Add(this.cbLZO);
            this.groupBox5.Location = new System.Drawing.Point(54, 207);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(289, 56);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Compression Algorithm";
            // 
            // cbLZMA
            // 
            this.cbLZMA.AutoSize = true;
            this.cbLZMA.Location = new System.Drawing.Point(198, 22);
            this.cbLZMA.Name = "cbLZMA";
            this.cbLZMA.Size = new System.Drawing.Size(57, 20);
            this.cbLZMA.TabIndex = 0;
            this.cbLZMA.Text = "LZMA";
            this.cbLZMA.UseVisualStyleBackColor = true;
            // 
            // cbDisableCompression
            // 
            this.cbDisableCompression.AutoSize = true;
            this.cbDisableCompression.Location = new System.Drawing.Point(6, 22);
            this.cbDisableCompression.Name = "cbDisableCompression";
            this.cbDisableCompression.Size = new System.Drawing.Size(74, 20);
            this.cbDisableCompression.TabIndex = 2;
            this.cbDisableCompression.TabStop = true;
            this.cbDisableCompression.Text = "Disabled";
            this.cbDisableCompression.UseVisualStyleBackColor = true;
            // 
            // cbGZIP
            // 
            this.cbGZIP.AutoSize = true;
            this.cbGZIP.Checked = true;
            this.cbGZIP.Location = new System.Drawing.Point(140, 22);
            this.cbGZIP.Name = "cbGZIP";
            this.cbGZIP.Size = new System.Drawing.Size(52, 20);
            this.cbGZIP.TabIndex = 1;
            this.cbGZIP.TabStop = true;
            this.cbGZIP.Text = "GZIP";
            this.cbGZIP.UseVisualStyleBackColor = true;
            // 
            // cbLZO
            // 
            this.cbLZO.AutoSize = true;
            this.cbLZO.Location = new System.Drawing.Point(86, 22);
            this.cbLZO.Name = "cbLZO";
            this.cbLZO.Size = new System.Drawing.Size(48, 20);
            this.cbLZO.TabIndex = 3;
            this.cbLZO.Text = "LZO";
            this.cbLZO.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbBlowFish);
            this.groupBox4.Controls.Add(this.cbDisabledCrypto);
            this.groupBox4.Controls.Add(this.cbAES);
            this.groupBox4.Controls.Add(this.cbRC4);
            this.groupBox4.Controls.Add(this.cbXOR);
            this.groupBox4.Location = new System.Drawing.Point(54, 47);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(289, 154);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Encryption Algorithm";
            // 
            // cbBlowFish
            // 
            this.cbBlowFish.AutoSize = true;
            this.cbBlowFish.Location = new System.Drawing.Point(6, 100);
            this.cbBlowFish.Name = "cbBlowFish";
            this.cbBlowFish.Size = new System.Drawing.Size(144, 20);
            this.cbBlowFish.TabIndex = 4;
            this.cbBlowFish.Text = "Blowfish (ECB Mode)";
            this.cbBlowFish.UseVisualStyleBackColor = true;
            // 
            // cbDisabledCrypto
            // 
            this.cbDisabledCrypto.AutoSize = true;
            this.cbDisabledCrypto.Location = new System.Drawing.Point(6, 22);
            this.cbDisabledCrypto.Name = "cbDisabledCrypto";
            this.cbDisabledCrypto.Size = new System.Drawing.Size(74, 20);
            this.cbDisabledCrypto.TabIndex = 3;
            this.cbDisabledCrypto.TabStop = true;
            this.cbDisabledCrypto.Text = "Disabled";
            this.cbDisabledCrypto.UseVisualStyleBackColor = true;
            // 
            // cbAES
            // 
            this.cbAES.AutoSize = true;
            this.cbAES.Location = new System.Drawing.Point(6, 126);
            this.cbAES.Name = "cbAES";
            this.cbAES.Size = new System.Drawing.Size(146, 20);
            this.cbAES.TabIndex = 2;
            this.cbAES.Text = "AES 256 (CBC Mode)";
            this.cbAES.UseVisualStyleBackColor = true;
            // 
            // cbRC4
            // 
            this.cbRC4.AutoSize = true;
            this.cbRC4.Checked = true;
            this.cbRC4.Location = new System.Drawing.Point(6, 74);
            this.cbRC4.Name = "cbRC4";
            this.cbRC4.Size = new System.Drawing.Size(49, 20);
            this.cbRC4.TabIndex = 0;
            this.cbRC4.TabStop = true;
            this.cbRC4.Text = "RC4";
            this.cbRC4.UseVisualStyleBackColor = true;
            // 
            // cbXOR
            // 
            this.cbXOR.AutoSize = true;
            this.cbXOR.Location = new System.Drawing.Point(6, 48);
            this.cbXOR.Name = "cbXOR";
            this.cbXOR.Size = new System.Drawing.Size(51, 20);
            this.cbXOR.TabIndex = 1;
            this.cbXOR.TabStop = true;
            this.cbXOR.Text = "XOR";
            this.cbXOR.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(359, 379);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(278, 379);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 4;
            this.btnConfirm.Text = "&Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(426, 361);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbDebug);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(418, 332);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Network Configuration";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cbDebug
            // 
            this.cbDebug.AutoSize = true;
            this.cbDebug.Location = new System.Drawing.Point(266, 247);
            this.cbDebug.Name = "cbDebug";
            this.cbDebug.Size = new System.Drawing.Size(98, 20);
            this.cbDebug.TabIndex = 15;
            this.cbDebug.Text = "Debug Mode";
            this.cbDebug.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.numBufferSize);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(54, 163);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(310, 78);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Limits";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(282, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "KB";
            // 
            // numBufferSize
            // 
            this.numBufferSize.Location = new System.Drawing.Point(8, 41);
            this.numBufferSize.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numBufferSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBufferSize.Name = "numBufferSize";
            this.numBufferSize.Size = new System.Drawing.Size(268, 23);
            this.numBufferSize.TabIndex = 13;
            this.numBufferSize.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 12;
            this.label1.Text = "Buffer Size:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(418, 332);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Data Configuration";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(418, 332);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Security";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.cbStartup);
            this.groupBox6.Controls.Add(this.txtPersonalPass);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.cbUnattended);
            this.groupBox6.Location = new System.Drawing.Point(54, 47);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(289, 91);
            this.groupBox6.TabIndex = 1;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Unattended Access";
            // 
            // cbStartup
            // 
            this.cbStartup.AutoSize = true;
            this.cbStartup.Enabled = false;
            this.cbStartup.Location = new System.Drawing.Point(9, 65);
            this.cbStartup.Name = "cbStartup";
            this.cbStartup.Size = new System.Drawing.Size(137, 20);
            this.cbStartup.TabIndex = 3;
            this.cbStartup.Text = "Start with windows";
            this.cbStartup.UseVisualStyleBackColor = true;
            // 
            // txtPersonalPass
            // 
            this.txtPersonalPass.Enabled = false;
            this.txtPersonalPass.Location = new System.Drawing.Point(9, 38);
            this.txtPersonalPass.Name = "txtPersonalPass";
            this.txtPersonalPass.PasswordChar = '*';
            this.txtPersonalPass.Size = new System.Drawing.Size(274, 23);
            this.txtPersonalPass.TabIndex = 2;
            this.txtPersonalPass.Enter += new System.EventHandler(this.txtPersonalPass_Enter);
            this.txtPersonalPass.Leave += new System.EventHandler(this.txtPersonalPass_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Password:";
            // 
            // cbUnattended
            // 
            this.cbUnattended.AutoSize = true;
            this.cbUnattended.Location = new System.Drawing.Point(218, 67);
            this.cbUnattended.Name = "cbUnattended";
            this.cbUnattended.Size = new System.Drawing.Size(65, 20);
            this.cbUnattended.TabIndex = 0;
            this.cbUnattended.Text = "Enable";
            this.cbUnattended.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lstProtected);
            this.groupBox3.Location = new System.Drawing.Point(54, 144);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(289, 123);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Protected Directories";
            // 
            // lstProtected
            // 
            this.lstProtected.ContextMenuStrip = this.mnuForbidden;
            this.lstProtected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstProtected.FormattingEnabled = true;
            this.lstProtected.ItemHeight = 16;
            this.lstProtected.Location = new System.Drawing.Point(3, 19);
            this.lstProtected.Name = "lstProtected";
            this.lstProtected.Size = new System.Drawing.Size(283, 101);
            this.lstProtected.TabIndex = 0;
            // 
            // mnuForbidden
            // 
            this.mnuForbidden.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.mnuForbidden.Name = "mnuForbidden";
            this.mnuForbidden.Size = new System.Drawing.Size(118, 48);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // frmPreferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 413);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPreferences";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmPreferences";
            this.Load += new System.EventHandler(this.frmPreferences_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBufferSize)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.mnuForbidden.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton cbLZMA;
        private System.Windows.Forms.RadioButton cbGZIP;
        private System.Windows.Forms.RadioButton cbDisableCompression;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton cbDisabledCrypto;
        private System.Windows.Forms.RadioButton cbAES;
        private System.Windows.Forms.RadioButton cbRC4;
        private System.Windows.Forms.RadioButton cbXOR;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.RadioButton cbBlowFish;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numBufferSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton cbLZO;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox lstProtected;
        private System.Windows.Forms.ContextMenuStrip mnuForbidden;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.CheckBox cbDebug;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtPersonalPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbUnattended;
        private System.Windows.Forms.CheckBox cbStartup;
    }
}