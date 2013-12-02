using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class InteractionForm : Form, CommandsDataGridViewForm, ShadowForm {
        private InteractionShadow shadow;

        public InteractionForm() {
            InitializeComponent();
            statePrefixTextBox.Focus();
        }

        public DialogResult ShowDialog(InteractionShadow shadow) {
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
            return confirmationDataGridView;
        }

        private void InteractionForm_Load(object sender, EventArgs e) {
            Table table;

            // State Name
            string stateId = shadow.GetStateId();
            CommonForm.LoadStateIdTextBoxes(statePrefixTextBox, stateNumberTextBox, stateNameTextBox, stateId);

            // Initialize Prompts
            table = shadow.GetPromptTypes();
            CommonForm.LoadPromptTypeDataGridView(promptsDataGridView, table);

            // Initialize Transitions
            table = shadow.GetCommandTransitions();
            CommonForm.LoadCommandTransitionDataGridView(commandsDataGridView, table);

            // Initialize Confirmations
            table = shadow.GetConfirmationPrompts();
            CommonForm.LoadConfirmationPromptGridView(confirmationDataGridView, table, commandsDataGridView);

            // Initialize MaxHandling
            table = shadow.GetMaxHandling();
            CommonForm.LoadMaxHandlingGridView(maxHandlingDataGridView, table);

            // Special Settings
            table = shadow.GetSpecialSettings();
            CommonForm.LoadSpecialSettingsTextBox(specialSettingsTextBox, table);

            // Developer Notes
            table = shadow.GetDeveloperNotes();
            CommonForm.LoadDeveloperNotesTextBox(developerNotesTextBox, table);

            statePrefixTextBox.Focus();
        }

        private void okButton_Click(object sender, EventArgs e) {
            Table table;

            // make sure the commands and confirmations are in sync
            DataGridViewComboBoxColumn confirmColumn = commandsDataGridView.Columns[ConfirmationPromptRow.OptionColumnName] as DataGridViewComboBoxColumn;

            List<string> commandsToConfirm = CommonForm.GetOptionsRequiringConfirmation(commandsDataGridView);
            List<string> confirmations = new List<string>();
            for (int i = 0; i < confirmationDataGridView.Rows.Count; i++) {
                string option = confirmationDataGridView.Rows[i].Cells[ConfirmationPromptRow.OptionColumnName].Value as string;

                // it's one row of nothing if there are no commands
                if (option == null)
                    break;

                // some old junk apparently had the [] in the confirmation option
                if (option.Length > 0) {
                    int firstBracket = option.IndexOf(Strings.LabelStartBracket);
                    int lastBracket = option.IndexOf(Strings.LabelEndBracket);
                    if (firstBracket >= 0 && lastBracket >= 0 && lastBracket > firstBracket)
                        option = option.Remove(firstBracket, lastBracket - firstBracket + 1);
                }

                confirmations.Add(option);
            }

            List<string> errorCommands = new List<string>();
            List<string> errorConfirmations = new List<string>();

            // make sure every command is covered
            foreach (string s in commandsToConfirm) {
                bool found = false;
                foreach (string t in confirmations)
                    if (s.Trim().Equals(t.Trim())) {
                        found = true;
                        break;
                    }
                if (!found)
                    errorCommands.Add(s);
            }
            foreach (string s in confirmations) {
                bool found = false;
                foreach (string t in commandsToConfirm) 
                    if (s.Trim().Equals(t.Trim())) {
                        found = true;
                        break;
                    }
                if (!found && s.Trim().Length != 0)
                    errorConfirmations.Add(s);
            }

            if (errorCommands.Count > 0 || errorConfirmations.Count > 0) {
                string msg = string.Empty;
                if (errorCommands.Count > 0) {
                    msg += "The following commands require confirmation:";
                    foreach (string s in errorCommands) {
                        if (s.Equals(string.Empty))
                            msg += "\n    (blank)";
                        else
                            msg += "\n    " + s;
                    }
                }
                if (errorConfirmations.Count > 0) {
                    if (errorCommands.Count > 0)
                        msg += "\n\n and the ";
                    else
                        msg += "The ";
                    msg += "following confirmations are not needed:";
                    foreach (string s in errorConfirmations) {
                        if (s.Equals(string.Empty))
                            msg += "\n    (blank)";
                        else
                            msg += "\n    " + s;
                    }
                }

                msg += "\n\n" + "Are you sure you want to continue?";
                System.Windows.Forms.DialogResult result;
                result = System.Windows.Forms.MessageBox.Show(msg, "Confirmation Error", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

                if (result == System.Windows.Forms.DialogResult.No)
                    return;
            }

            Hide();

            // State Name
            string stateId = CommonForm.UnloadStateIdTextBoxes(statePrefixTextBox, stateNumberTextBox, stateNameTextBox);
            shadow.SetStateId(stateId);

            // Prompts
            table = CommonForm.UnloadPromptTypeDataGridView(promptsDataGridView);
            shadow.SetPromptTypes(table);

            // Transitions
            table = CommonForm.UnloadCommandTransitionDataGridView(commandsDataGridView);
            shadow.SetCommandTransitions(table);

            // Confirmations
            table = CommonForm.UnloadConfirmationPromptGridView(confirmationDataGridView);
            shadow.SetConfirmationPrompts(table);

            // MaxHandling
            table = CommonForm.UnloadMaxHandlingGridView(maxHandlingDataGridView);
            shadow.SetMaxHandling(table);

            // Special Settings
            table = CommonForm.UnloadSpecialSettingsTextBox(specialSettingsTextBox);
            shadow.SetSpecialSettings(table);

            // Developer Notes
            table = CommonForm.UnloadDeveloperNotesTextBox(developerNotesTextBox);
            shadow.SetDeveloperNotes(table);
        }

        public void RedoFormPromptIdsIfNecessary(string promptIdFormat) {
            int wordCol = promptsDataGridView.Columns[PromptTypeRow.WordingColumnName].Index;
            int promptIdCol = promptsDataGridView.Columns[PromptTypeRow.IdColumnName].Index;

            CommonForm.FixPrompts(promptsDataGridView, wordCol, promptIdCol);
            //Set for Confirmation grid
            //promptIdCol does not exist for confirmation
            wordCol = confirmationDataGridView.Columns[PromptTypeRow.WordingColumnName].Index;
            CommonForm.FixPrompts(confirmationDataGridView, wordCol, promptIdCol);
        }
    }
}