using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public partial class StartForm : Form, CommandsDataGridViewForm, ShadowForm {
        private StartShadow shadow;
        string originalMode;
        string originalBargeIn;
        string originalPromptIdFormat;
        string originalRetries;
        string originalTimeouts;
        string originalDisconfirms;
        string originalSortOrder;

        public StartForm() {
            InitializeComponent();
            cancelButton.CausesValidation = false;
        }

        public DialogResult ShowDialog(StartShadow shadow) {
            this.shadow = shadow;
            return ShowDialog();
        }

        public Shadow GetShadow() {
            return shadow;
        }

        // used for handling confirmations
        public DataGridView GetCommandsDataGridView() {
            return commandsDataGridView;
        }

        public DataGridView GetConfirmationPromptsDataGridView() {
            return confirmationsDataGridView;
        }

        private void Start_Load(object sender, EventArgs e) {
            // Default Settings
            // I didn't like showing these in a table, so these are handled custom.
            Table table = shadow.GetDefaultSettings();

            modeComboBox.Format -= new ListControlConvertEventHandler(OnControlFormat);
            bargeInComboBox.Format -= new ListControlConvertEventHandler(OnControlFormat);
            promptIdFormatComboBox.Format -= new ListControlConvertEventHandler(OnControlFormat);
            retriesComboBox.Format -= new ListControlConvertEventHandler(OnControlFormat);
            timeoutsComboBox.Format -= new ListControlConvertEventHandler(OnControlFormat);
            disconfirmsComboBox.Format -= new ListControlConvertEventHandler(OnControlFormat);
            modeComboBox.Format += new ListControlConvertEventHandler(OnControlFormat);
            bargeInComboBox.Format += new ListControlConvertEventHandler(OnControlFormat);
            promptIdFormatComboBox.Format += new ListControlConvertEventHandler(OnControlFormat);
            retriesComboBox.Format += new ListControlConvertEventHandler(OnControlFormat);
            timeoutsComboBox.Format += new ListControlConvertEventHandler(OnControlFormat);
            disconfirmsComboBox.Format += new ListControlConvertEventHandler(OnControlFormat);
            sortOrderComboBox.Format += new ListControlConvertEventHandler(OnControlFormat);

            for (int r = 0; r < table.GetNumRows(); r++) {
                string name = table.GetData(r, (int)TableColumns.NameValuePairs.Name);
                string value = table.GetData(r, (int)TableColumns.NameValuePairs.Value);
                string dateString = table.GetData(r, (int)TableColumns.NameValuePairs.ValueDateStamp);

                System.Drawing.Color? color = null;
                DateTime date;
                if (DateTime.TryParse(dateString, out date)) {
                    color = Common.GetHighlightColor(date);
                }

                if (name.Equals(Strings.DefaultSettingsMode)) {
                    originalMode = value;
                    CommonForm.LoadModeComboBox(modeComboBox, value);
                    if (color != null)
                        modeComboBox.BackColor = color.Value;
                }
                else if (name.Equals(Strings.DefaultSettingsBargeIn)) {
                    originalBargeIn = value;
                    CommonForm.LoadYNComboBox(bargeInComboBox, value);
                    if (color != null)
                        bargeInComboBox.BackColor = color.Value;
                }
                else if (name.Equals(Strings.DefaultSettingsPromptIDFormat)) {
                    originalPromptIdFormat = value;
                    CommonForm.LoadPromptIdFormatComboBox(promptIdFormatComboBox, value);
                    if (color != null)
                        promptIdFormatComboBox.BackColor = color.Value;
                }
                else if (name.Equals(Strings.DefaultSettingsRetriesInTotalErrors)) {
                    originalRetries = value;
                    CommonForm.LoadYNComboBox(retriesComboBox, value);
                    if (color != null)
                        retriesComboBox.BackColor = color.Value;
                }
                else if (name.Equals(Strings.DefaultSettingsTimeoutsInTotalErrors)) {
                    originalTimeouts = value;
                    CommonForm.LoadYNComboBox(timeoutsComboBox, value);
                    if (color != null)
                        timeoutsComboBox.BackColor = color.Value;
                }
                else if (name.Equals(Strings.DefaultSettingsDisconfirmsInTotalErrors)) {
                    originalDisconfirms = value;
                    CommonForm.LoadYNComboBox(disconfirmsComboBox, value);
                    if (color != null)
                        disconfirmsComboBox.BackColor = color.Value;
                }
                else if (name.Equals(Strings.DefaultSettingsStateSortOrder)) {
                    originalSortOrder = value;
                    CommonForm.LoadSortOrderComboBox(sortOrderComboBox, value);
                    if (color != null)
                        sortOrderComboBox.BackColor = color.Value;
                }
            }

            // Initialization Name/Value Pairs
            table = shadow.GetInitialization();
            CommonForm.LoadNameValuePairDataGridView(initializationDataGridView, table);

            // Initialize Global Prompt Types
            table = shadow.GetPromptTypes();
            CommonForm.LoadPromptTypeDataGridView(promptsDataGridView, table);

            // Initialize Global Command Transitions
            table = shadow.GetCommandTransitions();
            CommonForm.LoadStartCommandTransitionDataGridView(commandsDataGridView, table);

            // Initialize Global Confirmation Prompts
            table = shadow.GetConfirmationPrompts();
            CommonForm.LoadConfirmationPromptGridView(confirmationsDataGridView, table, commandsDataGridView);

            // Initialize Max Handling
            table = shadow.GetMaxHandling();
            CommonForm.LoadMaxHandlingGridView(maxHandlingDataGridView, table);
        }

        void OnControlFormat(object sender, ListControlConvertEventArgs e) {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null)
                return;

            Table table = shadow.GetDefaultSettings();

            if (comboBox.Text.Length == 0)
                return;

            if (comboBox == modeComboBox) {
                if (originalMode.Equals(comboBox.Text))
                    return;
            }
            else if (comboBox == bargeInComboBox) {
                if (originalBargeIn.Equals(comboBox.Text))
                    return;
            }
            else if (comboBox == promptIdFormatComboBox) {
                if (originalPromptIdFormat.Equals(comboBox.Text))
                    return;
            }
            else if (comboBox == retriesComboBox) {
                if (originalRetries.Equals(comboBox.Text))
                    return;
            }
            else if (comboBox == timeoutsComboBox) {
                if (originalTimeouts.Equals(comboBox.Text))
                    return;
            }
            else if (comboBox == disconfirmsComboBox) {
                if (originalDisconfirms.Equals(comboBox.Text))
                    return;
            }
            else if (comboBox == sortOrderComboBox) {
                if (originalSortOrder.Equals(comboBox.Text))
                    return;
            }

            System.Drawing.Color? color = Common.GetHighlightColor(DateTime.Now);
            if (color != null)
                comboBox.BackColor = color.Value;
        }

        private void OK_Click(object sender, EventArgs e) {
            Table table;

            Hide();

            // Default Settings
            string mode = CommonForm.UnloadModeComboBox(modeComboBox);
            string bargeIn = CommonForm.UnloadYNComboBox(bargeInComboBox);
            string promptIdFormat = CommonForm.UnloadPromptIdFormatComboBox(promptIdFormatComboBox);
            string retries = CommonForm.UnloadYNComboBox(retriesComboBox);
            string timeouts = CommonForm.UnloadYNComboBox(timeoutsComboBox);
            string disconfirms = CommonForm.UnloadYNComboBox(disconfirmsComboBox);
            string sortOrder = CommonForm.UnloadSortOrderComboBox(sortOrderComboBox);

            table = shadow.GetDefaultSettings();
            for (int r = 0; r < table.GetNumRows(); r++) {
                string name = table.GetData(r, (int)TableColumns.NameValuePairs.Name);
                if (name.Equals(Strings.DefaultSettingsMode))
                    CommonForm.SetTableDataAndDateIfNecessary(table, r, mode, TableColumns.NameValuePairs.Value, TableColumns.NameValuePairs.ValueDateStamp);
                else if (name.Equals(Strings.DefaultSettingsBargeIn))
                    CommonForm.SetTableDataAndDateIfNecessary(table, r, bargeIn, TableColumns.NameValuePairs.Value, TableColumns.NameValuePairs.ValueDateStamp);
                else if (name.Equals(Strings.DefaultSettingsPromptIDFormat))
                    CommonForm.SetTableDataAndDateIfNecessary(table, r, promptIdFormat, TableColumns.NameValuePairs.Value, TableColumns.NameValuePairs.ValueDateStamp);
                else if (name.Equals(Strings.DefaultSettingsRetriesInTotalErrors))
                    CommonForm.SetTableDataAndDateIfNecessary(table, r, retries, TableColumns.NameValuePairs.Value, TableColumns.NameValuePairs.ValueDateStamp);
                else if (name.Equals(Strings.DefaultSettingsTimeoutsInTotalErrors))
                    CommonForm.SetTableDataAndDateIfNecessary(table, r, timeouts, TableColumns.NameValuePairs.Value, TableColumns.NameValuePairs.ValueDateStamp);
                else if (name.Equals(Strings.DefaultSettingsDisconfirmsInTotalErrors))
                    CommonForm.SetTableDataAndDateIfNecessary(table, r, disconfirms, TableColumns.NameValuePairs.Value, TableColumns.NameValuePairs.ValueDateStamp);
                else if (name.Equals(Strings.DefaultSettingsStateSortOrder))
                    CommonForm.SetTableDataAndDateIfNecessary(table, r, sortOrder, TableColumns.NameValuePairs.Value, TableColumns.NameValuePairs.ValueDateStamp);
            }

            shadow.SetDefaultSettings(table);

            // Initialization Name/Value Pairs
            table = CommonForm.UnloadNameValuePairDataGridView(initializationDataGridView);
            shadow.SetInitialization(table);

            // Initialize Global Prompt Types
            table = CommonForm.UnloadPromptTypeDataGridView(promptsDataGridView);
            shadow.SetPromptTypes(table);

            // Initialize Global Command Transitions
            table = CommonForm.UnloadStartCommandTransitionDataGridView(commandsDataGridView);
            shadow.SetCommandTransitions(table);

            // Initialize Global Confirmation Prompts
            table = CommonForm.UnloadConfirmationPromptGridView(confirmationsDataGridView);
            shadow.SetConfirmationPrompts(table);

            // Initialize Max Handling
            table = CommonForm.UnloadMaxHandlingGridView(maxHandlingDataGridView);
            shadow.SetMaxHandling(table);
        }

        public void RedoFormPromptIdsIfNecessary(string promptIdFormat) {
        //place holder
        }
   }
}
