using Infragistics.Windows.DataPresenter;
using static Infragistics.Windows.Utilities;
using CommonTypes;
using System;
using System.Windows.Controls;

namespace Controllers
{
    public class PartiallyCheckedState : IGridSelectorBehavior
    {
        private readonly GridSelector _gridSelector;

        public PartiallyCheckedState(GridSelector gridSelector)
        {
            _gridSelector = gridSelector;
        }

        public void ParentLevelHandler(Cell parentLevelCell)
        {
            var parentChkBox = GridUtilities.CellToCheckboxConverter(parentLevelCell, _gridSelector.SelectParentColumnName);
            // if it's NOT the same parent checkbox
            if (parentChkBox.IsChecked != null)
            {
                GridUtilities.UncheckAllParentsAndChildren(_gridSelector.Grid, _gridSelector.SelectParentColumnName, _gridSelector.SelectChildColumnName);
                //GridUtilities.DisableAllUndoButtons(_gridSelector.Grid, _gridSelector.UndoColumnName);
            }

            TransitionToAllChecked(parentLevelCell);
        }

        public void ChildLevelHandler(Cell childLevelCell)
        {
            // declare here for scope
            var parent = childLevelCell.Record.ParentDataRecord;
            var parentCell = parent.Cells[_gridSelector.SelectParentColumnName];
            var parentChkBox = GridUtilities.CellToCheckboxConverter(parentCell, _gridSelector.SelectParentColumnName);

            // if this child is already checked
            var childChkBox = GridUtilities.CellToCheckboxConverter(childLevelCell, _gridSelector.SelectChildColumnName);

            if (childChkBox.IsChecked == false)
            {
                // uncheck it
                //childChkBox.IsChecked = false;

                // see if all these children are NOT checked
                if (GridUtilities.AreThisParentsChildrenNotChecked(parentCell, _gridSelector.SelectChildColumnName))
                {
                    // uncheck the parent
                    parentChkBox.IsChecked = false;

                    // de-activate the Undo button
                    //childLevelCell.Row.ParentRow.Cells[_gridSelector.UndoColumnName].Activation = Activation.Disabled;

                    _gridSelector.SetState(_gridSelector.NoneChecked);
                }
            }
            else // it's not checked, but is it same parent family
            {
                // does this child belong to partially checked parent
                if (parentChkBox.IsChecked == null)
                {
                    //childChkBox.IsChecked = true;

                    // are all the children checked
                    if (GridUtilities.AllThisParentsChildrenChecked(parentCell, _gridSelector.SelectChildColumnName))
                    {
                        parentChkBox.IsChecked = true;

                        _gridSelector.SetState(_gridSelector.AllChecked);
                    }
                }
                else // user has clicked a child from another parent
                {
                    GridUtilities.UncheckAllParentsAndChildren(_gridSelector.Grid, _gridSelector.SelectParentColumnName, _gridSelector.SelectChildColumnName);
                    //GridUtilities.DisableAllUndoButtons(_gridSelector.Grid, _gridSelector.UndoColumnName);

                    // mark the new child
                    childChkBox.IsChecked = true;

                    // enable the button
                    //childLevelCell.Row.ParentRow.Cells[_gridSelector.UndoColumnName].Activation = Activation.ActivateOnly;

                    if (GridUtilities.AllThisParentsChildrenChecked(parentCell, _gridSelector.SelectChildColumnName))
                    {
                        parentChkBox.IsChecked = true;

                        _gridSelector.SetState(_gridSelector.AllChecked);
                    }
                    else // not all children are checked, we stay in partial state
                    {
                        parentChkBox.IsChecked = null;
                    }
                }
            }
        }

        public void TransitionToNoneChecked(Cell parentLevelCell)
        {
            var parentChkBox = GridUtilities.CellToCheckboxConverter(parentLevelCell, _gridSelector.SelectParentColumnName);
            parentChkBox.IsChecked = false;

            // mark each child as unselected
            foreach (var childRow in parentLevelCell.Record.ChildRecords[0].ChildRecords)
            {
                // get the child cell from the parent cell/row
                var childRecord = childRow as DataRecord;
                var childCell = childRecord.Cells[_gridSelector.SelectChildColumnName];

                var childCheckbox = GridUtilities.CellToCheckboxConverter(childCell, _gridSelector.SelectChildColumnName);

                childCheckbox.IsChecked = false;
            }

            // activate the Undo button
            //parentLevelCell.Row.Cells[_gridSelector.UndoColumnName].Activation = Activation.Disabled;

            _gridSelector.SetState(_gridSelector.NoneChecked);
        }

        public void TransitionToAllChecked(Cell parentLevelCell)
        {
            var parentChkBox = GridUtilities.CellToCheckboxConverter(parentLevelCell, _gridSelector.SelectParentColumnName);
            parentChkBox.IsChecked = true;

            // activate the Undo button
            //parentLevelCell.Row.Cells[_gridSelector.UndoColumnName].Activation = Activation.ActivateOnly;

            // mark each child as selected
            foreach (var childRow in parentLevelCell.Record.ChildRecords[0].ChildRecords)
            {
                // get the child cell from the parent cell/row
                var childRecord = childRow as DataRecord;
                var childCell = childRecord.Cells[_gridSelector.SelectChildColumnName];

                var childCheckbox = GridUtilities.CellToCheckboxConverter(childCell, _gridSelector.SelectChildColumnName);

                childCheckbox.IsChecked = true;
            }

            _gridSelector.SetState(_gridSelector.AllChecked);
        }

    }
}