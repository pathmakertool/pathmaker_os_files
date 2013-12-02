using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class ConnectorShadow : Shadow {
        public ConnectorShadow(Shape shape)
            : base(shape) {
                if (IsIllegalLoop()) {
                    Common.ErrorMessage("There's an improper loop caused by " + shape.Name + " on page" + shape.ContainingPage.Name + " - deleting it.");
                    shape.Delete();
                }
        }

        override public void OnBeforeShapeDelete() {
            // clean up our connections to other states - ConnectDelete is not called when the shape is deleted

            Connect c = GetConnect(false);

            if (c == null)
                return;

            // determine which end of the connector is connected to the nonConnectorShadow
            // In this case, it's always the c.FromCell that tells us
            bool arrowSide = false;
            if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                arrowSide = true;

            Shadow connectedToShadow = PathMaker.LookupShadowByShape(c.ToSheet);

            if (connectedToShadow != null) {
                if (arrowSide)
                    connectedToShadow.OnConnectDeleteInput(this);
                else
                    connectedToShadow.OnConnectDeleteOutput(this);
            }
            else
                Common.ErrorMessage("Connector goes to unknown shape " + c.ToSheet.Name);
        }

        /**
         * Gets the Connect (not Connector) object associated with one side (arrow) or the other
         */
        private Connect GetConnect(bool arrowSide) {
            if (arrowSide == true) {
                foreach (Connect c in shape.Connects) {
                    if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                        return c;
                }
            }
            else {
                foreach (Connect c in shape.Connects) {
                    if (c.FromCell.Name.Equals(Strings.BeginConnectionPointCellName))
                        return c;
                }
            }
            return null;
        }

        public override Shadow GetDestinationTarget() {
            Connect c;

            c = GetConnect(true);
            if (c != null) {
                Shadow connectedToShadow = PathMaker.LookupShadowByShape(c.ToSheet);
                return connectedToShadow.GetDestinationTarget();
            }
            else
                return this;
        }

        public override List<Shadow> GetSourceTargets() {
            List<Shadow> list = new List<Shadow>();
            Connect c;

            c = GetConnect(false);
            if (c != null) {
                Shadow connectedToShadow = PathMaker.LookupShadowByShape(c.ToSheet);
                list.AddRange(connectedToShadow.GetSourceTargets());
            }
            else
                list.Add(this);
            return list;
        }

        /**
         * Sets the shape text for the connector.  This is controlled by the transition
         * of the state that is at the non-arrow end of the connector
         */
        public void SetLabelName(string label) {
            Common.ForcedSetShapeText(shape, Common.MakeLabelName(label));
        }

        public string GetLabelName() {
            return shape.Text;
        }

        private bool IsIllegalLoop() {
            Connect inConnect = GetConnect(true);
            Connect outConnect = GetConnect(false);

            if (inConnect != null && outConnect != null && inConnect.ToSheet == outConnect.ToSheet) {
                ShapeTypes shapeType = Common.GetShapeType(inConnect.ToSheet);
                switch (shapeType) {
                    case ShapeTypes.Data:
                    case ShapeTypes.Decision:
                    case ShapeTypes.Interaction:
                    case ShapeTypes.Play:
                        break;
                    default:
                        return true;
                }
            }
            return false;
        }
        
        public override void OnConnectAddInput(Shadow shadow) {
            if (IsIllegalLoop()) {
                Common.ErrorMessage("Incorrectly creating a loop with connector - deleting");
                shape.Delete();
            }
        }

        public override void OnConnectAddOutput(Shadow shadow) {
            if (IsIllegalLoop()) {
                Common.ErrorMessage("Incorrectly creating a loop with connector - deleting");
                shape.Delete();
            }
            else {
                // if non-arrow side is connected, we should mark the goto transition as changed (datestamp)
                List<Shadow> sources = GetSourceTargets();

                foreach (Shadow s in sources)
                    s.OnConnectorChangeTarget(this);
            }
        }

        public override void OnShapeExitTextEdit() {
            // need to update the transitions for the shape on the non-arrow side
            List<Shadow> sourceTargets = GetSourceTargets();
            foreach (Shadow source in sourceTargets)
                source.OnConnectorLabelChange(this);
        }
    }
}
