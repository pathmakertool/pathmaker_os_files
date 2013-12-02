using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class OnPageRefInShadow : Shadow {
        public OnPageRefInShadow(Shape shape)
            : base(shape) {
        }

        public override Shadow GetDestinationTarget() {
            List<Connect> connects = GetShapeOutputs();

            // find outbound link (should only be one)
            if (connects.Count != 1)
                return this;

            // the FromSheet is the outgoing connector (always from = 1D, to = 2D)
            Shadow connectedToShadow = PathMaker.LookupShadowByShape(connects[0].FromSheet);
            if (connectedToShadow != null)
                return connectedToShadow.GetDestinationTarget();
            else
                return this;
        }

        /**
         * Gets the on page ref in that is associated with this one
         * this is done using the name of the shape
         */
        private List<Shadow> GetPartnerOnPageRefOutShadow() {
            List<Shadow> list = new List<Shadow>();

            // These are still only held together by name
            Page page = shape.ContainingPage;
            foreach (Shape s in page.Shapes)
                if (shape.Text.Equals(s.Text)) {
                    Shadow shadow = PathMaker.LookupShadowByShape(s);
                    if (shadow.GetShapeType() == ShapeTypes.OnPageRefOut)
                        list.Add(shadow);
                }
            return list;
        }

        public override List<Shadow> GetSourceTargets() {
            List<Shadow> results = new List<Shadow>();

            List<Shadow> list = GetPartnerOnPageRefOutShadow();
            foreach (Shadow shadow in list) {
                if (shadow != null)
                    results.AddRange(shadow.GetSourceTargets());
                else {
                    results.Add(this);
                }
            }
            return results;
        }

        public override void OnConnectAddInput(Shadow shadow) {
            Common.ErrorMessage("Incorrectly adding input to an Incoming On Page Reference");
            // get busy cursor without this
            shadow.SelectShape();
        }

        public override void OnShapeExitTextEdit() {
            List<Shadow> list = new List<Shadow>();

            // These are still only held together by name
            Page page = shape.ContainingPage;
            foreach (Shape s in page.Shapes) {
                if (shape.Text.Equals(s.Text)) {
                    Shadow shadow = PathMaker.LookupShadowByShape(s);
                    if (shadow.GetType() == this.GetType())
                        list.Add(shadow);
                }
            }
            
            if (list.Count > 1) {
                Common.ErrorMessage("Error - Incoming On Page Reference \"" + shape.Text + "\" already exists.");
                shape.Text = Strings.ToBeDeletedLabel;
            }
        }

    }
}
