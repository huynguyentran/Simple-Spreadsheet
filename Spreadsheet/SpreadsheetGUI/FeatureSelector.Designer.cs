
namespace SpreadsheetGUI
{
    partial class FeatureSelector
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
            this.operationLabel = new System.Windows.Forms.Label();
            this.upperLeftLabel = new System.Windows.Forms.Label();
            this.lowerRightLabel = new System.Windows.Forms.Label();
            this.upperLeftBox = new System.Windows.Forms.TextBox();
            this.lowerRightBox = new System.Windows.Forms.TextBox();
            this.outputCellLabel = new System.Windows.Forms.Label();
            this.outputCellBox = new System.Windows.Forms.TextBox();
            this.finishButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // operationLabel
            // 
            this.operationLabel.AutoSize = true;
            this.operationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.operationLabel.Location = new System.Drawing.Point(22, 28);
            this.operationLabel.Name = "operationLabel";
            this.operationLabel.Size = new System.Drawing.Size(186, 29);
            this.operationLabel.TabIndex = 0;
            this.operationLabel.Text = "Operation Label";
            // 
            // upperLeftLabel
            // 
            this.upperLeftLabel.AutoSize = true;
            this.upperLeftLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.upperLeftLabel.Location = new System.Drawing.Point(27, 90);
            this.upperLeftLabel.Name = "upperLeftLabel";
            this.upperLeftLabel.Size = new System.Drawing.Size(260, 29);
            this.upperLeftLabel.TabIndex = 1;
            this.upperLeftLabel.Text = "Upper Left Corner Cell:";
            // 
            // lowerRightLabel
            // 
            this.lowerRightLabel.AutoSize = true;
            this.lowerRightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lowerRightLabel.Location = new System.Drawing.Point(27, 148);
            this.lowerRightLabel.Name = "lowerRightLabel";
            this.lowerRightLabel.Size = new System.Drawing.Size(277, 29);
            this.lowerRightLabel.TabIndex = 2;
            this.lowerRightLabel.Text = "Lower Right Corner Cell:";
            // 
            // upperLeftBox
            // 
            this.upperLeftBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.upperLeftBox.Location = new System.Drawing.Point(365, 84);
            this.upperLeftBox.Name = "upperLeftBox";
            this.upperLeftBox.Size = new System.Drawing.Size(180, 35);
            this.upperLeftBox.TabIndex = 3;
            // 
            // lowerRightBox
            // 
            this.lowerRightBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lowerRightBox.Location = new System.Drawing.Point(365, 142);
            this.lowerRightBox.Name = "lowerRightBox";
            this.lowerRightBox.Size = new System.Drawing.Size(180, 35);
            this.lowerRightBox.TabIndex = 4;
            // 
            // outputCellLabel
            // 
            this.outputCellLabel.AutoSize = true;
            this.outputCellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputCellLabel.Location = new System.Drawing.Point(27, 201);
            this.outputCellLabel.Name = "outputCellLabel";
            this.outputCellLabel.Size = new System.Drawing.Size(139, 29);
            this.outputCellLabel.TabIndex = 5;
            this.outputCellLabel.Text = "Output Cell:";
            // 
            // outputCellBox
            // 
            this.outputCellBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputCellBox.Location = new System.Drawing.Point(199, 198);
            this.outputCellBox.Name = "outputCellBox";
            this.outputCellBox.Size = new System.Drawing.Size(180, 35);
            this.outputCellBox.TabIndex = 6;
            // 
            // finishButton
            // 
            this.finishButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finishButton.Location = new System.Drawing.Point(243, 259);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 33);
            this.finishButton.TabIndex = 7;
            this.finishButton.Text = "OK";
            this.finishButton.UseVisualStyleBackColor = true;
            // 
            // FeatureSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 304);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.outputCellBox);
            this.Controls.Add(this.outputCellLabel);
            this.Controls.Add(this.lowerRightBox);
            this.Controls.Add(this.upperLeftBox);
            this.Controls.Add(this.lowerRightLabel);
            this.Controls.Add(this.upperLeftLabel);
            this.Controls.Add(this.operationLabel);
            this.Name = "FeatureSelector";
            this.Text = "FeatureSelector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label operationLabel;
        private System.Windows.Forms.Label upperLeftLabel;
        private System.Windows.Forms.Label lowerRightLabel;
        private System.Windows.Forms.TextBox upperLeftBox;
        private System.Windows.Forms.TextBox lowerRightBox;
        private System.Windows.Forms.Label outputCellLabel;
        private System.Windows.Forms.TextBox outputCellBox;
        private System.Windows.Forms.Button finishButton;
    }
}