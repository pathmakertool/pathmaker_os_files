using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    /**
     * Used for things like comments - doesn't do anything other than give us a shadow we can ignore
     */
    class IgnoredShadow : Shadow {
        public IgnoredShadow(Shape shape)
            : base(shape) {
        }

        public override string GetGotoName() {
            return shape.Text;
        }
    }
}
