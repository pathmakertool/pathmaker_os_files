using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PathMaker.PropertyEditors.GridRows
{
    class PrefixListRow {
        public PrefixListRow() { }

        public static BindingList<PrefixListRow> GetRowsFromTable(Table table)
        {
            BindingList<PrefixListRow> list = new BindingList<PrefixListRow>();
            for (int row = 0; row < table.GetNumRows(); row++)
            {
                PrefixListRow cl = new PrefixListRow();
                cl.Prefix = table.GetData(row, (int)TableColumns.PrefixList.Prefix);
                cl.Meaning = table.GetData(row, (int)TableColumns.PrefixList.Meaning);
                list.Add(cl);
            }
            return list;
        }

        public static Table GetTableFromRows(BindingList<PrefixListRow> rows)
        {
            Table table = new Table(rows.Count, Enum.GetNames(typeof(TableColumns.PrefixList)).Length);

            int row = 0;
            foreach (PrefixListRow cl in rows)
            {
                table.SetData(row, (int)TableColumns.PrefixList.Prefix, cl.Prefix);
                table.SetData(row, (int)TableColumns.PrefixList.Meaning, cl.Meaning);
                row++;
            }

            return table;
        }

        // These routines represent the columns of the DataGridView
        // The DataGridView will pull them from here automatically
        // and we also use the names as the column headers - that's
        // why the strings below need to match the field names here
        public string Prefix { get; set; }
        public string Meaning { get; set; }
        
        // these must match the property names above
        public const string PrefixColumnName = "Prefix";
        public const string MeaningColumnName = "Meaning";

    }
}
