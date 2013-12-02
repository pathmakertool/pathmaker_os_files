using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Visio;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;

namespace PathMaker {
    partial class PathMaker {
        // using it like this will keep the last directory around
        private static OpenFileDialog openFileDialog = null;
        // using it like this will keep last directory around 
        private static SaveFileDialog saveFileDialog = null;
        private static bool exportPromptHyperLinks = false;

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            string fileName;

            DialogResult result;

            // Only one document can be open. Prompt user to save changes 
            // before closing the current document.
            result = SaveDrawing(visioControl, true, false);
            if (DialogResult.Cancel != result) {

                // Set up the open file dialog and let the user select the file to open.
                if (openFileDialog == null) {
                    openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = Common.GetResourceString(Strings.OpenDialogTitleRes);
                    openFileDialog.Filter = Common.GetResourceString(Strings.OpenDialogFilterRes);
                    openFileDialog.FilterIndex = 1;
                }

                openFileDialog.InitialDirectory = getCurrentFileDirectory();

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    // The user selected a valid file name and hit OK. Get the
                    // file name from the dialog and open the file.
                    fileName = openFileDialog.FileName;
                    visioControl.Src = "";

                    // without these garbage collection calls in here, we get an access violation...
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    // Open the new document.
                    visioControl.Src = fileName;
                    OnSrcDocumentChange();
                }
            }
        }

        private void fileOpentoolStripButton_Click(object sender, EventArgs e) {
            openToolStripMenuItem_Click(sender, e);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            DialogResult result;

            // Only one document can be open. Prompt user to save changes 
            // before closing the current document.
            result = SaveDrawing(visioControl, true, false);

            if (DialogResult.Cancel != result) {
                visioControl.Src = "";

                GC.Collect();
                GC.WaitForPendingFinalizers();

                visioControl.Src = System.Windows.Forms.Application.StartupPath + @"\\" + Strings.VisioTemplateFile;
                OnSrcDocumentChange();
            }
        }

        public DialogResult SaveDrawing(AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl, bool promptFirst, bool saveAs) {
            if (visioControl == null)
                return DialogResult.Cancel;

            if (visioControl.Document.Saved && !saveAs)
                return DialogResult.Ignore;

            DialogResult result = DialogResult.No;
            string targetFilename = string.Empty;
            string currentFileName = string.Empty;
            Document targetDocument;

            targetFilename = visioControl.Src;
            targetDocument = (Document)visioControl.Document;
            currentFileName = System.IO.Path.GetFileName(targetFilename);
            // Prompt to save changes.
            if (promptFirst == true) {
                string prompt = string.Empty;
                string title = string.Empty;

                title = Common.GetResourceString(Strings.SaveDialogTitleRes);

                if (targetFilename == null)
                    return DialogResult.Cancel;

                // Save changes to the existing drawing.
                if ((saveAs || targetFilename.Length > 0) && (!targetFilename.Contains(Strings.VisioTemplateFileSuffix))) {
                    prompt = Common.GetResourceString(Strings.SavePromptRes);
                    prompt += Environment.NewLine;
                    prompt += targetFilename;
                }
                else {
                    // Save changes as new drawing.
                    prompt = Common.GetResourceString(Strings.SaveAsPromptRes);
                }
                result = MessageBox.Show(prompt, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }
            else
                result = DialogResult.Yes;

            // Display a file browse dialog to select path and filename.
            if ((DialogResult.Yes == result) && (saveAs || targetFilename.Length == 0 || targetFilename.Contains(Strings.VisioTemplateFileSuffix))) {
                // Set up the save file dialog and let the user specify the
                // name to save the document to.
                if (saveFileDialog == null) {
                    saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = Common.GetResourceString(Strings.SaveDialogTitleRes);
                    saveFileDialog.Filter = Common.GetResourceString(Strings.SaveDialogFilterRes);
                    saveFileDialog.FilterIndex = 1;
                }

                saveFileDialog.InitialDirectory = getCurrentFileDirectory();

                if (targetFilename.Contains(Strings.VisioTemplateFileSuffix))
                    saveFileDialog.FileName = Strings.DefaultFileName;
                else
                    saveFileDialog.FileName = Common.StripExtensionFileName(currentFileName) + Strings.DefaultCopyFileNameSuffix;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    targetFilename = saveFileDialog.FileName;
                else
                    targetFilename = string.Empty;
            }
            // Save the document to the filename specified by
            // the end user in the save file dialog, or the existing file name.
            if ((DialogResult.Yes == result) && (targetFilename.Length > 0)) {
                if (targetDocument != null)
                    targetDocument.SaveAs(targetFilename);

                // without these garbage collection calls in here, we get an access violation...
                GC.Collect();
                GC.WaitForPendingFinalizers();

                if (!visioControl.Src.Equals(targetFilename)) {
                    visioControl.Src = targetFilename;
                    OnSrcDocumentChange();                        
                }
                visioControl.Document.Saved = true;
            }
            return result;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveDrawing(visioControl, false, false);
        }

        private void fileSaveStripButton_Click(object sender, EventArgs e) {
            SaveDrawing(visioControl, false, false);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveDrawing(visioControl, false, true);
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdOptionsPageSetup);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdFilePrint);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdEditUndo);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void repeatToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdEditRepeat);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdUFEditCut);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdUFEditCopy);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdUFEditPaste);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdUFEditSelectAll);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void deletePagesToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdOptionsDeletePages);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdEditFind);
            }
            catch {
                // ignore - it'll be fixed with the one below
            }
            // needed in either case - ok or cancel
            FixHourGlassAfterConfusingVisio();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdEditReplace);
            }
            catch {
                // ignore - it'll be fixed with the one below
            }
            // needed in either case - ok or cancel
            FixHourGlassAfterConfusingVisio();
        }

        private void shapesWindowToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdShapesWindow);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void rulersToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdViewRulers);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdViewGrid);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void guidesToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdViewGuides);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void lastZoomToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdZoomLast);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void pageWidthToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdZoomPageWidth);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void wholePageToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdViewFitInWindow);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void zoomToolStripMenuItem1_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdViewCustom);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdView400);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdView200);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdView150);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdView100);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdView75);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdView50);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void newPageToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                // For some reason our template will cause an exception if you add a new
                // page to it before you have put a shape on it.  My guess is something to 
                // do with it being a vst but it only happens in the activeX control, not
                // standard visio.  Either way, this little trick of putting a connector on and 
                // deleting it seems to solve the problem.  One other note, the shape you drop 
                // matters.  A comment shape didn't work...
                Page page = visioControl.Document.Application.ActivePage;
                Document stencil = visioControl.Document.Application.Documents[Strings.StencilFileName];
                Shape shape = page.Drop(stencil.Masters["Dynamic connector"], 1, 1);
                shape.Delete();
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdOptionsNewPage);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdFormatText);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdFormatLine);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void fillToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdFormatFill);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void shadowToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdFormatShadow);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void cornerRoundingToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdFormatCorners);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdFileSummaryInfoDlg);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void spellingToolStripMenuItem1_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdToolsSpelling);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void spellingOptionsToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdSpellingOptionsDlg);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void snapGlueToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdOptionsSnapGlueSetup);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void rulerGridToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdRulerGridDlg);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void colorPaletteToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdOptionsColorPaletteDlg);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void autoCorrectOptionsToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdMsoAutoCorrectDlg);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdOptionsPreferences);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void bringToFrontToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdObjectBringToFront);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void sendToBackToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdObjectSendToBack);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void bringForwardToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdObjectBringForward);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void sendBackwardToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdObjectSendBackward);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void alignShapesToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdObjectAlignObjects);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void distributeShapesToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdObjectDistributeDlg);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void centerDrawingToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdCenterDrawing);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void connectShapesToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdObjectConnectObjects);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void connectShapestoolStripButton_Click(object sender, EventArgs e) {
            connectShapesToolStripMenuItem_Click(sender, e);
        }

        private void PointerToolStripButton_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdDRPointerTool);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void ConnectorToolStripButton_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdDRConnectorTool);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void GluetoolStripButton_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdDrawGlue);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void aboutPathMakerToolStripMenuItem_Click(object sender, EventArgs e) {
            string info = "\r\n\r\nPathMaker Version ";

            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                info += System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
            else
                info += Assembly.GetExecutingAssembly().GetName().Version;

            info += "\r\nSchema Version ";
            info += Common.GetResourceString(Strings.PathMakerSchemaVersionRes);

            info += "\r\n\r\n";
            info += "Released to Open Source Community - January 2014";
            info += "\r\n\r\n";
            info += "Convergys Corp. - CIT Professional Services Team";
            info += "\r\n\r\n";
            info += "http://www.convergys.com";
            LogoBox aboutBox = new LogoBox(info);
            aboutBox.Text = "About PathMaker";
            aboutBox.ShowDialog();
        }

        private void userInterfaceSpecToolStripMenuItem_Click(object sender, EventArgs e) {
            WordActions.ExportUserInterfaceSpec(visioControl);
            FixHourGlassAfterConfusingVisio();
        }

        private void exportUItoolStripButton_Click(object sender, EventArgs e) {
            userInterfaceSpecToolStripMenuItem_Click(sender, e);
        }

        private void fastPathXMLToolStripMenuItem_Click(object sender, EventArgs e) {
            XmlActions.ExportFastPathXML(visioControl);
        }

        private void fastpathXMLToolStripButton_Click(object sender, EventArgs e) {
            fastPathXMLToolStripMenuItem_Click(sender, e);
        }

        private void highLevelDesignDocToolStripMenuItem_Click(object sender, EventArgs e) {
            WordActions.ExportHighLevelDesignDoc(visioControl);
            FixHourGlassAfterConfusingVisio();
        }

        private void importPromptListToolStripMenuItem_Click(object sender, EventArgs e) {
            ExcelActions.ImportPromptList(getCurrentFileDirectory());
        }

        private void importUISpecToolStripMenuItem_Click(object sender, EventArgs e) {
            WordActions.ImportUISpec(visioControl);
        }

        private void dialogStateCountToolStripMenuItem_Click(object sender, EventArgs e) {
            List<Shadow> shadows = PathMaker.LookupAllShadows();

            int interaction = 0;
            int play = 0;
            int data = 0;
            int decision = 0;
            int subdialog = 0;
            int callsub = 0;

            foreach (Shadow shadow in shadows) {
                switch (shadow.GetShapeType()) {
                    case ShapeTypes.Interaction:
                        interaction++;
                        break;
                    case ShapeTypes.Play:
                        play++;
                        break;
                    case ShapeTypes.Data:
                        data++;
                        break;
                    case ShapeTypes.Decision:
                        decision++;
                        break;
                    case ShapeTypes.SubDialog:
                        subdialog++;
                        break;
                    case ShapeTypes.CallSubDialog:
                        callsub++;
                        break;
                    default:
                        break;
                }
            }

            string info = "\r\n\r\nInteraction\t: " + interaction + "\r\n";
            info += "Play\t\t: " + play + "\r\n";
            info += "Data\t\t: " + data + "\r\n";
            info += "Decision\t\t: " + decision + "\r\n";
            info += "SubDialog\t: " + subdialog + "\r\n";
            info += "Call SubDialog\t: " + callsub + "\r\n";

            LogoBox aboutBox = new LogoBox(info);
            aboutBox.Text = "Dialog State Count";
            aboutBox.ShowDialog();
        }

        // For some reason (and others have seen this) after a Visio dialog box is put up and the 
        // user cancels it, we get hung up with an hour glass until you hit escape.  When cancel
        // is used, an exception is used back to us to tell us the user cancelled.  However, it
        // also seems to leave Visio in a wierd state.  This little trick below fixes things.
        public void FixHourGlassAfterConfusingVisio() {
            try {
                visioControl.Document.Application.ActiveWindow.Page = visioControl.Document.Application.ActivePage;
            }
            catch {
                // fails in undo sometimes... not sure why
            }
        }

        private void allPromptsToolStripMenuItem_Click(object sender, EventArgs e) {
            ExcelActions.ExportPromptList(null, exportPromptHyperLinks, visioControl);
            // required to make sure excel goes away
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void byDateToolStripMenuItem_Click(object sender, EventArgs e) {
            DateForm dateForm = new DateForm();

            if (dateForm.ShowDialog() == DialogResult.OK) {
                DateTime date = dateForm.dateTimePicker.Value;

                ExcelActions.ExportPromptList(date.Date, exportPromptHyperLinks, visioControl);
                // required to make sure excel goes away
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void addHyperlinksToolStripMenuItem_Click(object sender, EventArgs e) {
            exportPromptHyperLinks = !exportPromptHyperLinks;
        }

        private void ValidateUISpecToolStripMenuItem_Click(object sender, EventArgs e) {
            DialogResult result;
            string filename = string.Empty;

            // are we on checkbox or square
            // TODO is there a better/easier way to handle this check
            if (!ValidateUISpecToolStripButton.Text.Equals(Strings.UISPECVALIDATE)) {
                result = MessageBox.Show("Cancel UI Spec Validation in progress?", Strings.UISPECVALIDATE,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result.Equals(DialogResult.Yes)){
                    pathRunnerBackgroundWorker.StopValidationInProgress();
                    ResetValidateSpecOptions();
                }
                return;
            }

            string currentFileName = System.IO.Path.GetFileName(visioControl.Src);

            ValidateForm uiSpec = new ValidateForm(visioControl, currentFileName, pathRunnerBackgroundWorker);
            uiSpec.ShowDialog();

            if (uiSpec.DialogResult == DialogResult.OK) {
                ValidateUISpecToolStripButton.Image = Properties.Resources.icn_stop;
                ValidateUISpecToolStripMenuItem.Image = Properties.Resources.icn_stop;
                ValidateUISpecToolStripButton.Text = Strings.UISPECCANCEL;
                ValidateUISpecToolStripMenuItem.Text = Strings.UISPECCANCEL;
            }

            uiSpec.Dispose();

        }

        private void ValidateUISpecToolStripButton_Click(object sender, EventArgs e) {
            ValidateUISpecToolStripMenuItem_Click(sender, e);
        }

        private void ResetValidateSpecOptions() {
            ValidateUISpecToolStripButton.Image = Properties.Resources.icn_checkbox;
            ValidateUISpecToolStripMenuItem.Image = Properties.Resources.icn_checkbox;
            ValidateUISpecToolStripButton.Text = Strings.UISPECVALIDATE;
            ValidateUISpecToolStripMenuItem.Text = Strings.UISPECVALIDATE;
            ValidateUISpecToolStripMenuItem.Enabled = true;
            ValidateUISpecToolStripButton.Enabled = true;
        }

        private void redoPromptIdsToolStripMenuItem_Click(object sender, EventArgs e) {
            Common.RedoAllPromptIds();
        }

        private bool ReadyForConnectorMove(out Cell nonArrowSideCell, out Shape fromShape) {
            nonArrowSideCell = null;
            fromShape = null;

            if (visioControl.Document.Application.ActiveWindow.Selection.Count != 1) {
                Common.ErrorMessage("A single connector must be selected");
                return false;
            }
            Shape connector = visioControl.Document.Application.ActiveWindow.Selection[1];
            ShapeTypes type = Common.GetShapeType(connector);
            if (type != ShapeTypes.Connector) {
                Common.ErrorMessage("A single connector must be selected");
                return false;
            }

            Shadow connectorShadow = PathMaker.LookupShadowByShape(connector);
            if (connectorShadow == null) {
                Common.ErrorMessage("Connector is not a valid PathMaker shape");
                return false;
            }

            nonArrowSideCell = connector.get_Cells("BeginX");
            Connect nonArrowSideConnect = null;
            foreach (Connect c in connector.Connects) {
                if (c.FromCell.Name.Equals(Strings.BeginConnectionPointCellName))
                    nonArrowSideConnect = c;
            }

            if (nonArrowSideConnect == null) {
                Common.ErrorMessage("Connector must be connected on the non-arrow side");
                return false;
            }

            fromShape = nonArrowSideConnect.ToSheet;
            return true;
        }


        private void leftToolStripMenuItem_Click(object sender, EventArgs e) {
            Cell nonArrowSideCell;
            Shape fromShape;
            if (ReadyForConnectorMove(out nonArrowSideCell, out fromShape)) {
                SuspendConnectHandlingToMoveAConnectionPoint = true;
                nonArrowSideCell.GlueTo(fromShape.get_Cells("AlignLeft"));
                SuspendConnectHandlingToMoveAConnectionPoint = false;
            }
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e) {
            Cell nonArrowSideCell;
            Shape fromShape;
            if (ReadyForConnectorMove(out nonArrowSideCell, out fromShape)) {
                SuspendConnectHandlingToMoveAConnectionPoint = true;
                nonArrowSideCell.GlueTo(fromShape.get_Cells("AlignRight"));
                SuspendConnectHandlingToMoveAConnectionPoint = false;
            }
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e) {
            Cell nonArrowSideCell;
            Shape fromShape;
            if (ReadyForConnectorMove(out nonArrowSideCell, out fromShape)) {
                SuspendConnectHandlingToMoveAConnectionPoint = true;
                nonArrowSideCell.GlueTo(fromShape.get_Cells("AlignTop"));
                SuspendConnectHandlingToMoveAConnectionPoint = false;
            }
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e) {
            Cell nonArrowSideCell;
            Shape fromShape;
            if (ReadyForConnectorMove(out nonArrowSideCell, out fromShape)) {
                SuspendConnectHandlingToMoveAConnectionPoint = true;
                nonArrowSideCell.GlueTo(fromShape.get_Cells("AlignBottom"));
                SuspendConnectHandlingToMoveAConnectionPoint = false;
            }
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e) {
            Cell nonArrowSideCell;
            Shape fromShape;
            if (ReadyForConnectorMove(out nonArrowSideCell, out fromShape)) {
                SuspendConnectHandlingToMoveAConnectionPoint = true;
                nonArrowSideCell.GlueTo(fromShape.get_Cells("AlignCenter"));
                SuspendConnectHandlingToMoveAConnectionPoint = false;
            }
        }
        private class MyRenderer : ToolStripProfessionalRenderer {
            protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
                var btn = e.Item as ToolStripButton;
                if (btn != null && btn.CheckOnClick && btn.Checked) {
                    Rectangle bounds = new Rectangle(Point.Empty, e.Item.Size);
                    e.Graphics.FillRectangle(Brushes.Black, bounds);
                }
                else base.OnRenderButtonBackground(e);
            }
        }

        private void sizeAndPositionToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdSizePos);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }
        }

        private void fastPathClassroomToolStripMenuItem_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("http://fastpathserver.intervoice.int:8080/PathRunner/Classroom.jsp");
        }
    }
}
