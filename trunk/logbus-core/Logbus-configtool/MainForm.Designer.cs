namespace It.Unina.Dis.Logbus.Configtool
{
    partial class MainForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Liberare le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Logbus Node configuration");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Logbus Client configuration");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Logbus Source configuration");
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnFile = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtAppConfig = new System.Windows.Forms.TextBox();
            this.treeConfig = new System.Windows.Forms.TreeView();
            this.pnlEditControl = new System.Windows.Forms.Panel();
            this.mnContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnUpdate = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnFile,
            this.mnHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(608, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "mainMenu";
            // 
            // mnFile
            // 
            this.mnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.mnFile.Name = "mnFile";
            this.mnFile.Size = new System.Drawing.Size(37, 20);
            this.mnFile.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // mnHelp
            // 
            this.mnHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnAbout});
            this.mnHelp.Name = "mnHelp";
            this.mnHelp.Size = new System.Drawing.Size(24, 20);
            this.mnHelp.Text = "?";
            // 
            // mnAbout
            // 
            this.mnAbout.Name = "mnAbout";
            this.mnAbout.Size = new System.Drawing.Size(152, 22);
            this.mnAbout.Text = "About";
            this.mnAbout.Click += new System.EventHandler(this.mnAbout_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(13, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(317, 13);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Create an XML configuration file for Logbus-ng or one of its clients";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtAppConfig
            // 
            this.txtAppConfig.AccessibleRole = System.Windows.Forms.AccessibleRole.Document;
            this.txtAppConfig.CausesValidation = false;
            this.txtAppConfig.Location = new System.Drawing.Point(322, 44);
            this.txtAppConfig.Multiline = true;
            this.txtAppConfig.Name = "txtAppConfig";
            this.txtAppConfig.ReadOnly = true;
            this.txtAppConfig.Size = new System.Drawing.Size(274, 424);
            this.txtAppConfig.TabIndex = 0;
            // 
            // treeConfig
            // 
            this.treeConfig.Location = new System.Drawing.Point(13, 44);
            this.treeConfig.Name = "treeConfig";
            treeNode1.Name = "logbusNode";
            treeNode1.Text = "Logbus Node configuration";
            treeNode2.Name = "logbusClient";
            treeNode2.Text = "Logbus Client configuration";
            treeNode3.Name = "logbusSource";
            treeNode3.Text = "Logbus Source configuration";
            this.treeConfig.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.treeConfig.Size = new System.Drawing.Size(303, 175);
            this.treeConfig.TabIndex = 2;
            // 
            // pnlEditControl
            // 
            this.pnlEditControl.Location = new System.Drawing.Point(16, 254);
            this.pnlEditControl.Name = "pnlEditControl";
            this.pnlEditControl.Size = new System.Drawing.Size(300, 160);
            this.pnlEditControl.TabIndex = 3;
            // 
            // mnContext
            // 
            this.mnContext.Name = "contextMenuStrip1";
            this.mnContext.Size = new System.Drawing.Size(61, 4);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(16, 444);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Update XML";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 480);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.treeConfig);
            this.Controls.Add(this.pnlEditControl);
            this.Controls.Add(this.txtAppConfig);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Logbus-ng Configuration tool";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnFile;
        private System.Windows.Forms.ToolStripMenuItem mnHelp;
        private System.Windows.Forms.ToolStripMenuItem mnAbout;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtAppConfig;
        private System.Windows.Forms.TreeView treeConfig;
        private System.Windows.Forms.Panel pnlEditControl;
        private System.Windows.Forms.ContextMenuStrip mnContext;
        private System.Windows.Forms.Button btnUpdate;
    }
}

