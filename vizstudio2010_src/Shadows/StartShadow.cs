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
                                                           Strings.DefaultSettingsStateSortOrder
                                                       };
        public static string[] DefaultSettingsValues = {
                                                           Strings.ModeSpeech,
                                                           Strings.ValueYes,
                                                           Strings.PromptIdFormatPartial,
                                                           Strings.ValueYes,
                                                           Strings.ValueYes,
                                                           Strings.ValueYes,
                                                           Strings.StateSortOrderAlphaNumerical
                                                       };

        public StartShadow(Shape shape)
            : base(shape) {
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
                }
            }
            return table;
        }

        /**
         * Returns the Default Setting associated with the name passed in
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
            DateTime date = new DateTime(1966, 9, 3);

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

            table = GetMaxHandling();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.ActionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.CountDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.GotoDateStamp);

            return date;
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
                    table.SetData(r, (int)TableColumns.PromptTypes.WordingDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
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
                    table.SetData(r, (int)TableColumns.ConfirmationPrompts.WordingDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    didOne = true;
                }
            }
            if (didOne)
                SetConfirmationPrompts(table);
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
                        table.SetData(r, (int)TableColumns.MaxHandling.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
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
                        table.SetData(r, (int)TableColumns.CommandTransitions.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
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
