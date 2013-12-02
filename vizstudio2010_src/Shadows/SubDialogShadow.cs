using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public class SubDialogShadow : StateShadow {
        public class ReturnPair {
            public ReturnPair(Shadow previous, Shadow next) { PreviousState = previous; Return = next; }
            public Shadow PreviousState { get; set; }
            public Shadow Return { get; set; }
        }

        public SubDialogShadow(Shape shape) 
            : base(shape) {
        }

        internal override void SetStateId(string stateId) {
            base.SetStateId(stateId);

            // need to update any CallSubDialogs that call this
            List<Shadow> list = PathMaker.LookupShadowsByShapeType(ShapeTypes.CallSubDialog);
            foreach (Shadow shadow in list) {
                CallSubDialogShadow csdShadow = shadow as CallSubDialogShadow;
                csdShadow.OnSubDialogStateIdChanged(shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
            }
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick() {
            SubDialogForm form = new SubDialogForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        internal Table GetDeveloperNotes() {
            return Common.GetCellTable(shape, ShapeProperties.SubDialog.DeveloperNotes);
        }

        internal void SetDeveloperNotes(Table table) {
            Table tmp = GetDeveloperNotes();

            if ((table.GetData(0, 0) == null || table.GetData(0, 0).Length == 0) && tmp.IsEmpty())
                return;

            if (tmp.IsEmpty()) {
                table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                Common.SetCellTable(shape, ShapeProperties.Interaction.DeveloperNotes, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.DeveloperNotes.Text).Equals(table.GetData(0, (int)TableColumns.DeveloperNotes.Text))) {
                table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                Common.SetCellTable(shape, ShapeProperties.Interaction.DeveloperNotes, table);
            }
        }

        internal Shadow GetFirstStateTarget() {
            List<Connect> connects = GetShapeOutputs();

            if (connects.Count == 0) {
                Common.ErrorMessage("SubDialog shape " + GetGotoName() + " does not have a first state connector");
                return null;
            }
            else if (connects.Count > 1) {
                Common.ErrorMessage("SubDialog shape " + GetGotoName() + " has more than one output connector");
                return null;
            }

            Shape theOne = connects[0].FromSheet;
            return Common.GetGotoTargetFromData(theOne.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
        }

        internal List<ReturnPair> GetReturnPairs() {
            // find all the call sub dialogs that reference this one
            // find all their input and outputs and make a table from that

            List<Shadow> shadowList = PathMaker.LookupShadowsByShapeType(ShapeTypes.CallSubDialog);

            List<ReturnPair> returnPairs = new List<ReturnPair>();

            List<String> alreadyDone = new List<String>();

            foreach (Shadow s in shadowList) {
                CallSubDialogShadow shadow = s as CallSubDialogShadow;
                if (shadow.GetSubDialogUID() == shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID)) {
                    List<Shadow> inputs = shadow.GetInputs();
                    List<Shadow> outputs = shadow.GetOutputs();

                    foreach (Shadow input in inputs)
                        foreach (Shadow output in outputs) {
                            String key = input.GetGotoName() + output.GetGotoName();
                            if (!alreadyDone.Contains(key)) {
                                returnPairs.Add(new ReturnPair(input, output));
                                alreadyDone.Add(key);
                            }
                        }
                }
            }

            return returnPairs;
        }
    }
}
