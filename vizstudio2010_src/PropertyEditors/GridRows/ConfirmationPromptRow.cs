using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class ConfirmationPromptRow {
        public ConfirmationPromptRow() { }

        public static BindingList<ConfirmationPromptRow> GetRowsFromTable(Table table) {
            BindingList<ConfirmationPromptRow> list = new BindingList<ConfirmationPromptRow>();
            for (int row = 0; row < table.GetNumRows(); row++) {
                ConfirmationPromptRow cp = new ConfirmationPromptRow();
                cp.Option = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Option);
                cp.Condition = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Condition);
                cp.Wording = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Wording);
                cp.Id = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Id);
                cp.OptionDateStamp = table.GetData(row, (int)TableColumns.ConfirmationPrompts.OptionDateStamp);
                cp.ConditionDateStamp = table.GetData(row, (int)TableColumns.ConfirmationPrompts.ConditionDateStamp);
                cp.WordingDateStamp = table.GetData(row, (int)TableColumns.ConfirmationPrompts.WordingDateStamp);
                cp.IdDateStamp = table.GetData(row, (int)TableColumns.ConfirmationPrompts.IdDateStamp);
                list.Add(cp);
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<ConfirmationPromptRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.ConfirmationPrompts)).Length);

            int row = 0;
            foreach (ConfirmationPromptRow cp in rows) {
                table.SetData(row, (int)TableColumns.ConfirmationPrompts.Option, cp.Option);
                table.SetData(row, (int)TableColumns.ConfirmationPrompts.Condition, cp.Condition);
                table.SetData(row, (int)TableColumns.ConfirmationPrompts.Wording, cp.Wording);
                table.SetData(row, (int)TableColumns.ConfirmationPrompts.Id, cp.Id);
                table.SetData(row, (int)TableColumns.ConfirmationPrompts.OptionDateStamp, cp.OptionDateStamp);
                table.SetData(row, (int)TableColumns.ConfirmationPrompts.ConditionDateStamp, cp.ConditionDateStamp);
                table.SetData(row, (int)TableColumns.ConfirmationPrompts.WordingDateStamp, cp.WordingDateStamp);
                table.SetData(row, (int)TableColumns.ConfirmationPrompts.IdDateStamp, cp.IdDateStamp);
                row++;
            }
            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Option { get; set; }
        public string Condition { get; set; }
        public string Wording { get; set; }
        public string Id { get; set; }
        public string OptionDateStamp { get; set; }
        public string ConditionDateStamp { get; set; }
        public string WordingDateStamp { get; set; }
        public string IdDateStamp { get; set; }

        // these must match the property names above
        public const string OptionColumnName = "Option";
        public const string ConditionColumnName = "Condition";
        public const string WordingColumnName = "Wording";
        public const string IdColumnName = "Id";

        // these must have the DateStampColumnSuffix - it's used to automatically hide these columns
        public const string OptionDateStampColumnName = OptionColumnName + Strings.DateStampColumnSuffix;
        public const string ConditionDateStampColumnName = ConditionColumnName + Strings.DateStampColumnSuffix;
        public const string WordingDateStampColumnName = WordingColumnName + Strings.DateStampColumnSuffix;
        public const string IdDateStampColumnName = IdColumnName + Strings.DateStampColumnSuffix;
    }
}
