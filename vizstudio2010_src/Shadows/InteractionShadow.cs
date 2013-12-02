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
                table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                Common.SetCellTable(shape, ShapeProperties.Interaction.DeveloperNotes, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.DeveloperNotes.Text).Equals(table.GetData(0, (int)TableColumns.DeveloperNotes.Text))) {
                table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                Common.SetCellTable(shape, ShapeProperties.Interaction.DeveloperNotes, table);
            }
        }

        internal void SetSpecialSettings(Table table) {
            Table tmp = GetSpecialSettings();

            if ((table.GetData(0, 0) == null || table.GetData(0, 0).Length == 0) && tmp.IsEmpty())
                return;

            if (tmp.IsEmpty()) {
                table.SetData(0, (int)TableColumns.SpecialSettings.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                Common.SetCellTable(shape, ShapeProperties.Interaction.SpecialSettings, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.SpecialSettings.Text).Equals(table.GetData(0, (int)TableColumns.SpecialSettings.Text))) {
                table.SetData(0, (int)TableColumns.SpecialSettings.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
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
                    table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.ConditionDateStamp, DateTime.Today.ToString(Strings.DateColumnFormatString));
                }
            }

            table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.Goto, shadow.GetUniqueId());
            table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.GotoDateStamp, DateTime.Today.ToString(Strings.DateColumnFormatString));
            table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.Confirm, Strings.ConfirmIfNecessary);
            table.SetData(table.GetNumRows() - 1, (int)TableColumns.CommandTransitions.ConfirmDateStamp, DateTime.Today.ToString(Strings.DateColumnFormatString));
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

            table = GetMaxHandling();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.ActionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.CountDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.MaxHandling.GotoDateStamp);

            date = Common.MaxDateWithDateColumn(date, GetSpecialSettings(), (int)TableColumns.SpecialSettings.TextDateStamp);

            table = GetDeveloperNotes();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.DeveloperNotes.TextDateStamp);
            
            return date;
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
                    table.SetData(r, (int)TableColumns.MaxHandling.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
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
                        table.SetData(r, (int)TableColumns.CommandTransitions.OptionDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                        SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
                    }
                    break;
                }
        }

        public override void OnConnectorChangeTarget(ConnectorShadow shadow) {
            Table table = GetCommandTransitions();

            for (int r = 0; r < table.GetNumRows(); r++)
                if (table.GetData(r, (int)TableColumns.CommandTransitions.Goto).Equals(shadow.GetUniqueId())) {
                    table.SetData(r, (int)TableColumns.CommandTransitions.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    SetCommandTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
                    break;
                }
        }
    }
}
