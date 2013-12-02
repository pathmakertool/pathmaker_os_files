using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

// When adding a new form, here are the property settings used
// Name
// Cancel Button
// ControlBox = false
// FormBorderStyle = FixedSingle
// ShowIcon = False
// ShowIconInTaskBar = false
// MaximumSize = 1014,758 (for property editors)
// MinimumSize = 1014,758 (for property editors)
// Size = 1014,758 (for property editors) - if you don't set min/max, VisualStudio changes this sizing without notice
// StartPosition = CenterScreen
// Text

namespace PathMaker {
    public partial class CommonForm {
        public static void LoadNameValuePairDataGridView(DataGridView gridView, Table table) {
            BindingList<NameValuePairRow> nvList = NameValuePairRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddTextBoxColumn(gridView, NameValuePairRow.NameColumnName);
                AddTextBoxColumn(gridView, NameValuePairRow.ValueColumnName);
                AddTextBoxColumn(gridView, NameValuePairRow.NameDateStampColumnName);
                AddTextBoxColumn(gridView, NameValuePairRow.ValueDateStampColumnName);
                ApplyCommonDataGridViewSettings<NameValuePairRow>(gridView, true);
                HideDateStampColumns(gridView);
            }

            gridView.DataSource = nvList;
        }

        public static void LoadPromptTypeDataGridView(DataGridView gridView, Table table) {
            BindingList<PromptTypeRow> ptList = PromptTypeRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddEditableStringComboBoxColumn(gridView, PromptTypeRow.TypeColumnName);
                AddButtonColumn(gridView, Strings.IndentColumnName, Strings.IndentButtonName);
                AddTextBoxColumn(gridView, PromptTypeRow.ConditionColumnName);
                AddButtonColumn(gridView, Strings.OutdentColumnName, Strings.OutdentButtonName);
                AddTextBoxColumn(gridView, PromptTypeRow.WordingColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.IdColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.TypeDateStampColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.ConditionDateStampColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.WordingDateStampColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.IdDateStampColumnName);

                gridView.CellValidating -= new DataGridViewCellValidatingEventHandler(OnPromptTypeCellValidatingForDropDownCombo);
                gridView.CellValidating += new DataGridViewCellValidatingEventHandler(OnPromptTypeCellValidatingForDropDownCombo);

                gridView.CellValueChanged -= new DataGridViewCellEventHandler(OnPromptTypeCellValueChangedForAutoPromptId);
                gridView.CellValueChanged += new DataGridViewCellEventHandler(OnPromptTypeCellValueChangedForAutoPromptId);

                gridView.DefaultValuesNeeded -= new DataGridViewRowEventHandler(OnPromptTypeDefaultValuesNeeded);
                gridView.DefaultValuesNeeded += new DataGridViewRowEventHandler(OnPromptTypeDefaultValuesNeeded);

                ApplyCommonDataGridViewSettings<PromptTypeRow>(gridView, true);
                HideDateStampColumns(gridView);
            }
            
            // Loaded each time because they need to add in user defined ones
            LoadComboBoxColumn(gridView, PromptTypeRow.TypeColumnName, GetPromptTypeComboValues(table, (int)TableColumns.PromptTypes.Type));

            gridView.DataSource = ptList;
        }

        public static void LoadCommandTransitionDataGridView(DataGridView gridView, Table table) {
            BindingList<CommandTransitionRow> ctList = CommandTransitionRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddTextBoxColumn(gridView, CommandTransitionRow.OptionColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.VocabColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.DTMFColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ConditionColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ActionColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.GotoColumnName);
                AddStringComboBoxColumn(gridView, CommandTransitionRow.ConfirmColumnName);
                LoadComboBoxColumn(gridView, CommandTransitionRow.ConfirmColumnName, confirmValues);
                AddTextBoxColumn(gridView, CommandTransitionRow.OptionDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.VocabDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.DTMFDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ConditionDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ActionDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.GotoDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ConfirmDateStampColumnName);

                gridView.DefaultValuesNeeded -= new DataGridViewRowEventHandler(OnCommandTransitionDefaultValuesNeeded);
                gridView.DefaultValuesNeeded += new DataGridViewRowEventHandler(OnCommandTransitionDefaultValuesNeeded);

                ApplyCommonDataGridViewSettings<CommandTransitionRow>(gridView, false);
                HideDateStampColumns(gridView);
                gridView.Columns[CommandTransitionRow.GotoColumnName].ReadOnly = true;
            }

            gridView.DataSource = ctList;
        }

        public static void LoadStartCommandTransitionDataGridView(DataGridView gridView, Table table) {
            BindingList<StartCommandTransitionRow> ctList = StartCommandTransitionRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddTextBoxColumn(gridView, CommandTransitionRow.OptionColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.VocabColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.DTMFColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ConditionColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ActionColumnName);
                AddItemComboBoxColumn(gridView, CommandTransitionRow.GotoColumnName);
                AddStringComboBoxColumn(gridView, CommandTransitionRow.ConfirmColumnName);
                LoadComboBoxColumn(gridView, CommandTransitionRow.ConfirmColumnName, confirmValues);
                AddTextBoxColumn(gridView, CommandTransitionRow.OptionDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.VocabDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.DTMFDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ConditionDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ActionDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.GotoDateStampColumnName);
                AddTextBoxColumn(gridView, CommandTransitionRow.ConfirmDateStampColumnName);

                gridView.DefaultValuesNeeded -= new DataGridViewRowEventHandler(OnCommandTransitionDefaultValuesNeeded);
                gridView.DefaultValuesNeeded += new DataGridViewRowEventHandler(OnCommandTransitionDefaultValuesNeeded);

                ApplyCommonDataGridViewSettings<StartCommandTransitionRow>(gridView, true);
                HideDateStampColumns(gridView);
            }

            // loaded each time to make sure there's a full list of state names
            LoadItemComboBoxColumn(gridView, CommandTransitionRow.GotoColumnName, GetAllPossibleGotos());

            gridView.DataSource = ctList;
        }

        public static void LoadConfirmationPromptGridView(DataGridView gridView, Table table, DataGridView partnerCommandTransitionGridView) {
            BindingList<ConfirmationPromptRow> cpList = ConfirmationPromptRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddStringComboBoxColumn(gridView, ConfirmationPromptRow.OptionColumnName);
                gridView.Enter += new EventHandler(EnterConfirmationGridView);
                AddButtonColumn(gridView, Strings.IndentColumnName, Strings.IndentButtonName);
                AddTextBoxColumn(gridView, ConfirmationPromptRow.ConditionColumnName);
                AddButtonColumn(gridView, Strings.OutdentColumnName, Strings.OutdentButtonName);
                AddTextBoxColumn(gridView, ConfirmationPromptRow.WordingColumnName);
                AddTextBoxColumn(gridView, ConfirmationPromptRow.IdColumnName);
                AddTextBoxColumn(gridView, ConfirmationPromptRow.OptionDateStampColumnName);
                AddTextBoxColumn(gridView, ConfirmationPromptRow.ConditionDateStampColumnName);
                AddTextBoxColumn(gridView, ConfirmationPromptRow.WordingDateStampColumnName);
                AddTextBoxColumn(gridView, ConfirmationPromptRow.IdDateStampColumnName);

                gridView.CellValueChanged -= new DataGridViewCellEventHandler(OnConfirmationPromptCellValueChangedForAutoPromptId);
                gridView.CellValueChanged += new DataGridViewCellEventHandler(OnConfirmationPromptCellValueChangedForAutoPromptId);

                ApplyCommonDataGridViewSettings<ConfirmationPromptRow>(gridView, true);
                HideDateStampColumns(gridView);
            }

            // starts us off with the right set of options - we'll add dynamically later if new ones are added in EnterConfirmationGridView
            LoadComboBoxColumn(gridView, ConfirmationPromptRow.OptionColumnName, GetConfirmationPromptComboValues(table, (int)TableColumns.ConfirmationPrompts.Option));

            gridView.DataSource = cpList;
        }

        public static void LoadMaxHandlingGridView(DataGridView gridView, Table table) {
            BindingList<MaxHandlingRow> mhList = MaxHandlingRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddTextBoxColumn(gridView, MaxHandlingRow.ConditionColumnName);
                AddTextBoxColumn(gridView, MaxHandlingRow.CountColumnName);
                AddTextBoxColumn(gridView, MaxHandlingRow.ActionColumnName);
                AddItemComboBoxColumn(gridView, MaxHandlingRow.GotoColumnName);
                AddTextBoxColumn(gridView, MaxHandlingRow.CountDateStampColumnName);
                AddTextBoxColumn(gridView, MaxHandlingRow.ActionDateStampColumnName);
                AddTextBoxColumn(gridView, MaxHandlingRow.GotoDateStampColumnName);

                ApplyCommonDataGridViewSettings<MaxHandlingRow>(gridView, false);
                HideDateStampColumns(gridView);
                gridView.Columns[MaxHandlingRow.ConditionColumnName].ReadOnly = true;
            }

            // loaded each time to get an updated list of gotos
            LoadItemComboBoxColumn(gridView, MaxHandlingRow.GotoColumnName, GetAllPossibleGotos());

            gridView.DataSource = mhList;
        }

        internal static void LoadModeComboBox(ComboBox comboBox, string value) {
            comboBox.Items.Clear();
            foreach (string s in modeValues)
                comboBox.Items.Add(s);
            comboBox.SelectedItem = value;
        }

        internal static void LoadYNComboBox(ComboBox comboBox, string value) {
            comboBox.Items.Clear();
            foreach (string s in ynValues)
                comboBox.Items.Add(s);
            comboBox.SelectedItem = value;
        }

        internal static void LoadPromptIdFormatComboBox(ComboBox comboBox, string value) {
            comboBox.Items.Clear();
            foreach (string s in promptIdFormatValues)
                comboBox.Items.Add(s);
            comboBox.SelectedItem = value;
        }

        internal static string UnloadYNComboBox(ComboBox comboBox) {
            return comboBox.SelectedItem as string;
        }

        internal static string UnloadPromptIdFormatComboBox(ComboBox comboBox) {
            return comboBox.SelectedItem as string;
        }

        internal static string UnloadModeComboBox(ComboBox comboBox) {
            return comboBox.SelectedItem as string;
        }

        internal static Table UnloadNameValuePairDataGridView(DataGridView gridView) {
            BindingList<NameValuePairRow> list = gridView.DataSource as BindingList<NameValuePairRow>;
            return NameValuePairRow.GetTableFromRows(list);
        }

        internal static Table UnloadPromptTypeDataGridView(DataGridView gridView) {
            BindingList<PromptTypeRow> list = gridView.DataSource as BindingList<PromptTypeRow>;
            return PromptTypeRow.GetTableFromRows(list);
        }

        internal static Table UnloadCommandTransitionDataGridView(DataGridView gridView) {
            BindingList<CommandTransitionRow> list = gridView.DataSource as BindingList<CommandTransitionRow>;
            return CommandTransitionRow.GetTableFromRows(list);
        }

        internal static Table UnloadStartCommandTransitionDataGridView(DataGridView gridView) {
            BindingList<StartCommandTransitionRow> list = gridView.DataSource as BindingList<StartCommandTransitionRow>;
            return StartCommandTransitionRow.GetTableFromRows(list);
        }

        internal static Table UnloadConfirmationPromptGridView(DataGridView gridView) {
            BindingList<ConfirmationPromptRow> list = gridView.DataSource as BindingList<ConfirmationPromptRow>;
            return ConfirmationPromptRow.GetTableFromRows(list);
        }

        internal static Table UnloadMaxHandlingGridView(DataGridView gridView) {
            BindingList<MaxHandlingRow> list = gridView.DataSource as BindingList<MaxHandlingRow>;
            return MaxHandlingRow.GetTableFromRows(list);
        }

        internal static void LoadPromptDataGridView(DataGridView gridView, Table table) {
            BindingList<PromptRow> ptList = PromptRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddButtonColumn(gridView, Strings.IndentColumnName, Strings.IndentButtonName);
                AddTextBoxColumn(gridView, PromptTypeRow.ConditionColumnName);
                AddButtonColumn(gridView, Strings.OutdentColumnName, Strings.OutdentButtonName);
                AddTextBoxColumn(gridView, PromptTypeRow.WordingColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.IdColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.TypeDateStampColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.ConditionDateStampColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.WordingDateStampColumnName);
                AddTextBoxColumn(gridView, PromptTypeRow.IdDateStampColumnName);

                gridView.CellValueChanged -= new DataGridViewCellEventHandler(OnPromptCellValueChangedForAutoPromptId);
                gridView.CellValueChanged += new DataGridViewCellEventHandler(OnPromptCellValueChangedForAutoPromptId);

                ApplyCommonDataGridViewSettings<PromptRow>(gridView, true);
            }

            HideDateStampColumns(gridView);
            gridView.DataSource = ptList;
        }

        internal static void LoadTransitionDataGridView(DataGridView gridView, Table table) {
            BindingList<TransitionRow> tList = TransitionRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddTextBoxColumn(gridView, TransitionRow.ConditionColumnName);
                AddTextBoxColumn(gridView, TransitionRow.ActionColumnName);
                AddTextBoxColumn(gridView, TransitionRow.GotoColumnName);
                AddTextBoxColumn(gridView, TransitionRow.ConditionDateStampColumnName);
                AddTextBoxColumn(gridView, TransitionRow.ActionDateStampColumnName);
                AddTextBoxColumn(gridView, TransitionRow.GotoDateStampColumnName);

                ApplyCommonDataGridViewSettings<TransitionRow>(gridView, false);
                HideDateStampColumns(gridView);
                gridView.Columns[CommandTransitionRow.GotoColumnName].ReadOnly = true;
            }

            gridView.DataSource = tList;
        }

        internal static void LoadSpecialSettingsTextBox(TextBox textBox, Table table) {
            if (!table.IsEmpty())
                textBox.Text = table.GetData(0, (int)TableColumns.SpecialSettings.Text);
            else
                textBox.Text = string.Empty;
            textBox.KeyDown -= new KeyEventHandler(OnTextBoxKeyDownForEditorHotKey);
            textBox.KeyDown += new KeyEventHandler(OnTextBoxKeyDownForEditorHotKey);
        }

        internal static void LoadDeveloperNotesTextBox(TextBox textBox, Table table) {
            if (!table.IsEmpty())
                textBox.Text = table.GetData(0, (int)TableColumns.DeveloperNotes.Text);
            else
                textBox.Text = string.Empty;
            textBox.KeyDown -= new KeyEventHandler(OnTextBoxKeyDownForEditorHotKey);
            textBox.KeyDown += new KeyEventHandler(OnTextBoxKeyDownForEditorHotKey);
        }

        internal static void LoadStateIdTextBoxes(TextBox statePrefixTextBox, TextBox stateNumberTextBox, TextBox stateNameTextBox, string stateId) {
            if (stateId.Length != 0) {
                string tmp = StateShadow.StateIdForDisplay(stateId);

                string prefix, number, name;
                StateShadow.DisectStateIdIntoParts(tmp, out prefix, out number, out name);
                statePrefixTextBox.Text = prefix;
                stateNumberTextBox.Text = number;
                stateNameTextBox.Text = name;
            }
            else {
                statePrefixTextBox.Text = string.Empty;
                stateNumberTextBox.Text = string.Empty;
                stateNameTextBox.Text = string.Empty;
            }

            statePrefixTextBox.KeyPress -= new KeyPressEventHandler(StatePrefixKeyHandler);
            stateNumberTextBox.KeyPress -= new KeyPressEventHandler(StateNumberKeyHandler);
            statePrefixTextBox.KeyPress += new KeyPressEventHandler(StatePrefixKeyHandler);
            stateNumberTextBox.KeyPress += new KeyPressEventHandler(StateNumberKeyHandler);

            statePrefixTextBox.Validating -= new CancelEventHandler(OnStateIdTextBoxesValidating);
            statePrefixTextBox.Validating += new CancelEventHandler(OnStateIdTextBoxesValidating);
            stateNumberTextBox.Validating -= new CancelEventHandler(OnStateIdTextBoxesValidating);
            stateNumberTextBox.Validating += new CancelEventHandler(OnStateIdTextBoxesValidating);
            stateNameTextBox.Validating -= new CancelEventHandler(OnStateIdTextBoxesValidating);
            stateNameTextBox.Validating += new CancelEventHandler(OnStateIdTextBoxesValidating);

            statePrefixTextBox.Validated -= new EventHandler(OnStateIdTextBoxValidated);
            statePrefixTextBox.Validated += new EventHandler(OnStateIdTextBoxValidated);
            stateNumberTextBox.Validated -= new EventHandler(OnStateIdTextBoxValidated);
            stateNumberTextBox.Validated += new EventHandler(OnStateIdTextBoxValidated);
            stateNameTextBox.Validated -= new EventHandler(OnStateIdTextBoxValidated);
            stateNameTextBox.Validated += new EventHandler(OnStateIdTextBoxValidated);

            //Handle Text change on TextBox
            statePrefixTextBox.TextChanged -= new EventHandler(OnStateIdTextBoxChanged);
            statePrefixTextBox.TextChanged += new EventHandler(OnStateIdTextBoxChanged);
            stateNumberTextBox.TextChanged -= new EventHandler(OnStateIdTextBoxChanged);
            stateNumberTextBox.TextChanged += new EventHandler(OnStateIdTextBoxChanged);
            stateNameTextBox.TextChanged -= new EventHandler(OnStateIdTextBoxChanged);
            stateNameTextBox.TextChanged += new EventHandler(OnStateIdTextBoxChanged);
        }

        internal static string UnloadStateIdTextBoxes(TextBox statePrefixTextBox, TextBox stateNumberTextBox, TextBox stateNameTextBox) {
            return StateShadow.BuildStateIdForStorage(statePrefixTextBox.Text, stateNumberTextBox.Text, stateNameTextBox.Text);
        }

        internal static Table UnloadPromptDataGridView(DataGridView gridView) {
            BindingList<PromptRow> list = gridView.DataSource as BindingList<PromptRow>;
            return PromptRow.GetTableFromRows(list);
        }

        internal static Table UnloadTransitionDataGridView(DataGridView gridView) {
            BindingList<TransitionRow> list = gridView.DataSource as BindingList<TransitionRow>;
            return TransitionRow.GetTableFromRows(list);
        }

        internal static Table UnloadSpecialSettingsTextBox(TextBox textBox) {
            Table table = new Table(1, 2);
            table.SetData(0, (int)TableColumns.SpecialSettings.Text, textBox.Text);
            return table;
        }

        internal static Table UnloadDeveloperNotesTextBox(TextBox textBox) {
            Table table = new Table(1, 2);
            table.SetData(0, (int)TableColumns.DeveloperNotes.Text, textBox.Text);
            return table;
        }

        internal static void LoadSubDialogListBox(ListBox listBox, string currentValue) {
            List<ComboBoxItem> list = new List<ComboBoxItem>();

            List<Shadow> shadowList = PathMaker.LookupShadowsByShapeType(ShapeTypes.SubDialog);
            shadowList.Sort(Common.StateIdShadowSorterAlphaNumerical);

            foreach (Shadow shadow in shadowList) {
                SubDialogShadow subDialogShadow = shadow as SubDialogShadow;
                list.Add(new ComboBoxItem(subDialogShadow.GetStateId(), subDialogShadow.GetUniqueId()));
            }

            listBox.DataSource = list;
            listBox.DisplayMember = ComboBoxItem.DisplayMemberName;
            listBox.ValueMember = ComboBoxItem.ValueMemberName;
            listBox.SelectedValue = currentValue;
        }

        internal static string UnloadSubDialogListBox(ListBox listBox) {
            return listBox.SelectedValue as string;
        }

        internal static void LoadChangeLogDataGridView(DataGridView gridView, Table table) {
            BindingList<ChangeLogRow> clList = ChangeLogRow.GetRowsFromTable(table);

            if (gridView.Columns.Count == 0) {
                gridView.AutoGenerateColumns = false;
                AddTextBoxColumn(gridView, ChangeLogRow.DateColumnName);
                AddTextBoxColumn(gridView, ChangeLogRow.VersionColumnName);
                AddTextBoxColumn(gridView, ChangeLogRow.DetailsColumnName);
                AddTextBoxColumn(gridView, ChangeLogRow.AuthorColumnName);
                AddStringComboBoxColumn(gridView, ChangeLogRow.HighlightColumnName); 

                gridView.DefaultValuesNeeded -= new DataGridViewRowEventHandler(OnChangeLogDefaultValuesNeeded);
                gridView.DefaultValuesNeeded += new DataGridViewRowEventHandler(OnChangeLogDefaultValuesNeeded);

                gridView.CellValidating -= new DataGridViewCellValidatingEventHandler(OnChangeLogCellValidating);
                gridView.CellValidating += new DataGridViewCellValidatingEventHandler(OnChangeLogCellValidating);
                gridView.CellEndEdit -= new DataGridViewCellEventHandler(OnChangeLogCellEndEdit);
                gridView.CellEndEdit += new DataGridViewCellEventHandler(OnChangeLogCellEndEdit);

                LoadComboBoxColumn(gridView, ChangeLogRow.HighlightColumnName, highlightValues);
                ApplyCommonDataGridViewSettings<ChangeLogRow>(gridView, true);
            }

            gridView.DataSource = clList;
        }

        internal static Table UnloadChangeLogDataGridView(DataGridView gridView) {
            BindingList<ChangeLogRow> list = gridView.DataSource as BindingList<ChangeLogRow>;
            return ChangeLogRow.GetTableFromRows(list);
        }

        internal static void SetTableDataAndDateIfNecessary(Table table, int row, string newValue, 
            TableColumns.NameValuePairs valueColumn, TableColumns.NameValuePairs dateColumn) {
            string oldValue = table.GetData(row, (int)valueColumn);
            if (oldValue.Equals(newValue))
                return;

            table.SetData(row, (int)valueColumn, newValue);
            table.SetData(row, (int)dateColumn, DateTime.Today.ToString(Strings.DateColumnFormatString));
        }

        public static void FixPrompts(DataGridView view, int wordCol, int promptIdCol) {
            string promptType = string.Empty;
            char letter = Strings.DefaultPromptLetter;
            int promptTypeIndex = 0;

            //null out prompt id field
            for (int cnt = 0; cnt < view.RowCount - 1; cnt++) {
                view[promptIdCol, cnt].Value = null;
            }

            for (int cnt = 0; cnt < view.RowCount - 1; cnt++) {
                if (!view[wordCol, cnt].Value.Equals(null) && !view[wordCol, cnt].Value.Equals("")) {
                    int promptIdIndex = view.Columns[PromptTypeRow.IdColumnName].Index;

                    if (view.Columns[PromptTypeRow.TypeColumnName] != null) {
                        promptTypeIndex = view.Columns[PromptTypeRow.TypeColumnName].Index;
                        promptType = view[promptTypeIndex, cnt].Value as string;
                    }
                    else
                        if (view.Columns[ConfirmationPromptRow.OptionColumnName] != null) {
                            //default confirmation
                            letter = Strings.DefaultConfirmationPromptLetter;
                        }

                    if (promptType != null && promptType.Length > 0) {
                        letter = promptType.ToLower().Substring(0, 1)[0];
                    }
                    //Update prompt id's                    
                    CommonForm.CalculateDefaultPromptIdIfAppropriate(view, cnt, wordCol, promptIdCol, letter);

                }
            }
        }

        internal static void LoadSortOrderComboBox(ComboBox comboBox, string value) {
            comboBox.Items.Clear();
            foreach (string s in sortOrderValues)
                comboBox.Items.Add(s);
            comboBox.SelectedItem = value;
        }

        internal static string UnloadSortOrderComboBox(ComboBox comboBox) {
            return comboBox.SelectedItem as string;
        }
    }
}
