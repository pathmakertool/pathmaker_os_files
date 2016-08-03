using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class InteractionShadow : StateShadow {
        // this shape is made up of two others - here are their indices
        private const int StateIdShapeIndex = 0;
        private const int promptShapeIndex = 1;

        public InteractionShadow(Shape shape)
            : base(shape) {
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick() {
            InteractionForm form = new InteractionForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        internal void SetDeveloperNotes(Table table) {
            Table tmp = GetDeveloperNotes();

            if ((table.GetData(0, 0) == null || table.GetData(0, 0).Length == 0) && tmp.IsEmpty())
                return;

            if (tmp.IsEmpty()) {
                //table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.Interaction.DeveloperNotes, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.DeveloperNotes.Text).Equals(table.GetData(0, (int)TableColumns.DeveloperNotes.Text))) {
                //table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.Interaction.DeveloperNotes, table);
            }
        }

        internal void SetDesignNotes(Table table)
        {
            Table tmp = GetDesignNotes();

            if ((table.GetData(0, 0) == null || table.GetData(0, 0).Length == 0) && tmp.IsEmpty())
                return;

            if (tmp.IsEmpty())
            {
                //table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.DesignNotes.TextDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.Interaction.DesignNotes, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.DesignNotes.Text).Equals(table.GetData(0, (int)TableColumns.DesignNotes.Text)))
            {
                //table.SetData(0, (int)TableColumns.DesignNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.DesignNotes.TextDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                Common.SetCellTable(shape, ShapeProperties.Interaction.DesignNotes, table);
            }
        }

        internal void SetSpecialSettings(Table table) {
            Table tmp = GetSpecialSettings();

            if ((table.GetData(0, 0) == null || table.GetData(0, 0).Length == 0) && tmp.IsEmpty())
                return;

            if (tmp.IsEmpty()) {
                //table.SetData(0, (int)TableColumns.SpecialSettings.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.SpecialSettings.TextDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.Interaction.SpecialSettings, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.SpecialSettings.Text).Equals(table.GetData(0, (int)TableColumns.SpecialSettings.Text))) {
                //table.SetData(0, (int)TableColumns.SpecialSettings.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.SpecialSettings.TextDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.Interaction.SpecialSettings, table);
            }
        }

        internal void SetMaxHandling(Table table) {
            Common.SetCellTable(shape, ShapeProperties.Interaction.MaxHandling, table);
        }

        internal void SetConfirmationPrompts(Table table) {
            Common.SetCellTable(shape, ShapeProperties.Interaction.ConfirmationPrompts, table);
        }

        /**
         * Because SetTransitions calls RemoveOutputsForDeletedTransitions, it can result in 
         * a shape delete.  When it's being called because of a connector delete, we can end
         * up with an error because we try to delete the same shape twice.  This avoids that.
         **/
        private void SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(Table table) {
            for (int r = 0; r < table.GetNumRows(); r++) {
                string uid = table.GetData(r, (int)TableColumns.CommandTransitions.Goto);
                ConnectorShadow shadow = PathMaker.LookupShadowByUID(uid) as ConnectorShadow;
                if (shadow != null)
                    shadow.SetLabelName(table.GetData(r, (int)TableColumns.CommandTransitions.Option));
            }

            Common.SetCellTable(shape, ShapeProperties.Interaction.CommandTransitions, table);
        }

        internal void SetCommandTransitions(Table table) {
            SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
            RemoveOutputsIfNotInTableColumn(table, (int)TableColumns.CommandTransitions.Goto);
        }

        internal void SetPromptTypes(Table table) {
            // once they've edited these - no longer allow edits of the shape text for prompts
            Common.LockShapeText(shape.Shapes[promptShapeIndex]);

            bool foundOne = false;
            for (int row = 0; row < table.GetNumRows(); row++) {
                string prompt = table.GetData(row, (int)TableColumns.PromptTypes.Wording);
                if (prompt.Length > 0) {
                    Common.ForcedSetShapeText(shape.Shapes[promptShapeIndex], CommonShadow.PromptToShapeLabel(prompt));
                    foundOne = true;
                    break;
                }
            }
            if (!foundOne)
                Common.ForcedSetShapeText(shape.Shapes[promptShapeIndex], "");

            Common.SetCellTable(shape, ShapeProperties.Interaction.PromptTypes, table);
        }

        internal Table GetDeveloperNotes() {
            return Common.GetCellTable(shape, ShapeProperties.Interaction.DeveloperNotes);
        }

        internal Table GetDesignNotes()
        {
            return Common.GetCellTable(shape, ShapeProperties.Interaction.DesignNotes);
        }

        internal Table GetSpecialSettings() {
            return Common.GetCellTable(shape, ShapeProperties.Interaction.SpecialSettings);
        }

        internal Table GetMaxHandling() {
            Table table = Common.GetCellTable(shape, ShapeProperties.Interaction.MaxHandling);
            if (table.IsEmpty()) {
                table = new Table(CommonShadow.MaxHandlingConditions.Length, Enum.GetNames(typeof(TableColumns.MaxHandling)).Length);
                for (int row = 0; row < CommonShadow.MaxHandlingConditions.Length; row++) {
                    table.SetData(row, (int)TableColumns.MaxHandling.Condition, CommonShadow.MaxHandlingConditions[row]);
                }
            }
            return table;
        }

        internal Table GetConfirmationPrompts() {
            return Common.GetCellTable(shape, ShapeProperties.Interaction.ConfirmationPrompts);
        }

        internal Table GetCommandTransitions() {
            return Common.GetCellTable(shape, ShapeProperties.Interaction.CommandTransitions);
        }

        internal Table GetPromptTypes() {
            Table table = Common.GetCellTable(shape, ShapeProperties.Interaction.PromptTypes);
            string promptText = shape.Shapes[promptShapeIndex].Text;

            if (table.IsEmpty() && promptText.Length > 0) {
                table = new Table(1, Enum.GetNames(typeof(TableColumns.PromptTypes)).Length);
                table.SetData(0, (int)TableColumns.PromptTypes.Wording, promptText);
                table.SetData(0, (int)TableColumns.PromptTypes.Type, Strings.DefaultPromptType);
                StartShadow shadowStart = PathMaker.LookupStartShadow();
                if (shadowStart != null) {
                    string promptIdFormat = shadowStart.GetDefaultSetting(Strings.DefaultSettingsPromptIDFormat);
                    if (promptIdFormat.Equals(Strings.PromptIdFormatFull) || promptIdFormat.Equals(Strings.PromptIdFormatPartial))
                        RedoPromptIds(0, promptIdFormat, table);
                }
            }
            return table;
        }

        public override void OnConnectAddOutput(Shadow shadow) {
            Table table = GetCommandTransitions();

            // make sure it's not already in there - this can happen with undo/redo
            for (int r = 0; r < table.GetNumRows(); r++)
                if (table.GetData(r, (int)TableColumns.CommandTransitions.Goto).Equals(shadow.GetUniqueId()))
                    return;

            if (table.IsEmpty())
                table = new Table(1, Enum.GetNames(typeof(TableColumns.CommandTransitions)).Length);
            else
                table.AddRow();

            ConnectorShadow connector = shadow as ConnectorShadow;
            if (connector != null) {
                string label = connector.GetLabelName();
                if (label.Length > 0) {
                    table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.Condition, CommonShadow.GetStringWithNewConnectorLabel("", label));
                    //table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.ConditionDateStamp, DateTime.Today.ToString(Strings.DateColumnFormatString));
                    table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.ConditionDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
                }
            }

            table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.Goto, shadow.GetUniqueId());
            //table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.GotoDateStamp, DateTime.Today.ToString(Strings.DateColumnFormatString));
            table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.GotoDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
            
            table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.Confirm, Strings.ConfirmIfNecessary);
            table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.ConfirmDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
            SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
        }


        public override void OnConnectDeleteOutput(Shadow shadow) {
            Table table = GetCommandTransitions();
            for (int r = 0; r < table.GetNumRows(); r++) {
                string data = table.GetData(r, (int)TableColumns.CommandTransitions.Goto);
                string uid = shadow.GetUniqueId();

                if (data.Equals(uid)) {
                    // cleanup the connector label
                    ConnectorShadow connectorShadow = PathMaker.LookupShadowByUID(uid) as ConnectorShadow;
                    if (connectorShadow != null)
                        connectorShadow.SetLabelName(String.Empty);

                    table.DeleteRow(r);
                    SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
                    // only one per link possible
                    return;
                }
            }
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
                    table.SetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
                    table.SetData(r, (int)TableColumns.ConfirmationPrompts.IdDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
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
                        if (Common.ForcedStringVersionToDouble(wordingDateString) >= 0 || Common.ForcedStringVersionToDouble(idDateString) >= 0)
                        {
                            if (Common.ForcedStringVersionToDouble(wordingDateString) >= Common.ForcedStringVersionToDouble(onOrAfterVersion) ||
                                Common.ForcedStringVersionToDouble(idDateString) >= Common.ForcedStringVersionToDouble(onOrAfterVersion))
                            {
                                if (id != null && id.Length > 0)
                                    recordingList.AddPromptRecording(id, wording);
                            }
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
                    if (Common.ForcedStringToDate(wordingDateString) >= onOrAfterDate ||
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
                    if (Common.ForcedStringToDate(wordingDateString) >= onOrAfterDate ||
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

        internal override void AddDesignNotesToList(DesignNotesList designNotesList)
        {
            string id = GetStateId();
            string wording = GetDesignNotes().ToString();

            designNotesList.AddDesignNoteContent(id, wording);
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

            table = GetMaxHandling();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.ActionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.CountDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.GotoDateStamp);

            date = Common.MaxDateWithDateColumn(date, GetSpecialSettings(), (int)TableColumns.SpecialSettings.TextDateStamp);

            table = GetDeveloperNotes();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.DeveloperNotes.TextDateStamp);
            
            return date;
        }

        //JDK added this to switch code to version stamp for highlighting
        internal override String GetLastChangeVersion()
        {
            String versionStamp = "0.0";//JDK was base.GetLastChangeVersion() - just setting a default string

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

            table = GetMaxHandling();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.MaxHandling.ActionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.MaxHandling.CountDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.MaxHandling.GotoDateStamp);
            SetMaxHandling(table);//incase any updates were made

            table = GetSpecialSettings();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.SpecialSettings.TextDateStamp);
            SetSpecialSettings(table);

            table = GetDeveloperNotes();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.DeveloperNotes.TextDateStamp);
            SetDeveloperNotes(table);

            return versionStamp;
        }

        internal override void RedoHiddenDateMarkers(StateShadow stateShadow)
        {
            //use this to force hidden date fields to be version numbers
            //Common.WarningMessage("INTERACTION SHADOW:  Starting to loop thru table records");
            Table table = GetPromptTypes();
            String lastVersionStamp = base.GetLastChangeVersion();
            String tempVersionStamp = "";
            Boolean labelsUpdated = false;

            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string wordingDateString = table.GetData(r, (int)TableColumns.PromptTypes.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.PromptTypes.IdDateStamp);
                string conditionDateString = table.GetData(r, (int)TableColumns.PromptTypes.ConditionDateStamp);
                string typeDateString = table.GetData(r, (int)TableColumns.PromptTypes.TypeDateStamp);

                if (wordingDateString.Contains("/") &&  !wordingDateString.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(wordingDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("INTERACTION: Wording Date was: " + wordingDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.PromptTypes.WordingDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (idDateString.Contains("/") && !idDateString.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(idDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("INTERACTION: ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.PromptTypes.IdDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (conditionDateString.Contains("/") && !conditionDateString.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(conditionDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("INTERACTION: ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.PromptTypes.ConditionDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (typeDateString.Contains("/") && !typeDateString.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(typeDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("INTERACTION: ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.PromptTypes.TypeDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            if (labelsUpdated)
                SetPromptTypes(table);  //JDK - UNCOMMENT THIS TO MAKE EDITS STICK

            labelsUpdated = false;
            table = GetConfirmationPrompts();
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string wordingDateString = table.GetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.ConfirmationPrompts.IdDateStamp);

                if (wordingDateString.Contains("/") && !wordingDateString.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(wordingDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //.WarningMessage("INTERACTION: Conf Wording Date was: " + wordingDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (idDateString.Contains("/") && !idDateString.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(idDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("INTERACTION: Conf ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.ConfirmationPrompts.IdDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            if (labelsUpdated)
                SetConfirmationPrompts(table);  //JDK - UNCOMMENT THIS TO MAKE EDITS STICK


            labelsUpdated = false;
            table = GetMaxHandling();
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string actionDateStamp = table.GetData(r, (int)TableColumns.MaxHandling.ActionDateStamp);
                string countDateString = table.GetData(r, (int)TableColumns.MaxHandling.CountDateStamp);
                string gotoDateString = table.GetData(r, (int)TableColumns.MaxHandling.GotoDateStamp);

                if (actionDateStamp.Contains("/") || actionDateStamp.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(actionDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //.WarningMessage("INTERACTION: Conf Wording Date was: " + wordingDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.MaxHandling.ActionDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (countDateString.Contains("/") || countDateString.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(countDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("INTERACTION: Conf ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.MaxHandling.CountDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (gotoDateString.Contains("/") || gotoDateString.Equals(""))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(gotoDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //Common.WarningMessage("INTERACTION: Conf ID Date was: " + idDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.MaxHandling.GotoDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            if (labelsUpdated)
                SetMaxHandling(table);
            
            labelsUpdated = false;
            table = GetCommandTransitions();
            
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string actionDateStamp = table.GetData(r, (int)TableColumns.CommandTransitions.ActionDateStamp);
                string conditionDateStamp = table.GetData(r, (int)TableColumns.CommandTransitions.ConditionDateStamp);
                string confirmDateStamp = table.GetData(r, (int)TableColumns.CommandTransitions.ConfirmDateStamp);
                string dtmfDateStamp = table.GetData(r, (int)TableColumns.CommandTransitions.DTMFDateStamp);
                string gotoDateStamp = table.GetData(r, (int)TableColumns.CommandTransitions.GotoDateStamp);
                string optionDateStamp = table.GetData(r, (int)TableColumns.CommandTransitions.OptionDateStamp);
                string vocabDateStamp = table.GetData(r, (int)TableColumns.CommandTransitions.VocabDateStamp);

                if (!actionDateStamp.Equals("") && actionDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(actionDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.CommandTransitions.ActionDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (!conditionDateStamp.Equals("") && conditionDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(conditionDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.CommandTransitions.ConditionDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (!confirmDateStamp.Equals("") && confirmDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(confirmDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.CommandTransitions.ConfirmDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (!dtmfDateStamp.Equals("") && dtmfDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(dtmfDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.CommandTransitions.DTMFDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (!gotoDateStamp.Equals("") && gotoDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(gotoDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.CommandTransitions.GotoDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (!optionDateStamp.Equals("") && optionDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(optionDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.CommandTransitions.OptionDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (!vocabDateStamp.Equals("") && vocabDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(vocabDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.CommandTransitions.VocabDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            if (labelsUpdated)
                SetCommandTransitions(table);

            labelsUpdated = false;
            table = GetDeveloperNotes();
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string textDateStamp = table.GetData(r, (int)TableColumns.DeveloperNotes.TextDateStamp);

                if (textDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(textDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //.WarningMessage("INTERACTION: Conf Wording Date was: " + wordingDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.DeveloperNotes.TextDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }  
            }
            if (labelsUpdated)
                SetDeveloperNotes(table);

            labelsUpdated = false;
            table = GetSpecialSettings();
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string textDateStamp = table.GetData(r, (int)TableColumns.SpecialSettings.TextDateStamp);

                if (textDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(textDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        //.WarningMessage("INTERACTION: Conf Wording Date was: " + wordingDateString + " and label update is: " + tempVersionStamp);
                        table.SetData(r, (int)TableColumns.SpecialSettings.TextDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            if (labelsUpdated)
                SetSpecialSettings(table);

        }

        // returns true if something was changed
        internal override bool RemoveGotosThatDontUseConnectors(string uidBeingRemoved) {
            Table table = GetMaxHandling();

            if (table.IsEmpty())
                return false;

            bool changed = false;

            for (int r = 0; r < table.GetNumRows(); r++)
                if (table.GetData(r, (int)TableColumns.MaxHandling.Goto).Equals(uidBeingRemoved)) {
                    table.SetData(r, (int)TableColumns.MaxHandling.Goto, Strings.HangUpKeyword);
                    //table.SetData(r, (int)TableColumns.MaxHandling.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    table.SetData(r, (int)TableColumns.MaxHandling.GotoDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
                    
                    changed = true;
                }

            if (changed) 
                SetMaxHandling(table);

            return changed;
        }

        private int RedoPromptIds(int startNumber, string promptIdFormat, Table table) {
            string stateId = GetStateId();
            int added = CommonShadow.RedoPromptTypeIds(ref table, stateId, startNumber, promptIdFormat);
            if (added > 0)
                SetPromptTypes(table);
            table = GetConfirmationPrompts();
            int confirmAdded = CommonShadow.RedoConfirmationPromptIds(ref table, stateId, startNumber + added, promptIdFormat);
            if (confirmAdded > 0)
                SetConfirmationPrompts(table);
            return added + confirmAdded;
        }

        public override int RedoPromptIds(int startNumber, string promptIdFormat) {
            Table table = GetPromptTypes();
            return RedoPromptIds(startNumber, promptIdFormat, table);
        }

        public override void FixUIDReferencesAfterPaste(Dictionary<string, string> oldGUIDToNewGUIDMap) {
            Table table = GetCommandTransitions();

            for (int row = table.GetNumRows() - 1; row >= 0; row--) {
                string oldUID = table.GetData(row, (int)TableColumns.CommandTransitions.Goto);
                string newUID = CommonShadow.GetNewUIDAfterPaste(oldUID, oldGUIDToNewGUIDMap, false);
                if (newUID == null)
                    table.DeleteRow(row);
                else if (oldUID != newUID)
                    table.SetData(row, (int)TableColumns.CommandTransitions.Goto, newUID);
            }
            SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(table);

            table = GetMaxHandling();
            for (int row = table.GetNumRows() - 1; row >= 0; row--) {
                string oldUID = table.GetData(row, (int)TableColumns.MaxHandling.Goto);
                if (oldUID == null || oldUID.Length == 0)
                    continue;
                string newUID = CommonShadow.GetNewUIDAfterPaste(oldUID, oldGUIDToNewGUIDMap, true);
                if (newUID == null) {
                    Common.ErrorMessage("MaxHandler for " + GetStateId() + " refers to state which can't be found, changing to hang up");
                    table.SetData(row, (int)TableColumns.MaxHandling.Goto, Strings.HangUpKeyword);
                }
                else if (oldUID != newUID)
                    table.SetData(row, (int)TableColumns.MaxHandling.Goto, newUID);
            }
            SetMaxHandling(table);
        }

        public override void OnBeforeShapeDelete() {
            base.OnBeforeShapeDelete();
            Table table = GetCommandTransitions();

            for (int r = 0; r < table.GetNumRows(); r++) {
                string uid = table.GetData(r, (int)TableColumns.CommandTransitions.Goto);
                ConnectorShadow shadow = PathMaker.LookupShadowByUID(uid) as ConnectorShadow;
                if (shadow != null)
                    shadow.SetLabelName(string.Empty);
            }
        }

        public override void OnConnectorLabelChange(ConnectorShadow shadow) {
            Table table = GetCommandTransitions();

            for (int r = 0; r < table.GetNumRows(); r++)
                if (table.GetData(r, (int)TableColumns.CommandTransitions.Goto).Equals(shadow.GetUniqueId())) {

                    string option = table.GetData(r, (int)TableColumns.CommandTransitions.Option);
                    string newOption = CommonShadow.GetStringWithNewConnectorLabel(option, shadow.GetLabelName());

                    if (!option.Equals(newOption)) {
                        table.SetData(r, (int)TableColumns.CommandTransitions.Option, newOption);
                        //table.SetData(r, (int)TableColumns.CommandTransitions.OptionDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                        table.SetData(r, (int)TableColumns.CommandTransitions.OptionDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                        SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
                    }
                    break;
                }
        }

        public override void OnConnectorChangeTarget(ConnectorShadow shadow) {
            Table table = GetCommandTransitions();

            for (int r = 0; r < table.GetNumRows(); r++)
                if (table.GetData(r, (int)TableColumns.CommandTransitions.Goto).Equals(shadow.GetUniqueId())) {
                    //table.SetData(r, (int)TableColumns.CommandTransitions.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    table.SetData(r, (int)TableColumns.CommandTransitions.GotoDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                    
                    SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
                    break;
                }
        }
    }
}
