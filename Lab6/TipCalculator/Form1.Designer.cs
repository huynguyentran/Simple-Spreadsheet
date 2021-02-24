
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
            this.totalBillBox = new System.Windows.Forms.TextBox();
            this.bottomTextBox = new System.Windows.Forms.TextBox();
            this.tipBox = new System.Windows.Forms.TextBox();
            this.tipLabel = new System.Windows.Forms.Label();
            this.totalWithTipBox = new System.Windows.Forms.TextBox();
            this.percentSign = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // directions
            // 
            this.directions.AutoSize = true;
            this.directions.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.directions.Location = new System.Drawing.Point(60, 72);
            this.directions.Name = "directions";
            this.directions.Size = new System.Drawing.Size(171, 29);
            this.directions.TabIndex = 0;
            this.directions.Text = "Enter Total Bill";
            this.directions.Click += new System.EventHandler(this.label1_Click);
            // 
            // totalBillBox
            // 
            this.totalBillBox.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalBillBox.Location = new System.Drawing.Point(275, 68);
            this.totalBillBox.Name = "totalBillBox";
            this.totalBillBox.Size = new System.Drawing.Size(230, 35);
            this.totalBillBox.TabIndex = 2;
            this.totalBillBox.TextChanged += new System.EventHandler(this.totalBillBox_TextChanged);
            // 
            // bottomTextBox
            // 
            this.bottomTextBox.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bottomTextBox.Location = new System.Drawing.Point(275, 253);
            this.bottomTextBox.Name = "bottomTextBox";
            this.bottomTextBox.ReadOnly = true;
            this.bottomTextBox.Size = new System.Drawing.Size(230, 35);
            this.bottomTextBox.TabIndex = 3;
            this.bottomTextBox.TextChanged += new System.EventHandler(this.bottomTextBox_TextChanged);
            // 
            // tipBox
            // 
            this.tipBox.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tipBox.Location = new System.Drawing.Point(275, 160);
            this.tipBox.Name = "tipBox";
            this.tipBox.Size = new System.Drawing.Size(230, 35);
            this.tipBox.TabIndex = 4;
            this.tipBox.TextChanged += new System.EventHandler(this.tipBox_TextChanged);
            // 
            // tipLabel
            // 
            this.tipLabel.AutoSize = true;
            this.tipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tipLabel.Location = new System.Drawing.Point(93, 163);
            this.tipLabel.Name = "tipLabel";
            this.tipLabel.Size = new System.Drawing.Size(138, 29);
            this.tipLabel.TabIndex = 5;
            this.tipLabel.Text = "Tip Percent";
            // 
            // totalWithTipBox
            // 
            this.totalWithTipBox.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalWithTipBox.Location = new System.Drawing.Point(275, 341);
            this.totalWithTipBox.Name = "totalWithTipBox";
            this.totalWithTipBox.ReadOnly = true;
            this.totalWithTipBox.Size = new System.Drawing.Size(230, 35);
            this.totalWithTipBox.TabIndex = 6;
            // 
            // percentSign
            // 
            this.percentSign.AutoSize = true;
            this.percentSign.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.percentSign.Location = new System.Drawing.Point(520, 163);
            this.percentSign.Name = "percentSign";
            this.percentSign.Size = new System.Drawing.Size(35, 29);
            this.percentSign.TabIndex = 7;
            this.percentSign.Text = "%";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 435);
            this.Controls.Add(this.percentSign);
            this.Controls.Add(this.totalWithTipBox);
            this.Controls.Add(this.tipLabel);
            this.Controls.Add(this.tipBox);
            this.Controls.Add(this.bottomTextBox);
            this.Controls.Add(this.totalBillBox);
            this.Controls.Add(this.directions);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label directions;
        private System.Windows.Forms.TextBox totalBillBox;
        private System.Windows.Forms.TextBox bottomTextBox;
        private System.Windows.Forms.TextBox tipBox;
        private System.Windows.Forms.Label tipLabel;
        private System.Windows.Forms.TextBox totalWithTipBox;
        private System.Windows.Forms.Label percentSign;
    }
}

