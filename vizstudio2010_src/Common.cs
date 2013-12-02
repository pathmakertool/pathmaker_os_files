using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Office.Interop.Visio;
using System.Drawing;
using System.IO;

namespace PathMaker {
    /**
     * Contains stuff common to all PathMaker files
     */
    class Common {
        private static ResourceManager theResourceManager = new ResourceManager(Strings.ResourceFileName, Assembly.GetExecutingAssembly());
        private static Dictionary<string, int> earliestPageReference;

        public static string GetResourceString(string resourceName) {
            string resourceValue = "";

            resourceValue = theResourceManager.GetString(resourceName, System.Globalization.CultureInfo.CurrentUICulture);
            return resourceValue;
        }

        public static int GetResourceInt(string resourceName) {
            string resourceValue = "";

            resourceValue = theResourceManager.GetString(resourceName, System.Globalization.CultureInfo.CurrentUICulture);

            int returnValue;
            bool result = Int32.TryParse(resourceValue, out returnValue);

            if (result)
                return returnValue;
            else
                return 0;
        }

        // sets the document schema version to the current one
        internal static void SetDocumentSchemaVersion(Document targetDocument) {
            targetDocument.Category = Strings.DocumentSchemaVersionPrefixRes + GetResourceInt(Strings.PathMakerSchemaVersionRes).ToString();
        }

        internal static int GetDocumentSchemaVersion(Document document) {
            int version = 0;
            string vString;

            if (document.Category.StartsWith(Strings.DocumentSchemaVersionPrefixRes))
                vString = document.Category.Substring(Strings.DocumentSchemaVersionPrefixRes.Length, document.Category.Length - Strings.DocumentSchemaVersionPrefixRes.Length);
            else
                return 0;

            if (vString.Length == 0)
                return 0;
            bool success = Int32.TryParse(vString, out version);
            if (success)
                return version;
            else
                return 0;
        }

        /// <summary>The FormulaStringToString method is used to format a Visio
        /// formula as a standard string.</summary>
        /// <remarks>The Visio Formula[U] methods return the formula as entered
        /// in the shapesheet.  Therefore, string formulas are wrapped in quote
        /// marks and an extra quote char is used when it is desired to place a
        /// quote character within a formula string.</remarks>
        /// <param name="formula">Value returned from a call to the Formula[U]
        /// methods of a cell</param>
        /// <returns>Converted string  (If an error occurs with the conversion,
        /// an empty string is returned.)</returns>
        internal static string FormulaStringToString(string formula) {
            const string OneQuote = "\"";
            const string TwoQuotes = "\"\"";

            string convertedFormula = "";

            try {
                // Initialize the converted formula from the value passed in.
                convertedFormula = formula;

                // Check if this formula value is a quoted string.
                // If it is, remove extra quote characters.
                if (convertedFormula.StartsWith(OneQuote) && convertedFormula.EndsWith(OneQuote)) {

                    // Remove the wrapping quote characters as well as any
                    // extra quote marks in the body of the string.
                    convertedFormula = convertedFormula.Substring(1, (convertedFormula.Length - 2));
                    convertedFormula = convertedFormula.Replace(TwoQuotes, OneQuote);
                }
            }

            catch (Exception err) {
                // Return a empty string if error occurs.
                convertedFormula = "";

                // Display the error.
                System.Diagnostics.Debug.WriteLine(err.Message);
                throw;
            }

            return convertedFormula;
        }

        /// <summary>This method converts the input string to a Visio string by
        /// replacing each double quotation mark (") with a pair of
        /// double quotation marks ("") and then adding double quotation
        /// marks around the entire string.</summary>
        /// <param name="inputValue">Input string that will be converted
        /// to Visio string</param>
        /// <returns>A converted Visio string that can be programmatically assigned 
        /// to a ShapeSheet cell is returned.  Note that the string cannot be directly 
        /// pasted into a ShapeSheet cell because it doesn't have an "=" at its 
        /// beginning.</returns>
        internal static string StringToFormulaForString(string inputValue) {
            string result = "";
            string quote = "\"";
            string quoteQuote = "\"\"";

            try {
                result = inputValue != null ? inputValue : String.Empty;

                // Replace all (") with ("").
                result = result.Replace(quote, quoteQuote);

                // Add ("") around the whole string.
                result = quote + result + quote;
            }
            catch (Exception err) {
                System.Diagnostics.Debug.WriteLine(err.Message);
                throw;
            }

            return result;
        }

        internal static void SetCellFormula(Shape shape, string cellName, string value) {
            string rowName = cellName;
            string sectionName = "";

            if (cellName.Contains(".")) {
                string[] tmp = cellName.Split('.');
                System.Diagnostics.Debug.Assert(tmp.Length == 2);
                sectionName = tmp[0];
                rowName = tmp[1];
            }

            if (shape.get_CellExists(cellName, (short)VisExistsFlags.visExistsAnywhere) == 0) {
                // we should only be adding rows to the Prop or User sections
                System.Diagnostics.Debug.Assert(sectionName.ToUpper().Equals("PROP") || sectionName.ToUpper().Equals("USER"));

                if (sectionName.ToUpper().Equals("USER")) {
                    if (shape.get_SectionExists((short)VisSectionIndices.visSectionUser, (short)VisExistsFlags.visExistsAnywhere) == 0)
                        shape.AddSection((short)VisSectionIndices.visSectionUser);
                    shape.AddNamedRow((short)VisSectionIndices.visSectionUser, rowName, (short)VisRowTags.visTagDefault);
                }
                else {
                    if (shape.get_SectionExists((short)VisSectionIndices.visSectionProp, (short)VisExistsFlags.visExistsAnywhere) == 0)
                        shape.AddSection((short)VisSectionIndices.visSectionProp);
                    shape.AddNamedRow((short)VisSectionIndices.visSectionProp, rowName, (short)VisRowTags.visTagDefault);
                }
            }
            shape.get_Cells(cellName).Formula = value;
        }

        internal static void SetCellString(Shape shape, string cellName, string value) {
            SetCellFormula(shape, cellName, StringToFormulaForString(value));
        }

        internal static string GetCellFormula(Shape shape, string cellName) {
            if (shape.get_CellExists(cellName, (short)VisExistsFlags.visExistsAnywhere) != 0) {
                Cell cell = shape.get_Cells(cellName);
                return cell.Formula;
            }
            else
                return "";
        }

        internal static string GetCellString(Shape shape, string cellName) {
            return FormulaStringToString(GetCellFormula(shape, cellName));
        }

        // if a shape doesn't have a type, this will return ShapeTypes.None
        // most times you should be using the ShapeShadow for this info
        internal static ShapeTypes GetShapeType(Shape shape) {
            string tmp = GetCellString(shape, ShapeProperties.ShapeType);
            if (tmp.Length == 0)
                return ShapeTypes.None;
            else
                return (ShapeTypes)int.Parse(tmp);
        }

        /// <summary>The GetShapeFromArguments method returns a reference to
        /// a shape given the command line arguments.</summary>
        /// <param name="visioApplication">The Visio application.</param>
        /// <param name="arguments">The command line arguments string containing:
        ///  /doc=id /page=id /shape=sheet.id.</param>
        /// <returns>The Visio shape or null.</returns>
        public static Shape GetShapeFromArguments(
            Microsoft.Office.Interop.Visio.Application visioApplication, string arguments) {
            const char equal = '=';
            const char argumentDelimiter = '/';

            // Standard Visio add-on command line arguments.
            const string commandLineArgumentDoc = "doc";
            const string commandLineArgumentPage = "page";
            const string commandLineArgumentShape = "shape";

            int index;
            int docId = -1;
            int pageId = -1;
            string shapeId = "";
            string[] contextParts;
            string contextPart;
            string[] argumentParts;
            Document document = null;
            Page page = null;
            Shape targetShape = null;

            if (visioApplication == null || arguments == null)
                return null;

            // Parse the command line arguments.
            contextParts = arguments.Trim().Split(argumentDelimiter);

            for (index = contextParts.GetLowerBound(0); index <= contextParts.GetUpperBound(0); index++) {
                contextPart = contextParts[index].Trim();

                if (contextPart.Length > 0) {
                    // Separate the parameter from the parameter value.
                    argumentParts = contextPart.Split(equal);

                    if (argumentParts.GetUpperBound(0) == 1)
                        // Get the doc, page, and shape argument values.
                        if (commandLineArgumentDoc.Equals(argumentParts[0]))
                            docId = Convert.ToInt16(argumentParts[1], System.Globalization.CultureInfo.InvariantCulture);
                        else if (commandLineArgumentPage.Equals(argumentParts[0]))
                            pageId = Convert.ToInt16(argumentParts[1], System.Globalization.CultureInfo.InvariantCulture);
                        else if (commandLineArgumentShape.Equals(argumentParts[0]))
                            shapeId = argumentParts[1];
                }
            }

            // If the command line arguments contains document, page, and shape
            // then look up the shape.
            if ((docId > 0) && (pageId > 0) && (shapeId.Length > 0)) {
                document = visioApplication.Documents[docId];
                page = document.Pages[pageId];
                targetShape = page.Shapes[shapeId];
            }

            return targetShape;
        }

        // Reads a cell that's a table and creates a table from it
        // if the cell is empty, you'll still get a table - just use
        // table.IsEmpty() to tell if there's anything there.
        public static Table GetCellTable(Shape shape, string cellName) {
            string tmp = GetCellString(shape, cellName);
            return new Table(tmp);
        }

        public static void SetCellTable(Shape shape, string cellName, Table table) {
            SetCellString(shape, cellName, table.ToString());
        }

        // Creates a shadow from a shape.  Should only be called from PathMaker
        // event handlers when things are loaded, added, etc.
        public static Shadow MakeShapeShadow(Shape shape) {
            ShapeTypes shapeType = Common.GetShapeType(shape);
            Shadow shadow = null;

            switch (shapeType) {
                case ShapeTypes.CallSubDialog:
                    shadow = new CallSubDialogShadow(shape);
                    break;
                case ShapeTypes.ChangeLog:
                    shadow = new ChangeLogShadow(shape);
                    break;
                case ShapeTypes.Comment:
                    shadow = new IgnoredShadow(shape);
                    break;
                case ShapeTypes.Connector:
                    shadow = new ConnectorShadow(shape);
                    break;
                case ShapeTypes.Data:
                    shadow = new DataShadow(shape);
                    break;
                case ShapeTypes.Decision:
                    shadow = new DecisionShadow(shape);
                    break;
                case ShapeTypes.DocTitle:
                    shadow = new DocTitleShadow(shape);
                    break;
                case ShapeTypes.HangUp:
                    shadow = new HangUpShadow(shape);
                    break;
                case ShapeTypes.Interaction:
                    shadow = new InteractionShadow(shape);
                    break;
                case ShapeTypes.None:
                    break;
                case ShapeTypes.OffPageRef:
                    shadow = new OffPageRefShadow(shape);
                    break;
                case ShapeTypes.OnPageRefIn:
                    shadow = new OnPageRefInShadow(shape);
                    break;
                case ShapeTypes.OnPageRefOut:
                    shadow = new OnPageRefOutShadow(shape);
                    break;
                case ShapeTypes.Page:
                    break;
                case ShapeTypes.Placeholder:
                    shadow = new IgnoredShadow(shape);
                    break;
                case ShapeTypes.Play:
                    shadow = new PlayShadow(shape);
                    break;
                case ShapeTypes.Return:
                    shadow = new ReturnShadow(shape);
                    break;
                case ShapeTypes.Start:
                    shadow = new StartShadow(shape);
                    break;
                case ShapeTypes.SubDialog:
                    shadow = new SubDialogShadow(shape);
                    break;
                case ShapeTypes.Transfer:
                    shadow = new TransferShadow(shape);
                    break;
            }
            return shadow;
        }

        internal static Shadow GetGotoTargetFromData(string gotoData) {
            if (gotoData.Length == 0)
                return null;

            Guid uid;
            if (!Guid.TryParse(gotoData, out uid))
                return null;
            Shadow shadow = PathMaker.LookupShadowByUID(gotoData);
            if (shadow != null)
                return shadow.GetDestinationTarget();
            else 
                return null;
        }

        // Overrides the lock (which may or may not be set) and sets the shape text
        internal static void ForcedSetShapeText(Shape shape, string text) {
            Cell cell = shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                                    (short)VisRowIndices.visRowLock,
                                    (short)VisCellIndices.visLockTextEdit);
            string oldSetting = cell.FormulaU;
            cell.FormulaU = "0";
            shape.Text = text;
            cell.FormulaU = oldSetting;
        }

        // Central location for errors - easily changeable to go to logging if desired
        internal static void ErrorMessage(string msg) {
            bool useMessageBoxes = true;

            if (useMessageBoxes)
                System.Windows.Forms.MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                System.Diagnostics.Debug.WriteLine(msg);
        }

        internal static void LockShapeText(Shape shape) {
            shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                                    (short)VisRowIndices.visRowLock,
                                    (short)VisCellIndices.visLockTextEdit).FormulaU = "1";
        }

        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format) {
            using (MemoryStream ms = new MemoryStream()) {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static Image Base64ToImage(string base64String) {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static System.Drawing.Color? GetHighlightColor(string dateString) {
            DateTime date;
            if (DateTime.TryParse(dateString, out date))
                return GetHighlightColor(date);
            else
                return null;
        }

        /**
         * Returns the highlight color to be used given a change date. 
         * If no highlight is needed, it will return the DefaultHighlightColor
         */
        public static System.Drawing.Color? GetHighlightColor(DateTime date) {
            ChangeLogShadow shadow = PathMaker.LookupChangeLogShadow();
            if (shadow == null)
                return null;

            string color = shadow.GetColorStringForChange(date);

            return ConvertColorStringToColor(color);
        }

        public static System.Drawing.Color? ConvertColorStringToColor(string color) {
            if (color.Equals(Strings.HighlightColorNone))
                return null;
            else if (color.Equals(Strings.HighlightColorAqua))
                return System.Drawing.Color.Aqua;
            else if (color.Equals(Strings.HighlightColorBlue))
                return System.Drawing.Color.Blue;
            else if (color.Equals(Strings.HighlightColorGreen))
                return System.Drawing.Color.LightGreen;
            else if (color.Equals(Strings.HighlightColorPink))
                return System.Drawing.Color.Pink;
            else if (color.Equals(Strings.HighlightColorYellow))
                return System.Drawing.Color.Yellow;
            else
                return null;
        }


        internal static int GetConditionLevel(string condition) {
            int count = 0;
            int index = 0;
            while ((index = condition.IndexOf(Strings.IndentCharacterString, index)) != -1) {
                count++;
                index++;
            }
            return count;
        }

        internal static PromptRecordingList GetPromptRecordingList(DateTime? onOrAfterDate) {
            PromptRecordingList recordingList = new PromptRecordingList();

            foreach (Shadow shadow in PathMaker.LookupAllShadows()) {
                shadow.AddPromptsToRecordingList(recordingList, onOrAfterDate);
            }
            return recordingList;
        }

        // will make up a very early date if one is not provided
        internal static DateTime ForcedStringToDate(string dateString) {
            DateTime date;
            if (dateString != null && DateTime.TryParse(dateString, out date))
                return date;
            else
                return new DateTime(1966, 9, 3);
        }

        internal static void ApplyPromptRecordingList(PromptRecordingList recordingList) {
            foreach (Shadow shadow in PathMaker.LookupAllShadows()) {
                shadow.ApplyPromptsFromRecordingList(recordingList);
            }           
        }

        internal static DateTime MaxDateWithDateColumn(DateTime date, Table table, int column) {
            DateTime maxDate = date;
            for (int r = 0; r < table.GetNumRows(); r++) {
                DateTime changeDate = ForcedStringToDate(table.GetData(r, column));
                if (changeDate > maxDate)
                    maxDate = changeDate;
            }
            return maxDate;
        }

        internal static string MakeLabelName(string label) {
            string pretty = label;

            bool gotSquareBrackets = false;
            if (pretty.Length > 0) {
                int firstBracket = pretty.IndexOf(Strings.LabelStartBracket);
                int lastBracket = pretty.IndexOf(Strings.LabelEndBracket);
                if (firstBracket >= 0 && lastBracket >= 0 && lastBracket > firstBracket) {
                    pretty = pretty.Substring(firstBracket + 1, lastBracket - (firstBracket + 1));
                    gotSquareBrackets = true;
                }
            }
            if (!gotSquareBrackets && pretty.Contains(Strings.DynamicOptionKeyword))
                pretty = pretty.Remove(pretty.IndexOf(Strings.DynamicOptionKeyword), Strings.DynamicOptionKeyword.Length);

            pretty = pretty.Trim();
            return pretty;
        }

        public static void RedoAllPromptIds() {
            List<Shadow> shadowList = PathMaker.LookupAllShadows();
            int count = 100;
            StartShadow startShadow = PathMaker.LookupStartShadow();
            if (startShadow == null) {
                Common.ErrorMessage("No start shape available to determine prompt id format");
                return;
            }

            string promptIdFormat = startShadow.GetDefaultSetting(Strings.DefaultSettingsPromptIDFormat);
            startShadow.RedoPromptIds(count, promptIdFormat);

            foreach (Shadow s in shadowList) {
                StateShadow stateShadow = s as StateShadow;
                if (stateShadow != null)
                    count += stateShadow.RedoPromptIds(count, promptIdFormat);
            }
        }

        public static string StripBracketLabels(string input) {
            string pretty = input;

            if (pretty.Length > 0) {
                int firstBracket = pretty.IndexOf(Strings.LabelStartBracket);
                int lastBracket = pretty.IndexOf(Strings.LabelEndBracket);
                if (firstBracket >= 0 && lastBracket >= 0 && lastBracket > firstBracket) {
                    pretty = pretty.Substring(0, firstBracket) + pretty.Substring(lastBracket + 1);
                }
            }
            pretty = pretty.Trim();
            return pretty;
        }

        public static string StripExtensionFileName(string targetFileName) {
            int removeLength = 0;
            for (int i = targetFileName.Length - 1; i >= 0; i--)
            {
                char let = targetFileName[i];
                if (let == '.') 
                {
                    // add one to include the '.'
                    removeLength++;
                    break;
                }
                else
                {
                    removeLength++;
                }
            }
            if (removeLength > 0)
            {
                return targetFileName.Substring(0, targetFileName.Length - removeLength);
            }
            return targetFileName;
        }

        public static void FixConnectorTextControl(Shape shape) {
            if (Common.GetShapeType(shape) == ShapeTypes.Connector) {
                string pinX = Common.GetCellFormula(shape, "TxtPinX");
                // if the current TxtPinX already is updating another cell via setatref, we don't need to
                // do anything
                if (!pinX.Contains("SETATREF")) {
                    shape.AddNamedRow((short)VisSectionIndices.visSectionControls, "TextPosition", 0);
                    // get old values and reuse them if we already have text
                    string pinY = Common.GetCellFormula(shape, "TxtPinY");
                    if (shape.Text.Length > 0) {
                        shape.get_CellsU("Controls.TextPosition.X").Formula = pinX;
                        shape.get_CellsU("Controls.TextPosition.Y").Formula = pinY;
                    }
                    else {
                        shape.get_CellsU("Controls.TextPosition.Y").FormulaU = "Height*0.5";
                        shape.get_CellsU("Controls.TextPosition.X").FormulaU = "Width*0.5";
                    }
                    shape.get_CellsU("Controls.TextPosition.XDyn").FormulaU = "Controls.TextPosition.X";
                    shape.get_CellsU("Controls.TextPosition.YDyn").FormulaU = "Controls.TextPosition.Y";
                    shape.get_CellsU("Controls.TextPosition.XCon").FormulaU = "IF(OR(STRSAME(SHAPETEXT(TheText),\"\"),HideText),5,0)";
                    shape.get_CellsU("Controls.TextPosition.CanGlue").FormulaU = "FALSE";
                    shape.get_CellsU("Controls.TextPosition.Prompt").FormulaU = "\"Reposition Text\"";
                    shape.get_CellsU("TxtPinX").FormulaU = "SETATREF(Controls.TextPosition)";
                    shape.get_CellsU("TxtPinY").FormulaU = "SETATREF(Controls.TextPosition.Y)";
                }
            }
        }

        public static int StateIdShadowSorterAlphaNumerical(Shadow a, Shadow b) {
            if (a == b)
                return 0;

            StateShadow stateA = a as StateShadow;
            StateShadow stateB = b as StateShadow;
            if (stateA == null)
                return 1;
            if (stateB == null)
                return -1;

            return stateA.GetStateId().CompareTo(stateB.GetStateId());
        }

        public static int StateIdShadowSorterNumericalAlpha(Shadow a, Shadow b) {
            if (a == b)
                return 0;

            StateShadow stateA = a as StateShadow;
            StateShadow stateB = b as StateShadow;
            if (stateA == null)
                return 1;
            if (stateB == null)
                return -1;

            string stateAPrefix, stateANumber, stateAName;
            string stateBPrefix, stateBNumber, stateBName;

            StateShadow.DisectStateIdIntoParts(stateA.GetStateId(), out stateAPrefix, out stateANumber, out stateAName);
            StateShadow.DisectStateIdIntoParts(stateB.GetStateId(), out stateBPrefix, out stateBNumber, out stateBName);

            int result = stateANumber.CompareTo(stateBNumber);

            if (result == 0)
                return stateAPrefix.CompareTo(stateBPrefix);
            else
                return result;
        }

        private static int StateIdShadowSorterVisioHeuristicHelper(Shadow a, Shadow b) {
            if (a == b)
                return 0;

            StateShadow stateA = a as StateShadow;
            StateShadow stateB = b as StateShadow;
            if (stateA == null)
                return 1;
            if (stateB == null)
                return -1;

            string stateAPrefix, stateANumber, stateAName;
            string stateBPrefix, stateBNumber, stateBName;

            StateShadow.DisectStateIdIntoParts(stateA.GetStateId(), out stateAPrefix, out stateANumber, out stateAName);
            StateShadow.DisectStateIdIntoParts(stateB.GetStateId(), out stateBPrefix, out stateBNumber, out stateBName);

            int earliestPageA = earliestPageReference[stateAPrefix];
            int earliestPageB = earliestPageReference[stateBPrefix];

            if (earliestPageA != earliestPageB)
                return earliestPageA - earliestPageB;

            return stateA.GetStateId().CompareTo(stateB.GetStateId());
        }
        
        public static void StateIdShadowSorterVisioHeuristic(List<Shadow> shadowList, Document doc, StartShadow startShadow) {
            // group by alpha prefix, then by number within that
            // then sort groups by first visio page reference
            // if both on same page, startPrefix is first, the sort alphanumerically

            string firstPrefix = String.Empty;
            Shadow firstShadow = startShadow.GetFirstStateGotoTarget();
            if (firstShadow != null) {
                StateShadow firstState = firstShadow as StateShadow;
                string firstNumber, firstName;
                StateShadow.DisectStateIdIntoParts(firstState.GetStateId(), out firstPrefix, out firstNumber, out firstName);
            }

            // make a list of first page references of each alpha prefix
            earliestPageReference = new Dictionary<string, int>();
            foreach (Shadow s in shadowList) {
                StateShadow sState = s as StateShadow;
                if (sState == null)
                    continue;
                int pageNumber = s.GetPageNumber();
                string statePrefix, stateNumber, stateName;
                StateShadow.DisectStateIdIntoParts(sState.GetStateId(), out statePrefix, out stateNumber, out stateName);

                // always make this highest priority
                if (statePrefix.Equals(firstPrefix))
                    pageNumber = -1;

                int earliest;
                if (earliestPageReference.TryGetValue(statePrefix, out earliest)) {
                    if (pageNumber < earliest)
                        earliestPageReference[statePrefix] = pageNumber;
                }
                else {
                    earliestPageReference.Add(statePrefix, pageNumber);
                }
            }

            shadowList.Sort(StateIdShadowSorterVisioHeuristicHelper);
        }
    }
}