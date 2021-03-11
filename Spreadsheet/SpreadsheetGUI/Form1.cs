using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            spreadSheetPanel.SetSelection(0, 0);
            spreadSheetPanel.SelectionChanged += displaySelection;
            displaySelection(spreadSheetPanel);

      
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            ss.GetSelection(out col, out row);

            row = row + 1;
            string rowStr = "" + row;
            char c = (char)(col+65);

            cellNameBox.Text = c + rowStr ;
        }

    }
}
