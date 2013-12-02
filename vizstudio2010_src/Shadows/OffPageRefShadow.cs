using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class OffPageRefShadow : Shadow {
        private bool initialCleanUpDone = false;

        public OffPageRefShadow(Shape shape)
            : base(shape) {
        }

        public override void OnShapeExitTextEdit() {
            base.OnShapeExitTextEdit();

            // hyperlinks are placed in pairs and apparently modifed by Visio after added
            // this fixes them after they are settled
            if (!initialCleanUpDone) {
                // hyperlinks point to pages, not shapes, even if the shape is deleted - drop these
                //                short row = shape.get_CellsRowIndex(ShapeProperties.OffPageRef.HyperLink);
                //                shape.DeleteRow((short)VisSectionIndices.visSectionHyperlink, row);
                // keep the text synchronized
                Common.SetCellFormula(shape, ShapeProperties.TheText, Strings.OffPageConnectorTextCommand);
            }
        }

        public override Shadow GetDestinationTarget() {
            List<Connect> list = GetShapeOutputs();

            System.Diagnostics.Debug.WriteLine("offpage - " + shape.Text + " page - " + shape.ContainingPage.Name);

            if (list.Count == 0 && GetShapeInputs().Count == 0) {
                // no in or outbound connectors
                return this;
            }
            else if (list.Count > 0) {
                // find outbound link (should only be one)
                if (list.Count != 1)
                    return this;

                // the FromSheet is the outgoing connector (always from = 1D, to = 2D)
                Shadow connectedToShadow = PathMaker.LookupShadowByShape(list[0].FromSheet);
                if (connectedToShadow != null)
                    return connectedToShadow.GetDestinationTarget();
                else
                    return this;
            }
            else {
                OffPageRefShadow partnerShadow = GetPartnerOffPageRefShadow() as OffPageRefShadow;

                if (partnerShadow != null && partnerShadow.GetShapeOutputs().Count > 0)
                    return partnerShadow.GetDestinationTarget();
                else {
                    if (partnerShadow != null && partnerShadow.GetShapeInputs().Count > 0)
                        Common.ErrorMessage("Off page connector " + shape.Text + " on page " + shape.ContainingPage.Name + " and it's partner both have inputs");
                    return this;
                }
            }            
        }

        public override List<Shadow> GetSourceTargets() {
            List<Shadow> list = new List<Shadow>();
            List<Connect> connects = GetShapeInputs();

            if (connects.Count == 0) {
                // no inbound connectors, find the partner and go from there
                OffPageRefShadow partnerShadow = GetPartnerOffPageRefShadow() as OffPageRefShadow;
                if (partnerShadow != null) {
                    List<Connect> partnerConnects = partnerShadow.GetShapeInputs();
                    if (partnerConnects.Count == 0) {
                        if (GetShapeOutputs().Count == 0)
                            Common.ErrorMessage("No inputs for off-page connector " + shape.Text + " on page " + shape.ContainingPage.Name);
                        else
                            Common.ErrorMessage("No inputs for off-page connector " + partnerShadow.shape.Text + " on page " + partnerShadow.shape.ContainingPage.Name);
                        list.Add(this);
                    }
                    else
                        list.AddRange(partnerShadow.GetSourceTargets());
                }
                else
                    list.Add(this);
            }
            else {
                foreach (Connect c in connects) {
                    // always the from sheet = connector (fromsheet = 1D, tosheet = 2D)
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
         * Find the off page connector associated with this one
         * This is done using the OPC fields on the shape that store
         * the partner page sheet and shape ids
         */
        private Shadow GetPartnerOffPageRefShadow() {
            string pageSheetId = Common.GetCellString(shape, ShapeProperties.OffPageConnectorDestinationPageID);
            string shapeId = Common.GetCellString(shape, ShapeProperties.OffPageConnectorDestinationShapeID);

            foreach (Page page in shape.Document.Pages) {
                if (page.PageSheet.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID).Equals(pageSheetId)) {
                    Shape partner = null;

                    try {
                        partner = page.Shapes[shapeId];
                    }
                    catch {
                        Common.ErrorMessage("Off page connector \"" + shape.Text + "\" missing partner on page " + shape.ContainingPage.Name + ".\nRenaming to " + Strings.ToBeDeletedLabel + ".");
                        shape.Text = Strings.ToBeDeletedLabel;
                    }

                    if (partner != null)
                        return PathMaker.LookupShadowByShape(partner);
                    else
                        return null;
                }
            }

            return null;
        }


        public override void OnConnectAddInput(Shadow shadow) {
            if (GetOutputs().Count > 0) {
                Common.ErrorMessage("Incorrectly adding input to an Off Page Reference that has outputs");
                // get busy cursor without this
                shadow.SelectShape();
            }
        }

        public override void OnConnectAddOutput(Shadow shadow) {
            if (GetInputs().Count > 0) {
                Common.ErrorMessage("Incorrectly adding output to an Off Page Reference that has inputs");
                // get busy cursor without this
                shadow.SelectShape();
            }
        }
    }
}
