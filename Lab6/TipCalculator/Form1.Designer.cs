
namespace TipCalculator
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
            this.directions = new System.Windows.Forms.Label();
            this.computeTipButton = new System.Windows.Forms.Button();
            this.totalBillBox = new System.Windows.Forms.TextBox();
            this.bottomTextBox = new System.Windows.Forms.TextBox();
            this.tipBox = new System.Windows.Forms.TextBox();
            this.tipLabel = new System.Windows.Forms.Label();
            this.totalWithTipBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // directions
            // 
            this.directions.AutoSize = true;
            this.directions.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.directions.Location = new System.Drawing.Point(77, 72);
            this.directions.Name = "directions";
            this.directions.Size = new System.Drawing.Size(171, 29);
            this.directions.TabIndex = 0;
            this.directions.Text = "Enter Total Bill";
            this.directions.Click += new System.EventHandler(this.label1_Click);
            // 
            // computeTipButton
            // 
            this.computeTipButton.Enabled = false;
            this.computeTipButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.computeTipButton.Location = new System.Drawing.Point(74, 249);
            this.computeTipButton.Name = "computeTipButton";
            this.computeTipButton.Size = new System.Drawing.Size(182, 44);
            this.computeTipButton.TabIndex = 1;
            this.computeTipButton.Text = "Compute Tip";
            this.computeTipButton.UseVisualStyleBackColor = true;
            this.computeTipButton.Click += new System.EventHandler(this.copmuteTipButton_Click);
            // 
            // totalBillBox
            // 
            this.totalBillBox.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalBillBox.Location = new System.Drawing.Point(292, 68);
            this.totalBillBox.Name = "totalBillBox";
            this.totalBillBox.Size = new System.Drawing.Size(230, 35);
            this.totalBillBox.TabIndex = 2;
            this.totalBillBox.TextChanged += new System.EventHandler(this.totalBillBox_TextChanged);
            // 
            // bottomTextBox
            // 
            this.bottomTextBox.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bottomTextBox.Location = new System.Drawing.Point(292, 253);
            this.bottomTextBox.Name = "bottomTextBox";
            this.bottomTextBox.Size = new System.Drawing.Size(230, 35);
            this.bottomTextBox.TabIndex = 3;
            // 
            // tipBox
            // 
            this.tipBox.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tipBox.Location = new System.Drawing.Point(292, 160);
            this.tipBox.Name = "tipBox";
            this.tipBox.Size = new System.Drawing.Size(230, 35);
            this.tipBox.TabIndex = 4;
            this.tipBox.TextChanged += new System.EventHandler(this.tipBox_TextChanged);
            // 
            // tipLabel
            // 
            this.tipLabel.AutoSize = true;
            this.tipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tipLabel.Location = new System.Drawing.Point(110, 163);
            this.tipLabel.Name = "tipLabel";
            this.tipLabel.Size = new System.Drawing.Size(138, 29);
            this.tipLabel.TabIndex = 5;
            this.tipLabel.Text = "Tip Percent";
            // 
            // totalWithTipBox
            // 
            this.totalWithTipBox.Font = new System.Drawing.Font("Papyrus", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalWithTipBox.Location = new System.Drawing.Point(292, 341);
            this.totalWithTipBox.Name = "totalWithTipBox";
            this.totalWithTipBox.Size = new System.Drawing.Size(230, 45);
            this.totalWithTipBox.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 457);
            this.Controls.Add(this.totalWithTipBox);
            this.Controls.Add(this.tipLabel);
            this.Controls.Add(this.tipBox);
            this.Controls.Add(this.bottomTextBox);
            this.Controls.Add(this.totalBillBox);
            this.Controls.Add(this.computeTipButton);
            this.Controls.Add(this.directions);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label directions;
        private System.Windows.Forms.Button computeTipButton;
        private System.Windows.Forms.TextBox totalBillBox;
        private System.Windows.Forms.TextBox bottomTextBox;
        private System.Windows.Forms.TextBox tipBox;
        private System.Windows.Forms.Label tipLabel;
        private System.Windows.Forms.TextBox totalWithTipBox;
    }
}

