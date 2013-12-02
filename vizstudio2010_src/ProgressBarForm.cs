using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class ProgressBarForm : Form {
        public bool Cancelled = false;
        public delegate bool Callback(Object arg, ProgressBarForm progressBarForm);
        private Callback callback;
        private Object arg = null;

        public ProgressBarForm(string title, Callback callback, Object arg) {
            InitializeComponent();
            this.callback = callback;
            this.arg = arg;
            this.Text = title;
        }

        public void SetProgressPercentage(int progress, int outOfHowMany) {
            if (outOfHowMany == 0)
                outOfHowMany = 1;
            progressBar.Value = (int)(((double)progress / outOfHowMany) * 100);
            Application.DoEvents();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            Cancelled = true;
            Hide();
            Application.DoEvents();
        }

        private void ProgressBarForm_Shown(object sender, EventArgs e) {
            bool success = callback(arg, this);
            if (!Cancelled && success)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Cancel;
        }
    }
}
