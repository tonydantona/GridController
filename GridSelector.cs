using Infragistics.Windows.DataPresenter;
using System;
using System.Collections.Generic;
using CommonTypes;

namespace Controllers
{
    public class GridSelector
    {
        private readonly XamDataGrid _grid;
        private readonly IGridSelectorBehavior _noneCheckedState;
        private readonly IGridSelectorBehavior _allCheckedState;
        private readonly IGridSelectorBehavior _partiallyCheckedState;
        private IGridSelectorBehavior _currState;

        private readonly Dictionary<String, String> _checkboxSelectedHander;

        public GridSelector(XamDataGrid grid, String selectParentColumnName, String selectChildColumnName, String undoColumnName)
        {
            _grid = grid;
            _noneCheckedState = new NoneCheckedState(this);
            _allCheckedState = new AllCheckedState(this);
            _partiallyCheckedState = new PartiallyCheckedState(this);

            SelectParentColumnName = selectParentColumnName;
            SelectChildColumnName = selectChildColumnName;
            UndoColumnName = undoColumnName;

            _checkboxSelectedHander = new Dictionary<String, String>();
            _checkboxSelectedHander[selectParentColumnName] = "ParentLevelHandler";
            _checkboxSelectedHander[selectChildColumnName] = "ChildLevelHandler";

            _currState = _noneCheckedState;
        }

        public void SetState(IGridSelectorBehavior state)
        {
            _currState = state;
        }

        public void UpdateGrid(Cell cell)
        {
            string handler;
            _checkboxSelectedHander.TryGetValue(cell.Field.Name, out handler);

            // notice the _currState in the parameter upon which we are invoking the handler
            if (handler != null)
            {
                typeof(IGridSelectorBehavior).GetMethod(handler).Invoke(_currState, new Object[] { cell });
            }
        }

        public IGridSelectorBehavior NoneChecked
        {
            get { return _noneCheckedState; }
        }

        public IGridSelectorBehavior AllChecked
        {
            get { return _allCheckedState; }
        }

        public IGridSelectorBehavior PartiallChecked
        {
            get { return _partiallyCheckedState; }
        }

        public XamDataGrid Grid
        {
            get { return _grid; }
        }

        public String SelectParentColumnName { get; private set; }
        public String SelectChildColumnName { get; private set; }
        public String UndoColumnName { get; private set; }
    }
}