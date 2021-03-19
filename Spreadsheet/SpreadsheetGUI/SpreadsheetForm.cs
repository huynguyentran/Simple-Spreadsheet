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
        /// A Queue for saving content in the cell. 
        /// </summary>
        private Queue<Tuple<string, string, bool>> saveContentQueue;

        /// <summary>
        /// A bool to indicate whether the disco mode is enabled or not. 
        /// </summary>
        private bool discoModeEnabled;

        /// <summary>
        /// A thread to run the disco function. 
        /// </summary>
        private Thread discoThread;

        /// <summary>
        /// Creates a basic empty spreadsheet.
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();

            //Making sure that everytime we open a new form, disco mode has been disabled. 
            discoModeEnabled = false;

            //Creates a queue to save content. 
            saveContentQueue = new Queue<Tuple<string, string, bool>>();

            spreadsheet = new Spreadsheet(IsCellName, s => s.ToUpper(), "ps6");

            //Setting the default selection to A1
            spreadSheetPanel.SetSelection(0, 0);
            cellNameBox.Text = "A1";
            displaySelection(spreadSheetPanel, false);

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

            //Making sure that everytime we open a new form, disco mode has been disabled. 
            discoModeEnabled = false;

            saveContentQueue = new Queue<Tuple<string, string, bool>>();

            //Creating backing spreadsheet from file.
            spreadsheet = new Spreadsheet(filename, IsCellName, s => s.ToUpper(), "ps6");

            //Setting the default selection to A1
            spreadSheetPanel.SetSelection(0, 0);
            cellNameBox.Text = "A1";
            displaySelection(spreadSheetPanel, false);

            //Every time the selection is changed, we update the visual representation.
            spreadSheetPanel.SelectionChanged += displaySelection;

            //Show all non-empty cells.
            foreach (string cell in spreadsheet.GetNamesOfAllNonemptyCells())
            {
                (int, int) coordinates = GetCellRowAndCol(cell);
                spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, spreadsheet.GetCellValue(cell).ToString());
            }
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
        private void displaySelection(SpreadsheetPanel ss, bool savePreviousData)
        {
            //We don't want to override the contents of A1 when loading from a file.
            if (savePreviousData && !discoModeEnabled)
            {
                //Saves the contents of the previous cell before moving to the next.
                SaveContents(cellNameBox.Text, cellContentBox.Text, true);
            }

            //Get the name of the cell selected.
            ss.GetSelection(out int col, out int row);
            string cellName = GetNameOfCell(col, row);

            //Update the cell name box.
            cellNameBox.Text = cellName;
            //Update the cell value and contents boxes.
            UpdateTopCellVisual(cellName);

            //Highlight the selected box.
            ss.SetValue(col, row, spreadsheet.GetCellValue(cellName).ToString());

            spreadSheetPanel.Focus();
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            displaySelection(ss, true);
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
            cellContentBox.Text = ContentsToString(contents);
        }

        /// <summary>
        ///  Turnings the cell content into string. 
        /// </summary>
        /// <param name="contents">The content of the cell</param>
        /// <returns></returns>
        private string ContentsToString(object contents)
        {
            if (contents is Formula f)
                return "=" + f.ToString();
            else
                return contents.ToString();
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
                SaveContents(cellName, cellContentBox.Text, false);
                spreadSheetPanel.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Saves the contents of a cell to the backing spreadsheet.
        /// </summary>
        /// <param name="cellName">The name of the cell to save.</param>
        private void SaveContents(string cellName, string content, bool movingToNewCell)
        {
            //When there are content needs to be save, the lock will prevent multiple workers calling on each other. 
            lock (saveContentQueue)
            {
                saveContentQueue.Enqueue(new Tuple<string, string, bool>(cellName, content, movingToNewCell));
                if (dependencyCalculator.IsBusy == false)
                {
                    cellContentBox.Enabled = false;
                    dependencyCalculator.RunWorkerAsync(saveContentQueue.Dequeue());
                }
            }

        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {

            if (spreadSheetPanel.Focused == true)
            {
                int row, col;
                spreadSheetPanel.GetSelection(out col, out row);
                string cellName = GetNameOfCell(col, row);
                //SaveContents(cellName, cellContentBox.Text);
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
                DialogResult result = MessageBox.Show("You have unsaved changes. Do you want to save them?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

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
            //Make sures that the disco function has been turn off before exit the application. 
            TurnOffDisco();

            if (CloseDialogBox() == false)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Creates a help menu to give information to the user on how to use the spreadsheet.
        /// </summary>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult helpMenu = MessageBox.Show("To change the content of the cell, clicks on the panel and writes on the text box. To confirm your new change, either presses enter or click on another cell.", "Help menu.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// When pressing Enter while on the spreadsheet panel, the focus will move to the content box so user can input in.
        /// </summary>
        private void spreadSheetPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                cellContentBox.Focus();
            }
        }

        /// <summary>
        /// Calculated and updated the dependencies of the cell, using a background worker to not intrude users' experience. 
        /// </summary>
        private void dependencyCalculator_DoWork(object sender, DoWorkEventArgs e)
        {

            Tuple<string, string, bool> arguments = (Tuple<string, string, bool>)e.Argument;

            string cellName = arguments.Item1;
            string contents = arguments.Item2;
            bool movingToNewCell = arguments.Item3;

            //The other cells' values that need to be updated after changing this cell's contents.
            IList<string> dependencies;

            //Tries to update the cell contents.
            try
            {
                dependencies = spreadsheet.SetContentsOfCell(cellName, contents);
            }
            catch (Exception exception)
            { //If an exception is encountered, show the exception in a message box.
                String message = exception.Message;
                if (exception is SS.CircularException)
                {
                    message = "A circular logic has been detected.";
                }

                Invoke(new MethodInvoker(() => MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return;
            }

            if (!movingToNewCell)
                Invoke(new MethodInvoker(() => UpdateTopCellVisual(cellName)));

            //Update the values of each cell that is dependent on the modified cell.
            foreach (string dependency in dependencies)
            {
                (int, int) coordinates = GetCellRowAndCol(dependency);
                Invoke(new MethodInvoker(() => spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, spreadsheet.GetCellValue(dependency).ToString())));
            }



        }

        /// <summary>
        /// Enable the cellContentBox after the spreadsheet has finished updated the dependencies. 
        /// </summary>
        private void dependencyCalculator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (saveContentQueue)
            {
                if (saveContentQueue.Count != 0)
                {
                    dependencyCalculator.RunWorkerAsync(saveContentQueue.Dequeue());
                }
                else
                {
                    cellContentBox.Enabled = true;
                }
            }
        }


        /// <summary>
        /// A dependecies tool that highlight the cell depedents. 
        /// </summary>
        private void dependenciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            spreadSheetPanel.GetSelection(out int col, out int row);
            string cellName = GetNameOfCell(col, row);
            object contents = spreadsheet.GetCellContents(cellName);

            //Gets the list of dependents. 
            IList<string> dependencies = spreadsheet.SetContentsOfCell(cellName, ContentsToString(contents));

            //Creates a random color everytime for the dependencies. 
            Random rnd = new Random();
            HashSet<Color> colors = new HashSet<Color>();

            //If we have highlighted other dependencies on the panel, it will choose another color. 
            Color randomColor;
            do
            {
                randomColor = Color.FromArgb(rnd.Next(165, 255), rnd.Next(165, 255), rnd.Next(165, 255));
            }
            while (!colors.Add(randomColor));

            //Highlights all depdends. 
            foreach (string cell in dependencies)
            {
                (int, int) coordinates = GetCellRowAndCol(cell);
                spreadSheetPanel.Highlight(coordinates.Item1, coordinates.Item2, randomColor);
            }

            displaySelection(spreadSheetPanel);
        }

        /// <summary>
        /// Clear all the dependencies. 
        /// </summary>
        private void clearHighlightsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            spreadSheetPanel.ClearHighlights();
            displaySelection(spreadSheetPanel);
        }

        /// <summary>
        /// Enables disco mode. 
        /// </summary>
        private void discoModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //If disco mode is not enabled. 
            if (!discoModeEnabled)
            {
                //We enable the disco
                cellContentBox.Enabled = false;
                discoModeEnabled = true;
                //Set the inital Colors of cells.
                Random rnd = new Random();
                Dictionary<(int, int), Color> cellColors = colorChange(rnd);
                //Starts a disco thread.
                discoThread = new Thread(discoWorker_DoWork);
                discoThread.Start();
            }
            //If disco mode is enabled.
            else
            {
                //We shuts down the dance floor.
                cellContentBox.Enabled = true;
                //TurnOffDisco();


                //Comment out unncessary method.
                //discoWorker_RunWorkerCompleted();
                spreadSheetPanel.ClearHighlights();
                displaySelection(spreadSheetPanel);
            }
        }

        private void TurnOffDisco()
        {
            if (discoModeEnabled)
            {
                discoModeEnabled = false;
                discoThread.Abort();
                discoThread.Join();
                Console.WriteLine("Disco effects removed");
            }
        }

        /// <summary>
        /// A private method to get a random color for a cell. 
        /// </summary>
        /// <param name="rnd">A random element for the color</param>
        /// <returns>A dictionary with cell as key and color as value</returns>
        private Dictionary<(int, int), Color> colorChange(Random rnd)
        {
            Color[] discoColors = new Color[] { Color.Red, Color.Cyan, Color.Orange, Color.Lime, Color.Magenta };

            Dictionary<(int, int), Color> cellColors = new Dictionary<(int, int), Color>();

            //Loops through every visible cells. 
            for (int col = 0; col < spreadSheetPanel.NumCols; col++)
            {
                for (int row = 0; row < spreadSheetPanel.NumRows; row++)
                {
                    cellColors.Add((col, row), discoColors[rnd.Next(discoColors.Length)]);
                }
            }
            return cellColors;
        }

        /// <summary>
        /// Changing the color of the cells constantly while disco mode is enabled. 
        /// </summary>
        private void discoWorker_DoWork()
        {
            float time = 0;
            float timeToAnimate = 0.5f;
            int deltaTime = 100;

            Random rnd = new Random();
            Dictionary<(int, int), Color> cellColors = colorChange(rnd);
            Dictionary<(int, int), Color> nextColors = colorChange(rnd);

            while (discoModeEnabled)
            {
                foreach (KeyValuePair<(int, int), Color> cellData in cellColors)
                {
                    Color interpolation;
                    if (time < timeToAnimate)

                        interpolation = SpreadsheetPanel.InterpolateColor(cellData.Value, Color.Black, time / timeToAnimate);
                    else
                        interpolation = SpreadsheetPanel.InterpolateColor(Color.Black, nextColors[cellData.Key], (time - timeToAnimate) / timeToAnimate);


                    Invoke(new MethodInvoker(() => spreadSheetPanel.Highlight(cellData.Key.Item1, cellData.Key.Item2, interpolation)));

                }

                Invoke(new MethodInvoker(() => displaySelection(spreadSheetPanel, false)));

                Thread.Sleep(deltaTime);
                time += ((float)deltaTime) / 1000;
                if (time >= timeToAnimate * 2)
                {
                    time = time % timeToAnimate * 2;
                    cellColors = nextColors;
                    nextColors = colorChange(rnd);
                }
            }
        }

        //
        //private void discoWorker_RunWorkerCompleted()
        //{
        //    spreadSheetPanel.ClearHighlights();
        //    displaySelection(spreadSheetPanel);
        //}

    }
}
