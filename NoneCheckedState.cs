using Infragistics.Windows.DataPresenter;
using static Infragistics.Windows.Utilities;
using CommonTypes;
using System;
using System.Windows.Controls;

namespace Controllers
{
    public class NoneCheckedState : IGridSelectorBehavior
    {
        private readonly GridSelector _gridSelector;

        // ctor
        public NoneCheckedState(GridSelector gridSelector)
        {
            _gridSelector = gridSelector;
        }

        // parent level checkbox clicked 
        public void ParentLevelHandler(Cell cell)
        {
           // for None Checked State, parent level check, checks all
           TransitionToAllChecked(cell);
        }

        // Child level checkbox clicked
        public void ChildLevelHandler(Cell cell)
        {
            try
            {
                // cast the cell to a checkbox so we can set its values
                var chkBox = GridUtilities.CellToCheckboxConverter(cell, _gridSelector.SelectChildColumnName);

                // for None Checked State, child level check, checks this child
                chkBox.IsChecked = true;

                // activate the Uncomplete button
                // TODO: Need to put an Undo column in grid
                //(cell.Record.ParentRecord as DataRecord).Cells[_gridSelector.UndoColumnName].IsActive = true;

                // first get the cell's parent data record (two steps for readability)
                var parent = cell.Record.ParentDataRecord;
                // when all children of a parent are checked, the parent level checkbox gets checked
                if (GridUtilities.AllThisParentsChildrenChecked(parent.Cells[_gridSelector.SelectParentColumnName], _gridSelector.SelectChildColumnName))
                {
                    var parentChkBox = GridUtilities.CellToCheckboxConverter(parent.Cells[_gridSelector.SelectParentColumnName], _gridSelector.SelectParentColumnName);
                    parentChkBox.IsChecked = true;

                    _gridSelector.SetState(_gridSelector.AllChecked);
                }
                else
                {
                    TransitionToPartiallyChecked(cell);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ChildLevelHandler " + e.Message);
            }
            
        }

        public void TransitionToAllChecked(Cell cell)
        {
            try
            {
                cell.Record.Cells[_gridSelector.SelectParentColumnName].Value = true;

                // activate the Uncomplete button
                // TODO: Need to put an Undo column in grid
                //cell.Record.Cells[_gridSelector.UndoColumnName].IsActive = false;

                var count = cell.Record.ChildRecords[0].ChildRecords.Count;

                // mark each child as selected
                foreach (var consigneeRecord in cell.Record.ChildRecords[0].ChildRecords)
                {
                    var conDataRecord = consigneeRecord as DataRecord;
                    if (conDataRecord == null) continue;

                    var conField = consigneeRecord.FieldLayout.Fields["SelectConsignee"];

                    var cvp = CellValuePresenter.FromRecordAndField(conDataRecord, conField);
                    if (cvp == null) continue;

                    var consigneeCheckbox = GetDescendantFromType(cvp, typeof(CheckBox), true) as CheckBox;
                    if (consigneeCheckbox != null)
                        consigneeCheckbox.IsChecked = true;
                  }

                _gridSelector.SetState(_gridSelector.AllChecked);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        public void TransitionToPartiallyChecked(Cell childCell)
        {
            var parent = childCell.Record.ParentDataRecord;
            var parentCell = parent.Cells[_gridSelector.SelectParentColumnName];
            var parentChkBox = GridUtilities.CellToCheckboxConverter(parentCell, _gridSelector.SelectParentColumnName);
            parentChkBox.IsChecked = null;

            _gridSelector.SetState(_gridSelector.PartiallChecked);
        }
    }
}