using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class PlayForm : Form, ShadowForm {
        private PlayShadow shadow;

        public PlayForm() {
            InitializeComponent();
            cancelButton.CausesValidation = false;


        }

        public DialogResult ShowDialog(PlayShadow shadow) {
            this.shadow = shadow;
            return ShowDialog();
        }

        public Shadow GetShadow() {
            return shadow;
        }

        private void PlayForm_Load(object sender, EventArgs e) {
            Table table;

            // State Name
            string stateId = shadow.GetStateId();
            CommonForm.LoadStateIdTextBoxes(statePrefixTextBox, stateNumberTextBox, stateNameTextBox, stateId);

            // Initialize Prompts
            table = shadow.GetPrompts();
            CommonForm.LoadPromptDataGridView(promptsDataGridView, table);

            // Initialize Transitions
            table = shadow.GetTransitions();
            CommonForm.LoadTransitionDataGridView(transitionsDataGridView, table);

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

            Hide();

            // State Name
            string stateId = CommonForm.UnloadStateIdTextBoxes(statePrefixTextBox, stateNumberTextBox, stateNameTextBox);
            shadow.SetStateId(stateId);

            // Prompts
            table = CommonForm.UnloadPromptDataGridView(promptsDataGridView);
            shadow.SetPrompts(table);

            // Transitions
            table = CommonForm.UnloadTransitionDataGridView(transitionsDataGridView);
            shadow.SetTransitions(table);

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
        }
    }
}
