using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class TextEditorForm : Form {
        public TextEditorForm() {
            InitializeComponent();
        }

        public DialogResult ShowDialog(string text) {
            textBox.Text = text;
            textBox.Focus();
            return ShowDialog();
        }

        public string GetText() {
            return textBox.Text;
        }

        private void okButton_Click(object sender, EventArgs e) {
            Hide();
            DialogResult = DialogResult.OK;
        }
    }
}
