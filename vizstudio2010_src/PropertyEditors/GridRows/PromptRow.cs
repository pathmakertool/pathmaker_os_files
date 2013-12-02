using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class PromptRow {
        public PromptRow() { }

        public static BindingList<PromptRow> GetRowsFromTable(Table table) {
            BindingList<PromptRow> list = new BindingList<PromptRow>();
            for (int row = 0; row < table.GetNumRows(); row++) {
                PromptRow pt = new PromptRow();
                pt.Condition = table.GetData(row, (int)TableColumns.Prompts.Condition);
                pt.Wording = table.GetData(row, (int)TableColumns.Prompts.Wording);
                pt.Id = table.GetData(row, (int)TableColumns.Prompts.Id);
                pt.ConditionDateStamp = table.GetData(row, (int)TableColumns.Prompts.ConditionDateStamp);
                pt.WordingDateStamp = table.GetData(row, (int)TableColumns.Prompts.WordingDateStamp);
                pt.IdDateStamp = table.GetData(row, (int)TableColumns.Prompts.IdDateStamp);
                list.Add(pt);
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<PromptRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.Prompts)).Length);

            int row = 0;
            foreach (PromptRow pt in rows) {
                table.SetData(row, (int)TableColumns.Prompts.Condition, pt.Condition);
                table.SetData(row, (int)TableColumns.Prompts.Wording, pt.Wording);
                table.SetData(row, (int)TableColumns.Prompts.Id, pt.Id);
                table.SetData(row, (int)TableColumns.Prompts.ConditionDateStamp, pt.ConditionDateStamp);
                table.SetData(row, (int)TableColumns.Prompts.WordingDateStamp, pt.WordingDateStamp);
                table.SetData(row, (int)TableColumns.Prompts.IdDateStamp, pt.IdDateStamp);
                row++;
            }

            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Condition { get; set; }
        public string Wording { get; set; }
        public string Id { get; set; }
        public string ConditionDateStamp { get; set; }
        public string WordingDateStamp { get; set; }
        public string IdDateStamp { get; set; }

        // these must match the property names above
        public const string ConditionColumnName = "Condition";
        public const string WordingColumnName = "Wording";
        public const string IdColumnName = "Id";

        // these must have the DateStampColumnSuffix - it's used to automatically hide these columns
        public const string ConditionDateStampColumnName = ConditionColumnName + Strings.DateStampColumnSuffix;
        public const string WordingDateStampColumnName = WordingColumnName + Strings.DateStampColumnSuffix;
        public const string IdDateStampColumnName = IdColumnName + Strings.DateStampColumnSuffix;
    }
}
