namespace SampleApp
{
    partial class Form1
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.putJobButton = new System.Windows.Forms.Button();
            this.reservedJobsListBox = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.deleteJobButton = new System.Windows.Forms.Button();
            this.putJobsLog = new System.Windows.Forms.TextBox();
            this.reservedJobsLog = new System.Windows.Forms.TextBox();
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
            this.portTextBox.TabIndex = 2;
            this.portTextBox.Text = "11300";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(585, 12);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(133, 30);
            this.connectButton.TabIndex = 4;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // disconnectButton
            // 
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
            this.groupBox1.Controls.Add(this.putJobsLog);
            this.groupBox1.Controls.Add(this.putJobButton);
            this.groupBox1.Location = new System.Drawing.Point(35, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 796);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Put";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.reservedJobsLog);
            this.groupBox2.Controls.Add(this.deleteJobButton);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.reservedJobsListBox);
            this.groupBox2.Location = new System.Drawing.Point(549, 86);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(495, 780);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Reserve";
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
            // reservedJobsListBox
            // 
            this.reservedJobsListBox.FormattingEnabled = true;
            this.reservedJobsListBox.ItemHeight = 20;
            this.reservedJobsListBox.Location = new System.Drawing.Point(36, 359);
            this.reservedJobsListBox.Name = "reservedJobsListBox";
            this.reservedJobsListBox.Size = new System.Drawing.Size(426, 384);
            this.reservedJobsListBox.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(36, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(175, 41);
            this.button2.TabIndex = 2;
            this.button2.Text = "Reserve Job";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // deleteJobButton
            // 
            this.deleteJobButton.Location = new System.Drawing.Point(36, 312);
            this.deleteJobButton.Name = "deleteJobButton";
            this.deleteJobButton.Size = new System.Drawing.Size(175, 41);
            this.deleteJobButton.TabIndex = 3;
            this.deleteJobButton.Text = "Delete Job";
            this.deleteJobButton.UseVisualStyleBackColor = true;
            // 
            // putJobsLog
            // 
            this.putJobsLog.AcceptsReturn = true;
            this.putJobsLog.Location = new System.Drawing.Point(22, 95);
            this.putJobsLog.Multiline = true;
            this.putJobsLog.Name = "putJobsLog";
            this.putJobsLog.Size = new System.Drawing.Size(459, 664);
            this.putJobsLog.TabIndex = 1;
            // 
            // reservedJobsLog
            // 
            this.reservedJobsLog.Location = new System.Drawing.Point(36, 79);
            this.reservedJobsLog.Multiline = true;
            this.reservedJobsLog.Name = "reservedJobsLog";
            this.reservedJobsLog.Size = new System.Drawing.Size(426, 215);
            this.reservedJobsLog.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1070, 889);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hostnameTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.ListBox reservedJobsListBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button deleteJobButton;
        private System.Windows.Forms.TextBox putJobsLog;
        private System.Windows.Forms.TextBox reservedJobsLog;
    }
}

