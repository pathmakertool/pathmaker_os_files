using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class ChangeLogRow {
        public static BindingList<ChangeLogRow> GetRowsFromTable(Table table) {
            BindingList<ChangeLogRow> list = new BindingList<ChangeLogRow>();
            for (int row = 0; row < table.GetNumRows(); row++) {
                ChangeLogRow cl = new ChangeLogRow();
                cl.Date = table.GetData(row, (int)TableColumns.ChangeLog.Date);
                cl.Version = table.GetData(row, (int)TableColumns.ChangeLog.Version);
                cl.Details = table.GetData(row, (int)TableColumns.ChangeLog.Details);
                cl.Author = table.GetData(row, (int)TableColumns.ChangeLog.Author);
                cl.Highlight = table.GetData(row, (int)TableColumns.ChangeLog.Highlight);
                list.Add(cl);
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<ChangeLogRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.ChangeLog)).Length);

            int row = 0;
            foreach (ChangeLogRow cl in rows) {
                table.SetData(row, (int)TableColumns.ChangeLog.Date, cl.Date);
                table.SetData(row, (int)TableColumns.ChangeLog.Version, cl.Version);
                table.SetData(row, (int)TableColumns.ChangeLog.Details, cl.Details);
                table.SetData(row, (int)TableColumns.ChangeLog.Author, cl.Author);
                table.SetData(row, (int)TableColumns.ChangeLog.Highlight, cl.Highlight);
                row++;
            }

            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Date { get; set; }
        public string Version { get; set; }
        public string Details { get; set; }
        public string Author { get; set; }
        public string Highlight { get; set; }

        // these must match the property names above
        public const string DateColumnName = "Date";
        public const string VersionColumnName = "Version";
        public const string DetailsColumnName = "Details";
        public const string AuthorColumnName = "Author";
        public const string HighlightColumnName = "Highlight";
    }
}
