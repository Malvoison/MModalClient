namespace MModalClientHost
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
            this.wbSpaHost = new System.Windows.Forms.WebBrowser();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hostAlertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setHostMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setFileBlobToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.speechMikeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initializeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deinitializeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wbSpaHost
            // 
            this.wbSpaHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbSpaHost.Location = new System.Drawing.Point(0, 24);
            this.wbSpaHost.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbSpaHost.Name = "wbSpaHost";
            this.wbSpaHost.Size = new System.Drawing.Size(467, 203);
            this.wbSpaHost.TabIndex = 0;
            this.wbSpaHost.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testToolStripMenuItem,
            this.speechMikeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(467, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hostAlertToolStripMenuItem,
            this.setHostMessageToolStripMenuItem,
            this.setFileBlobToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // hostAlertToolStripMenuItem
            // 
            this.hostAlertToolStripMenuItem.Name = "hostAlertToolStripMenuItem";
            this.hostAlertToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.hostAlertToolStripMenuItem.Text = "hostAlert";
            this.hostAlertToolStripMenuItem.Click += new System.EventHandler(this.hostAlertToolStripMenuItem_Click);
            // 
            // setHostMessageToolStripMenuItem
            // 
            this.setHostMessageToolStripMenuItem.Name = "setHostMessageToolStripMenuItem";
            this.setHostMessageToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.setHostMessageToolStripMenuItem.Text = "Set Host Message";
            this.setHostMessageToolStripMenuItem.Click += new System.EventHandler(this.setHostMessageToolStripMenuItem_Click);
            // 
            // setFileBlobToolStripMenuItem
            // 
            this.setFileBlobToolStripMenuItem.Name = "setFileBlobToolStripMenuItem";
            this.setFileBlobToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.setFileBlobToolStripMenuItem.Text = "Set File Blob";
            this.setFileBlobToolStripMenuItem.Click += new System.EventHandler(this.setFileBlobToolStripMenuItem_Click);
            // 
            // speechMikeToolStripMenuItem
            // 
            this.speechMikeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initializeToolStripMenuItem,
            this.deinitializeToolStripMenuItem});
            this.speechMikeToolStripMenuItem.Name = "speechMikeToolStripMenuItem";
            this.speechMikeToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.speechMikeToolStripMenuItem.Text = "SpeechMike";
            // 
            // initializeToolStripMenuItem
            // 
            this.initializeToolStripMenuItem.Name = "initializeToolStripMenuItem";
            this.initializeToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.initializeToolStripMenuItem.Text = "Initialize";
            this.initializeToolStripMenuItem.Click += new System.EventHandler(this.initializeToolStripMenuItem_Click);
            // 
            // deinitializeToolStripMenuItem
            // 
            this.deinitializeToolStripMenuItem.Name = "deinitializeToolStripMenuItem";
            this.deinitializeToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
            this.deinitializeToolStripMenuItem.Text = "Deinitialize";
            this.deinitializeToolStripMenuItem.Click += new System.EventHandler(this.deinitializeToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(467, 227);
            this.Controls.Add(this.wbSpaHost);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MModal Client Host";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser wbSpaHost;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hostAlertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setHostMessageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem speechMikeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initializeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deinitializeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setFileBlobToolStripMenuItem;
    }
}

