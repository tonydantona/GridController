using CommonTypes;
using Infragistics.Windows.DataPresenter;

namespace Controllers
{
    public class AllCheckedState : IGridSelectorBehavior
    {
        private readonly GridSelector _gridSelector;

        // ctor
        public AllCheckedState(GridSelector gridSelector)
        {
            _gridSelector = gridSelector;
        }

        // parent level checkbox clicked 
        public void ParentLevelHandler(Cell parentCell)
        {
            var parentChkBox = GridUtilities.CellToCheckboxConverter(parentCell, _gridSelector.SelectParentColumnName);

            // if its the same parent checkbox (all children must be checked) then...
            bool allChildrenChecked = GridUtilities.AllThisParentsChildrenChecked(parentCell, _gridSelector.SelectChildColumnName);
            if (allChildrenChecked)
            {
                TransitionToNoneChecked(parentCell);
                return;
            }

            // we've switched parents.  uncheck all parents and children
            GridUtilities.UncheckAllParentsAndChildren(_gridSelector.Grid, _gridSelector.SelectParentColumnName, _gridSelector.SelectChildColumnName);

            // disable all Undo buttons
            //GridUtilities.DisableAllUndoButtons(_gridSelector.Grid, _gridSelector.UndoColumnName);

            parentChkBox.IsChecked = true;

            // activate the Undo button
            //parentCell.Row.Cells[_gridSelector.UndoColumnName].Activation = Activation.ActivateOnly;

            // mark each child as selected
            foreach (var childRow in parentCell.Record.ChildRecords[0].ChildRecords)
            {
                var childRecord = childRow as DataRecord;
                if (childRecord != null)
                {
                    var childCell = childRecord.Cells[_gridSelector.SelectChildColumnName];
                    var childChkBox = GridUtilities.CellToCheckboxConverter(childCell, _gridSelector.SelectChildColumnName);
                    childChkBox.IsChecked = true;
                }
            }

            // remain in AllChecked state
        }

        // child level checkbox clicked
        public void ChildLevelHandler(Cell childLevelCell)
        {
            // get the child cell's chkbox
            var childChkBox = GridUtilities.CellToCheckboxConverter(childLevelCell, _gridSelector.SelectChildColumnName);

            // get the parent cell
            var parent = childLevelCell.Record.ParentDataRecord;
            var parentCell = parent.Cells[_gridSelector.SelectParentColumnName];
            var parentChkBox = GridUtilities.CellToCheckboxConverter(parentCell, _gridSelector.SelectParentColumnName);

            // if this child is part of the all checked parents's children
            if (parentChkBox.IsChecked == true)
            {
                // we're in all checked state so uncheck child
                childChkBox.IsChecked = false;

                // if all children are unchecked, uncheck parent
                if (GridUtilities.AreThisParentsChildrenNotChecked(parentCell, _gridSelector.SelectChildColumnName))
                {
                    parentChkBox.IsChecked = false;

                    // activate the Undo button
                    //childLevelCell.Row.ParentRow.Cells[_gridSelector.UndoColumnName].Activation = Activation.Disabled;

                    _gridSelector.SetState(_gridSelector.NoneChecked);
                }
                else
                {
                    TransitionToPartiallyChecked(childLevelCell);
                }
            }
            else  // we've switched parents.
            {
                // uncheck all parents and children
                GridUtilities.UncheckAllParentsAndChildren(_gridSelector.Grid, _gridSelector.SelectParentColumnName, _gridSelector.SelectChildColumnName);
                //GridUtilities.DisableAllUndoButtons(_gridSelector.Grid, _gridSelector.UndoColumnName);

                // mark the new child
                childChkBox.IsChecked = true;

                // enable the Undo button
                //childLevelCell.Row.ParentRow.Cells[_gridSelector.UndoColumnName].Activation = Activation.ActivateOnly;

                if (GridUtilities.AllThisParentsChildrenChecked(parentCell, _gridSelector.SelectChildColumnName))
                {
                    parentChkBox.IsChecked = true;

                    // remain in AllChecked state
                }
                else // not all children are checked, go to partial state
                {
                    TransitionToPartiallyChecked(childLevelCell);
                }
            }
        }

        public void TransitionToNoneChecked(Cell parentCell)
        {
            // get the chkbox for this cell
            var parentChkbox = GridUtilities.CellToCheckboxConverter(parentCell, _gridSelector.SelectParentColumnName);
            parentChkbox.IsChecked = false;

            // mark each child as unselected
            foreach (var childRow in parentCell.Record.ChildRecords[0].ChildRecords)
            {
                var childRecord = childRow as DataRecord;
                if (childRecord != null)
                {
                    var childCell = childRecord.Cells[_gridSelector.SelectChildColumnName];
                    var childChkBox = GridUtilities.CellToCheckboxConverter(childCell, _gridSelector.SelectChildColumnName);
                    childChkBox.IsChecked = false;
                }
            }

            // activate the Undo button
            //cell.Row.Cells[_gridSelector.UndoColumnName].Activation = Activation.Disabled;

            _gridSelector.SetState(_gridSelector.NoneChecked);
        }

        public void TransitionToPartiallyChecked(Cell childLevelCell)
        {
            // activate the Undo button
            //childLevelCell.Row.ParentRow.Cells[_gridSelector.UndoColumnName].Activation = Activation.ActivateOnly;

            // get the parent cell
            var parent = childLevelCell.Record.ParentDataRecord;
            var parentCell = parent.Cells[_gridSelector.SelectParentColumnName];
            var parentChkBox = GridUtilities.CellToCheckboxConverter(parentCell, _gridSelector.SelectParentColumnName);
            parentChkBox.IsChecked = null;

            _gridSelector.SetState(_gridSelector.PartiallChecked);
        }
    }
}