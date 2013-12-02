using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class PlayShadow : StateWithTransitionShadow {
        // this shape is made up of two others - here are their indices
        private const int StateIdShapeIndex = 0;
        private const int promptShapeIndex = 1;

        public PlayShadow(Shape shape)
            : base(shape) {
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick() {
            PlayForm form = new PlayForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        internal void SetSpecialSettings(Table table) {
            Table tmp = GetSpecialSettings();

            if ((table.GetData(0, 0) == null || table.GetData(0, 0).Length == 0) && tmp.IsEmpty())
                return;

            if (tmp.IsEmpty()) {
                table.SetData(0, (int)TableColumns.SpecialSettings.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                Common.SetCellTable(shape, ShapeProperties.Play.SpecialSettings, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.SpecialSettings.Text).Equals(table.GetData(0, (int)TableColumns.SpecialSettings.Text))) {
                table.SetData(0, (int)TableColumns.SpecialSettings.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                Common.SetCellTable(shape, ShapeProperties.Play.SpecialSettings, table);
            }
        }

        internal void SetPrompts(Table table) {
            // once they've edited these - no longer allow edits of the shape text for prompts
            Common.LockShapeText(shape.Shapes[promptShapeIndex]);

            bool foundOne = false;
            for (int row = 0; row < table.GetNumRows(); row++) {
                string prompt = table.GetData(row, (int)TableColumns.Prompts.Wording);
                if (prompt.Length > 0) {
                    Common.ForcedSetShapeText(shape.Shapes[promptShapeIndex], CommonShadow.PromptToShapeLabel(prompt));
                    foundOne = true;
                    break;
                }
            }
            if (!foundOne)
                Common.ForcedSetShapeText(shape.Shapes[promptShapeIndex], "");

            Common.SetCellTable(shape, ShapeProperties.Play.Prompts, table);
        }

        internal Table GetSpecialSettings() {
            return Common.GetCellTable(shape, ShapeProperties.Play.SpecialSettings);
        }

        internal Table GetPrompts() {
            Table table = Common.GetCellTable(shape, ShapeProperties.Play.Prompts);
            string promptText = shape.Shapes[promptShapeIndex].Text;

            if (table.IsEmpty() && promptText.Length > 0) {
                table = new Table(1, Enum.GetNames(typeof(TableColumns.Prompts)).Length);
                table.SetData(0, (int)TableColumns.Prompts.Wording, promptText);
                StartShadow shadowStart = PathMaker.LookupStartShadow();
                if (shadowStart != null) {
                    string promptIdFormat = shadowStart.GetDefaultSetting(Strings.DefaultSettingsPromptIDFormat);
                    if (promptIdFormat.Equals(Strings.PromptIdFormatFull) || promptIdFormat.Equals(Strings.PromptIdFormatPartial))
                        RedoPromptIds(0, promptIdFormat, table);
                }
            }
            return table;
        }

        internal override void ApplyPromptsFromRecordingList(PromptRecordingList recordingList) {
            Table table = GetPrompts();
            bool didOne = false;
            for (int r = 0; r < table.GetNumRows(); r++) {
                string wording = table.GetData(r, (int)TableColumns.Prompts.Wording);
                string id = table.GetData(r, (int)TableColumns.Prompts.Id);

                string newWording = recordingList.getWording(id);
                if (newWording != null && newWording != wording) {
                    table.SetData(r, (int)TableColumns.Prompts.Wording, newWording);
                    table.SetData(r, (int)TableColumns.Prompts.WordingDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    didOne = true;
                }
            }
            if (didOne)
                SetPrompts(table);
        }

        internal override void AddPromptsToRecordingList(PromptRecordingList recordingList, DateTime? onOrAfterDate) {
            Table table = GetPrompts();
            for (int r = 0; r < table.GetNumRows(); r++) {
                string wording = table.GetData(r, (int)TableColumns.Prompts.Wording);
                string id = table.GetData(r, (int)TableColumns.Prompts.Id);
                string wordingDateString = table.GetData(r, (int)TableColumns.Prompts.WordingDateStamp);
                string idDateString = table.GetData(r, (int)TableColumns.Prompts.IdDateStamp);

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
            DateTime date = base.GetLastChangeDate();

            date = Common.MaxDateWithDateColumn(date, GetSpecialSettings(), (int)TableColumns.SpecialSettings.TextDateStamp);
            Table table = GetPrompts();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.Prompts.ConditionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.Prompts.IdDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.Prompts.WordingDateStamp);
            return date;
        }

        private int RedoPromptIds(int startNumber, string promptIdFormat, Table table) {
            if (promptIdFormat.Equals(Strings.PromptIdFormatFull) || promptIdFormat.Equals(Strings.PromptIdFormatPartial))
            {
                string stateId = GetStateId();
                string statePrefix, stateNumber, stateName;

                DisectStateIdIntoParts(stateId, out statePrefix, out stateNumber, out stateName);

                int nextNum = 1;

                for (int row = 0; row < table.GetNumRows(); row++)
                {
                    string wording = table.GetData(row, (int)TableColumns.Prompts.Wording);
                    if (wording == null || wording.Length == 0 || wording.Trim().StartsWith(Strings.CalculatedPromptStartString) || wording.Trim().StartsWith(Strings.PromptTypeMacroStartString))
                        continue;

                    string newPromptId;
                    if (stateId != null)
                    {
                        if (promptIdFormat.Equals(Strings.PromptIdFormatFull))
                            newPromptId = stateId + Strings.PromptIdSeparationChar + Strings.DefaultPromptLetter + Strings.PromptIdSeparationChar + nextNum.ToString();
                        else
                            newPromptId = statePrefix + stateNumber + Strings.PromptIdSeparationChar + Strings.DefaultPromptLetter + Strings.PromptIdSeparationChar + nextNum.ToString();
                    }
                    else
                        newPromptId = Strings.DefaultPromptLetter.ToString() + Strings.PromptIdSeparationChar + nextNum;

                    if (!table.GetData(row, (int)TableColumns.Prompts.Id).Equals(newPromptId))
                    {
                        table.SetData(row, (int)TableColumns.Prompts.Id, newPromptId);
                        table.SetData(row, (int)TableColumns.Prompts.IdDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    }
                    nextNum++;
                }

                if (nextNum - 1 > 0)
                    SetPrompts(table);
                return nextNum - 1;
            }
            else if (promptIdFormat.Equals(Strings.PromptIdFormatNumeric))
            {
                int nextNum = startNumber;

                for (int row = 0; row < table.GetNumRows(); row++)
                {
                    string wording = table.GetData(row, (int)TableColumns.Prompts.Wording);
                    if (wording == null || wording.Length == 0 || wording.Trim().StartsWith(Strings.CalculatedPromptStartString) || wording.Trim().StartsWith(Strings.PromptTypeMacroStartString))
                        continue;

                    table.SetData(row, (int)TableColumns.Prompts.Id, nextNum.ToString());
                    table.SetData(row, (int)TableColumns.Prompts.IdDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    nextNum++;
                }

                if (nextNum - startNumber > 0)
                    SetPrompts(table);

                return nextNum - startNumber;
            }
            else
                return 0;
        }

        public override int RedoPromptIds(int startNumber, string promptIdFormat) {
            Table table = GetPrompts();
            return RedoPromptIds(startNumber, promptIdFormat, table);
        }
    }
}
