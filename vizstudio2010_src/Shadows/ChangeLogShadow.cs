using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class ChangeLogShadow : Shadow {

        public ChangeLogShadow(Shape shape)
            : base(shape) {
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick() {
            ChangeLogForm form = new ChangeLogForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        internal Table GetChangeLog() {
            return Common.GetCellTable(shape, ShapeProperties.ChangeLog.Changes);
        }

        internal string GetLastChangeVersion() {
            Table table = GetChangeLog();
            if (table.IsEmpty())
                return string.Empty;
            return table.GetData(table.GetNumRows()-1, (int)TableColumns.ChangeLog.Version);
        }

        internal string GetLastLogChangeDate() {
            Table table = GetChangeLog();
            if (table.IsEmpty())
                return string.Empty;
            return table.GetData(table.GetNumRows() - 1, (int)TableColumns.ChangeLog.Date);
        }

        internal void SetChangeLog(Table table) {
            Common.SetCellTable(shape, ShapeProperties.ChangeLog.Changes, table);
        }

        /**
         * Utility method to return the highlight color given a change date
         */
        internal string GetColorStringForChange(DateTime date) {
            Table table = GetChangeLog();
            string color = Strings.HighlightColorNone;

            if (date == null)
                return color;

            for (int r = 0; r < table.GetNumRows(); r++) {
                DateTime revisionDate;
                if (DateTime.TryParse(table.GetData(r, (int)TableColumns.ChangeLog.Date), out revisionDate)) {
                    if (revisionDate > date)
                        return color;
                    else
                        color = table.GetData(r, (int)TableColumns.ChangeLog.Highlight);
                }
            }
            return color;
        }
    }
}
