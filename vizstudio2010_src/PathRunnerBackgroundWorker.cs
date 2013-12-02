using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PathMaker {
    class PathRunnerBackgroundWorker : BackgroundWorker {
        private Thread workerThread;
        private Guid guid = Guid.Empty;
        private HttpWebRequest httpWebRequest;
        private ValidationData validationData;
        private Boolean cancelUIRun = false;
        private string debugFileName = string.Empty;
        private string filename = string.Empty;
        private string htmlResults = string.Empty;

        public class ValidationData {
            public string xmlFileName;
            public string vuiFileName;
            public string postDataString;
            public AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl;
        };

        protected override void OnDoWork(DoWorkEventArgs e) {
            workerThread = Thread.CurrentThread;
            try {
                base.OnDoWork(e);
                PathRunnerBackgroundWorker.ValidationData fileNameData = (PathRunnerBackgroundWorker.ValidationData)e.Argument;
                CallValidateSpecWorkerJSP(fileNameData);
            }
            catch (ThreadAbortException) {
                e.Cancel = true;        //We must set Cancel property to true!
                Thread.ResetAbort();    //Prevents ThreadAbortException propagation
            }
        }

        ~PathRunnerBackgroundWorker() {
            if (guid != Guid.Empty) {
                HttpWebRequest cancelRequest = HttpWebRequest.Create(Strings.SERVERNAME + "PathRunnerRequestHandler.jsp?Action=StopWalkingClientTag&ClientTag=" + guid) as HttpWebRequest;
                HttpWebResponse cancelResponse = null;

                try {
                    cancelResponse = cancelRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException) {
                    // usually because we aren't connected which has already been reported on the original request, ignore
                }
                finally {
                    if (cancelResponse != null)
                        cancelResponse.Close();
                }
            }
        }

        public void Abort() {
            if (workerThread != null) {
                workerThread.Abort();
                workerThread = null;
            }
        }

        public void CallValidateSpecWorkerJSP(ValidationData validationData) {
            this.validationData = validationData;

            HttpWebResponse httpWebResponse = null;
            string xmlFileName = Common.StripExtensionFileName(validationData.vuiFileName) + ".xml";

            try {
                //Creating the Web Request.
                guid = Guid.NewGuid();
                httpWebRequest = HttpWebRequest.Create(Strings.SERVERNAME + Strings.VALIDATEJSPNAME + "?ClientTag=" + guid) as HttpWebRequest;
                //Specifing the Method
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "multipart/form-data=-----------------------------";
                // Create POST data and convert it to a byte array.
                string bufferTmp = "---------------------------Content-Disposition: form-data; name=\"fileField\";filename=" + "\"" + xmlFileName + "\"Content-Type: text/xml\n\n\n";
                Byte[] buffer = Encoding.ASCII.GetBytes(bufferTmp);
                //need to fix targetfilename with a real value
                string path = validationData.xmlFileName;
                //Open xml file for reading
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                int length = (int)fs.Length;
                byte[] buffer2 = new byte[length];
                int count;
                int sum = 0;
                //Read xml into buffer2       
                while ((count = fs.Read(buffer2, sum, length - sum)) > 0)
                    sum += count;
                fs.Close();

                //postDataString has to be formatted just right otherwise it won't work
                string tmpPostDataString = "\n\n\n\n-----------------------------\nContent-Disposition: form-data; ";
                tmpPostDataString += validationData.postDataString;
                tmpPostDataString += ("\n-----------------------------");
                tmpPostDataString += ("\nfilefield: " + Common.StripExtensionFileName(validationData.vuiFileName));

                Console.WriteLine("postDataString: " + tmpPostDataString);

                byte[] buffer3 = Encoding.ASCII.GetBytes(tmpPostDataString);

                //Combine the buffers
                byte[] bufferCombined = new byte[buffer.Length + buffer2.Length + buffer3.Length];
                Array.Copy(buffer, 0, bufferCombined, 0, buffer.Length);
                Array.Copy(buffer2, 0, bufferCombined, buffer.Length, buffer2.Length);
                Array.Copy(buffer3, 0, bufferCombined, buffer.Length + buffer2.Length, buffer3.Length);

                //Send Requst with PostData
                httpWebRequest.ContentLength = bufferCombined.Length;
                httpWebRequest.Timeout = System.Threading.Timeout.Infinite;

                Stream PostData = httpWebRequest.GetRequestStream();
                PostData.Write(bufferCombined, 0, bufferCombined.Length);
                PostData.Close();

                //Getting the Response and reading the result.
                httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;

                using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream())) {
                    htmlResults = sr.ReadToEnd();
                }

                httpWebResponse.Close();

            }
            catch (WebException e) {
                if (e.Status == WebExceptionStatus.ProtocolError) {
                    MessageBox.Show("Exception Message :" + e.Message +
                        "\nStatus Code : " + ((HttpWebResponse)e.Response).StatusCode +
                        "\nStatus Description : " + ((HttpWebResponse)e.Response).StatusDescription +
                        "\nPlease Contact PathMaker Support", "XML Validate Error");
                }
                else {
                    if (cancelUIRun.Equals(false))
                        MessageBox.Show("Unable to Validate XML.\nPlease make sure you are connected to Convergys Network.", "XML Validate Error");
                }
                cancelUIRun = false;
                base.OnRunWorkerCompleted(new RunWorkerCompletedEventArgs(this, null, true));
            }
            finally {
                if (httpWebResponse != null)
                    httpWebResponse.Close();
            }
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e) {
            base.OnRunWorkerCompleted(e);

            // Check to see if the background process was cancelled.
            if (e.Cancelled || cancelUIRun) {
                DeleteTmpFile();
                HttpWebRequest cancelRequest = HttpWebRequest.Create(Strings.SERVERNAME + "PathRunnerRequestHandler.jsp?Action=StopWalkingClientTag&ClientTag=" + guid) as HttpWebRequest;
                HttpWebResponse cancelResponse = null;

                try {
                    cancelResponse = cancelRequest.GetResponse() as HttpWebResponse;
                }
                catch (WebException) {
                    // usually because we aren't connected which has already been reported on the original request, ignore
                }
                finally {
                    if (cancelResponse != null)
                        cancelResponse.Close();
                }

                guid = Guid.Empty;
                cancelUIRun = false;
            }
            else {
                guid = Guid.Empty;
                //Change html special characters
                htmlResults = WebUtility.HtmlDecode(htmlResults);
                htmlResults = htmlResults.Replace("<br>", "\n");
                htmlResults = htmlResults.TrimStart('\r','\n');
                if (htmlResults.Length > 0) {
                    if (htmlResults.Contains("SaveFile.jsp?filename=")) {
                        string[] splitLine = Regex.Split(htmlResults, "\r\n\n");
                        string tmpline = splitLine.Last();
                        int start = tmpline.LastIndexOf("SaveFile.jsp?filename=") + 21;
                        int end = tmpline.LastIndexOf("debug.zip") + 9;
                        if (end == -1)
                            end = tmpline.Length;
                        filename = tmpline.Substring(start + 1, end - start - 1);
                        GetSaveFile();
                        //Remove last line that has zip file information so it doesn't show up in output
                        string newhtml = string.Empty;
                        for (int x=0; x < splitLine.Length - 1; x++) {
                            if (x < splitLine.Length - 2)
                                newhtml += splitLine[x] + "\r\n\n";
                            else {
                                //don't add line return for last line, it causes a blank line in grid
                                newhtml += splitLine[x];
                            }
                        }
                        htmlResults = newhtml;
                    }
                    DeleteTmpFile();
                }
                else
                    htmlResults = "No results returned from PathMaker";
            }
            if (htmlResults.Length > 0) {
                ValidateResultsForm UISpecResults = new ValidateResultsForm(htmlResults, validationData.visioControl, Strings.UISPECRESULTSFORM);
                UISpecResults.Show();
            }
        }

        //Used for Abort
        protected void SaveFile(string fileName) {
            debugFileName = fileName;
        }

        protected void OnSaveFileCompleted(IAsyncResult result) {
            Action<string> invoker = (Action<string>)result.AsyncState;
            invoker.EndInvoke(result);
        }

        private void GetSaveFile() {
            SaveFileDialog saveFileDialog = null;

            using (var wc = new System.Net.WebClient()) {
                try {
                    saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "Save Debug Zip File";
                    saveFileDialog.Filter = "Zip file (*.zip) | *.zip";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.FileName = filename;
                    if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                        // call asnchronously
                        Action<string> invoker = new Action<string>(SaveFile);
                        invoker.BeginInvoke(saveFileDialog.FileName, OnSaveFileCompleted, invoker);
                    }

                    wc.UseDefaultCredentials = true;
                    wc.DownloadFileAsync(new Uri(Strings.SERVERNAME + Strings.SAVEJSPNAME + Strings.FILENAME + filename), debugFileName);
                }
                catch (WebException e) {
                    MessageBox.Show("Error downloading debug file: " + e.Message, Strings.UISPECVALIDATE);
                }
            }
        }

        private void DeleteTmpFile() {
            //Delete tmp file for web page use
            if (System.IO.File.Exists(validationData.xmlFileName)) {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try {
                    System.IO.File.Delete(validationData.xmlFileName);
                }
                catch (System.IO.IOException e) {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }

        delegate void CloseMethod(Form form);
        static private void CloseForm(Form form) {
            if (!form.IsDisposed) {
                if (form.InvokeRequired) {
                    CloseMethod method = new CloseMethod(CloseForm);
                    form.Invoke(method, new object[] { form });
                }
                else {
                    form.Close();
                }
            }
        }

        //Loop through forms to find UISpecResutlsForm
        static public void CloseAllForms() {
            // get array because collection changes as we close forms
            Form[] forms = OpenForms;

            // close every open form
            foreach (Form form in forms) {
                if (form.Name.ToString().Equals(Strings.UISPECRESULTSFORM))
                    CloseForm(form);
            }
        }

        //Find open forms
        public static Form[] OpenForms {
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

        internal void StopValidationInProgress() {
            cancelUIRun = true;

            if (IsBusy) {
                if (httpWebRequest != null) 
                    httpWebRequest.Abort();
                CancelAsync();
                Abort();
            }


        }
    }

}
