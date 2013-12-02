using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class TransferShadow : Shadow {
        public TransferShadow(Shape shape)
            : base(shape) {
        }

        public override string GetGotoName() {
            return shape.Text;
        }

        public override void OnConnectAddOutput(Shadow shadow) {
            Common.ErrorMessage("Incorrectly adding output to a Transfer");
            // get busy cursor without this
            shadow.SelectShape();
        }
    }
}
