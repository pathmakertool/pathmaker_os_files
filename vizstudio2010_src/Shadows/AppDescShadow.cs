using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class AppDescShadow : Shadow {

        public AppDescShadow(Shape shape)
            : base(shape) {
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick()
        {
            TextEditorForm form = new TextEditorForm();
            if (GetDescriptionText() == "") {
                form.ShowDialog("Enter a brief description about your application.");
            } else {
                form.ShowDialog(GetDescriptionText());
            }
            
            SetDescriptionText(form.GetText());
            form.Dispose();
        }

        internal Table GetAppDescTable() {
            return Common.GetCellTable(shape, ShapeProperties.AppDesc.Description); 
        }

        internal string GetDescriptionText()
        {
            return Common.GetCellString(shape, ShapeProperties.AppDesc.Description);
        }

        internal void SetDescriptionText(string descText)
        {
            Common.SetCellString(shape, ShapeProperties.AppDesc.Description, descText);
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

