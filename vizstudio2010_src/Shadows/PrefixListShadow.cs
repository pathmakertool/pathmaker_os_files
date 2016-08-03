using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class PrefixListShadow : Shadow {

        public PrefixListShadow(Shape shape)
            : base(shape) {
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick()
        {
            PrefixListForm form = new PrefixListForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        internal Table GetPrefixListTable() {
            return Common.GetCellTable(shape, ShapeProperties.PrefixList.Prefix); 
        }
        


        internal void SetPrefixListTable(Table prefixList)
        {
            Common.SetCellTable(shape, ShapeProperties.PrefixList.Prefix, prefixList);
        }
        
        /**
         * Utility method to return the highlight color given a change date
         */
        internal string GetColorStringForChange(DateTime date) {
            //Table table = GetAppDesc();
            string color = Strings.HighlightColorNone;

            if (date == null)
                return color;

            //color = table.GetData(1, (int)TableColumns.AppDesc.Highlight);
            return color;
        }
    }
}

