using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class PromptTypeRow {
        public PromptTypeRow() { }

        public static BindingList<PromptTypeRow> GetRowsFromTable(Table table) {
            BindingList<PromptTypeRow> list = new BindingList<PromptTypeRow>();
            for (int row = 0; row < table.GetNumRows(); row++) {
                PromptTypeRow pt = new PromptTypeRow();
                pt.Type = table.GetData(row, (int)TableColumns.PromptTypes.Type);
                pt.Condition = table.GetData(row, (int)TableColumns.PromptTypes.Condition);
                pt.Wording = table.GetData(row, (int)TableColumns.PromptTypes.Wording);
                pt.Id = table.GetData(row, (int)TableColumns.PromptTypes.Id);
                pt.TypeDateStamp = table.GetData(row, (int)TableColumns.PromptTypes.TypeDateStamp);
                pt.ConditionDateStamp = table.GetData(row, (int)TableColumns.PromptTypes.ConditionDateStamp);
                pt.WordingDateStamp = table.GetData(row, (int)TableColumns.PromptTypes.WordingDateStamp);
                pt.IdDateStamp = table.GetData(row, (int)TableColumns.PromptTypes.IdDateStamp);
                list.Add(pt);
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<PromptTypeRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.PromptTypes)).Length);

            int row = 0;
            foreach (PromptTypeRow pt in rows) {
                table.SetData(row, (int)TableColumns.PromptTypes.Type, pt.Type);
                table.SetData(row, (int)TableColumns.PromptTypes.Condition, pt.Condition);
                table.SetData(row, (int)TableColumns.PromptTypes.Wording, pt.Wording);
                table.SetData(row, (int)TableColumns.PromptTypes.Id, pt.Id);
                table.SetData(row, (int)TableColumns.PromptTypes.TypeDateStamp, pt.TypeDateStamp);
                table.SetData(row, (int)TableColumns.PromptTypes.ConditionDateStamp, pt.ConditionDateStamp);
                table.SetData(row, (int)TableColumns.PromptTypes.WordingDateStamp, pt.WordingDateStamp);
                table.SetData(row, (int)TableColumns.PromptTypes.IdDateStamp, pt.IdDateStamp);
                row++;
            }

            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Type { get; set; }
        public string Condition { get; set; }
        public string Wording { get; set; }
        public string Id { get; set; }
        public string TypeDateStamp { get; set; }
        public string ConditionDateStamp { get; set; }
        public string WordingDateStamp { get; set; }
        public string IdDateStamp { get; set; }

        // these must match the property names above
        public const string TypeColumnName = "Type";
        public const string ConditionColumnName = "Condition";
        public const string WordingColumnName = "Wording";
        public const string IdColumnName = "Id";

        // these must have the DateStampColumnSuffix - it's used to automatically hide these columns
        public const string TypeDateStampColumnName = TypeColumnName + Strings.DateStampColumnSuffix;
        public const string ConditionDateStampColumnName = ConditionColumnName + Strings.DateStampColumnSuffix;
        public const string WordingDateStampColumnName = WordingColumnName + Strings.DateStampColumnSuffix;
        public const string IdDateStampColumnName = IdColumnName + Strings.DateStampColumnSuffix;
    }
}
