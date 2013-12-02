using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Threading;
using PathMaker;
namespace PathMaker {
    public partial class ValidateForm : Form  {
        private string vuiFileName = string.Empty;
        private BackgroundWorker backgroundWorkerPathMaker;
        public AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl;

        public ValidateForm (AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl, string vuiFileName, BackgroundWorker backgroundWorkerPathMaker) {
            this.vuiFileName = vuiFileName;
            this.backgroundWorkerPathMaker = backgroundWorkerPathMaker;
            this.visioControl = visioControl;
            InitializeComponent();
        }

        private void button_Click(object sender, EventArgs e) {
            string postDataString = string.Empty;

            

            postDataString += ("&button=Validate");

            DialogResult = DialogResult.OK;
            Hide();
   
       
            MessageBox.Show("Validation feature is not enabled in this version.  Contact Convergys PS for validation services.");
           
        }

        private void CancelBtn_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
