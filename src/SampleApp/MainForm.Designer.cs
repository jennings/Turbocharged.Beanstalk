namespace SampleApp
{
    partial class MainForm
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
            this.hostnameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.putJobsListBox = new System.Windows.Forms.ListBox();
            this.putJobButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.stateTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ttrTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ageTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tubeTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.reservedJobsListBox = new System.Windows.Forms.ListBox();
            this.deleteJobButton = new System.Windows.Forms.Button();
            this.reserveButton = new System.Windows.Forms.Button();
            this.spawnWorkerButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // hostnameTextBox
            // 
            this.hostnameTextBox.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hostnameTextBox.Location = new System.Drawing.Point(101, 12);
            this.hostnameTextBox.Name = "hostnameTextBox";
            this.hostnameTextBox.Size = new System.Drawing.Size(228, 30);
            this.hostnameTextBox.TabIndex = 0;
            this.hostnameTextBox.Text = "172.16.80.1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hostname";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(369, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port";
            // 
            // portTextBox
            // 
            this.portTextBox.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.portTextBox.Location = new System.Drawing.Point(413, 12);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(103, 30);
            this.portTextBox.TabIndex = 1;
            this.portTextBox.Text = "11300";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(585, 12);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(133, 30);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Location = new System.Drawing.Point(724, 12);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(133, 30);
            this.disconnectButton.TabIndex = 5;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.putJobsListBox);
            this.groupBox1.Controls.Add(this.putJobButton);
            this.groupBox1.Location = new System.Drawing.Point(35, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 796);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Put";
            // 
            // putJobsListBox
            // 
            this.putJobsListBox.FormattingEnabled = true;
            this.putJobsListBox.ItemHeight = 20;
            this.putJobsListBox.Location = new System.Drawing.Point(22, 95);
            this.putJobsListBox.Name = "putJobsListBox";
            this.putJobsListBox.Size = new System.Drawing.Size(426, 664);
            this.putJobsListBox.TabIndex = 14;
            // 
            // putJobButton
            // 
            this.putJobButton.Location = new System.Drawing.Point(22, 48);
            this.putJobButton.Name = "putJobButton";
            this.putJobButton.Size = new System.Drawing.Size(175, 41);
            this.putJobButton.TabIndex = 0;
            this.putJobButton.Text = "Put New Job";
            this.putJobButton.UseVisualStyleBackColor = true;
            this.putJobButton.Click += new System.EventHandler(this.putJobButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.stateTextBox);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.ttrTextBox);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.ageTextBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tubeTextBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.idTextBox);
            this.groupBox2.Controls.Add(this.reservedJobsListBox);
            this.groupBox2.Controls.Add(this.deleteJobButton);
            this.groupBox2.Controls.Add(this.reserveButton);
            this.groupBox2.Location = new System.Drawing.Point(549, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(495, 780);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Reserve";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 650);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 20);
            this.label7.TabIndex = 13;
            this.label7.Text = "State";
            // 
            // stateTextBox
            // 
            this.stateTextBox.Location = new System.Drawing.Point(84, 644);
            this.stateTextBox.Name = "stateTextBox";
            this.stateTextBox.Size = new System.Drawing.Size(100, 26);
            this.stateTextBox.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 618);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 20);
            this.label6.TabIndex = 11;
            this.label6.Text = "TTR";
            // 
            // ttrTextBox
            // 
            this.ttrTextBox.Location = new System.Drawing.Point(84, 612);
            this.ttrTextBox.Name = "ttrTextBox";
            this.ttrTextBox.Size = new System.Drawing.Size(100, 26);
            this.ttrTextBox.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(32, 586);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Age";
            // 
            // ageTextBox
            // 
            this.ageTextBox.Location = new System.Drawing.Point(84, 580);
            this.ageTextBox.Name = "ageTextBox";
            this.ageTextBox.Size = new System.Drawing.Size(100, 26);
            this.ageTextBox.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(32, 554);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Tube";
            // 
            // tubeTextBox
            // 
            this.tubeTextBox.Location = new System.Drawing.Point(84, 548);
            this.tubeTextBox.Name = "tubeTextBox";
            this.tubeTextBox.Size = new System.Drawing.Size(100, 26);
            this.tubeTextBox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 522);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Id";
            // 
            // idTextBox
            // 
            this.idTextBox.Location = new System.Drawing.Point(84, 516);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.Size = new System.Drawing.Size(100, 26);
            this.idTextBox.TabIndex = 4;
            // 
            // reservedJobsListBox
            // 
            this.reservedJobsListBox.FormattingEnabled = true;
            this.reservedJobsListBox.ItemHeight = 20;
            this.reservedJobsListBox.Location = new System.Drawing.Point(36, 79);
            this.reservedJobsListBox.Name = "reservedJobsListBox";
            this.reservedJobsListBox.Size = new System.Drawing.Size(426, 404);
            this.reservedJobsListBox.TabIndex = 3;
            this.reservedJobsListBox.SelectedIndexChanged += new System.EventHandler(this.reservedJobsListBox_SelectedIndexChanged);
            // 
            // deleteJobButton
            // 
            this.deleteJobButton.Location = new System.Drawing.Point(287, 32);
            this.deleteJobButton.Name = "deleteJobButton";
            this.deleteJobButton.Size = new System.Drawing.Size(175, 41);
            this.deleteJobButton.TabIndex = 1;
            this.deleteJobButton.Text = "Delete Selected Job";
            this.deleteJobButton.UseVisualStyleBackColor = true;
            this.deleteJobButton.Click += new System.EventHandler(this.deleteJobButton_Click);
            // 
            // reserveButton
            // 
            this.reserveButton.Location = new System.Drawing.Point(36, 32);
            this.reserveButton.Name = "reserveButton";
            this.reserveButton.Size = new System.Drawing.Size(175, 41);
            this.reserveButton.TabIndex = 0;
            this.reserveButton.Text = "Reserve Job";
            this.reserveButton.UseVisualStyleBackColor = true;
            this.reserveButton.Click += new System.EventHandler(this.reserveButton_Click);
            // 
            // spawnWorkerButton
            // 
            this.spawnWorkerButton.Location = new System.Drawing.Point(863, 15);
            this.spawnWorkerButton.Name = "spawnWorkerButton";
            this.spawnWorkerButton.Size = new System.Drawing.Size(148, 30);
            this.spawnWorkerButton.TabIndex = 8;
            this.spawnWorkerButton.Text = "Spawn Worker";
            this.spawnWorkerButton.UseVisualStyleBackColor = true;
            this.spawnWorkerButton.Click += new System.EventHandler(this.spawnWorkerButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 889);
            this.Controls.Add(this.spawnWorkerButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hostnameTextBox);
            this.Name = "MainForm";
            this.Text = "Turbocharged.Beanstalk SampleApp";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox hostnameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button putJobButton;
        private System.Windows.Forms.Button reserveButton;
        private System.Windows.Forms.Button deleteJobButton;
        private System.Windows.Forms.ListBox reservedJobsListBox;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tubeTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ageTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ttrTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox stateTextBox;
        private System.Windows.Forms.ListBox putJobsListBox;
        private System.Windows.Forms.Button spawnWorkerButton;
    }
}

