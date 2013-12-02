using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class CommandTransitionRow {
        public CommandTransitionRow() { }

        public static BindingList<CommandTransitionRow> GetRowsFromTable(Table table) {
            BindingList<CommandTransitionRow> list = new BindingList<CommandTransitionRow>();
            for (int row = 0; row < table.GetNumRows(); row++) {
                CommandTransitionRow ct = new CommandTransitionRow();
                ct.Action = table.GetData(row, (int)TableColumns.CommandTransitions.Action);
                ct.Condition = table.GetData(row, (int)TableColumns.CommandTransitions.Condition);
                ct.Confirm = table.GetData(row, (int)TableColumns.CommandTransitions.Confirm);
                ct.DTMF = table.GetData(row, (int)TableColumns.CommandTransitions.DTMF);
                // stash the real goto data in a hidden column
                ct.GotoData_TreatAsDateStamp = table.GetData(row, (int)TableColumns.CommandTransitions.Goto);
                Shadow targetShadow = Common.GetGotoTargetFromData(ct.GotoData_TreatAsDateStamp);
                if (targetShadow == null)
                    ct.Goto = ct.GotoData_TreatAsDateStamp;
                else
                    ct.Goto = targetShadow.GetGotoName();
                ct.Option = table.GetData(row, (int)TableColumns.CommandTransitions.Option);
                ct.Vocab = table.GetData(row, (int)TableColumns.CommandTransitions.Vocab);
                ct.ActionDateStamp = table.GetData(row, (int)TableColumns.CommandTransitions.ActionDateStamp);
                ct.ConditionDateStamp = table.GetData(row, (int)TableColumns.CommandTransitions.ConditionDateStamp);
                ct.ConfirmDateStamp = table.GetData(row, (int)TableColumns.CommandTransitions.ConfirmDateStamp);
                ct.DTMFDateStamp = table.GetData(row, (int)TableColumns.CommandTransitions.DTMFDateStamp);
                ct.GotoDateStamp = table.GetData(row, (int)TableColumns.CommandTransitions.GotoDateStamp);
                ct.OptionDateStamp = table.GetData(row, (int)TableColumns.CommandTransitions.OptionDateStamp);
                ct.VocabDateStamp = table.GetData(row, (int)TableColumns.CommandTransitions.VocabDateStamp);
                list.Add(ct);
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<CommandTransitionRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.CommandTransitions)).Length);

            int row = 0;
            foreach (CommandTransitionRow ct in rows) {
                table.SetData(row, (int)TableColumns.CommandTransitions.Action, ct.Action);
                table.SetData(row, (int)TableColumns.CommandTransitions.Condition, ct.Condition);
                table.SetData(row, (int)TableColumns.CommandTransitions.Confirm, ct.Confirm);
                table.SetData(row, (int)TableColumns.CommandTransitions.DTMF, ct.DTMF);
                // leave the old actual goto data in place
                table.SetData(row, (int)TableColumns.CommandTransitions.Goto, ct.GotoData_TreatAsDateStamp);
                table.SetData(row, (int)TableColumns.CommandTransitions.Option, ct.Option);
                table.SetData(row, (int)TableColumns.CommandTransitions.Vocab, ct.Vocab);
                table.SetData(row, (int)TableColumns.CommandTransitions.ActionDateStamp, ct.ActionDateStamp);
                table.SetData(row, (int)TableColumns.CommandTransitions.ConditionDateStamp, ct.ConditionDateStamp);
                table.SetData(row, (int)TableColumns.CommandTransitions.ConfirmDateStamp, ct.ConfirmDateStamp);
                table.SetData(row, (int)TableColumns.CommandTransitions.DTMFDateStamp, ct.DTMFDateStamp);
                table.SetData(row, (int)TableColumns.CommandTransitions.GotoDateStamp, ct.GotoDateStamp);
                table.SetData(row, (int)TableColumns.CommandTransitions.OptionDateStamp, ct.OptionDateStamp);
                table.SetData(row, (int)TableColumns.CommandTransitions.VocabDateStamp, ct.VocabDateStamp);
                row++;
            }
            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Option { get; set; }
        public string Vocab { get; set; }
        public string DTMF { get; set; }
        public string Condition { get; set; }
        public string Action { get; set; }
        public string Goto { get; set; }
        public string Confirm { get; set; }
        public string OptionDateStamp { get; set; }
        public string VocabDateStamp { get; set; }
        public string DTMFDateStamp { get; set; }
        public string ConditionDateStamp { get; set; }
        public string ActionDateStamp { get; set; }
        public string GotoDateStamp { get; set; }
        public string ConfirmDateStamp { get; set; }
        public string GotoData_TreatAsDateStamp { get; set; }

        // these must match the property names above
        public const string OptionColumnName = "Option";
        public const string VocabColumnName = "Vocab";
        public const string DTMFColumnName = "DTMF";
        public const string ConditionColumnName = "Condition";
        public const string ActionColumnName = "Action";
        public const string GotoColumnName = "Goto";
        public const string ConfirmColumnName = "Confirm";

        // these must have the DateStampColumnSuffix - it's used to automatically hide these columns
        public const string OptionDateStampColumnName = OptionColumnName + Strings.DateStampColumnSuffix;
        public const string VocabDateStampColumnName = VocabColumnName + Strings.DateStampColumnSuffix;
        public const string DTMFDateStampColumnName = DTMFColumnName + Strings.DateStampColumnSuffix;
        public const string ConditionDateStampColumnName = ConditionColumnName + Strings.DateStampColumnSuffix;
        public const string ActionDateStampColumnName = ActionColumnName + Strings.DateStampColumnSuffix;
        public const string GotoDateStampColumnName = GotoColumnName + Strings.DateStampColumnSuffix;
        public const string ConfirmDateStampColumnName = ConfirmColumnName + Strings.DateStampColumnSuffix;
        public const string GotoData_TreatAsDateStampColumnName = "GotoData_TreatAs" + Strings.DateStampColumnSuffix;
    }
}
