using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker {
    class NameValuePairRow {
        public NameValuePairRow() { }

        public static BindingList<NameValuePairRow> GetRowsFromTable(Table table) {
            BindingList<NameValuePairRow> list = new BindingList<NameValuePairRow>();

            Boolean newStartStep = false;
            //Version 1.6.1.13+ uses a new set of fields for init data NV pairs - Notes column was added JDK 08-25-2014
            if (table.GetNumColumns() >= 5)
            {
                newStartStep = true;
            }

            if (newStartStep)
            {
                for (int r = 0; r < table.GetNumRows(); r++)
                {
                    NameValuePairRow nv = new NameValuePairRow();
                    nv.Name = table.GetData(r, (int)TableColumns.NameValuePairs.Name);
                    nv.Value = table.GetData(r, (int)TableColumns.NameValuePairs.Value);
                    nv.Notes = table.GetData(r, (int)TableColumns.NameValuePairs.Notes);
                    nv.NameDateStamp = table.GetData(r, (int)TableColumns.NameValuePairs.NameDateStamp);
                    nv.ValueDateStamp = table.GetData(r, (int)TableColumns.NameValuePairs.ValueDateStamp);
                    nv.NotesDateStamp = table.GetData(r, (int)TableColumns.NameValuePairs.NotesDateStamp);
                    list.Add(nv);
                }
            } else {
                for (int r = 0; r < table.GetNumRows(); r++)
                {
                    NameValuePairRow nv = new NameValuePairRow();
                    nv.Name = table.GetData(r, (int)TableColumns.NameValuePairs.Name);
                    nv.Value = table.GetData(r, (int)TableColumns.NameValuePairs.Value);
                    nv.Notes = "";
                    nv.NameDateStamp = table.GetData(r, (int)TableColumns.NameValuePairs.NameDateStamp);
                    nv.ValueDateStamp = table.GetData(r, (int)TableColumns.NameValuePairs.ValueDateStamp);
                    nv.NotesDateStamp = "";
                    list.Add(nv);
                }
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<NameValuePairRow> rows) {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.NameValuePairs)).Length);

            Boolean newStartStep = false;
            //Version 1.6.1.13+ uses a new set of fields for init data NV pairs - Notes column was added JDK 08-25-2014
            if (table.GetNumColumns() >= 5)
            {
                newStartStep = true;
            }

            if (newStartStep)
            {
                int row = 0;
                foreach (NameValuePairRow nv in rows)
                {
                    table.SetData(row, (int)TableColumns.NameValuePairs.Name, nv.Name);
                    table.SetData(row, (int)TableColumns.NameValuePairs.Value, nv.Value);
                    table.SetData(row, (int)TableColumns.NameValuePairs.Notes, nv.Notes);
                    table.SetData(row, (int)TableColumns.NameValuePairs.NameDateStamp, nv.NameDateStamp);
                    table.SetData(row, (int)TableColumns.NameValuePairs.ValueDateStamp, nv.ValueDateStamp);
                    table.SetData(row, (int)TableColumns.NameValuePairs.NotesDateStamp, nv.NotesDateStamp);
                    row++;
                }
            }
            else
            {
                int row = 0;
                foreach (NameValuePairRow nv in rows)
                {
                    table.SetData(row, (int)TableColumns.NameValuePairs.Name, nv.Name);
                    table.SetData(row, (int)TableColumns.NameValuePairs.Value, nv.Value);
                    //table.SetData(row, (int)TableColumns.NameValuePairs.Notes, nv.Notes);
                    table.SetData(row, (int)TableColumns.NameValuePairs.NameDateStamp, nv.NameDateStamp);
                    table.SetData(row, (int)TableColumns.NameValuePairs.ValueDateStamp, nv.ValueDateStamp);
                    //table.SetData(row, (int)TableColumns.NameValuePairs.NotesDateStamp, nv.NotesDateStamp);
                    row++;
                }
            }
            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Name { get; set; }
        public string Value { get; set; }
        public string Notes { get; set; }
        public string NameDateStamp { get; set; }
        public string ValueDateStamp { get; set; }
        public string NotesDateStamp { get; set; }

        // these must match the property names above
        public const string NameColumnName = "Name";
        public const string ValueColumnName = "Value";
        public const string NotesColumnName = "Notes";

        // these must have the DateStampColumnSuffix - it's used to automatically hide these columns
        public const string NameDateStampColumnName = NameColumnName + Strings.DateStampColumnSuffix;
        public const string ValueDateStampColumnName = ValueColumnName + Strings.DateStampColumnSuffix;
        public const string NotesDateStampColumnName = NotesColumnName + Strings.DateStampColumnSuffix;
    }
}

