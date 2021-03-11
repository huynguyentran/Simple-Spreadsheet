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
using System.Text.RegularExpressions;

namespace SpreadsheetGUI
{
    public partial class Form1 : Form
    {
        private AbstractSpreadsheet spreadsheet;

        public Form1()
        {
            InitializeComponent();
            spreadSheetPanel.SetSelection(0, 0);

            spreadsheet = new Spreadsheet(IsCellName, s => s.ToUpper(), "ps6");

            spreadsheet.SetContentsOfCell("A2", "This Is Some Contents");
            spreadsheet.SetContentsOfCell("B3", "2.33");
            spreadsheet.SetContentsOfCell("C4", "=B3 + 9");
            spreadsheet.SetContentsOfCell("E5", "=1 / (1-1)");

            spreadSheetPanel.SelectionChanged += displaySelection;
            displaySelection(spreadSheetPanel);
        }

        private bool IsCellName(string varName)
        {
            //Only looks for one letter at start.
            return Regex.IsMatch(varName, @"[A-Z][\d]+");
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            ss.GetSelection(out col, out row);

            string cellName = ((char)(col + 65)).ToString() + (row + 1);
            cellNameBox.Text = cellName;

            cellValueBox.Text = spreadsheet.GetCellValue(cellName).ToString();

            cellContentBox.Text = spreadsheet.GetCellContents(cellName).ToString();

            ss.SetValue(col, row, spreadsheet.GetCellValue(cellName).ToString());
        }

    }
}
