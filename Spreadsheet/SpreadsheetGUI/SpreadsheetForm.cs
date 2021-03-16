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
using System.Threading;

namespace SpreadsheetGUI
{
    /// <summary>
    /// A form that emulates a spreadsheet editor.
    /// </summary>
    /// <author>Huy Nguyen</author>
    /// <author>William Erignac</author>
    /// <version>3/13/2021</version>
    public partial class SpreadsheetForm : Form
    {
        /// <summary>
        /// Spreadsheet that holds cell contents and values.
        /// </summary>
        private AbstractSpreadsheet spreadsheet;
        
        /// <summary>
        /// Whether this spreadsheet was opened from a file.
        /// </summary>
        private bool openExistingFile = false;
        
        /// <summary>
        /// Creates a basic empty spreadsheet.
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();

            spreadsheet = new Spreadsheet(IsCellName, s => s.ToUpper(), "ps6");

            //Setting the default selection to A1
            spreadSheetPanel.SetSelection(0, 0);
            cellNameBox.Text = "A1";
            displaySelection(spreadSheetPanel);

            //Every time the selection is changed, we update the visual representation.
            spreadSheetPanel.SelectionChanged += displaySelection;
        }

        /// <summary>
        /// Creates a spreadsheet from a file.
        /// </summary>
        /// <param name="filename">The file to generate the spreadsheet from.</param>
        public SpreadsheetForm(string filename)
        {
            InitializeComponent();

            //We're opening from an existing file.
            openExistingFile = true;
            
            //Creating backing spreadsheet from file.
            spreadsheet = new Spreadsheet(filename, IsCellName, s => s.ToUpper(), "ps6");

            //Setting the default selection to A1
            spreadSheetPanel.SetSelection(0, 0);
            cellNameBox.Text = "A1";
            displaySelection(spreadSheetPanel);

            //Every time the selection is changed, we update the visual representation.
            spreadSheetPanel.SelectionChanged += displaySelection;

            //Show all non-empty cells.
            foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
            {
                (int, int) coordinates = GetCellRowAndCol(cell);
                spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, spreadsheet.GetCellValue(cell).ToString());
            }

            openExistingFile = false;
        }

        /// <summary>
        /// Checks if a cell name exists in the range of the spreadsheet.
        /// </summary>
        /// <param name="varName">The name of a variable referencing a cell.</param>
        /// <returns>Whether the name exists in the range of the spreadsheet.</returns>
        private bool IsCellName(string varName)
        {
            return Regex.IsMatch(varName, @"^[A-Z][1-9][0-9]?$");
        }

        /// <summary>
        /// Updates the SpreadsheetPanel to show the latest selection.
        /// </summary>
        /// <param name="ss">The SpreadsheetPanel to update.</param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            //We don't want to override the contents of A1 when loading from a file.
            if (!openExistingFile)
            {
                SaveContents(cellNameBox.Text);
            }

            spreadSheetPanel.Focus();
            //Get the name of the cell selected.
            ss.GetSelection(out int col, out int row);
            string cellName = GetNameOfCell(col, row);

            //Update the cell name box.
            cellNameBox.Text = cellName;
            //Update the cell value and contents boxes.
            UpdateTopCellVisual(cellName);

            //Highlight the selected box.
            ss.SetValue(col, row, spreadsheet.GetCellValue(cellName).ToString());
        }

        /// <summary>
        /// Converts cell coordinates to the cell name.
        /// </summary>
        /// <param name="col">The column of the cell.</param>
        /// <param name="row">The row of the cell.</param>
        /// <returns>The cell name.</returns>
        private string GetNameOfCell(int col, int row)
        {
            string cellName = ((char)(col + 65)).ToString() + (row + 1);
            return cellName;
        }

        /// <summary>
        /// Gets the cell coordinates for the cell name.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <returns>The cell column and row.</returns>
        private (int, int) GetCellRowAndCol(string name)
        {
            int col = name[0] - 65;
            int row = int.Parse(name.Substring(1)) - 1;

            return (col, row);
        }

        /// <summary>
        /// Updates the top cell boxes that show the contents and the value.
        /// </summary>
        /// <param name="cellName">The name of the cell to update the visuals.</param>
        private void UpdateTopCellVisual(string cellName)
        {
           
            cellValueBox.Text = spreadsheet.GetCellValue(cellName).ToString();
         
            object contents = spreadsheet.GetCellContents(cellName);
            //Check if the contents of the cell is a formula and display the formula correctly.
            if (contents is Formula f)
                cellContentBox.Text = "=" + f.ToString();
            else
                cellContentBox.Text = contents.ToString();
        }

        /// <summary>
        /// Checks if enter was pressed while in the contents text box.
        /// </summary>
        /// <param name="e">The key that is pressed.</param>
        private void cellContentBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //If enter is pressed, update the cell contents and value.
            if (e.KeyChar == (char)Keys.Return)
            {
                int row, col;
                spreadSheetPanel.GetSelection(out col, out row);
                string cellName = GetNameOfCell(col, row);
                SaveContents(cellName);
                spreadSheetPanel.Focus();
            }
        }

        /// <summary>
        /// Saves the contents of a cell to the backing spreadsheet.
        /// </summary>
        /// <param name="cellName">The name of the cell to save.</param>
        private void SaveContents(string cellName)
        {
            //The other cells' values that need to be updated after changing this cell's contents.
            IList<string> dependencies;

            //Tries to update the cell contents.
            try
            {
                dependencies = spreadsheet.SetContentsOfCell(cellName, cellContentBox.Text);
            }
            catch (Exception e)
            { //If an exception is encountered, show the exception in a message box.
                MessageBox.Show(e.Message);
                return;
            }

            UpdateTopCellVisual(cellName);
            //backgroundWorker1.RunWorkerAsync(dependencies);
            foreach (string dependency in dependencies)
            {
                (int, int) coordinates = GetCellRowAndCol(dependency);
                spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, spreadsheet.GetCellValue(dependency).ToString());
            }
            //Update the values of each cell that is dependent on the modified cell.

        }


        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
        
            if (spreadSheetPanel.Focused == true)
            {
                int row, col;
                spreadSheetPanel.GetSelection(out col, out row);
                string cellName = GetNameOfCell(col, row);
                SaveContents(cellName);
                switch (keyData)
                {
                    case Keys.Up:
                        spreadSheetPanel.SetSelection(col, row - 1);
                        displaySelection(spreadSheetPanel);
                        return true;
                    case Keys.Down:
                        spreadSheetPanel.SetSelection(col, row + 1);
                        displaySelection(spreadSheetPanel);
                        return true;
                    case Keys.Left:
                        spreadSheetPanel.SetSelection(col - 1, row);
                        displaySelection(spreadSheetPanel);
                        return true;
                    case Keys.Right:
                        spreadSheetPanel.SetSelection(col + 1, row);
                        displaySelection(spreadSheetPanel);
                        return true;
                }
            }
      
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Opens a new spreadsheet from the "New" menu button.
        /// </summary>
        private void newSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
              SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetForm());
           // Thread thread = new Thread(Application.Run(new SpreadsheetForm()));
        }

        /// <summary>
        /// Closes the spreadsheet editor when the "Close" button is pressed in
        /// </summary>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Creates a new dialog box if the program tries to close without saving.
        /// </summary>
        /// <returns>Whether the user wants to continue closing.</returns>
        private bool CloseDialogBox()
        {
            if (spreadsheet.Changed == true)
            {
                //Open the box if the spreadsheet was changed.
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save them?", "Unsaved Changes", MessageBoxButtons.YesNoCancel);
                
                switch (result)
                {
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.Abort:
                        return false;
                    //If the user decides to save, we open the save dialog and ensure that a save occurs.
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

        /// <summary>
        /// Creates a save dialogue to save the spreadsheet with the default tag .sprd.
        /// </summary>
        /// <param name="saveFile">The dialog box.</param>
        /// <returns>The result for running the box.</returns>
        private DialogResult SaveDialogBox(out SaveFileDialog saveFile)
        {
            saveFile = new SaveFileDialog();
            saveFile.Filter = "Spreadsheet|*.sprd|All File|*";
            saveFile.Title = "Save your spreadsheet";
            return saveFile.ShowDialog();
        }

        /// <summary>
        /// Saves a spreadsheet when the "Save" button in the menu is clicked.
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = SaveDialogBox(out SaveFileDialog saveFile);
            if (result == DialogResult.OK)
                spreadsheet.Save(saveFile.FileName);
        }

        /// <summary>
        /// Opens a spreadsheet when the "Open" button in the menu is clicked.
        /// </summary>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Spreadsheet|*.sprd|All File|*";
            openFile.Title = "Open your spreadsheet";
            DialogResult result = openFile.ShowDialog();
            if (result == DialogResult.OK)
               SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetForm(openFile.FileName));

        }

        /// <summary>
        /// Checks if the spreadsheet has changes before closing.
        /// If the user wants to cancel the closing process is stopped.
        /// </summary>
        private void SpreadsheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseDialogBox() == false)
            {
                e.Cancel = true;
            }
          //  if (Application.OpenForms.count)

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult helpMenu = MessageBox.Show("To change the content of the cell, clicks on the panel and writes on the text box. To confirm your new change, either presses enter or click on another cell.","Help menu.");
        }

        private void spreadSheetPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                cellContentBox.Focus();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
       
        
        }


        /* Ask TA about
        * how to access the individual cells from spreadsheet panel.
        * how to change our code to using threads and lock, and method invoker.
        * what constitution at the features, can we use c# built in excel. 
        * modification of earlier exercies. Add more to methods for features. 
        * Independent closing
        *    the sound when pressing enter
        */

        /* Ideas for additonal future
         * Changing fonts
         * Plot
         * Showing, delete. dependencies
         * 1
         */

    }
}
