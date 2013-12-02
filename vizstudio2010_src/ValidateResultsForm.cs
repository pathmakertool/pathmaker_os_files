using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Visio;
using System.IO;

namespace PathMaker {
    public partial class ValidateResultsForm : Form {
        private AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl;
        private string results = string.Empty;
        private static SaveFileDialog saveFileDialog = null;

        public class ResultsRow {
            public ResultsRow() { }

            public static BindingList<ResultsRow> GetRowsFromTable(Table table) {
                BindingList<ResultsRow> list = new BindingList<ResultsRow>();

                for (int r = 0; r < table.GetNumRows(); r++) {
                    ResultsRow txt = new ResultsRow();
                    txt.Error = table.GetData(r, 0);
                    list.Add(txt);
                }
                return list;
            }

            public static Table GetTableFromRows(BindingList<ResultsRow> rows) {
                Table table = new Table(rows.Count, 1);

                int row = 0;
                foreach (ResultsRow txt in rows) {
                    table.SetData(row, 0, txt.Error);
                    row++;
                }
                return table;
            }

            // These routines represent the columns of the DataGridView
            // The DataGridView will pull them from here automatically
            // and we also use the names as the column headers - that's
            // why the strings below need to match the field names here
            public string Error { get; set; }

            // these must match the property names above
            public const string ErrorStringName = "Error";

        }


        public ValidateResultsForm(string results, AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControlIn, string formName) {
            InitializeComponent();
            UISpecResults_Load(results);
            this.BringToFront();
            visioControl = visioControlIn;
            this.results = results;

            //Close any open UI Spec results form
            CloseAllForms(formName);
        }

        private void OkayBtn_Click(object sender, EventArgs e) {
            Close();
            Dispose();
        }

        private void UISpecResults_Load(string results) {
            Table table;
            int cnt = 0;
            int lastLine = 0;

            string[] splitLine = Regex.Split(results, "\r\n\n");
            if (splitLine[splitLine.Length - 1].Equals("\r\n\r\n"))
                lastLine = splitLine.Length - 1;
            else
                lastLine = splitLine.Length;

                table = new Table(lastLine, 1);

            foreach (string line in splitLine) {
                if (cnt < lastLine) {
                    table.SetData(cnt, 0, line);
                    cnt++;
                }
            }

            BindingList<ResultsRow> eList = ResultsRow.GetRowsFromTable(table);

            UISpecResultsGridView.DataSource = eList;        
        }

        private void UISpecResultsGridView_SelectionChanged(object sender, EventArgs e) {
            DataGridViewSelectedRowCollection rows = UISpecResultsGridView.SelectedRows;
            if (rows.Count == 0)
                return;

            int rowIndex = rows[0].Index;

            highlightStateForRow(rowIndex);
        }

        private void highlightStateForRow(int rowIndex) {
            string tmpline = UISpecResultsGridView[0, rowIndex].Value.ToString();
            string matchName = string.Empty;

            Regex r1 = new Regex(Strings.StartTargetName);
            Match match = r1.Match(tmpline);
            //check to see if it's is a Start shape or something like it without ID Number.
            if (match.Success) {
                matchName = match.Groups[0].Value;
            }
            else {
                r1 = new Regex("[A-Z]{2}[0-9]{4}");
                match = r1.Match(tmpline);
                if (match.Success) {
                    matchName = match.Groups[0].Value;
                }
                else
                    return;
            }

            List<Shadow> shadowList = PathMaker.LookupAllShadows();
            foreach (Shadow shadow in shadowList) {
                StateShadow stateShadow = shadow as StateShadow;
                if (stateShadow != null) {

                    string stateName = stateShadow.GetStateId();

                    if (stateName.Contains(matchName)) {
                        visioControl.Document.Application.ActiveWindow.Page = visioControl.Document.Application.ActivePage;
                        shadow.SelectShape();
                        break;
                    }
                }
                else {
                    //if stateShadow is null then if it is a start shape
                    if (shadow.GetShapeType().ToString().Contains(matchName)) {
                        visioControl.Document.Application.ActiveWindow.Page = visioControl.Document.Application.ActivePage;
                        shadow.SelectShape();
                        break;
                    }
                }
            }
        }

        private void UISpecResultsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            highlightStateForRow(e.RowIndex);
        }

        delegate void CloseMethod(Form form);
        private void CloseForm(Form form) {
            if (!form.IsDisposed) {
                if (form.InvokeRequired) {
                    CloseMethod method = new CloseMethod(CloseForm);
                    form.Invoke(method, new object[] { form });
                }
                else {
                    form.Close();
                    form.Dispose();
                }
            }
        }

        //Loop through forms to find UISpecResutlsForm
        public void CloseAllForms(string formName) {
            // get array because collection changes as we close forms
            Form[] forms = OpenForms;

            // close every open form
            foreach (Form form in forms) {
                if (form.Name.ToString().Equals(formName) && form != this)
                    CloseForm(form);
            }
        }

        //Find open forms
        static Form[] OpenForms {
            get {
                Form[] forms = null;
                int count = System.Windows.Forms.Application.OpenForms.Count;
                forms = new Form[count];
                if (count > 0) {
                    int index = 0;
                    foreach (Form form in System.Windows.Forms.Application.OpenForms) {
                        forms[index++] = form;
                    }
                }
                return forms;
            }
        }

        private void UISpecResultsForm_FormClosed(object sender, FormClosedEventArgs e) {
            Dispose();
        }

        private void SaveResultsBtn_Click(object sender, EventArgs e) {
            string currentFileName = string.Empty;
            currentFileName = System.IO.Path.GetFileName(visioControl.Src);
            if (saveFileDialog == null) {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save Results Text File";
                saveFileDialog.Filter = "Text file (*.txt) | *.txt";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.FileName = Common.StripExtensionFileName(currentFileName) + "_result";
            }
            else {
                saveFileDialog.FileName = Common.StripExtensionFileName(saveFileDialog.FileName);
            }

            saveFileDialog.InitialDirectory = PathMaker.getCurrentFileDirectory(visioControl);

            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                //remove extra \n, otherwise it will show up as a special character when they are looking at file
                string newresults = results.Replace("\r\n\n","\r\n");
                File.WriteAllText(saveFileDialog.FileName, newresults);
            }
        }
    }



}
