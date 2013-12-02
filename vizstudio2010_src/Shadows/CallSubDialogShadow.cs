using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class CallSubDialogShadow : Shadow {

        public CallSubDialogShadow(Shape shape)
            : base(shape) {
        }

        override public void OnShapeProperties() {
            CallSubDialogForm form = new CallSubDialogForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        public override string GetGotoName() {
            string uid = GetSubDialogUID();

            if (uid == null || uid.Length == 0)
                return Strings.DisconnectedConnectorTarget;
            else {
                Shadow shadow = PathMaker.LookupShadowByUID(GetSubDialogUID());
                if (shadow != null)
                    return shadow.GetGotoName();
                else
                    return Strings.DisconnectedConnectorTarget;
            }
            
        }

        override public void OnShapeDoubleClick() {
            if (shape.Text.Length == 0) {
                OnShapeProperties();
            }
            else {
                Shadow shadow = PathMaker.LookupShadowByUID(GetSubDialogUID());
                if (shadow != null) {
                    shadow.SelectShape();
                }
            }
        }

        internal string GetSubDialogUID() {
            return Common.GetCellString(shape, ShapeProperties.CallSubDialog.SubDialogUID);
        }

        /**
         * Called when the stateId of the SubDialog associated with this CallSubDialog is changed
         * Needed to update the shape text of this shape
         */
        internal void OnSubDialogStateIdChanged(string subDialogUID) {
            if (GetSubDialogUID().Equals(subDialogUID)) {
                StateShadow shadow = PathMaker.LookupShadowByUID(subDialogUID) as StateShadow;
                if (shadow != null)
                    Common.ForcedSetShapeText(shape, StateShadow.StateIdForDisplay(shadow.GetStateId()));
            }
        }

        internal void SetSubDialogUID(string subDialogUID) {
            Common.SetCellString(shape, ShapeProperties.CallSubDialog.SubDialogUID, subDialogUID);
            StateShadow shadow = PathMaker.LookupShadowByUID(subDialogUID) as StateShadow;
            if (shadow != null)
                Common.ForcedSetShapeText(shape, StateShadow.StateIdForDisplay(shadow.GetStateId()));
        }

        public override Shadow GetDestinationTarget() {
            Shadow shadow = PathMaker.LookupShadowByUID(GetSubDialogUID());
            if (shadow == null)
                return this;
            else
                return shadow;
        }

        public override void OnConnectAddOutput(Shadow shadow) {
            if (GetOutputs().Count > 1) {
                Common.ErrorMessage("Incorrectly adding more than one output to a Call SubDialog");
                // get busy cursor without this
                shadow.SelectShape();
            }
        }

        public override void FixUIDReferencesAfterPaste(Dictionary<string, string> oldGUIDToNewGUIDMap) {
            string oldUID = GetSubDialogUID();
            string newUID;

            newUID = CommonShadow.GetNewUIDAfterPaste(oldUID, oldGUIDToNewGUIDMap, true);
            if (newUID.Length == 0) {
                Common.ErrorMessage("Call SubDialog refers to a SubDialog which can't be found, clearing");
                newUID = "";
            }
            SetSubDialogUID(newUID);
        }
    }
}
