using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class OnPageRefOutShadow : Shadow {
        public OnPageRefOutShadow(Shape shape)
            : base(shape) {
        }

        public override Shadow GetDestinationTarget() {
            Shadow partner = GetPartnerOnPageRefInShadow();

            if (partner != null)
                return partner.GetDestinationTarget();
            else
                return this;
        }

        public override List<Shadow> GetSourceTargets() {
            List<Shadow> list = new List<Shadow>();
            List<Connect> connects = GetShapeInputs();

            if (connects.Count == 0)
                list.Add(this);
            else {
                foreach (Connect c in connects) {
                    Shape sourceShape = c.FromSheet;
                    Shadow sourceShadow = PathMaker.LookupShadowByShape(sourceShape);
                    if (sourceShadow != null)
                        list.AddRange(sourceShadow.GetSourceTargets());
                    else
                        list.Add(this);
                }
            }
            return list;
        }
        
        /**
         * This returns the partner on page ref in that is associated with this one
         */
        private Shadow GetPartnerOnPageRefInShadow() {
            // These are still only held together by name
            Page page = shape.ContainingPage;
            foreach (Shape s in page.Shapes)
                if (shape.Text.Equals(s.Text)) {
                    Shadow shadow = PathMaker.LookupShadowByShape(s);
                    if (shadow.GetShapeType() == ShapeTypes.OnPageRefIn)
                        return shadow;
                }
            return null;
        }

        public override void OnConnectAddOutput(Shadow shadow) {
            Common.ErrorMessage("Incorrectly adding output to an Outgoing On Page Reference");
            // get busy cursor without this
            shadow.SelectShape();
        }
    }
}
