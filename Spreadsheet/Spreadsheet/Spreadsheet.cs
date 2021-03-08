using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SS
{
    /// <summary>
    /// A spreadsheet that keeps track of the contents and relations of cells.
    /// </summary>
    /// <author>Wiliam Erignac</author>
    /// <version>2/26/2021</version>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// The relationships between the spreadsheet cells.
        /// </summary>
        private DependencyGraph dependencies;

        /// <summary>
        /// A map of cell names to cells.
        /// </summary>
        private Dictionary<string, Cell> cells;

        /// <summary>
        /// Holds the name of a cell being added to the spreadsheet and the values the cell depends on.
        /// Used in the SetCellContents() method to prevent altering the Spreadsheet when an error
        /// occurs.
        /// </summary>
        private KeyValuePair<string, HashSet<string>> changedCell;

        /// <summary>
        /// Whether this spreadsheet has been changed since construction or saving.
        /// </summary>
        private bool _changed = false;

        /// <summary>
        /// A wrapper property for the changed parameter.
        /// </summary>
        public override bool Changed { get => _changed; protected set => _changed = value; }

        /// <summary>
        /// An object that saves the spreadsheet as a file.
        /// </summary>
        private SpreadsheetFileManager manager = new SpreadsheetXMLManager();

        /// <summary>
        /// Creates an empty spreadsheet.
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            dependencies = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Creates an empty spreadsheet with the specified validator, normalizer, and version.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            dependencies = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Creates a spreadsheet with the cells designated by a file (that also
        /// takes a validator, normalizer, and version).
        /// </summary>
        /// <param name="filepath">The file to read.</param>
        public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            dependencies = new DependencyGraph();
            cells = new Dictionary<string, Cell>();

            manager.Load(this, filepath);

            Changed = false;
        }

        /// <summary>
        /// Gets the contents of a cell using the cell's name.
        /// By default, an empty string is returned as the contents of the cell.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <returns>The contents of the cell.</returns>
        public override object GetCellContents(string name)
        {
            if (TryGetCell(name, out Cell cell))
                return cell.Contents;
            else
                return "";
        }

        /// <summary>
        /// Gets the value of a cell using the cell's name.
        /// By default, an empty string is returned as the contents of the cell.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <returns>The value of the cell.</returns>
        public override object GetCellValue(string name)
        {
            if (TryGetCell(name, out Cell cell))
                return cell.Value;
            else
                return "";
        }

        /// <summary>
        /// Gets a cell in the spraedsheet.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <param name="cell">The cell.</param>
        /// <returns>Whether that cell has contents.</returns>
        private bool TryGetCell(string name, out Cell cell)
        {
            cell = null;
            name = CheckName(name);

            if (cells.TryGetValue(name, out Cell c))
            {
                cell = c;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retreives the names of all cells in the spreadsheet that have been
        /// assigned a value.
        /// </summary>
        /// <returns>The names of all the cells that have been assigned values.</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        /// <summary>
        /// Sets the contents of the cell with the given name to a double.
        /// </summary>
        /// <param name="name">The name of the cell to set the contents of.</param>
        /// <param name="number">The double to set the cell's contents to.</param>
        /// <returns>All the cells that need to be updated now that this cell has been modified.</returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            return AddCell(name, number);
        }

        /// <summary>
        /// Sets the contents of the cell with the given name to a string.
        /// </summary>
        /// <param name="name">The name of the cell to set the contents of.</param>
        /// <param name="text">The string to set the cell's contents to.</param>
        /// <returns>All the cells that need to be updated now that this cell has been modified.</returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            IList<string> toRecalculate = AddCell(name, text);

            /* If the cell contents is the same as if it wasn't added to the spreadsheet
             * why keep it?
             * Answer: we don't.
             */
            if (text == "")
                cells.Remove(name);

            return toRecalculate;
        }

        /// <summary>
        /// Sets the contents of the cell with the given name to a formula.
        /// </summary>
        /// <param name="name">The name of the cell to set the contents of.</param>
        /// <param name="formula">The formula to set the cell's contents to.</param>
        /// <returns>All the cells that need to be updated now that this cell has been modified.</returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            if (formula == null)
                throw new ArgumentNullException("Cannot use a null formula as a value for a cell.");

            return AddCell(name, formula, formula.GetVariables(), LookupCellValue);
        }

        /// <summary>
        /// A method that serves as the lookup for formulas in cells.
        /// It retrieves the double value of a cell, and throws an ArgumentException
        /// if the value of the cell is not a double.
        /// </summary>
        /// <param name="name">The name of the cell to lookup.</param>
        /// <returns>The double value of the cell.</returns>
        private double LookupCellValue(string name)
        {
            object value = GetCellValue(name);
            if (value is double d)
                return d;
            else
                throw new ArgumentException("Cell " + name + " needs to provide a double value but has the value " + value + " of type " + value.GetType() +".");
        }

        /// <summary>
        /// Adds a non-empty cell to the spreadsheet.
        /// </summary>
        /// <param name="name">The name of the cell to add.</param>
        /// <param name="cont">The contents of the cell to add.</param>
        /// <param name="newDependees">The cells this cell will depend on.</param>
        /// <returns>All the cells that need to be updated now that this cell has been added.</returns>
        private IList<string> AddCell(string name, object cont, IEnumerable<string> newDependees, Func<string, double> lookup)
        {
            /* Use changedCell to avoid putting the spreadsheet into an illegal state. 
             * (i.e. We only know whether adding this cell will result in a cycle once
             * we run GetCellsToRecalculate, which will stop this method).
             */
            changedCell = new KeyValuePair<string, HashSet<string>>(name, new HashSet<string>(newDependees));

            IList<string> toRecalculate = new List<string>(GetCellsToRecalculate(name));

            //Now that we know the cell is good to add, we add it to the Spreadsheet.
            dependencies.ReplaceDependees(name, changedCell.Value);
            cells.Remove(name);
            Cell newCell;
            if (!ReferenceEquals(lookup, null))
                newCell = new Cell(cont, lookup);
            else
                newCell = new Cell(cont);

            cells[name] = newCell;

            if (!Changed)
                Changed = true;

            return toRecalculate;
        }

        /// <summary>
        /// Adds a cell that depends on no other cells.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <param name="cont">The contents of the cell.</param>
        /// <returns>All the cells that need to be updated now that this cell has been added.</returns>
        private IList<string> AddCell(string name, object cont)
        {
            return AddCell(name, cont, Array.Empty<string>(), null);
        }

        /// <summary>
        /// Gets the direct depends of the spreadsheet, considering any nodes being added.
        /// </summary>
        /// <param name="name">The name of the node to get dependents of.</param>
        /// <returns>The direct dependents of the node.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            /* Using HashSet to avoid getting two instances of the same
             * cell name after the if statement.
             */

            HashSet<string> dependents = new HashSet<string>(dependencies.GetDependents(name));

            if (changedCell.Value.Contains(name))
                dependents.Add(changedCell.Key);

            return dependents;
        }

        /// <summary>
        /// Checks if a cell name is valid by the global definition that all names must follow.
        /// (i.e. the name must consist a letter or underscore followed by letters, underscored,
        /// and / or digits)
        /// </summary>
        /// <param name="name">The name of the cell to check.</param>
        /// <returns>Whether the name is valid.</returns>
        private static bool IsGlobalValidName(string name)
        {
            return (!ReferenceEquals(name, null)) && Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z_\d]*$");
        }

        /// <summary>
        /// Throws an InvalidNameException if the name of a cell is invalid
        /// either by the global definition or the user-specified one.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <returns>The normalized name.</returns>
        private string CheckName(string name)
        {
            if (!IsGlobalValidName(name))
                throw new InvalidNameException();

            name = Normalize(name);
            
            if (!IsValid(name))
                throw new InvalidNameException();
            
            return name;
        }

        /// <summary>
        /// Gets the version of a saved spreadsheet.
        /// </summary>
        /// <param name="filename">The name of the file to read.</param>
        /// <returns>The version of the spreadsheet.</returns>
        public override string GetSavedVersion(string filename)
        {
            return manager.GetVersion(filename);
        }

        /// <summary>
        /// Saves the spreadsheet as a file.
        /// </summary>
        /// <param name="filename">Where to save the spreadsheet.</param>
        public override void Save(string filename)
        {
            manager.Save(this, filename);
            Changed = false;
        }

        /// <summary>
        /// Sets the contents of a cell based on a string inputted by a user.
        /// If the string is a double, the cell contains a double.
        /// If the string starts with an =, the cell contains a formula.
        /// Otherwise the cell contains the literal string.
        /// </summary>
        /// <param name="name">The name of the cell to set the contents of.</param>
        /// <param name="content">The string representing the contents of the cell.</param>
        /// <returns>A list of the cells that have new values from changing the specified cell.</returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            //We can't have a cell with null contents!
            if (ReferenceEquals(content, null))
                throw new ArgumentNullException("Content of a cell cannot be null");

            //Making sure the name of the cell is valid.
            name = CheckName(name);

            IList<string> cellsToUpdate;

            //Determining the contents type of the cell.
            if (Double.TryParse(content, out double d))
                cellsToUpdate = SetCellContents(name, d);
            else if (content.Length > 0 && content[0].Equals('='))
                cellsToUpdate = SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            else
                cellsToUpdate = SetCellContents(name, content);

            //Updating the values of dependent cells.
            foreach(string relatedCellName in cellsToUpdate)
                if (TryGetCell(relatedCellName, out Cell relatedCell))
                    relatedCell.UpdateValue();

            return cellsToUpdate;
        }
    }
}
