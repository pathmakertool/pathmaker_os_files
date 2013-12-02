using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Media;
using System.Drawing;

namespace PathMaker {
    public partial class CommonForm {
        private static int IndentOutdentColumnFillWeight = 1;
        private static int IndentOutdentColumnMinimumWidth = 20;
        private static ErrorProvider StateIdErrorProvider = null;

        // typeNames is a seed, but this will track all type names in use for this session of PathMaker
        private static List<string> activeTypeNames = null;

        private static string[] typeNames = 
        {
            "Initial",
            "Timeout 1",
            "Timeout 2",
            "Retry 1",
            "Retry 2",
            "Disconfirm 1",
            "Disconfirm 2",
            "Retry / Timeout",
            "Retry 1 / Timeout 1",
            "Retry 1 / Disconfirm 1",
            "Retry / Timeout / Disconfirm",
            "Retry 1 / Timeout 1 / Disconfirm 1",
            "Retry 2 / Timeout 2",
            "Retry 2 / Disconfirm 2",
            "Retry 2 / Timeout 2 / Disconfirm 2",
            "Initial / Retry / Timeout",

            "Help",
            "",
        };

        private static string[] confirmValues = 
        {
            Strings.ConfirmIfNecessary,
            Strings.ConfirmAlways,
            Strings.ConfirmNever,
        };

        private static string[] modeValues = 
        {
            Strings.ModeSpeech,
            Strings.ModeDTMF,
        };

        private static string[] promptIdFormatValues = 
        {
            Strings.PromptIdFormatPartial,
            Strings.PromptIdFormatFull,
            Strings.PromptIdFormatNumeric,
            Strings.PromptIdFormatDisabled,
        };

        private static string[] ynValues = 
        {
            Strings.ValueYes,
            Strings.ValueNo,
        };

        private static string[] highlightValues = 
        {
            Strings.HighlightColorNone,
            Strings.HighlightColorYellow,
            Strings.HighlightColorGreen,
            Strings.HighlightColorPink,
            Strings.HighlightColorAqua,
            Strings.HighlightColorBlue,
        };

        private static string[] sortOrderValues = 
        {
            Strings.StateSortOrderAlphaNumerical,
            Strings.StateSortOrderNumericalOnly,
            Strings.StateSortOrderVisioHeuristic,
        };

        private class ComboBoxItem {
            public ComboBoxItem(string display, string value) { Display = display; Value = value; }
            public ComboBoxItem() { }
            public string Display { get; set; }
            public string Value { get; set; }
            // these must match the fields above
            public const string DisplayMemberName = "Display";
            public const string ValueMemberName = "Value";
        }

        public static void ApplyCommonDataGridViewSettings<T>(DataGridView gridView, bool allowUserToAddAndDelete) where T : new() {
            if (StateIdErrorProvider != null)
                StateIdErrorProvider.Clear();

            if (allowUserToAddAndDelete) {
                gridView.AllowUserToAddRows = true;
                gridView.AllowUserToDeleteRows = true;
            }
            else {
                gridView.AllowUserToAddRows = false;
                gridView.AllowUserToDeleteRows = false;
            }

            // if there are indent/outdent columns, set them up here
            DataGridViewColumn indentColumn = gridView.Columns[Strings.IndentColumnName];
            DataGridViewColumn outdentColumn = gridView.Columns[Strings.OutdentColumnName];
            if (indentColumn != null) {
                indentColumn.FillWeight = IndentOutdentColumnFillWeight;
                indentColumn.MinimumWidth = IndentOutdentColumnMinimumWidth;
                indentColumn.Resizable = DataGridViewTriState.False;
                gridView.CellContentClick -= new DataGridViewCellEventHandler(OnCellContentClick);
                gridView.CellContentClick += new DataGridViewCellEventHandler(OnCellContentClick);
            }
            if (outdentColumn != null) {
                outdentColumn.FillWeight = IndentOutdentColumnFillWeight;
                outdentColumn.MinimumWidth = IndentOutdentColumnMinimumWidth;
                outdentColumn.Resizable = DataGridViewTriState.False;
                if (indentColumn == null) {
                    // shouldn't happen but we'll be prepared for anything
                    gridView.CellContentClick -= new DataGridViewCellEventHandler(OnCellContentClick);
                    gridView.CellContentClick += new DataGridViewCellEventHandler(OnCellContentClick);
                }
            }

            gridView.CellValueChanged -= new DataGridViewCellEventHandler(OnCellValueChangedForUpdateDateColumns);
            gridView.CellValueChanged += new DataGridViewCellEventHandler(OnCellValueChangedForUpdateDateColumns);

            gridView.CellFormatting -= new DataGridViewCellFormattingEventHandler(OnCellFormattingForHighlighting);
            gridView.CellFormatting += new DataGridViewCellFormattingEventHandler(OnCellFormattingForHighlighting);

            gridView.EditingControlShowing -= new DataGridViewEditingControlShowingEventHandler(OnEditingControlShowingForEditorHotKey);
            gridView.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(OnEditingControlShowingForEditorHotKey);

            gridView.AllowUserToOrderColumns = false;
            gridView.AllowUserToResizeColumns = true;
            gridView.AllowUserToResizeRows = false;
            gridView.MultiSelect = false;
            gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            gridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            gridView.EditMode = DataGridViewEditMode.EditOnKeystroke;
            gridView.MultiSelect = false;
            gridView.BackgroundColor = SystemColors.Window;
            gridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            gridView.DataError -= new DataGridViewDataErrorEventHandler(OnGridViewDataError);
            gridView.DataError += new DataGridViewDataErrorEventHandler(OnGridViewDataError);

            //Context Menu pop up for short cuts
            if (typeof(MaxHandlingRow) != typeof(T)) {
                gridView.ContextMenuStrip = new ContextMenuStrip();
                ToolStripMenuItem menuItem = new ToolStripMenuItem(Strings.MoveRowUpText, null, new EventHandler(PopUp_Clicked<T>));
                menuItem.ShortcutKeyDisplayString = Keys.PageUp.ToString();
                menuItem.Tag = Keys.PageUp;
                gridView.ContextMenuStrip.Items.Add(menuItem);

                menuItem = new ToolStripMenuItem(Strings.MoveRowDownText, null, new EventHandler(PopUp_Clicked<T>));
                // for some reason the string rep of Keys.PageDown is "Next"
                menuItem.ShortcutKeyDisplayString = "PageDown";
                menuItem.Tag = Keys.PageDown;
                gridView.ContextMenuStrip.Items.Add(menuItem);

                menuItem = new ToolStripMenuItem(Strings.TextEditorText, null, new EventHandler(PopUp_Clicked<T>));
                menuItem.ShortcutKeyDisplayString = Keys.F2.ToString();
                menuItem.Tag = Keys.F2;
                gridView.ContextMenuStrip.Items.Add(menuItem);

                if (gridView.AllowUserToAddRows) {
                    menuItem = new ToolStripMenuItem(Strings.InsertRowText, null, new EventHandler(PopUp_Clicked<T>));
                    menuItem.ShortcutKeyDisplayString = Keys.Insert.ToString();
                    menuItem.Tag = Keys.Insert;
                    gridView.ContextMenuStrip.Items.Add(menuItem);

                    menuItem = new ToolStripMenuItem(Strings.DuplicateRowText, null, new EventHandler(PopUp_Clicked<T>));
                    menuItem.ShortcutKeyDisplayString = "Ctrl+D";
                    menuItem.Tag = Keys.ControlKey | Keys.D;
                    gridView.ContextMenuStrip.Items.Add(menuItem);
                }

                if (gridView.AllowUserToAddRows || typeof(TransitionRow) == typeof(T) || typeof(CommandTransitionRow) == typeof(T)) {
                    menuItem = new ToolStripMenuItem(Strings.DeleteRowText, null, new EventHandler(PopUp_Clicked<T>));
                    menuItem.Tag = Keys.Delete;
                    menuItem.ShortcutKeyDisplayString = Keys.Delete.ToString();
                    gridView.ContextMenuStrip.Items.Add(menuItem);
                }

                // key handler which matches popup menu - the keys we're using are not allowed
                // as context menu shortcuts so we need to grab them ourselves
                gridView.KeyDown -= new KeyEventHandler(OnGridViewKeyDown<T>);
                gridView.KeyDown += new KeyEventHandler(OnGridViewKeyDown<T>);
            }
        }

        private static void OnGridViewDataError(object sender, DataGridViewDataErrorEventArgs e) {
            System.Diagnostics.Debug.WriteLine("GridViewDataError...");
        }

        private static void AddTextBoxColumn(DataGridView gridView, string columnName) {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = columnName;
            column.HeaderText = columnName;
            column.Name = columnName;
            column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            gridView.Columns.Add(column);
        }

        private static void AddButtonColumn(DataGridView gridView, string columnName, string buttonLabel) {
            DataGridViewButtonColumn column = new DataGridViewButtonColumn();
            column.Name = columnName;
            column.HeaderText = columnName;
            column.Text = buttonLabel;

            column.UseColumnTextForButtonValue = true;
            gridView.Columns.Add(column);
        }

        private static void AddEditableStringComboBoxColumn(DataGridView gridView, string columnName) {
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            column.DataPropertyName = columnName;
            column.HeaderText = columnName;
            column.Name = columnName;

            gridView.EditingControlShowing -= HandleComboBoxEditingControlShowing;
            gridView.EditingControlShowing += HandleComboBoxEditingControlShowing;

            gridView.Columns.Add(column);
        }

        private static void AddStringComboBoxColumn(DataGridView gridView, string columnName) {
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            column.DataPropertyName = columnName;
            column.HeaderText = columnName;
            column.Name = columnName;

            gridView.Columns.Add(column);
        }

        private static void AddItemComboBoxColumn(DataGridView gridView, string columnName) {
            DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
            column.DataPropertyName = columnName;
            column.DisplayMember = ComboBoxItem.DisplayMemberName;
            column.ValueMember = ComboBoxItem.ValueMemberName;
            column.HeaderText = columnName;
            column.Name = columnName;

            gridView.Columns.Add(column);
        }

        private static void LoadItemComboBoxColumn(DataGridView gridView, string columnName, List<ComboBoxItem> values) {
            DataGridViewComboBoxColumn column = gridView.Columns[columnName] as DataGridViewComboBoxColumn;
            column.DataSource = values;
        }

        private static void LoadComboBoxColumn(DataGridView gridView, string columnName, string[] values) {
            DataGridViewComboBoxColumn column = gridView.Columns[columnName] as DataGridViewComboBoxColumn;
            column.Items.Clear();
            foreach (string v in values)
                column.Items.Add(v);
        }

        private static void HandleComboBoxEditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e) {
            ComboBox combo = e.Control as ComboBox;
            if (combo == null) {
                return;
            }
            combo.DropDownStyle = ComboBoxStyle.DropDown;
        }

        static void OnPromptTypeCellValidatingForDropDownCombo(object sender, DataGridViewCellValidatingEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            if (e.ColumnIndex == gridView.Columns["Type"].Index) {
                DataGridViewComboBoxColumn comboColumn = gridView.Columns[e.ColumnIndex] as DataGridViewComboBoxColumn;

                if (e.FormattedValue.ToString().Length > 0 && e.FormattedValue.ToString().Trim().Length == 0) {
                    Common.ErrorMessage("Type name contains only whitespace characters");
                    e.Cancel = true;
                    return;
                }

                // Add the value to column's Items to pass validation
                if (!comboColumn.Items.Contains(e.FormattedValue.ToString())) {
                    comboColumn.Items.Add(e.FormattedValue);
                    if (!activeTypeNames.Contains(e.FormattedValue.ToString()))
                        activeTypeNames.Add(e.FormattedValue.ToString());
                    gridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.FormattedValue;
                    gridView.NotifyCurrentCellDirty(true);
                }
            }
        }

        private static void HideDateStampColumns(DataGridView gridView) {
            for (int c = 0; c < gridView.Columns.Count; c++)
                if (gridView.Columns[c].Name.Contains(Strings.DateStampColumnSuffix))
                    gridView.Columns[c].Visible = false;
        }

        private static List<ComboBoxItem> GetAllPossibleGotos() {
            List<ComboBoxItem> possibles = new List<ComboBoxItem>();
            //change #288 Sort list before display
            List<ComboBoxItem> possibleSorted = new List<ComboBoxItem>();

            possibles.Add(new ComboBoxItem(string.Empty, string.Empty));
            possibles.Add(new ComboBoxItem(Strings.CurrentStateKeyword, Strings.CurrentStateKeyword));

            foreach (Shadow shadow in PathMaker.LookupAllShadows()) {
                switch (shadow.GetShapeType()) {
                    case ShapeTypes.Interaction:
                    case ShapeTypes.Play:
                    case ShapeTypes.Data:
                    case ShapeTypes.Decision:
                        possibleSorted.Add(new ComboBoxItem(shadow.GetGotoName(), shadow.GetUniqueId()));
                        break;
                    case ShapeTypes.Transfer:
                    case ShapeTypes.HangUp:
                    case ShapeTypes.Return:
                        // we'll just add defaults for these
                        break;
                }
            }

            //Sort by name
            possibleSorted.Sort((lName1, lName2) => string.Compare(lName1.Display, lName2.Display));
            //Add to original list
            foreach (ComboBoxItem item in possibleSorted) {
                possibles.Add(new ComboBoxItem(item.Display, item.Value));
            }

            possibles.Add(new ComboBoxItem(Strings.ReturnKeyword, Strings.ReturnKeyword));
            possibles.Add(new ComboBoxItem(Strings.TransferKeyword, Strings.TransferKeyword));
            possibles.Add(new ComboBoxItem(Strings.HangUpKeyword, Strings.HangUpKeyword));
            possibles.Add(new ComboBoxItem(Strings.PlaceHolderKeyword, Strings.PlaceHolderKeyword));
            return possibles;
        }

        private static string[] GetPromptTypeComboValues(Table table, int itemColumn) {
            List<string> list = new List<string>();

            if (activeTypeNames == null) {
                activeTypeNames = new List<string>();
                foreach (string s in typeNames)
                    activeTypeNames.Add(s);
            }

            foreach (string s in activeTypeNames)
                list.Add(s);

            // add ones from the table which aren't already in the list
            for (int r = 0; r < table.GetNumRows(); r++) {
                string tableItem = table.GetData(r, itemColumn);
                if (!list.Contains(tableItem))
                    list.Add(tableItem);
                if (!activeTypeNames.Contains(tableItem))
                    activeTypeNames.Add(tableItem);
            }

            return list.ToArray();
        }

        private static void OnCommandTransitionDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e) {
            e.Row.Cells[CommandTransitionRow.ConfirmColumnName].Value = confirmValues[0];
        }

        static void OnPromptTypeDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e) {
            if (e.Row.Index > 0)
                e.Row.Cells[PromptTypeRow.TypeColumnName].Value = "";
            else
                e.Row.Cells[PromptTypeRow.TypeColumnName].Value = typeNames[0];
        }

        private static void OnChangeLogDefaultValuesNeeded(object sender, DataGridViewRowEventArgs e) {
            e.Row.Cells[ChangeLogRow.DateColumnName].Value = DateTime.Today.ToString(Strings.DateColumnFormatString);
            e.Row.Cells[ChangeLogRow.HighlightColumnName].Value = Strings.HighlightColorNone;
        }

        private static string[] GetConfirmationPromptComboValues(Table table, int itemColumn) {
            List<string> list = new List<string>();

            for (int r = 0; r < table.GetNumRows(); r++)
                if (!list.Contains(table.GetData(r, itemColumn)))
                    list.Add(table.GetData(r, itemColumn));
            return list.ToArray();
        }

        private static void EnterConfirmationGridView(object sender, EventArgs e) {
            DataGridView confirmView = sender as DataGridView;
            if (confirmView != null) {
                DataGridView commandView = null;

                CommandsDataGridViewForm form = confirmView.FindForm() as CommandsDataGridViewForm;
                if (form != null)
                    commandView = form.GetCommandsDataGridView();

                if (commandView != null) {
                    DataGridViewComboBoxColumn confirmColumn = confirmView.Columns[ConfirmationPromptRow.OptionColumnName] as DataGridViewComboBoxColumn;
                    List<string> confirmationOptions = GetOptionsRequiringConfirmation(commandView);

                    if (confirmationOptions != null) 
                        foreach (string s in confirmationOptions)
                            if (!confirmColumn.Items.Contains(s))
                                confirmColumn.Items.Add(s);
                }
                else
                    Common.ErrorMessage("EnterConfirmationGridView could not find the command transition grid view");
            }
        }

        public static System.Collections.Generic.List<string> GetOptionsRequiringConfirmation(DataGridView commandsDataGridView) {
            List<string> confirmOptions = new List<string>();

            DataGridViewComboBoxColumn commandColumn = commandsDataGridView.Columns[CommandTransitionRow.OptionColumnName] as DataGridViewComboBoxColumn;
            for (int i = 0; i < commandsDataGridView.Rows.Count; i++) {
                string option = commandsDataGridView.Rows[i].Cells[CommandTransitionRow.OptionColumnName].Value as string;
                string confirm = commandsDataGridView.Rows[i].Cells[CommandTransitionRow.ConfirmColumnName].Value as string;

                // it's one row of nothing if there are no commands
                if (option == null || confirm == null)
                    return null;

                // if confirm is never it shouldn't be in the list of things to confirm
                if (confirm.Equals(Strings.ConfirmNever))
                    continue;

                if (option.Length > 0) {
                    int firstBracket = option.IndexOf(Strings.LabelStartBracket);
                    int lastBracket = option.IndexOf(Strings.LabelEndBracket);
                    if (firstBracket >= 0 && lastBracket >= 0 && lastBracket > firstBracket)
                        option = option.Remove(firstBracket, lastBracket - firstBracket + 1);
                }
                string optionLower = option.ToLower();
                if (optionLower.Contains(Strings.DynamicOptionKeyword.ToLower()))
                    option = option.Remove(optionLower.IndexOf(Strings.DynamicOptionKeyword.ToLower()), Strings.DynamicOptionKeyword.Length);

                option = option.Trim();
                if (option != null && !confirmOptions.Contains(option))
                    confirmOptions.Add(option);
            }
            return confirmOptions;
        }

        static void StateNumberKeyHandler(object sender, KeyPressEventArgs e) {
            if (StateShadow.AllowedNumberChars.IndexOf(e.KeyChar) == -1) {
                SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }

        static void StatePrefixKeyHandler(object sender, KeyPressEventArgs e) {
            if (StateShadow.AllowedPrefixChars.IndexOf(e.KeyChar) == -1) {
                SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }

        static void OnCellValueChangedForUpdateDateColumns(object sender, DataGridViewCellEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            // avoid the header column
            if (e.RowIndex < 0)
                return;

            // avoid date columns
            if (gridView.Columns[e.ColumnIndex].Name.Contains(Strings.DateStampColumnSuffix))
                return;

            //update date columns
            string columnName = gridView.Columns[e.ColumnIndex].Name;
            string dateColumnName = columnName + Strings.DateStampColumnSuffix;

            if (gridView.Columns.Contains(dateColumnName)) {
                DateTime date = DateTime.Today;
                string outputDate = date.ToString(Strings.DateColumnFormatString);
                gridView.Rows[e.RowIndex].Cells[dateColumnName].Value = outputDate;
            }
        }


        static void OnCellContentClick(object sender, DataGridViewCellEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;
            int columnIndex = e.ColumnIndex;
            int rowIndex = e.RowIndex;

            // avoid the header column and the last column
            if (rowIndex >= 0 && rowIndex < gridView.Rows.Count - 1) {
                // the condition column is, by convention, always between the in and out button
                if (gridView.Columns[columnIndex].Name.Equals(Strings.IndentColumnName)) {
                    string existingValue = gridView[columnIndex + 1, rowIndex].Value as string;
                    gridView[columnIndex + 1, rowIndex].Value = Strings.IndentCharacterString + existingValue;
                }
                else if (gridView.Columns[columnIndex].Name.Equals(Strings.OutdentColumnName)) {
                    string existingValue = gridView[columnIndex - 1, rowIndex].Value as string;
                    if (existingValue.StartsWith(Strings.IndentCharacterString))
                        gridView[columnIndex - 1, rowIndex].Value = existingValue.Substring(1);
                }
            }
        }

        static void OnEditingControlShowingForEditorHotKey(object sender, DataGridViewEditingControlShowingEventArgs e) {
            if (e.Control.GetType() == typeof(DataGridViewTextBoxEditingControl)) {
                e.Control.KeyDown -= new KeyEventHandler(OnEditingControlKeyDownForEditorHotKey);
                e.Control.KeyDown += new KeyEventHandler(OnEditingControlKeyDownForEditorHotKey);
            }
        }

        static void OnEditingControlKeyDownForEditorHotKey(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F2) {
                DataGridViewTextBoxEditingControl control = sender as DataGridViewTextBoxEditingControl;
                if (control != null) {
                    TextEditorForm editor = new TextEditorForm();
                    string text = control.Text;
                    DialogResult res = editor.ShowDialog(text);
                    if (res == DialogResult.OK) {
                        text = editor.GetText();
                        control.Text = text;
                        control.Refresh();
                    }
                    editor.Dispose();
                    editor = null;
                }
            }
        }


        static void OnTextBoxKeyDownForEditorHotKey(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.F2) {
                TextBox control = sender as TextBox;
                if (control != null) {
                    TextEditorForm editor = new TextEditorForm();
                    string text = control.Text;
                    DialogResult res = editor.ShowDialog(text);
                    if (res == DialogResult.OK) {
                        text = editor.GetText();
                        control.Text = text;
                        control.Refresh();
                    }
                    editor.Dispose();
                    editor = null;
                }
            }
        }

        internal static void PopUp_Clicked<T>(object sender, EventArgs e) where T : new() {
            ToolStripMenuItem miClicked = sender as ToolStripMenuItem;
            if (miClicked == null)
                return;

            ContextMenuStrip menu = miClicked.Owner as ContextMenuStrip;
            if (menu == null)
                return;

            if (miClicked.Tag == null)
                return;

            Keys key = (Keys)miClicked.Tag;
            string item = miClicked.Text;
            DataGridView gridView = menu.SourceControl as DataGridView;

            if (gridView == null)
                return;

            OnGridViewKeyDown<T>(gridView, new KeyEventArgs(key));
        }

        static void OnGridViewKeyDown<T>(object sender, KeyEventArgs e) where T : new() {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            if (gridView.SelectedCells.Count == 0)
                return;

            int position = gridView.SelectedCells[0].RowIndex;
            BindingList<T> list = gridView.DataSource as BindingList<T>;
            if (list == null)
                return;



            if (e.KeyCode == Keys.PageDown) {
                e.Handled = true;
                // already at the bottom
                // Change #285 fixed page down if last entry. Check to see if Transition grid or not
                if (position >= gridView.Rows.Count - 2 && (gridView.AllowUserToAddRows) ||
                    (!gridView.AllowUserToAddRows) && (position == gridView.Rows.Count - 1))
                    return;

                gridView.EndEdit();
                T tmp = list[position];
                list[position] = list[position + 1];
                list[position + 1] = tmp;
                gridView.Rows[position + 1].Selected = true;
                gridView.CurrentCell = gridView.Rows[position + 1].Cells[0];
            }
            else if (e.KeyCode == Keys.PageUp) {
                e.Handled = true;
                // already at the top or on the last blank row
                // Change #285 fixed page up if last entry. Check to see if Transition grid or not
                if (position == 0 || (position == gridView.Rows.Count - 1 && gridView.AllowUserToAddRows))
                    return;

                gridView.EndEdit();
                T tmp = list[position];
                list[position] = list[position - 1];
                list[position - 1] = tmp;
                gridView.Rows[position - 1].Selected = true;
                gridView.CurrentCell = gridView.Rows[position - 1].Cells[0];
            }
            else if (e.KeyCode == Keys.F2) {
                e.Handled = true;
                // this handles f2 when the cell is highlighted but has not entered edit mode yet
                if (gridView.SelectedCells.Count != 1)
                    return;

                DataGridViewCell cell = gridView.CurrentCell;
                if (cell != null) {
                    Type type = cell.EditType;
                    if (type == typeof(DataGridViewTextBoxEditingControl)) {
                        TextEditorForm editor = new TextEditorForm();
                        string text = string.Empty;
                        if (cell.Value != null)
                            text = cell.Value.ToString();
                        DialogResult res = editor.ShowDialog(text);
                        if (res == DialogResult.OK) {
                            text = editor.GetText();
                            cell.Value = text;
                        }
                        editor.Dispose();
                        editor = null;
                    }
                }
            }
            else if (e.KeyCode == Keys.Insert) {
                e.Handled = true;
                if (!gridView.AllowUserToAddRows)
                    return;

                if (position == gridView.Rows.Count - 1)
                    return;
                list.Insert(position, new T());
                gridView.Rows[position].Selected = true;
                gridView.CurrentCell = gridView.Rows[position].Cells[0];
            }
            else if (e.KeyCode == (Keys.ControlKey | Keys.D) || (e.KeyCode == Keys.D && e.Control == true)) {
                e.Handled = true;
                if (!gridView.AllowUserToAddRows)
                    return;

                if (position == gridView.Rows.Count - 1)
                    return;

                list.AddNew();
                DataGridViewRow source = gridView.Rows[position];
                DataGridViewRow destination = gridView.Rows[list.Count - 1];
                for (int index = 0; index < source.Cells.Count; index++)
                    destination.Cells[index].Value = gridView.Rows[position].Cells[index].Value;
            }
            else if (e.KeyCode == Keys.Delete) {
                e.Handled = true;
                if (!gridView.AllowUserToAddRows && typeof(TransitionRow) != typeof(T) && typeof(CommandTransitionRow) != typeof(T))
                    return;

                if (gridView.AllowUserToAddRows && position == gridView.Rows.Count - 1)
                    return;

                gridView.EndEdit();
                list.RemoveAt(position);

                if (position < gridView.Rows.Count) {
                    gridView.Rows[position].Selected = true;
                    gridView.CurrentCell = gridView.Rows[position].Cells[0];
                }
            }
        }

        static void OnChangeLogCellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            if (gridView.Columns[e.ColumnIndex].Name.Equals(ChangeLogRow.DateColumnName)) {
                string dateString = e.FormattedValue.ToString();
                DateTime date;

                if (DateTime.TryParse(dateString, out date)) {
                    string normalized = date.ToString(Strings.DateColumnFormatString);
                    // we don't want to change this value unless the user typed something in that
                    // doesn't match the default format because changing it is no different than
                    // the user entering a value - which means that the last row of the grid
                    // which allows for new rows to be entered ALWAYS enters a new row because 
                    // we set the value - so it thinks the user made a change and keeps it
                    if (!dateString.Equals(normalized))
                        gridView[e.ColumnIndex, e.RowIndex].Value = date.ToString(Strings.DateColumnFormatString);
                }
                else {
                    e.Cancel = true;
                    gridView.Rows[e.RowIndex].ErrorText = "Date entry must be a valid date";
                }

            }
        }

        static void OnChangeLogCellEndEdit(object sender, DataGridViewCellEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            gridView.Rows[e.RowIndex].ErrorText = String.Empty;
        }

        static void OnStateIdTextBoxesValidating(object sender, CancelEventArgs e) {
            TextBox textBox = sender as TextBox;
            TextBox statePrefix = textBox.FindForm().Controls[Strings.StatePrefixTextBoxName] as TextBox;
            TextBox stateNumber = textBox.FindForm().Controls[Strings.StateNumberTextBoxName] as TextBox;
            TextBox stateName = textBox.FindForm().Controls[Strings.StateNameTextBoxName] as TextBox;

            ShadowForm form = textBox.FindForm() as ShadowForm;
            if (form == null)
                return;
            StateShadow shadow = form.GetShadow() as StateShadow;
            if (shadow == null)
                return;

            string errorMessage;
            if (!shadow.ValidateStateIdTextBoxStrings(statePrefix.Text, stateNumber.Text, stateName.Text, out errorMessage)) {
                if (StateIdErrorProvider == null) {
                    StateIdErrorProvider = new ErrorProvider();
                    StateIdErrorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
                }
                StateIdErrorProvider.SetIconAlignment(textBox, ErrorIconAlignment.MiddleLeft);
                StateIdErrorProvider.SetIconPadding(textBox, 2);
                StateIdErrorProvider.SetError(textBox, errorMessage);
                e.Cancel = true;
            }
        }

        static void OnStateIdTextBoxChanged(object sender, EventArgs e) {
            TextBox textBox = sender as TextBox;
            TextBox statePrefix = textBox.FindForm().Controls[Strings.StatePrefixTextBoxName] as TextBox;
            TextBox stateNumber = textBox.FindForm().Controls[Strings.StateNumberTextBoxName] as TextBox;

            ShadowForm form = textBox.FindForm() as ShadowForm;
            if (form == null)
                return;

            StartShadow shadowStart = PathMaker.LookupStartShadow();
            if (shadowStart == null)
                return;

            string promptIdFormat = shadowStart.GetDefaultSetting(Strings.DefaultSettingsPromptIDFormat);
            if (((textBox.Equals(statePrefix) || textBox.Equals(stateNumber)) && promptIdFormat.Equals(Strings.PromptIdFormatPartial)) ||
                (promptIdFormat.Equals(Strings.PromptIdFormatFull))) {
                form.RedoFormPromptIdsIfNecessary(promptIdFormat);
            }

        }

        static void OnStateIdTextBoxValidated(object sender, EventArgs e) {
            TextBox textBox = sender as TextBox;

            if (StateIdErrorProvider != null)
                StateIdErrorProvider.SetError(textBox, String.Empty);
        }

        static void OnPromptTypeCellValueChangedForAutoPromptId(object sender, DataGridViewCellEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            if (!gridView.Columns[e.ColumnIndex].Name.Equals(PromptTypeRow.WordingColumnName))
                return;

            int promptIdIndex = gridView.Columns[PromptTypeRow.IdColumnName].Index;
            int promptTypeIndex = gridView.Columns[PromptTypeRow.TypeColumnName].Index;
            char letter = Strings.DefaultPromptLetter;

            int row = e.RowIndex;

            while (row >= 0) {
                string promptType = gridView[promptTypeIndex, row].Value as string;
                if (promptType != null && promptType.Length > 0) {
                    letter = promptType.ToLower().Substring(0, 1)[0];
                    break;
                }
                row--;
            }

            CalculateDefaultPromptIdIfAppropriate(gridView, e.RowIndex, e.ColumnIndex, promptIdIndex, letter);
        }

        static void OnPromptCellValueChangedForAutoPromptId(object sender, DataGridViewCellEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            if (!gridView.Columns[e.ColumnIndex].Name.Equals(PromptRow.WordingColumnName))
                return;

            int promptIdIndex = gridView.Columns[PromptRow.IdColumnName].Index;

            CalculateDefaultPromptIdIfAppropriate(gridView, e.RowIndex, e.ColumnIndex, promptIdIndex, Strings.DefaultPromptLetter);
        }

        static void OnConfirmationPromptCellValueChangedForAutoPromptId(object sender, DataGridViewCellEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            if (!gridView.Columns[e.ColumnIndex].Name.Equals(ConfirmationPromptRow.WordingColumnName))
                return;

            int promptIdIndex = gridView.Columns[ConfirmationPromptRow.IdColumnName].Index;

            CalculateDefaultPromptIdIfAppropriate(gridView, e.RowIndex, e.ColumnIndex, promptIdIndex, Strings.DefaultConfirmationPromptLetter);
        }

        public static void CalculateDefaultPromptIdIfAppropriate(DataGridView gridView, int rowIndex, int wordingColumn, int promptIdColumn, char letter) {
            if (gridView[wordingColumn, rowIndex].Value == null || gridView[wordingColumn, rowIndex].Value.Equals("") ||
                gridView[wordingColumn, rowIndex].Value.ToString().Trim().StartsWith(Strings.CalculatedPromptStartString) ||
                gridView[wordingColumn, rowIndex].Value.ToString().Trim().StartsWith(Strings.PromptTypeMacroStartString)) {
                gridView[promptIdColumn, rowIndex].Value = null;
                return;
            }

            if (gridView[promptIdColumn, rowIndex].Value != null && !gridView[promptIdColumn, rowIndex].Value.Equals(""))
                return;

            StartShadow shadow = PathMaker.LookupStartShadow();
            if (shadow == null)
                return;

            string promptIdFormat = shadow.GetDefaultSetting(Strings.DefaultSettingsPromptIDFormat);

            if (promptIdFormat.Equals(Strings.PromptIdFormatDisabled))
                return;

            if (promptIdFormat.Equals(Strings.PromptIdFormatFull) || promptIdFormat.Equals(Strings.PromptIdFormatPartial)) {
                TextBox statePrefix = gridView.FindForm().Controls[Strings.StatePrefixTextBoxName] as TextBox;
                TextBox stateNumber = gridView.FindForm().Controls[Strings.StateNumberTextBoxName] as TextBox;
                TextBox stateName = gridView.FindForm().Controls[Strings.StateNameTextBoxName] as TextBox;

                int nextNum = 1;
                foreach (DataGridViewRow row in gridView.Rows) {
                    string promptId = row.Cells[promptIdColumn].Value as string;
                    if (promptId != null && promptId.Length > 0) {
                        string[] split = promptId.Split(Strings.PromptIdSeparationChar);
                        if (split.Length > 2) {
                            char rowLetter = split[split.Length - 2][0];
                            if (letter.Equals(rowLetter)) {

                                int last = promptId.LastIndexOf(Strings.PromptIdSeparationChar);
                                if (last != -1) {
                                    int num;
                                    if (int.TryParse(promptId.Substring(last + 1), out num)) {
                                        if (num >= nextNum)
                                            nextNum = num + 1;
                                    }
                                }
                            }
                        }
                    }
                }

                string newPromptId;
                if (statePrefix != null) {
                    if (promptIdFormat.Equals(Strings.PromptIdFormatFull))
                        newPromptId = StateShadow.BuildStateIdForStorage(statePrefix.Text, stateNumber.Text, stateName.Text) +
                            Strings.PromptIdSeparationChar + letter + Strings.PromptIdSeparationChar + nextNum.ToString();
                    else
                        newPromptId = statePrefix.Text + stateNumber.Text +
                            Strings.PromptIdSeparationChar + letter + Strings.PromptIdSeparationChar + nextNum.ToString();
                }
                else {
                    // start shape
                    newPromptId = Strings.GlobalPromptPrefix + Strings.PromptIdSeparationChar + letter + Strings.PromptIdSeparationChar + nextNum.ToString();
                }

                gridView[promptIdColumn, rowIndex].Value = newPromptId;
            }
            else if (promptIdFormat.Equals(Strings.PromptIdFormatNumeric)) {
                // no automatic numbering for Numeric at this point...
            }
            else
                Common.ErrorMessage("Unknown prompt id format");
        }

        static void OnCellFormattingForHighlighting(object sender, DataGridViewCellFormattingEventArgs e) {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null)
                return;

            if (!(e.ColumnIndex >= 0 && e.RowIndex >= 0))
                return;

            DataGridViewColumn col = gridView.Columns[e.ColumnIndex];

            if (col.Name.Contains(Strings.DateStampColumnSuffix))
                return;
            if (gridView.Columns.Contains(col.Name + Strings.DateStampColumnSuffix)) {
                string dateString = gridView.Rows[e.RowIndex].Cells[col.Name + Strings.DateStampColumnSuffix].Value as string;
                if (dateString != null) {
                    DateTime date;
                    if (DateTime.TryParse(dateString, out date)) {
                        Color? color = Common.GetHighlightColor(date);
                        if (color != null)
                            e.CellStyle.BackColor = color.Value;
                    }
                }
            }
        }
    }
}

