using SS;
using SpreadsheetUtilities;
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
            spreadSheetPanel.SelectionChanged += displaySelection;
            displaySelection(spreadSheetPanel);
        }

        private bool IsCellName(string varName)
        {
            //Only looks for one letter at start.
            return Regex.IsMatch(varName, @"^[A-Z][1-9]{1,2}$");
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            int row, col;
            ss.GetSelection(out col, out row);
            string cellName = GetNameOfCell(col, row);

            cellNameBox.Text = cellName;
            UpdateTopCellVisual(cellName);
            
            ss.SetValue(col, row, spreadsheet.GetCellValue(cellName).ToString());
        }

        private string GetNameOfCell(int col , int row)
        {    
            string cellName = ((char)(col + 65)).ToString() + (row + 1);
            return cellName;
        }

        private (int, int) GetCellRowAndCol(string name)
        {
            int col = name[0] - 65;
            int row = int.Parse(name.Substring(1)) - 1;
            return (col, row);
        }

        private void UpdateTopCellVisual(string cellName)
        {
            cellValueBox.Text = spreadsheet.GetCellValue(cellName).ToString();

            object contents = spreadsheet.GetCellContents(cellName);

            if (contents is Formula f)
                cellContentBox.Text = "=" + f.ToString();
            else
                cellContentBox.Text = contents.ToString();
        }

        private void cellContentBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                int row, col;
                spreadSheetPanel.GetSelection(out col, out row);
                string cellName = GetNameOfCell(col, row);
                IList<string> dependencies = spreadsheet.SetContentsOfCell(cellName, cellContentBox.Text);

                UpdateTopCellVisual(cellName);

                foreach (string dependency in dependencies)
                {
                    (int, int) coordinates = GetCellRowAndCol(dependency);
                    spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, spreadsheet.GetCellValue(dependency).ToString());
                }
            }
           
        }
    }
}
