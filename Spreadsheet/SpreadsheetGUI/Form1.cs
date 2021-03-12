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
            cellNameBox.Text = "A1";
            spreadSheetPanel.SelectionChanged += displaySelection;
            displaySelection(spreadSheetPanel);
        }

        private bool IsCellName(string varName)
        {
            return Regex.IsMatch(varName, @"^[A-Z][1-9]{1,2}$");
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            SaveContents(cellNameBox.Text);
            int row, col;
            ss.GetSelection(out col, out row);
            string cellName = GetNameOfCell(col, row);

            cellNameBox.Text = cellName;
            UpdateTopCellVisual(cellName);

            ss.SetValue(col, row, spreadsheet.GetCellValue(cellName).ToString());
        }

        private string GetNameOfCell(int col, int row)
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
                SaveContents(cellName);
            }
         
        }

        private void SaveContents(string cellName)
        {
            IList<string> dependencies;

            try
            {
                dependencies = spreadsheet.SetContentsOfCell(cellName, cellContentBox.Text);
            }
            catch (CircularException c)
            {
                MessageBox.Show(c.Message);
                return;
            }

            UpdateTopCellVisual(cellName);

            foreach (string dependency in dependencies)
            {
                (int, int) coordinates = GetCellRowAndCol(dependency);
                spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, spreadsheet.GetCellValue(dependency).ToString());
            }
        }

        private void spreadSheetPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Up || e.KeyChar == (char)Keys.Down || e.KeyChar == (char)Keys.Left || e.KeyChar == (char)Keys.Right)
            {
              
                int row, col;
                spreadSheetPanel.GetSelection(out col, out row);
                string cellName = GetNameOfCell(col, row);
                SaveContents(cellName);
                switch (e.KeyChar)
                {
                    case (char)Keys.Up: spreadSheetPanel.SetSelection(col,  row + 1);
                        break;
                    case (char)Keys.Down: spreadSheetPanel.SetSelection(col, row - 1);
                        break;
                    case (char)Keys.Left: spreadSheetPanel.SetSelection(col-1, row );
                        break;
                    case (char)Keys.Right: spreadSheetPanel.SetSelection(col + 1, row );
                        break;
                }
                 
            }
        }

        private void newSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new Form1());
        }
    }
}
