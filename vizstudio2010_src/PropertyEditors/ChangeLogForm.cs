using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class ChangeLogForm : Form, ShadowForm {
        private ChangeLogShadow shadow = null;

        public ChangeLogForm() {
            InitializeComponent();
            cancelButton.CausesValidation = false;
        }

        public DialogResult ShowDialog(ChangeLogShadow shadow) {
            this.shadow = shadow;
            return ShowDialog();
        }

        public Shadow GetShadow() {
            return shadow;
        }

        private void ChangeLogForm_Load(object sender, EventArgs e) {
            Table table = shadow.GetChangeLog();
            CommonForm.LoadChangeLogDataGridView(changeLogGridView, table);
            changeLogGridView.Focus();
        }

        private void okButton_Click(object sender, EventArgs e) {
            Hide();
            Table table = CommonForm.UnloadChangeLogDataGridView(changeLogGridView);
            shadow.SetChangeLog(table);
        }

        public void RedoFormPromptIdsIfNecessary(string promptIdFormat) {
        //place holder
        }
    }
}
