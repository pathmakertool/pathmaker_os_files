using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class SubDialogForm : Form, ShadowForm {
        private SubDialogShadow shadow;

        public SubDialogForm() {
            InitializeComponent();
            cancelButton.CausesValidation = false;
        }

        public DialogResult ShowDialog(SubDialogShadow shadow) {
            this.shadow = shadow;
            return ShowDialog();
        }
        
        public Shadow GetShadow() {
            return shadow;
        }

        private void SubDialogForm_Load(object sender, EventArgs e) {
            Table table;

            // State Name
            string stateId = shadow.GetStateId();
            CommonForm.LoadStateIdTextBoxes(statePrefixTextBox, stateNumberTextBox, stateNameTextBox, stateId);

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

            // Developer Notes
            table = CommonForm.UnloadDeveloperNotesTextBox(developerNotesTextBox);
            shadow.SetDeveloperNotes(table);
        }

        public void RedoFormPromptIdsIfNecessary(string promptIdFormat) {
        //place holder
        }
    }
}
