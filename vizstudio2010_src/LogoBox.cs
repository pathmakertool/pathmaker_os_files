using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class LogoBox : Form {
        public LogoBox(string info) {
            InitializeComponent();
            infoTextBox.Text = info;
        }

        private void okButton_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
