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
        private bool openExistingFile = false;
        public Form1()
        {
            InitializeComponent();
            spreadSheetPanel.SetSelection(0, 0);
            spreadsheet = new Spreadsheet(IsCellName, s => s.ToUpper(), "ps6");
            cellNameBox.Text = "A1";
            spreadSheetPanel.SelectionChanged += displaySelection;
            displaySelection(spreadSheetPanel);
        }

        public Form1(string filename)
        {
            openExistingFile = true;
            InitializeComponent();
            spreadSheetPanel.SetSelection(0, 0);

            spreadsheet = new Spreadsheet(filename, IsCellName, s => s.ToUpper(), "ps6");

            cellNameBox.Text = "A1";
            spreadSheetPanel.SelectionChanged += displaySelection;
            displaySelection(spreadSheetPanel);
            foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
            {
                (int, int) coordinates = GetCellRowAndCol(cell);
                spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, spreadsheet.GetCellValue(cell).ToString());
            }
            openExistingFile = false;
        }

        private bool IsCellName(string varName)
        {
            return Regex.IsMatch(varName, @"^[A-Z][1-9]{1,2}$");
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            if (!openExistingFile)
            {
                SaveContents(cellNameBox.Text);
            }

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
            //Ask TA 
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
               //Ask TA 
                int row, col;
                spreadSheetPanel.GetSelection(out col, out row);
                string cellName = GetNameOfCell(col, row);
                SaveContents(cellName);
                switch (e.KeyChar)
                {
                    case (char)Keys.Up:
                        spreadSheetPanel.SetSelection(col, row + 1);
                        break;
                    case (char)Keys.Down:
                        spreadSheetPanel.SetSelection(col, row - 1);
                        break;
                    case (char)Keys.Left:
                        spreadSheetPanel.SetSelection(col - 1, row);
                        break;
                    case (char)Keys.Right:
                        spreadSheetPanel.SetSelection(col + 1, row);
                        break;
                }

            }
        }

        private void newSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new Form1());
        }


        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool CloseDialog()
        {
            if (spreadsheet.Changed == true)
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save them?", "Unsaved Changes", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.Abort:
                        return false;
                    case DialogResult.Yes:
                        DialogResult saveResult = SaveDialogBox(out SaveFileDialog saveFile);
                        if (saveResult == DialogResult.OK)
                            spreadsheet.Save(saveFile.FileName);
                        else
                            return false;
                        break;
                }
            }
            return true;
        }

        private DialogResult SaveDialogBox(out SaveFileDialog saveFile)
        {
            saveFile = new SaveFileDialog();
            saveFile.Filter = "Spreadsheet|*.sprd|All File|*";
            saveFile.Title = "Save your spreadsheet";
            return saveFile.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = SaveDialogBox(out SaveFileDialog saveFile);
            if (result == DialogResult.OK)
                spreadsheet.Save(saveFile.FileName);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Spreadsheet|*.sprd|All File|*";
            openFile.Title = "Open your spreadsheet";
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.OK)
                SpreadsheetApplicationContext.getAppContext().RunForm(new Form1(openFile.FileName));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseDialog() == false)
            {
                e.Cancel = true;
            }
        }
        /* Ask TA about
         * how to access the individual cells from spreadsheet panel.
         * how to change our code to using threads and lock, and method invoker.
         * what constitution at the features, can we use c# built in excel. 
         * modification of earlier exercies. Add more to methods for features. 
         * Independent closing
         */

        /* Ideas for additonal future
         * Changing fonts
         * Plot
         * Showing, delete. dependencies
         * 1
         */

    }
}
