using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class TransitionRow {
        public TransitionRow() { }

        public static BindingList<TransitionRow> GetRowsFromTable(Table table) {
            BindingList<TransitionRow> list = new BindingList<TransitionRow>();
            for (int row = 0; row < table.GetNumRows(); row++) {
                TransitionRow ct = new TransitionRow();
                ct.Action = table.GetData(row, (int)TableColumns.Transitions.Action);
                ct.Condition = table.GetData(row, (int)TableColumns.Transitions.Condition);
                // stash the real goto data in a hidden column
                ct.GotoData_TreatAsDateStamp = table.GetData(row, (int)TableColumns.Transitions.Goto);
                Shadow targetShadow = Common.GetGotoTargetFromData(ct.GotoData_TreatAsDateStamp);
                if (targetShadow == null)
                    ct.Goto = ct.GotoData_TreatAsDateStamp;
                else
                    ct.Goto = targetShadow.GetGotoName();
                ct.ActionDateStamp = table.GetData(row, (int)TableColumns.Transitions.ActionDateStamp);
                ct.ConditionDateStamp = table.GetData(row, (int)TableColumns.Transitions.ConditionDateStamp);
                ct.GotoDateStamp = table.GetData(row, (int)TableColumns.Transitions.GotoDateStamp);
                list.Add(ct);
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<TransitionRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.Transitions)).Length);

            int row = 0;
            foreach (TransitionRow ct in rows) {
                table.SetData(row, (int)TableColumns.Transitions.Action, ct.Action);
                table.SetData(row, (int)TableColumns.Transitions.Condition, ct.Condition);
                // leave the old actual goto data in place
                table.SetData(row, (int)TableColumns.Transitions.Goto, ct.GotoData_TreatAsDateStamp);
                table.SetData(row, (int)TableColumns.Transitions.ActionDateStamp, ct.ActionDateStamp);
                table.SetData(row, (int)TableColumns.Transitions.ConditionDateStamp, ct.ConditionDateStamp);
                table.SetData(row, (int)TableColumns.Transitions.GotoDateStamp, ct.GotoDateStamp);
                row++;
            }
            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Condition { get; set; }
        public string Action { get; set; }
        public string Goto { get; set; }
        public string ConditionDateStamp { get; set; }
        public string ActionDateStamp { get; set; }
        public string GotoDateStamp { get; set; }
        public string GotoData_TreatAsDateStamp { get; set; }

        // these must match the property names above
        public const string ConditionColumnName = "Condition";
        public const string ActionColumnName = "Action";
        public const string GotoColumnName = "Goto";

        // these must have the DateStampColumnSuffix - it's used to automatically hide these columns
        public const string ConditionDateStampColumnName = ConditionColumnName + Strings.DateStampColumnSuffix;
        public const string ActionDateStampColumnName = ActionColumnName + Strings.DateStampColumnSuffix;
        public const string GotoDateStampColumnName = GotoColumnName + Strings.DateStampColumnSuffix;
        public const string GotoData_TreatAsDateStampColumnName = "GotoData_TreatAs" + Strings.DateStampColumnSuffix;
    }
}
