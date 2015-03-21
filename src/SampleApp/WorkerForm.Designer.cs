namespace SampleApp
{
    partial class WorkerForm
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
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.jobsListBox = new System.Windows.Forms.ListBox();
            this.watchTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(12, 44);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(173, 67);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Enabled = false;
            this.disconnectButton.Location = new System.Drawing.Point(191, 44);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(173, 67);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "Disconnect";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // jobsListBox
            // 
            this.jobsListBox.FormattingEnabled = true;
            this.jobsListBox.ItemHeight = 20;
            this.jobsListBox.Location = new System.Drawing.Point(12, 117);
            this.jobsListBox.Name = "jobsListBox";
            this.jobsListBox.Size = new System.Drawing.Size(352, 524);
            this.jobsListBox.TabIndex = 2;
            // 
            // watchTextBox
            // 
            this.watchTextBox.Location = new System.Drawing.Point(73, 12);
            this.watchTextBox.Name = "watchTextBox";
            this.watchTextBox.Size = new System.Drawing.Size(291, 26);
            this.watchTextBox.TabIndex = 3;
            this.watchTextBox.Text = "default";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Watch";
            // 
            // WorkerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 665);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.watchTextBox);
            this.Controls.Add(this.jobsListBox);
            this.Controls.Add(this.disconnectButton);
            this.Controls.Add(this.connectButton);
            this.Name = "WorkerForm";
            this.Text = "WorkerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.ListBox jobsListBox;
        private System.Windows.Forms.TextBox watchTextBox;
        private System.Windows.Forms.Label label1;
    }
}