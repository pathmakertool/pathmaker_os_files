using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class MaxHandlingRow {
        public MaxHandlingRow() { }

        public static BindingList<MaxHandlingRow> GetRowsFromTable(Table table) {
            BindingList<MaxHandlingRow> list = new BindingList<MaxHandlingRow>();
            for (int row = 0; row < table.GetNumRows(); row++) {
                MaxHandlingRow mh = new MaxHandlingRow();
                mh.Condition = table.GetData(row, (int)TableColumns.MaxHandling.Condition);
                mh.Count = table.GetData(row, (int)TableColumns.MaxHandling.Count);
                mh.Action = table.GetData(row, (int)TableColumns.MaxHandling.Action);
                mh.Goto = table.GetData(row, (int)TableColumns.MaxHandling.Goto);
                mh.CountDateStamp = table.GetData(row, (int)TableColumns.MaxHandling.CountDateStamp);
                mh.ActionDateStamp = table.GetData(row, (int)TableColumns.MaxHandling.ActionDateStamp);
                mh.GotoDateStamp = table.GetData(row, (int)TableColumns.MaxHandling.GotoDateStamp);
                list.Add(mh);
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<MaxHandlingRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.MaxHandling)).Length);

            int row = 0;
            foreach (MaxHandlingRow mh in rows) {
                table.SetData(row, (int)TableColumns.MaxHandling.Condition, mh.Condition);
                table.SetData(row, (int)TableColumns.MaxHandling.Count, mh.Count);
                table.SetData(row, (int)TableColumns.MaxHandling.Action, mh.Action);
                table.SetData(row, (int)TableColumns.MaxHandling.Goto, mh.Goto);
                table.SetData(row, (int)TableColumns.MaxHandling.CountDateStamp, mh.CountDateStamp);
                table.SetData(row, (int)TableColumns.MaxHandling.ActionDateStamp, mh.ActionDateStamp);
                table.SetData(row, (int)TableColumns.MaxHandling.GotoDateStamp, mh.GotoDateStamp);
                row++;
            }
            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Condition { get; set; }
        public string Count { get; set; }
        public string Action { get; set; }
        public string Goto { get; set; }
        public string CountDateStamp { get; set; }
        public string ActionDateStamp { get; set; }
        public string GotoDateStamp { get; set; }

        // these must match the property names above
        public const string ConditionColumnName = "Condition";
        public const string CountColumnName = "Count";
        public const string ActionColumnName = "Action";
        public const string GotoColumnName = "Goto";

        // these must have the DateStampColumnSuffix - it's used to automatically hide these columns
        public const string CountDateStampColumnName = "Count" + Strings.DateStampColumnSuffix;
        public const string ActionDateStampColumnName = "Action" + Strings.DateStampColumnSuffix;
        public const string GotoDateStampColumnName = "Goto" + Strings.DateStampColumnSuffix;
    }
}
