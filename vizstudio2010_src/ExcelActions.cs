using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;

namespace PathMaker {
    class ExcelActions {
        // by keeping it static like this, we can reuse one - which means it will stay
        // in the old directory each time you do it
        private static SaveFileDialog saveFileDialog = null;
        private static OpenFileDialog openFileDialog = null;
        private const char DuplicateIdDelimiter = ',';

        internal static void ExportPromptList(DateTime? onOrAfterDate, bool hyperLinks, AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl)
        {
            string targetFilename;
            string currentFileName;

            if (saveFileDialog == null) {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = Common.GetResourceString(Strings.SavePromptsTitleRes);
                saveFileDialog.Filter = Common.GetResourceString(Strings.SavePromptsFilterRes);
                saveFileDialog.FilterIndex = 1;
               
                // Excel will ask about overwriting and I can't find a way to bypass that - so 
                // skip it here and let excel do it on wb.close
                saveFileDialog.OverwritePrompt = false;
            }

            saveFileDialog.InitialDirectory = PathMaker.getCurrentFileDirectory(visioControl);

            targetFilename = visioControl.Src;
            currentFileName = System.IO.Path.GetFileName(targetFilename);
            saveFileDialog.FileName = Common.StripExtensionFileName(currentFileName) + ".xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                targetFilename = saveFileDialog.FileName;
            else
                return;

            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            if (excelApp == null) {
                Common.ErrorMessage("Couldn't start Excel - make sure it's installed");
                return;
            }
            excelApp.Visible = false;

            Workbook wb = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet ws = (Worksheet)wb.Worksheets[1];

            if (ws == null) {
                Common.ErrorMessage("Excel worksheet couldn't be created.");
                return;
            }

            DocTitleShadow shadow = PathMaker.LookupDocTitleShadow();
            string client = "";
            string project = "";
            if (shadow != null) {
                client = shadow.GetClientName();
                project = shadow.GetProjectName();
            }

            ws.Cells[1, 1].Value = "Client: " + client;
            ws.Cells[2, 1].Value = "Project: " + project;
            ws.Cells[3, 1].Value = "Date: " + DateTime.Now.ToString(Strings.DateColumnFormatString);
            ws.Columns["A:A"].ColumnWidth = 8;
            ws.Columns["B:C"].ColumnWidth = 30;
            ws.Columns["D:E"].ColumnWidth = 50;

            ((Range)ws.Columns["C:E"]).EntireColumn.WrapText = true;

            ws.Cells[5, 1].Value = "Count";
            ws.Cells[5, 2].Value = "Prompt ID";
            ws.Cells[5, 3].Value = "Duplicate IDs";
            ws.Cells[5, 4].Value = "Prompt Wording";
            ws.Cells[5, 5].Value = "Notes";

            ws.Cells[5, 1].Font.Bold = true;
            ws.Cells[5, 2].Font.Bold = true;
            ws.Cells[5, 3].Font.Bold = true;
            ws.Cells[5, 4].Font.Bold = true;
            ws.Cells[5, 5].Font.Bold = true;

            PromptRecordingList recordingList = Common.GetPromptRecordingList(onOrAfterDate);

            List<string> duplicateIdList = recordingList.GetDuplicatePromptIds();
            if (duplicateIdList.Count > 0) {

                string list = String.Empty;
                int lineCounter = 1;
                foreach (string s in duplicateIdList) {
                    list += s;
                    list += ", ";
                    if (list.Length > (lineCounter * 60)) {
                        list += "\n";
                        lineCounter++;
                    }
                }
                list = list.Substring(0, list.Length - 2);

                Common.ErrorMessage("Warning: multiple copies of prompt ids in the design.\n" +
                                    "Management and testing of each is NOT handled by the tools.\n" +
                                    "You are responsible for reviewing and testing that each is correct.\n" +
                                    "Recommended that you fix the prompt numbers and let the tools handle it.\n" + 
                                    "\n" + 
                                    "Duplicates:\n" + 
                                    list);
            }

            int row = 7;
            int count = 1;
            foreach (PromptRecordingList.PromptRecording recording in recordingList.GetPromptRecordings()) {
                ws.Cells[row, 1] = count;
                ws.Cells[row, 2] = recording.PromptId;
                ws.Cells[row, 3] = MakeDuplicateString(recording.GetDuplicateIds());
                string wording = Common.StripBracketLabels(recording.Wording);
                ws.Cells[row, 4] = wording;

                // if the whole wording is the label, there are no []s
                string label = Common.MakeLabelName(recording.Wording);
                if (label.Length != wording.Length)
                    ws.Cells[row, 5] = Common.MakeLabelName(recording.Wording);

                if (hyperLinks) {
                    string recordingFile = Common.GetResourceString(Strings.PromptRecordingLocationRes);
                    recordingFile += "\\" + recording.PromptId + ".wav";
                    ws.Hyperlinks.Add(ws.Cells[row, 2], recordingFile);
                }

                row++;
                count++;
            }

            try {
                wb.SaveAs(targetFilename);
            }
            catch {
            }
            excelApp.Quit(); ;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            excelApp = null;
        }

        private static dynamic MakeDuplicateString(List<string> list) {
            string dups = "";

            if (list != null) {
                foreach (string d in list) {
                    if (dups.Length != 0)
                        dups += DuplicateIdDelimiter;
                    dups += d;
                }
            }
            return dups;
        }

        internal static void ImportPromptList(string initialDirectory) {
            string fileName;

            // Set up the open file dialog and let the user select the file to open.
            if (openFileDialog == null) {
                openFileDialog = new OpenFileDialog();
                openFileDialog.Title = Common.GetResourceString(Strings.OpenPromptsTitleRes);
                openFileDialog.Filter = Common.GetResourceString(Strings.OpenPromptsFilterRes);
                openFileDialog.FilterIndex = 1;
            }

            openFileDialog.InitialDirectory = initialDirectory;

            if (openFileDialog.ShowDialog() == DialogResult.OK) {
                // The user selected a valid file name and hit OK. Get the
                // file name from the dialog and open the file.
                fileName = openFileDialog.FileName;

                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

                if (excelApp == null) {
                    Common.ErrorMessage("Couldn't start Excel - make sure it's installed");
                    return;
                }
                excelApp.Visible = false;

                Workbook wb = excelApp.Workbooks.Open(fileName, ReadOnly: true);
                if (wb.Worksheets.Count > 0) {
                    Worksheet ws = (Worksheet)wb.Worksheets[1];

                    string promptId, dupIds, wording, notes;

                    PromptRecordingList recordingList = new PromptRecordingList();

                    int row = 7;
                    do {
                        promptId = ws.Cells[row, 2].Text;
                        dupIds = ws.Cells[row, 3].Text;
                        wording = ws.Cells[row, 4].Text;
                        notes = ws.Cells[row, 5].Text;

                        if (notes.Length > 0)
                            wording = wording + " " + Strings.LabelStartBracket + notes + Strings.LabelEndBracket;

                        if (promptId != null && promptId.Length > 0 && wording != null) {
                            recordingList.AddPromptRecording(promptId, wording);

                            if (dupIds != null && dupIds.Length > 0) {
                                string[] otherIds = dupIds.Split(DuplicateIdDelimiter);

                                foreach (string dupId in otherIds)
                                    recordingList.AddPromptRecording(dupId, wording);
                            }
                        }
                        row++;
                    } while (promptId != null && promptId.Length > 0);

                    Common.ApplyPromptRecordingList(recordingList);
                }

                wb.Close();
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                excelApp = null;
            }
        }
    }
}
