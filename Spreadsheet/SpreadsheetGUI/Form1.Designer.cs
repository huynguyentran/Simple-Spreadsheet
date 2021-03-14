
namespace SpreadsheetGUI
{
    partial class SpreadsheetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpreadsheetForm));
            this.cellNameBox = new System.Windows.Forms.TextBox();
            this.cellValueBox = new System.Windows.Forms.TextBox();
            this.cellContentBox = new System.Windows.Forms.TextBox();
            this.spreadSheetPanel = new SS.SpreadsheetPanel();
            this.menu = new System.Windows.Forms.ToolStrip();
            this.fileMenuButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.newSpreadsheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // cellNameBox
            // 
            this.cellNameBox.Enabled = false;
            this.cellNameBox.Location = new System.Drawing.Point(13, 41);
            this.cellNameBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cellNameBox.Name = "cellNameBox";
            this.cellNameBox.Size = new System.Drawing.Size(100, 22);
            this.cellNameBox.TabIndex = 1;
            // 
            // cellValueBox
            // 
            this.cellValueBox.Enabled = false;
            this.cellValueBox.Location = new System.Drawing.Point(132, 41);
            this.cellValueBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cellValueBox.Name = "cellValueBox";
            this.cellValueBox.Size = new System.Drawing.Size(142, 22);
            this.cellValueBox.TabIndex = 2;
            // 
            // cellContentBox
            // 
            this.cellContentBox.Location = new System.Drawing.Point(295, 41);
            this.cellContentBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cellContentBox.Name = "cellContentBox";
            this.cellContentBox.Size = new System.Drawing.Size(918, 22);
            this.cellContentBox.TabIndex = 3;
            this.cellContentBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cellContentBox_KeyPress);
            // 
            // spreadSheetPanel
            // 
            this.spreadSheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadSheetPanel.Location = new System.Drawing.Point(13, 78);
            this.spreadSheetPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.spreadSheetPanel.Name = "spreadSheetPanel";
            this.spreadSheetPanel.Size = new System.Drawing.Size(1340, 560);
            this.spreadSheetPanel.TabIndex = 4;
            this.spreadSheetPanel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.spreadSheetPanel_KeyPress);
            // 
            // menu
            // 
            this.menu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuButton});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(1357, 27);
            this.menu.TabIndex = 5;
            this.menu.Text = "toolStrip1";
            // 
            // fileMenuButton
            // 
            this.fileMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileMenuButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSpreadsheetToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileMenuButton.Image = ((System.Drawing.Image)(resources.GetObject("fileMenuButton.Image")));
            this.fileMenuButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileMenuButton.Name = "fileMenuButton";
            this.fileMenuButton.Size = new System.Drawing.Size(46, 24);
            this.fileMenuButton.Text = "File";
            // 
            // newSpreadsheetToolStripMenuItem
            // 
            this.newSpreadsheetToolStripMenuItem.Name = "newSpreadsheetToolStripMenuItem";
            this.newSpreadsheetToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.newSpreadsheetToolStripMenuItem.Text = "New";
            this.newSpreadsheetToolStripMenuItem.Click += new System.EventHandler(this.newSpreadsheetToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1357, 641);
            this.Controls.Add(this.menu);
            this.Controls.Add(this.spreadSheetPanel);
            this.Controls.Add(this.cellContentBox);
            this.Controls.Add(this.cellValueBox);
            this.Controls.Add(this.cellNameBox);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SpreadsheetForm";
            this.Text = "SpreadsheetForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpreadsheetForm_FormClosing);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox cellNameBox;
        private System.Windows.Forms.TextBox cellValueBox;
        private System.Windows.Forms.TextBox cellContentBox;
        private SS.SpreadsheetPanel spreadSheetPanel;
        private System.Windows.Forms.ToolStrip menu;
        private System.Windows.Forms.ToolStripDropDownButton fileMenuButton;
        private System.Windows.Forms.ToolStripMenuItem newSpreadsheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    }
}

