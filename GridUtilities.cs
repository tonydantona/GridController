using Infragistics.Windows.DataPresenter;
using static Infragistics.Windows.Utilities;
using System;
using System.Windows.Controls;

namespace Controllers
{
    public class GridUtilities
    {
        public static CheckBox CellToCheckboxConverter(Cell cell, string colName)
        {
            try
            {
                var cellDataRecord = cell.Record as DataRecord;
                var field = cell.Record.FieldLayout.Fields[colName];

                var cvp = CellValuePresenter.FromRecordAndField(cellDataRecord, field);

                var checkBox = GetDescendantFromType(cvp, typeof(CheckBox), true) as CheckBox;
                return checkBox;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            

        }

        public static bool AllThisParentsChildrenChecked(Cell cell, String selectChildColumnName)
        {
            // check each child cell to see if it's checked
            foreach (var childRow in cell.Record.ChildRecords[0].ChildRecords)
            {
                // get the child cell from the parent cell/row
                var childRecord = childRow as DataRecord;
                if (childRecord != null)
                {
                    var childCell = childRecord.Cells[selectChildColumnName];

                    var consigneeCheckbox = CellToCheckboxConverter(childCell, selectChildColumnName);

                    if (consigneeCheckbox.IsChecked == false)
                        return false;
                }
            }

            return true;
        }

        public static bool AreThisParentsChildrenNotChecked(Cell parentCell, String selectChildColumnName)
        {
            // check each child cell to see if it's checked
            try
            {
                foreach (var childRow in parentCell.Record.ChildRecords[0].ChildRecords)
                {
                    // get the child cell from the parent cell/row
                    var childRecord = childRow as DataRecord;
                    if (childRecord != null)
                    {
                        var childCell = childRecord.Cells[selectChildColumnName];

                        var consigneeCheckbox = CellToCheckboxConverter(childCell, selectChildColumnName);

                        if (consigneeCheckbox.IsChecked == true)
                            return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public static void UncheckAllParentsAndChildren(XamDataGrid grid, String selectParentColumnName, String selectChildColumnName)
        {
             //This will loop through all the rows of the grid, including rows 
            // from child bands.
            foreach (DataRecord row in grid.Records)
            {
                var parentCell = row.Cells[selectParentColumnName];
                var parentChkBox = CellToCheckboxConverter(parentCell, selectParentColumnName);
                parentChkBox.IsChecked = false;

                // another way to do this
                //var parentRecord = row as DataRecord;
                //var parentCell = parentRecord.Cells[selectParentColumnName];

                foreach (var childRow in parentCell.Record.ChildRecords[0].ChildRecords)
                {
                    var childRecord = childRow as DataRecord;
                    if (childRecord != null)
                    {
                        var childCell = childRecord.Cells[selectChildColumnName];
                        var childChkBox = CellToCheckboxConverter(childCell, selectChildColumnName);
                        childChkBox.IsChecked = false;
                    }
                }
            }
        }

        public static void DisableAllUndoButtons(XamDataGrid grid, String undoColumnName)
        {
            foreach (DataRecord row in grid.Records)
            {
                row.Cells[undoColumnName].IsActive.Equals(false);
            }
        }
    }
}