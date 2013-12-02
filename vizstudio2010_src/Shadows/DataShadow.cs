using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class DataShadow : StateWithTransitionShadow {
        public DataShadow(Shape shape)
            : base(shape) {
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick() {
            DataForm form = new DataForm();
            form.ShowDialog(this);
            form.Dispose();
        }
    }
}
