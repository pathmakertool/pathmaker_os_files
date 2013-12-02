using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class CallSubDialogForm : Form, ShadowForm {
        CallSubDialogShadow shadow;

        public CallSubDialogForm() {
            InitializeComponent();
            cancelButton.CausesValidation = false;
        }

        public DialogResult ShowDialog(CallSubDialogShadow shadow) {
            this.shadow = shadow;
            return ShowDialog();
        }

        public Shadow GetShadow() {
            return shadow;
        }

        private void CallSubDialogForm_Load(object sender, EventArgs e) {
            CommonForm.LoadSubDialogListBox(subDialogListBox, shadow.GetSubDialogUID());
        }

        private void okButton_Click(object sender, EventArgs e) {
            Hide();

            string chosen = CommonForm.UnloadSubDialogListBox(subDialogListBox);
            if (chosen != null && chosen.Length > 0)
                shadow.SetSubDialogUID(chosen);
        }

        public void RedoFormPromptIdsIfNecessary(string promptIdFormat) {
        //place holder
        }
    }
}
