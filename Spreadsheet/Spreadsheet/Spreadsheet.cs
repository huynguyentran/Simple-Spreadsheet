using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {

        private DependencyGraph dependencies;

        private Dictionary<string, Cell> cells;

        private KeyValuePair<string, HashSet<string>> changedCell;

        public Spreadsheet()
        {
            dependencies = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        public override object GetCellContents(string name)
        {
            CheckName(name);

            if (cells.TryGetValue(name, out Cell cell))
                return cell.Contents;
            else
                return "";
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        public override IList<string> SetCellContents(string name, double number)
        {
            return AddCell(name, number);
        }

        public override IList<string> SetCellContents(string name, string text)
        {
            return AddCell(name, text);
        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {
            if (formula == null)
                throw new ArgumentNullException("Cannot use a null formula as a value for a cell.");

            return AddCell(name, formula, formula.GetVariables());
        }

        private IList<string> AddCell(string name, object cont, IEnumerable<string> newDependees)
        {
            CheckName(name);

            changedCell = new KeyValuePair<string, HashSet<string>>(name, new HashSet<string>(newDependees));

            IList<string> toRecalculate = new List<string>(GetCellsToRecalculate(name));

            dependencies.ReplaceDependees(name, changedCell.Value);
            cells.Remove(name);
            cells[name] = new Cell(cont);

            return toRecalculate;
        }

        private IList<string> AddCell(string name, object cont)
        {
            return AddCell(name, cont, new string[0]);
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            List<string> dependents = new List<string>(dependencies.GetDependents(name));

            if (changedCell.Value.Contains(name))
                dependents.Add(changedCell.Key);

            return dependents;
        }

        private static bool IsValidName(string name)
        {
            return (!ReferenceEquals(name, null)) && Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z_\d]*$");
        }

        private static void CheckName(string name)
        {
            if (!IsValidName(name))
                throw new InvalidNameException();
        }
    }
}
