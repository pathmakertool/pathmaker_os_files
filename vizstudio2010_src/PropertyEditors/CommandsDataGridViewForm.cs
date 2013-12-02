using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// use this interface to enable confirmation combo boxes to find their associated commandsDataGridViews
// and vice versa so they can put up a list of available options for confirmation

namespace PathMaker {
    interface CommandsDataGridViewForm {
        DataGridView GetCommandsDataGridView();
        DataGridView GetConfirmationPromptsDataGridView();
    }
}
