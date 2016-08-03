using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker
{
    public partial class versionStampForm : Form
    {
        public versionStampForm()
        {
            InitializeComponent();
        }

    
        private void cancelButton_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OKbutton_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void versionStamp_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
