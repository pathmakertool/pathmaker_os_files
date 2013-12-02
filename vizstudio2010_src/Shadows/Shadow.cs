using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    /**
     * This is where we do most of our work.  Each shape has a shadow which
     * is our access to the shape properties and where we add our intelligence
     * and event handlers.  In the future, this will also allow for caching.
     */
    public class Shadow {
        protected Shape shape;
        protected ShapeTypes shapeType;

        public Shadow(Shape shape) {
            this.shape = shape;
            this.shapeType = Common.GetShapeType(shape);
        }

        public ShapeTypes GetShapeType() {
            return shapeType;
        }

        // The UID is the "pointer" we should store anywhere we need
        // a reference to a shape
        public string GetUniqueId() {
            return shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);
        }

        // returns the name of this item to be used in a Goto
        virtual public string GetGotoName() {
            return Strings.DisconnectedConnectorTarget;
        }

        /**
         * Returns the shadow of the target shape.  For most shadows,
         * this is just the shadow of the shape.  For things like connectors,
         * on and off page references, etc. this will work through the 
         * connections and get to the real target.  If the end shadow is
         * not connected, it will be returned as the target - so you can
         * get connectors, off page connectors, etc.
         */
        virtual public Shadow GetDestinationTarget() {
            return this;
        }

        /**
         * Returns the target of the source shape.  For most shadows,
         * this is just the NickName of the shape.  For things like connectors,
         * on and off page references, etc. this will work back through the 
         * connections and get to the target.  
         */
        virtual public List<Shadow> GetSourceTargets() {
            List<Shadow> list = new List<Shadow>();
            list.Add(this);
            return list;
        }

        /**
         * Selects the shape associated with the shadow
         */
        virtual public void SelectShape() {
            shape.Document.Application.ActiveWindow.DeselectAll();
            shape.Document.Application.ActiveWindow.Page = shape.ContainingPage;
            shape.Document.Application.ActiveWindow.Select(shape, (short)VisSelectArgs.visSelect);
        }

        // this will be an empty list for a connector
        // it doesn't have any connectors pointing to it
        protected List<Connect> GetShapeOutputs() {
            // The 1D connector is always the To and 2D shapes are always the From
            List<Connect> list = new List<Connect>();

            foreach (Connect c in shape.FromConnects) {
                // determine which end of the connector is connected to the nonConnectorShadow
                bool arrowSide = false;
                if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                    arrowSide = true;

                // if we are on the non-arrow side of the connector, that means this is an output connector
                if (!arrowSide)
                    list.Add(c);
            }
            return list;
        }

        // this will be an empty list for a connector
        // it doesn't have any connectors pointing to it
        protected List<Connect> GetShapeInputs() {
            // The 1D connector is always the To and 2D shapes are always the From
            List<Connect> list = new List<Connect>();

            foreach (Connect c in shape.FromConnects) {
                // determine which end of the connector is connected to the nonConnectorShadow
                bool arrowSide = false;
                if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                    arrowSide = true;

                // if we are on the arrow side of the connector, that means this is an input connector
                if (arrowSide)
                    list.Add(c);
            }
            return list;
        }

        // this will be an empty list for a connector
        // it doesn't have any connectors pointing to it
        internal List<Shadow> GetInputs() {
            List<Connect> connects = GetShapeInputs();

            List<Shadow> list = new List<Shadow>();

            foreach (Connect connect in connects) {
                // The 1D connector is always the To and 2D shapes are always the From
                Shape fromShape = connect.FromSheet;
                Shadow shadow = PathMaker.LookupShadowByShape(fromShape);
                foreach (Shadow s in shadow.GetSourceTargets())
                    list.Add(s);
            }
            return list;
        }

        // this will be an empty list for a connector
        // it doesn't have any connectors pointing to it
        internal List<Shadow> GetOutputs() {
            List<Connect> connects = GetShapeOutputs();

            List<Shadow> list = new List<Shadow>();

            foreach (Connect connect in connects) {
                // The 1D connector is always the To and 2D shapes are always the From
                Shape toShape = connect.FromSheet;
                Shadow shadow = PathMaker.LookupShadowByShape(toShape);
                list.Add(shadow.GetDestinationTarget());
            }
            return list;
        }

        virtual internal void AddPromptsToRecordingList(PromptRecordingList recordingList, DateTime? onOrAfterDate) { }
        virtual internal void ApplyPromptsFromRecordingList(PromptRecordingList recordingList) { }

        virtual internal DateTime GetLastChangeDate() {
            //Return Dave's birthdate to prevent highlighing if there is no data to compare to
            return new DateTime(1966, 9, 3);
        }

        virtual internal bool RemoveGotosThatDontUseConnectors(string uidBeingRemoved) { return false; }


        /**
         * Renumbers any prompts according to the current format and passes back
         * the number of prompts that were renumbered (used if format = numbers)
         * startNumber is only used if format = numbers
         **/
        virtual public int RedoPromptIds(int startNumber, string promptIdFormat) {
            return 0;
        }

        // after a paste, uids get redone if there are duplicates - things like transitions
        // have pointers to old uids - this needs to remap them to the new ones
        virtual public void FixUIDReferencesAfterPaste(Dictionary<string, string> oldGUIDToNewGUIDMap) {
            // do nothing by default, to be overridden by shadows that have work to do
        }

        internal void RemoveOutputsIfNotInTableColumn(Table table, int gotoColumn) {
            List<Connect> connects = GetShapeOutputs();
            List<Shadow> shadows = new List<Shadow>();

            foreach (Connect connect in connects) {
                // The 1D connector is always the To and 2D shapes are always the From
                Shape toShape = connect.FromSheet;
                Shadow shadow = PathMaker.LookupShadowByShape(toShape);
                shadows.Add(shadow);
            }

            for (int row = 0; row < table.GetNumRows(); row++) {
                string uid = table.GetData(row, gotoColumn);
                Shadow shadow = PathMaker.LookupShadowByUID(uid);
                if (shadow != null)
                    shadows.Remove(shadow);
            }

            if (shadows.Count > 0) {
                foreach (Shadow shadow in shadows)
                    shadow.shape.Delete();
            }
        }

        public int GetPageNumber() {
            return shape.ContainingPage.Index;
        }

        // Event Handlers
        virtual public void OnShapeProperties() { }
        virtual public void OnShapeDoubleClick() { }
        virtual public void OnShapeExitTextEdit() { }
        virtual public void OnBeforeShapeDelete() { }
        virtual public void OnShapeAdd() { }
        virtual public void OnConnectAddInput(Shadow shadow) { }
        virtual public void OnConnectDeleteInput(Shadow shadow) { }
        virtual public void OnConnectAddOutput(Shadow shadow) { }
        virtual public void OnConnectDeleteOutput(Shadow shadow) { }
        virtual public void OnConnectorChangeTarget(ConnectorShadow shadow) { }
        virtual public void OnConnectorLabelChange(ConnectorShadow shadow) { }
    }
}
