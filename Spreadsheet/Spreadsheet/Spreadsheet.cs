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
    /// <version>2/19/2021</version>
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
        /// Creates an empty spreadsheet.
        /// </summary>
        public Spreadsheet()
        {
            dependencies = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// Gets the contents of a cell using the cell's name.
        /// By default, an empty string is resturned as the contents of the cell.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        /// <returns>The contents of the cell.</returns>
        public override object GetCellContents(string name)
        {
            CheckName(name);

            if (cells.TryGetValue(name, out Cell cell))
                return cell.Contents;
            else
                return "";
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

            return AddCell(name, formula, formula.GetVariables());
        }

        /// <summary>
        /// Adds a non-empty cell to the spreadsheet.
        /// </summary>
        /// <param name="name">The name of the cell to add.</param>
        /// <param name="cont">The contents of the cell to add.</param>
        /// <param name="newDependees">The cells this cell will depend on.</param>
        /// <returns>All the cells that need to be updated now that this cell has been added.</returns>
        private IList<string> AddCell(string name, object cont, IEnumerable<string> newDependees)
        {
            CheckName(name);

            /* Use changedCell to avoid putting the spreadsheet into an illegal state. 
             * (i.e. We only know whether adding this cell will result in a cycle once
             * we run GetCellsToRecalculate, which will stop this method).
             */
            changedCell = new KeyValuePair<string, HashSet<string>>(name, new HashSet<string>(newDependees));

            IList<string> toRecalculate = new List<string>(GetCellsToRecalculate(name));

            //Now that we know the cell is good to add, we add it to the Spreadsheet.
            dependencies.ReplaceDependees(name, changedCell.Value);
            cells.Remove(name);
            cells[name] = new Cell(cont);

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
            return AddCell(name, cont, new string[0]);
        }

        /// <summary>
        /// Gets the direct depends of the spreadsheet, considering any nodes being added.
        /// </summary>
        /// <param name="name">The name of the node to get dependents of.</param>
        /// <returns>The direct dependents of the node.</returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            List<string> dependents = new List<string>(dependencies.GetDependents(name));

            if (changedCell.Value.Contains(name))
                dependents.Add(changedCell.Key);

            return dependents;
        }

        /// <summary>
        /// Checks if a cell name is valid.
        /// </summary>
        /// <param name="name">The name of the cell to check.</param>
        /// <returns>Whether the name is valid.</returns>
        private static bool IsValidName(string name)
        {
            return (!ReferenceEquals(name, null)) && Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z_\d]*$");
        }

        /// <summary>
        /// Throws an InvalidNameException if the name of a cell is invalid.
        /// </summary>
        /// <param name="name">The name of the cell.</param>
        private static void CheckName(string name)
        {
            if (!IsValidName(name))
                throw new InvalidNameException();
        }
    }
}
