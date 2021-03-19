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
    /// <version>3/19/2021</version>
    public partial class SpreadsheetForm : Form
    {
        /// <summary>
        /// Spreadsheet that holds cell contents and values.
        /// </summary>
        private AbstractSpreadsheet spreadsheet;

        /// <summary>
        /// A queue for buffering calls to save contents in cells. 
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
        /// A set of colors used to highlight dependency groups.
        /// </summary>
        private HashSet<Color> dependencyGroupColors;

        /// <summary>
        /// Creates a basic empty spreadsheet.
        /// </summary>
        public SpreadsheetForm()
        {
            InitializeComponent();

            //Making sure that every time we open a new form, disco mode has been disabled. 
            discoModeEnabled = false;

            //Creates a queue to buffer calls to save contents. 
            saveContentQueue = new Queue<Tuple<string, string, bool>>();

            dependencyGroupColors = new HashSet<Color>();

            spreadsheet = new Spreadsheet(IsCellName, s => s.ToUpper(), "ps6");

            //Setting the default selection to A1
            spreadSheetPanel.SetSelection(0, 0);
            cellNameBox.Text = "A1";
            displaySelection(spreadSheetPanel, false);

            //Every time the selection is changed, we update the visual representation.
            spreadSheetPanel.SelectionChanged += displaySelection;
        }

        /// <summary>
        /// Changes the state of the spreadsheet to a spreadsheet in a file.
        /// </summary>
        /// <param name="filename">The file to generate the spreadsheet from.</param>
        public void SpreadsheetFormOpenFromFile(string filename)
        {

            //Making sure that every time we open a new form, disco mode has been disabled. 
            TurnOffDisco();

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
                spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, ValueToString(spreadsheet.GetCellValue(cell)));
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
            //Check if we want to save unentered contents (nothing is saved in disco mode).
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
            ss.SetValue(col, row, ValueToString(spreadsheet.GetCellValue(cellName)));

            /*
             * Move focus back to the spreadsheet, unless if we're in disco mode
             * (focus is always on the spreadsheet in disco mode).
             */
            if (!discoModeEnabled)
            {
                spreadSheetPanel.Focus();
            }
        }

        /// <summary>
        /// By default we save the previous contents of the cell we're leaving.
        /// </summary>
        /// <param name="ss">The spreadsheet panel to update.</param>
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
            //Check if the value of the cell is a formula error and display the error correctly.
            cellValueBox.Text = ValueToString(spreadsheet.GetCellValue(cellName));
            object contents = spreadsheet.GetCellContents(cellName);
            //Check if the contents of the cell is a formula and display the formula correctly.
            cellContentBox.Text = ContentsToString(contents);
        }

        /// <summary>
        ///  Turning the cell content into string. 
        /// </summary>
        /// <param name="contents">The content of the cell</param>
        /// <returns>The string representaion of the contents to print.</returns>
        private string ContentsToString(object contents)
        {
            if (contents is Formula f)
                return "=" + f.ToString();
            else
                return contents.ToString();
        }

        /// <summary>
        /// Turns the value of a cell into a string to be displayed.
        /// (i.e. checks for FormulaErrors.)
        /// </summary>
        /// <param name="value">The value of a cell.</param>
        /// <returns>The string representaion of the value to print.</returns>
        private string ValueToString(object value)
        {
            if (value is FormulaError f)
            {
                return "ERROR";
            }
            else
                return value.ToString();
        }

        /// <summary>
        /// Checks if enter was pressed while in the contents text box,
        /// and save the contents if it is the case.
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
            //When there are multiple calls to save contents, the lock will prevent overlapping workers calls. 
            lock (saveContentQueue)
            {
                //Queue keeps track of all the saves that need to occur.
                saveContentQueue.Enqueue(new Tuple<string, string, bool>(cellName, content, movingToNewCell));
                if (dependencyCalculator.IsBusy == false)
                {
                    cellContentBox.Enabled = false;
                    dependencyCalculator.RunWorkerAsync(saveContentQueue.Dequeue());
                }
            }

        }

        /// <summary>
        /// Overrides the arrow keys operations so that selections move across the screen.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            if (spreadSheetPanel.Focused == true)
            {
                int row, col;
                spreadSheetPanel.GetSelection(out col, out row);
                string cellName = GetNameOfCell(col, row);
                //Condtions when moving with key arrows. 
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
        /// Opens a new empty spreadsheet from the "New" menu button.
        /// </summary>
        private void newSpreadsheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.getAppContext().RunForm(new SpreadsheetForm());
        }

        /// <summary>
        /// Closes the spreadsheet editor when the "Close" button is pressed in the menu.
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
        /// Creates a save dialogue box to save the spreadsheet with the default tag .sprd.
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
            //Opens the Closing warning box if necessary AND gets a response from the user.
            bool saveResult = CloseDialogBox();
            if (saveResult)
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "Spreadsheet|*.sprd|All File|*";
                openFile.Title = "Open your spreadsheet";
                DialogResult result = openFile.ShowDialog();
                if (result == DialogResult.OK)
                    SpreadsheetFormOpenFromFile(openFile.FileName);
            }
        }

        /// <summary>
        /// Checks if the spreadsheet has changes before closing.
        /// If the user wants to cancel the closing process is stopped.
        /// </summary>
        private void SpreadsheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Make sure that the disco function has been turned off before the application closes. 
            TurnOffDisco();

            if (CloseDialogBox() == false)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Creates a help menu to give information to the user on how to select cells in the spreadsheet.
        /// </summary>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult helpMenu = MessageBox.Show("To change the content of the cell, clicks on the panel and writes on the text box. \nTo confirm your new change, either presses enter or click on another cell. \nYou can also move to adjacent cells using the arrow key when a cell is selected", "Help menu.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// When pressing Enter while on the spreadsheet panel, the focus will move to the content box so the user can change the cell contents.
        /// </summary>
        private void spreadSheetPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                cellContentBox.Focus();
            }
        }

        /// <summary>
        /// Calculates and updates the dependencies of the cell, using a background worker to not interrupt users' experiences. 
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
                Invoke(new MethodInvoker(() => spreadSheetPanel.SetValue(coordinates.Item1, coordinates.Item2, ValueToString(spreadsheet.GetCellValue(dependency)))));
            }
        }

        /// <summary>
        /// Enable the cellContentBox after the spreadsheet has finished updating the dependencies. 
        /// </summary>
        private void dependencyCalculator_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /*
             * Keep the cell content box disabled to make sure the content queue isn't 
             * modified while moving to the next cell to save.
             */
            lock (saveContentQueue)
            {
                //If there are other save calls in the queue, finish the queue before enabling the content box. 
                if (saveContentQueue.Count != 0)
                {
                    dependencyCalculator.RunWorkerAsync(saveContentQueue.Dequeue());
                }
                //Other than that, enabled the content box again for the user.
                else
                {
                    cellContentBox.Enabled = true;
                }
            }
        }


        /// <summary>
        /// A dependecies tool that highlights the cell dependents. 
        /// </summary>
        private void dependenciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            spreadSheetPanel.GetSelection(out int col, out int row);
            string cellName = GetNameOfCell(col, row);
            object contents = spreadsheet.GetCellContents(cellName);

            //Gets the list of dependents. 
            IList<string> dependencies = spreadsheet.SetContentsOfCell(cellName, ContentsToString(contents));

            //Creates a random color (always in the same order) every time for the dependencies. 
            Random rnd = new Random(dependencyGroupColors.Count);

            //If we have highlighted other dependencies on the panel, it will choose unique color. 
            Color randomColor;
            do
            {
                randomColor = Color.FromArgb(rnd.Next(165, 255), rnd.Next(165, 255), rnd.Next(165, 255));
            }
            while (!dependencyGroupColors.Add(randomColor));

            //Highlights all depdendents. 
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
            dependencyGroupColors.Clear();
            spreadSheetPanel.ClearHighlights();
            displaySelection(spreadSheetPanel, false);
        }

        /// <summary>
        /// Toggles disco mode. 
        /// </summary>
        private void discoModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //If disco mode is not enabled.
            if (!discoModeEnabled)
            {
                //We enable disco mode.
                cellContentBox.Enabled = false;
                discoModeEnabled = true;

                //Set the inital Colors of cells.
                Random rnd = new Random();
                Dictionary<(int, int), Color> cellColors = colorChange(rnd);

                //Starts a disco thread.
                discoThread = new Thread(discoWorker_DoWork);
                discoThread.Start();
            }
            //If disco mode is already enabled.
            else
            {
                // We re-enabled the content box and turn off the dance floor.
                cellContentBox.Enabled = true;
                TurnOffDisco();
            }
        }


        /// <summary>
        /// Stop the disco. 
        /// </summary>
        private void TurnOffDisco()
        {
            //If disco mode is enabled. 
            if (discoModeEnabled)
            {
                // Then we disable it.
                discoModeEnabled = false;

                //Abort and wait for the thread to finish. 
                discoThread.Abort();
                discoThread.Join();
                //Clear the effects of the disco party.
                spreadSheetPanel.ClearHighlights();
                displaySelection(spreadSheetPanel, false);
            }
        }

        /// <summary>
        /// Prevents highlighting cells after disco mode has been 
        /// closed due to main thread event processing.
        /// (i.e. Some highlights from disco mode may be called after disco mode closes).
        /// </summary>
        /// <param name="col">The col of the cell to highlight.</param>
        /// <param name="row">The row of the cell to highlight.</param>
        /// <param name="color">The color to highlight the cell to.</param>
        private void DiscoHighlight(int col, int row, Color color)
        {
            if (discoModeEnabled)
            {
                spreadSheetPanel.Highlight(col, row, color);
            }
        }

        /// <summary>
        /// Creates a disctionary that conatains cells as keys and a random
        /// color (from a set of colors) as their value to generate colors for different cells.
        /// </summary>
        /// <param name="rnd">Random number generator.</param>
        /// <returns>A dictionary with cells and their colors.</returns>
        private Dictionary<(int, int), Color> colorChange(Random rnd)
        {
            //Color options.
            Color[] discoColors = new Color[] { Color.DarkRed, Color.DarkCyan, Color.DarkOrange, Color.DarkGreen, Color.DarkMagenta };

            Dictionary<(int, int), Color> cellColors = new Dictionary<(int, int), Color>();

            //Loop through the entire spreadsheet. 
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
        /// Animates the colors of the cells in disco mode.
        /// Goes from disco colors to black to new disco colors.
        /// </summary>
        private void discoWorker_DoWork()
        {
            //The current time in this animation loop.
            float time = 0;
            //The time it takes switch from one color to another in seconds.
            float timeToAnimate = 1f;
            //The time between frames (4 frames a second).
            int deltaTime = 250;

            Random rnd = new Random();
            Dictionary<(int, int), Color> cellColors = colorChange(rnd);
            Dictionary<(int, int), Color> nextColors = colorChange(rnd);

            //While disco mode is enabled. 
            while (discoModeEnabled)
            {
                //Interpolate the colors (see the interpolate methods in SpreadsheetPanel).
                foreach (KeyValuePair<(int, int), Color> cellData in cellColors)
                {
                    Color interpolation;
                    if (time < timeToAnimate) //Disco to black.
                        interpolation = SpreadsheetPanel.InterpolateColor(cellData.Value, Color.Black, time / timeToAnimate);
                    else //Black to disco.
                        interpolation = SpreadsheetPanel.InterpolateColor(Color.Black, nextColors[cellData.Key], (time - timeToAnimate) / timeToAnimate);

                    Invoke(new MethodInvoker(() => DiscoHighlight(cellData.Key.Item1, cellData.Key.Item2, interpolation)));
                }

                Invoke(new MethodInvoker(() => displaySelection(spreadSheetPanel, false)));

                //Wait between frames.
                Thread.Sleep(deltaTime);
                //Update the current time (approximately).
                time += ((float)deltaTime) / 1000;
                //If the animation loop has finished, move on the the next loop.
                if (time >= timeToAnimate * 2)
                {
                    time = time % timeToAnimate * 2;
                    cellColors = nextColors;
                    nextColors = colorChange(rnd);
                }
            }
        }

        /// <summary>
        /// A message box to help the user with highlight feature.
        /// </summary>
        private void highlightsDependentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult helpMenu = MessageBox.Show("To show the depedents of a cell, select a cell then go to Features -> Highlight Dependents. This will color the cell and its dependents. \nTo clear all highlights, go to Features -> Clear All Highlights  \n \nNote: If the cell does not have any dependents, the feature will highlight that cell only.", "Highlighting depdendents.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// A message box to help the user with the disco feature.
        /// </summary>
        private void discoModeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult helpMenu = MessageBox.Show("To enable the dance floor, go to Features -> Toggle Disco Mode. \nTo shut down the party, press Toggle Disco Mode again.", "Disco Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
