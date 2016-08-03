using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class StartShadow : Shadow {

        public static string[] DefaultSettingsLabels = {
                                                           Strings.DefaultSettingsMode,
                                                           Strings.DefaultSettingsBargeIn,
                                                           Strings.DefaultSettingsPromptIDFormat,
                                                           Strings.DefaultSettingsRetriesInTotalErrors,
                                                           Strings.DefaultSettingsTimeoutsInTotalErrors,
                                                           Strings.DefaultSettingsDisconfirmsInTotalErrors,
                                                           Strings.DefaultSettingsStateSortOrder,
                                                           Strings.DefaultSettingsConfirmMode,
                                                       };
        public static string[] DefaultSettingsValues = {
                                                           Strings.ModeSpeech,
                                                           Strings.ValueYes,
                                                           Strings.PromptIdFormatPartial,
                                                           Strings.ValueYes,
                                                           Strings.ValueYes,
                                                           Strings.ValueYes,
                                                           Strings.StateSortOrderAlphaNumerical,
                                                           Strings.DefaultConfirmModeValue,
                                                       };
        
        public StartShadow(Shape shape)
            : base(shape) {
        }

        
        public string GetDefaultConfirmMode() {
            string myString = PathMaker.LookupStartShadow().GetDefaultSetting(Strings.DefaultSettingsConfirmMode);
            //string tempDefaultConfirmValue = myShadow.GetDefaultSetting(Strings.DefaultSettingsConfirmMode);
            return myString;
        }


        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick() {
            StartForm form = new StartForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        internal void SetMaxHandling(Table table) {
            Common.SetCellTable(shape, ShapeProperties.Start.MaxHandling, table);
        }

        internal void SetConfirmationPrompts(Table table) {
            Common.SetCellTable(shape, ShapeProperties.Start.ConfirmationPrompts, table);
        }

        internal void SetCommandTransitions(Table table) {
            Common.SetCellTable(shape, ShapeProperties.Start.CommandTransitions, table);
        }

        internal void SetPromptTypes(Table table) {
            Common.SetCellTable(shape, ShapeProperties.Start.PromptTypes, table);
        }

        
        internal void SetInitialization(Table table) {
            Common.SetCellTable(shape, ShapeProperties.Start.Initialization, table);
        }

        internal void SetDefaultSettings(Table table) {
            Common.SetCellTable(shape, ShapeProperties.Start.DefaultSettings, table);
        }

        internal Table GetDefaultSettings() {
            Table table = Common.GetCellTable(shape, ShapeProperties.Start.DefaultSettings);
            if (table.IsEmpty()) {
                table = new Table(DefaultSettingsLabels.Length, Enum.GetNames(typeof(TableColumns.NameValuePairs)).Length);
                for (int row = 0; row < DefaultSettingsLabels.Length; row++) {
                    table.SetData(row, (int)TableColumns.NameValuePairs.Name, DefaultSettingsLabels[row]);
                    table.SetData(row, (int)TableColumns.NameValuePairs.Value, DefaultSettingsValues[row]);
                    table.SetData(row, (int)TableColumns.NameValuePairs.Notes, DefaultSettingsValues[row]);

                    table.SetData(row, (int)TableColumns.NameValuePairs.NameDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
                    table.SetData(row, (int)TableColumns.NameValuePairs.ValueDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
                    table.SetData(row, (int)TableColumns.NameValuePairs.NotesDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
                }
            }
            return table;
        }

        /**
         * Returns the Default Setting associated with the name passed in - JDK 
         */
        internal string GetDefaultSetting(string setting) {
            Table table = GetDefaultSettings();
            for (int row = 0; row < table.GetNumRows(); row++)
                if (table.GetData(row, (int)TableColumns.NameValuePairs.Name).Equals(setting))
                    return table.GetData(row, (int)TableColumns.NameValuePairs.Value);
            return String.Empty;
        }

        internal Table GetMaxHandling() {
            Table table = Common.GetCellTable(shape, ShapeProperties.Start.MaxHandling);
            if (table.IsEmpty()) {
                table = new Table(CommonShadow.MaxHandlingConditions.Length, Enum.GetNames(typeof(TableColumns.MaxHandling)).Length);
                for (int row = 0; row < CommonShadow.MaxHandlingConditions.Length; row++) {
                    table.SetData(row, (int)TableColumns.MaxHandling.Condition, CommonShadow.MaxHandlingConditions[row]);
                    table.SetData(row, (int)TableColumns.MaxHandling.Count, CommonShadow.MaxHandlingDefaultCounts[row]);
                    table.SetData(row, (int)TableColumns.MaxHandling.Goto, CommonShadow.MaxHandlingDefaultGotos[row]);
                }
            }
            return table;
        }

        internal override DateTime GetLastChangeDate() {
            DateTime date = new DateTime(1965, 4, 1);

            Table table = GetPromptTypes();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.PromptTypes.ConditionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.PromptTypes.IdDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.PromptTypes.TypeDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.PromptTypes.WordingDateStamp);

            table = GetCommandTransitions();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.CommandTransitions.ActionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.CommandTransitions.ConditionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.CommandTransitions.ConfirmDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.CommandTransitions.DTMFDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.CommandTransitions.GotoDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.CommandTransitions.OptionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.CommandTransitions.VocabDateStamp);

            table = GetConfirmationPrompts();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.ConfirmationPrompts.ConditionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.ConfirmationPrompts.IdDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.ConfirmationPrompts.OptionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.ConfirmationPrompts.WordingDateStamp);

            table = GetDefaultSettings();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.NameValuePairs.NameDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.NameValuePairs.ValueDateStamp);

            table = GetInitialization();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.NameValuePairs.NameDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.NameValuePairs.ValueDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.NameValuePairs.NotesDateStamp);

            table = GetMaxHandling();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.ActionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.CountDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.GotoDateStamp);

            return date;
        }

        //JDK added this to switch code to version stamp for highlighting
        internal override String GetLastChangeVersion()
        {
            String versionStamp = Strings.DefaultVersionStamp;//JDK was base.GetLastChangeVersion() - just setting a default string

            Table table = GetPromptTypes();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.PromptTypes.ConditionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.PromptTypes.IdDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.PromptTypes.TypeDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.PromptTypes.WordingDateStamp);
            SetPromptTypes(table);//incase any updates were made

            table = GetCommandTransitions();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.CommandTransitions.ActionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.CommandTransitions.ConditionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.CommandTransitions.ConfirmDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.CommandTransitions.DTMFDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.CommandTransitions.GotoDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.CommandTransitions.OptionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.CommandTransitions.VocabDateStamp);
            SetCommandTransitions(table);//incase any updates were made

            table = GetConfirmationPrompts();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.ConfirmationPrompts.ConditionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.ConfirmationPrompts.IdDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.ConfirmationPrompts.OptionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.ConfirmationPrompts.WordingDateStamp);
            SetConfirmationPrompts(table);//incase any updates were made

            table = GetDefaultSettings();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.NameValuePairs.NameDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.NameValuePairs.ValueDateStamp);
            SetDefaultSettings(table);//incase any updates were made

            table = GetInitialization();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.NameValuePairs.NameDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.NameValuePairs.ValueDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.NameValuePairs.NotesDateStamp);
            SetInitialization(table);//incase any updates were made

            table = GetMaxHandling();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.MaxHandling.ActionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.MaxHandling.CountDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.MaxHandling.GotoDateStamp);
            SetMaxHandling(table);//incase any updates were made

            return versionStamp;
        }

        public override string GetGotoName() {
            return Strings.StartTargetName;
        }
        
        internal Table GetConfirmationPrompts() {
            return Common.GetCellTable(shape, ShapeProperties.Start.ConfirmationPrompts);
        }

        internal Table GetCommandTransitions() {
            return Common.GetCellTable(shape, ShapeProperties.Start.CommandTransitions);
        }

        internal Table GetPromptTypes() {
            return Common.GetCellTable(shape, ShapeProperties.Start.PromptTypes);
        }

        internal Table GetInitialization() {
            return Common.GetCellTable(shape, ShapeProperties.Start.Initialization);
        }

        internal Shadow GetFirstStateGotoTarget() {
            List<Connect> connects = GetShapeOutputs();

            if (connects.Count == 0) {
                Common.ErrorMessage("Start shape does not have a first state connector");
                return null;
            }
            else if (connects.Count > 1) {
                Common.ErrorMessage("Start shape has more than one output connector");
                return null;
            }

            Shape connector = connects[0].FromSheet;
            return Common.GetGotoTargetFromData(connector.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
        }

        internal override void ApplyPromptsFromRecordingList(PromptRecordingList recordingList) {
            Table table = GetPromptTypes();
            bool didOne = false;
            for (int r = 0; r < table.GetNumRows(); r++) {
                string wording = table.GetData(r, (int)TableColumns.PromptTypes.Wording);
                string id = table.GetData(r, (int)TableColumns.PromptTypes.Id);

                string newWording = recordingList.getWording(id);
                if (newWording != null && newWording != wording) {
                    table.SetData(r, (int)TableColumns.PromptTypes.Wording, newWording);
                    //table.SetData(r, (int)TableColumns.PromptTypes.WordingDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    table.SetData(r, (int)TableColumns.PromptTypes.WordingDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                    table.SetData(r, (int)TableColumns.PromptTypes.IdDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                    
                    didOne = true;
                }
            }
            if (didOne)
                SetPromptTypes(table);

            didOne = false;
            table = GetConfirmationPrompts();
            for (int r = 0; r < table.GetNumRows(); r++) {
                string wording = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Wording);
                string id = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Id);

                string newWording = recordingList.getWording(id);
                if (newWording != null && newWording != wording) {
                    table.SetData(r, (int)TableColumns.ConfirmationPrompts.Wording, newWording);
                    //table.SetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    table.SetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                    
                    didOne = true;
                }
            }
            if (didOne)
                SetConfirmationPrompts(table);
        }

        internal override void AddPromptsToRecordingListVer(PromptRecordingList recordingList, String onOrAfterVersion)
        {
            Table table = GetPromptTypes();
            //double number;

            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string wording = table.GetData(r, (int)TableColumns.PromptTypes.Wording);
                string id = table.GetData(r, (int)TableColumns.PromptTypes.Id);
                string wordingDateString = table.GetData(r, (int)TableColumns.PromptTypes.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.PromptTypes.IdDateStamp);
                
                if (wordingDateString.Contains("/"))
                {
                    DateTime tempDTStamp;
                    if (DateTime.TryParse(onOrAfterVersion, out tempDTStamp))
                    {
                        string tempVersionStampFix = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(tempDTStamp);
                        wordingDateString = tempVersionStampFix;
                    }
                }

                if (idDateString.Contains("/"))
                {
                    DateTime tempDTStamp;
                    if (DateTime.TryParse(onOrAfterVersion, out tempDTStamp))
                    {
                        string tempVersionStampFix = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(tempDTStamp);
                        idDateString = tempVersionStampFix;
                    }
                }

                if (onOrAfterVersion != null) 
                {
                    if (!wordingDateString.Contains("/") && !idDateString.Contains("/"))
                    {
                        if (Common.ForcedStringVersionToDouble(wordingDateString) >= Common.ForcedStringVersionToDouble(onOrAfterVersion) ||
                            Common.ForcedStringVersionToDouble(idDateString) >= Common.ForcedStringVersionToDouble(onOrAfterVersion))
                        {
                            if (id != null && id.Length > 0)
                                recordingList.AddPromptRecording(id, wording);
                        }
                    }
                }
                else
                    if (id != null && id.Length > 0)
                        recordingList.AddPromptRecording(id, wording);
            }

            table = GetConfirmationPrompts();
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string wording = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Wording);
                string id = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Id);
                string wordingDateString = table.GetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.ConfirmationPrompts.IdDateStamp);
                if (idDateString == "" && wordingDateString != "")
                {
                    idDateString = wordingDateString;
                }
                else if (wordingDateString == "" && idDateString != "")
                {
                    wordingDateString = idDateString;
                }

                if (onOrAfterVersion != null)
                {
                    if (!wordingDateString.Contains("/") && !idDateString.Contains("/"))
                    {
                        if (Common.ForcedStringVersionToDouble(wordingDateString) >= Common.ForcedStringVersionToDouble(onOrAfterVersion) &&
                            Common.ForcedStringVersionToDouble(idDateString) >= Common.ForcedStringVersionToDouble(onOrAfterVersion))
                        {
                            if (id != null && id.Length > 0)
                                recordingList.AddPromptRecording(id, wording);
                        }
                    }
                }
                else
                    if (id != null && id.Length > 0)
                        recordingList.AddPromptRecording(id, wording);
            }
        }


        internal override void AddPromptsToRecordingList(PromptRecordingList recordingList, DateTime? onOrAfterDate) {
            Table table = GetPromptTypes();
            for (int r = 0; r < table.GetNumRows(); r++) {
                string wording = table.GetData(r, (int)TableColumns.PromptTypes.Wording);
                string id = table.GetData(r, (int)TableColumns.PromptTypes.Id);
                string wordingDateString = table.GetData(r, (int)TableColumns.PromptTypes.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.PromptTypes.IdDateStamp);

                if (onOrAfterDate != null) {
                    if (Common.ForcedStringToDate(wordingDateString) >= onOrAfterDate &&
                        Common.ForcedStringToDate(idDateString) >= onOrAfterDate) {
                        if (id != null && id.Length > 0)
                            recordingList.AddPromptRecording(id, wording);
                    }
                }
                else
                    if (id != null && id.Length > 0)
                        recordingList.AddPromptRecording(id, wording);
            }

            table = GetConfirmationPrompts();
            for (int r = 0; r < table.GetNumRows(); r++) {
                string wording = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Wording);
                string id = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Id);
                string wordingDateString = table.GetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.ConfirmationPrompts.IdDateStamp);

                if (onOrAfterDate != null) {
                    if (Common.ForcedStringToDate(wordingDateString) >= onOrAfterDate &&
                        Common.ForcedStringToDate(idDateString) >= onOrAfterDate) {
                        if (id != null && id.Length > 0)
                            recordingList.AddPromptRecording(id, wording);
                    }
                }
                else
                    if (id != null && id.Length > 0)
                        recordingList.AddPromptRecording(id, wording);
            }
        }

        internal override void RedoHiddenDateMarkers()
        {
            //use this to force hidden date fields to be version numbers
            //Common.WarningMessage("START SHAPE:  Starting to loop thru table records");
            Table table = GetPromptTypes();
            String lastVersionStamp = Strings.DefaultVersionStamp;
            String tempVersionStamp = "";
            Boolean labelsUpdated = false;

            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string wordingDateString = table.GetData(r, (int)TableColumns.PromptTypes.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.PromptTypes.IdDateStamp);


                if (wordingDateString != "" && wordingDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(wordingDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("START SHAPE: Wording Date was: " + wordingDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.PromptTypes.WordingDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (idDateString != "" && idDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(idDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("START SHAPE: ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.PromptTypes.IdDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            if (labelsUpdated)
                SetPromptTypes(table);//JDK Uncomment later

            labelsUpdated = false;
            table = GetConfirmationPrompts();
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string wordingDateString = table.GetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.ConfirmationPrompts.IdDateStamp);

                if (wordingDateString != "" && wordingDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(wordingDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("START SHAPE: Conf Wording Date was: " + wordingDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (idDateString != "" && idDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(idDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("START SHAPE: Conf ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.ConfirmationPrompts.IdDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            //Common.WarningMessage("START SHAPE:  Completed process for this Interaction state");
            if (labelsUpdated)
                SetConfirmationPrompts(table);

            labelsUpdated = false;
            table = GetInitialization();
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string nameDateString = table.GetData(r, (int)TableColumns.NameValuePairs.NameDateStamp);
                string notesDateString = table.GetData(r, (int)TableColumns.NameValuePairs.NotesDateStamp);
                string valueDateString = table.GetData(r, (int)TableColumns.NameValuePairs.ValueDateStamp);

                if (nameDateString != "" && nameDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(nameDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("START SHAPE: Conf Wording Date was: " + wordingDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.NameValuePairs.NameDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (notesDateString != "" && notesDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(notesDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("START SHAPE: Conf ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.NameValuePairs.NotesDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (valueDateString != "" && valueDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(valueDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("START SHAPE: Conf ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.NameValuePairs.ValueDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            //Common.WarningMessage("START SHAPE:  Completed process for this Interaction state");
            if (labelsUpdated)
                SetInitialization(table);

        }


        public override void OnConnectAddInput(Shadow shadow) {
            Common.ErrorMessage("Incorrectly adding input to Start");
            // get busy cursor without this
            shadow.SelectShape();
        }

        // returns true if something was changed
        internal override bool RemoveGotosThatDontUseConnectors(string uidBeingRemoved) {
            Table table = GetMaxHandling();
            bool changed = false;

            if (!table.IsEmpty()) {
                for (int r = 0; r < table.GetNumRows(); r++)
                    if (table.GetData(r, (int)TableColumns.MaxHandling.Goto).Equals(uidBeingRemoved)) {
                        table.SetData(r, (int)TableColumns.MaxHandling.Goto, Strings.HangUpKeyword);
                        //table.SetData(r, (int)TableColumns.MaxHandling.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                        table.SetData(r, (int)TableColumns.MaxHandling.GotoDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                        
                        changed = true;
                    }

                if (changed)
                    SetMaxHandling(table);
            }

            table = GetCommandTransitions();

            if (!table.IsEmpty()) {
                for (int r = 0; r < table.GetNumRows(); r++)
                    if (table.GetData(r, (int)TableColumns.CommandTransitions.Goto).Equals(uidBeingRemoved)) {
                        table.SetData(r, (int)TableColumns.CommandTransitions.Goto, Strings.HangUpKeyword);
                        //table.SetData(r, (int)TableColumns.CommandTransitions.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                        table.SetData(r, (int)TableColumns.CommandTransitions.GotoDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                        
                        changed = true;
                    }

                if (changed)
                    SetCommandTransitions(table);
            }

            return changed;
        }


        public override int RedoPromptIds(int startNumber, string promptIdFormat) {
            Table table = GetPromptTypes();
            int added = CommonShadow.RedoPromptTypeIds(ref table, null, startNumber, promptIdFormat);
            if (added > 0)
                SetPromptTypes(table);
            table = GetConfirmationPrompts();
            int confirmAdded = CommonShadow.RedoConfirmationPromptIds(ref table, null, startNumber + added, promptIdFormat);
            if (confirmAdded > 0)
                SetConfirmationPrompts(table);
            return added + confirmAdded;
        }

        public override void FixUIDReferencesAfterPaste(Dictionary<string, string> oldGUIDToNewGUIDMap) {
            Table table = GetCommandTransitions();

            for (int row = table.GetNumRows() - 1; row >= 0; row--) {
                string oldUID = table.GetData(row, (int)TableColumns.CommandTransitions.Goto);
                string newUID = CommonShadow.GetNewUIDAfterPaste(oldUID, oldGUIDToNewGUIDMap, false);
                if (newUID == null) {
                    Common.ErrorMessage("Transition in Start refers to state which can't be found, changing to hang up");
                    table.SetData(row, (int)TableColumns.CommandTransitions.Goto, Strings.HangUpKeyword);
                }
                else if (oldUID != newUID)
                    table.SetData(row, (int)TableColumns.CommandTransitions.Goto, newUID);
            }
            SetCommandTransitions(table);

            table = GetMaxHandling();
            for (int row = table.GetNumRows() - 1; row >= 0; row--) {
                string oldUID = table.GetData(row, (int)TableColumns.MaxHandling.Goto);
                if (oldUID == null || oldUID.Length == 0)
                    continue;
                string newUID = CommonShadow.GetNewUIDAfterPaste(oldUID, oldGUIDToNewGUIDMap, true);
                if (newUID == null) {
                    Common.ErrorMessage("MaxHandler for Start refers to state which can't be found, changing to hang up");
                    table.SetData(row, (int)TableColumns.MaxHandling.Goto, Strings.HangUpKeyword);
                }
                else if (oldUID != newUID)
                    table.SetData(row, (int)TableColumns.MaxHandling.Goto, newUID);
            }
            SetMaxHandling(table);
        }

    }
}
