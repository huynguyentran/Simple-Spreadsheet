﻿
namespace SpreadsheetGUI
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
            this.cellNameBox = new System.Windows.Forms.TextBox();
            this.cellValueBox = new System.Windows.Forms.TextBox();
            this.cellContentBox = new System.Windows.Forms.TextBox();
            this.spreadSheetPanel = new SS.SpreadsheetPanel();
            this.SuspendLayout();
            // 
            // cellNameBox
            // 
            this.cellNameBox.Enabled = false;
            this.cellNameBox.Location = new System.Drawing.Point(10, 6);
            this.cellNameBox.Margin = new System.Windows.Forms.Padding(2);
            this.cellNameBox.Name = "cellNameBox";
            this.cellNameBox.Size = new System.Drawing.Size(76, 20);
            this.cellNameBox.TabIndex = 1;
            // 
            // cellValueBox
            // 
            this.cellValueBox.Enabled = false;
            this.cellValueBox.Location = new System.Drawing.Point(110, 6);
            this.cellValueBox.Margin = new System.Windows.Forms.Padding(2);
            this.cellValueBox.Name = "cellValueBox";
            this.cellValueBox.Size = new System.Drawing.Size(76, 20);
            this.cellValueBox.TabIndex = 2;
            // 
            // cellContentBox
            // 
            this.cellContentBox.Location = new System.Drawing.Point(221, 6);
            this.cellContentBox.Margin = new System.Windows.Forms.Padding(2);
            this.cellContentBox.Name = "cellContentBox";
            this.cellContentBox.Size = new System.Drawing.Size(76, 20);
            this.cellContentBox.TabIndex = 3;
       
            this.cellContentBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cellContentBox_KeyPress);
            // 
            // spreadSheetPanel
            // 
            this.spreadSheetPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadSheetPanel.Location = new System.Drawing.Point(10, 37);
            this.spreadSheetPanel.Margin = new System.Windows.Forms.Padding(2);
            this.spreadSheetPanel.Name = "spreadSheetPanel";
            this.spreadSheetPanel.Size = new System.Drawing.Size(651, 382);
            this.spreadSheetPanel.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 422);
            this.Controls.Add(this.spreadSheetPanel);
            this.Controls.Add(this.cellContentBox);
            this.Controls.Add(this.cellValueBox);
            this.Controls.Add(this.cellNameBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox cellNameBox;
        private System.Windows.Forms.TextBox cellValueBox;
        private System.Windows.Forms.TextBox cellContentBox;
        private SS.SpreadsheetPanel spreadSheetPanel;
    }
}
