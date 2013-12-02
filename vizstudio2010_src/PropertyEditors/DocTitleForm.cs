using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    public partial class DocTitleForm : Form, ShadowForm {
        private DocTitleShadow shadow;

        public DocTitleForm() {
            InitializeComponent();
            cancelButton.CausesValidation = false;
        }

        public DialogResult ShowDialog(DocTitleShadow shadow) {
            this.shadow = shadow;
            return ShowDialog();
        }

        public Shadow GetShadow() {
            return shadow;
        }

        private void DocTitleForm_Load(object sender, EventArgs e) {
            clientNameTextBox.Text = shadow.GetClientName();
            projectNameTextBox.Text = shadow.GetProjectName();

            string imageData = shadow.GetLogoData();
            if (imageData.Length > 0) {
                Image image = Common.Base64ToImage(imageData);
                logoPictureBox.Image = image;
            }

            logoPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            
        }

        private void okButton_Click(object sender, EventArgs e) {
            Hide();

            shadow.SetClientName(clientNameTextBox.Text);
            shadow.SetProjectName(projectNameTextBox.Text);

            if (logoPictureBox.Image != null) {
                Image image = logoPictureBox.Image;
                System.Drawing.Imaging.ImageFormat format = image.RawFormat;
                string imageData = Common.ImageToBase64(image, format);
                shadow.SetLogoData(imageData);
            }
            else
                shadow.SetLogoData("");
        }

        private void fileNameButton_Click(object sender, EventArgs e) {
            OpenFileDialog openFileDialog = null;

            try {
                // Set up the open file dialog and let the user select the file to open.
                openFileDialog = new OpenFileDialog();
                openFileDialog.Title = Common.GetResourceString(Strings.OpenLogoDialogTitleRes);
                openFileDialog.Filter = Common.GetResourceString(Strings.OpenLogoDialogFilterRes);
                openFileDialog.FilterIndex = 1;
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    logoPictureBox.ImageLocation = openFileDialog.FileName;
            }
            finally {
                // Make sure the dialog is cleaned up.
                if (openFileDialog != null) {
                    openFileDialog.Dispose();
                }
            }
        }

        private void clearButton_Click(object sender, EventArgs e) {
            logoPictureBox.ImageLocation = "";
            logoPictureBox.Image = null;
        }

        public void RedoFormPromptIdsIfNecessary(string promptIdFormat) {
        //place holder
        }
    }
}
