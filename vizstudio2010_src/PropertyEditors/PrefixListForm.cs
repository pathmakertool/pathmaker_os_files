using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class PrefixListForm : Form, ShadowForm {
        private PrefixListShadow shadow = null;

        public PrefixListForm() {
            InitializeComponent();
            cancelButton.CausesValidation = false;
        }

        public DialogResult ShowDialog(PrefixListShadow shadow) {
            this.shadow = shadow;
            return ShowDialog();
        }

        public Shadow GetShadow() {
            return shadow;
        }

        private void PrefixListForm_Load(object sender, EventArgs e) {
            Table table = shadow.GetPrefixListTable();
            CommonForm.LoadPrefixListDataGridView(prefixListGridView, table);
            prefixListGridView.Focus();
        }

        private void okButton_Click(object sender, EventArgs e) {
            Hide();
            Table table = CommonForm.UnloadPrefixListDataGridView(prefixListGridView);
            shadow.SetPrefixListTable(table);
        }

        public void RedoFormPromptIdsIfNecessary(string promptIdFormat) {
        //place holder
        }
    }
}
