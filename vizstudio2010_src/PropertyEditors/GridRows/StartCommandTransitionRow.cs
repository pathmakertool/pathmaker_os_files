using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class StartCommandTransitionRow {
        public StartCommandTransitionRow() { }

        public static BindingList<StartCommandTransitionRow> GetRowsFromTable(Table table) {
            BindingList<StartCommandTransitionRow> list = new BindingList<StartCommandTransitionRow>();
            for (int row = 0; row < table.GetNumRows(); row++) {
                StartCommandTransitionRow ct = new StartCommandTransitionRow();
                ct.Action = table.GetData(row, (int)TableColumns.CommandTransitions.Action);
                ct.Condition = table.GetData(row, (int)TableColumns.CommandTransitions.Condition);
                ct.Confirm = table.GetData(row, (int)TableColumns.CommandTransitions.Confirm);
                ct.DTMF = table.GetData(row, (int)TableColumns.CommandTransitions.DTMF);
                ct.Goto = table.GetData(row, (int)TableColumns.CommandTransitions.Goto);
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

        public static Table GetTableFromRows(BindingList<StartCommandTransitionRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.CommandTransitions)).Length);

            int row = 0;
            foreach (StartCommandTransitionRow ct in rows) {
                table.SetData(row, (int)TableColumns.CommandTransitions.Action, ct.Action);
                table.SetData(row, (int)TableColumns.CommandTransitions.Condition, ct.Condition);
                table.SetData(row, (int)TableColumns.CommandTransitions.Confirm, ct.Confirm);
                table.SetData(row, (int)TableColumns.CommandTransitions.DTMF, ct.DTMF);
                table.SetData(row, (int)TableColumns.CommandTransitions.Goto, ct.Goto);
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
    }
}
