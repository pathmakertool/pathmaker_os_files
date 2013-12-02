using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    partial class Repair {
        private static string NeedsToBeRepairedString = "NeedsToBeRepaired";

        // changes fields which are A$$B to be A
        private static void FixDollarDollarTableFields(Shape shape, string cellName) {
            const string dataSeparator = "$$";

            Table table = Common.GetCellTable(shape, cellName);
            if (!table.IsEmpty()) {
                for (int r = 0; r < table.GetNumRows(); r++) {
                    for (int c = 0; c < table.GetNumColumns(); c++) {
                        string data = table.GetData(r, c);
                        if (data.Contains(dataSeparator)) {
                            data = data.Substring(0, data.IndexOf(dataSeparator));
                            table.SetData(r, c, data);
                        }
                    }
                }
                Common.SetCellTable(shape, cellName, table);
            }
        }

        // Changes AA1000_FooBar to the UID for that state for all rows in a table cell
        private static void ReplaceStateIdsWithUniqueIds(Shape shape, Dictionary<string, List<string>> uniqueIdMap, string cellName, int column) {
            Table table = Common.GetCellTable(shape, cellName);
            if (!table.IsEmpty()) {
                bool needsToBeRepaired = false;
                bool allNeedToBeRepaired = true;
                for (int i = 0; i < table.GetNumRows(); i++) {
                    string gotoShapeName = table.GetData(i, column);
                    string uniqueId = TranslateGotoToUniqueIdForShapeOrKeyword(shape, gotoShapeName, uniqueIdMap, Common.MakeLabelName(table.GetData(i, 0)));
                    if (uniqueId != null) {
                        if (uniqueId.Equals(NeedsToBeRepairedString))
                            needsToBeRepaired = true;
                        else
                            allNeedToBeRepaired = false;
                        table.SetData(i, column, uniqueId);
                    }
                }

                if (needsToBeRepaired) {
                    // First, see if the label in the first column matches the label on an outbound connector
                    List<Connect> connectList = new List<Connect>();

                    foreach (Connect c in shape.FromConnects) {
                        // determine which end of the connector is connected to the nonConnectorShadow
                        bool arrowSide = false;
                        if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                            arrowSide = true;

                        // if we are on the non-arrow side of the connector, that means this is an output connector
                        if (!arrowSide)
                            connectList.Add(c);  // the c.FromSheet will be the connector shape
                    }

                    // if they are already in use, remove them from the list
                    for (int i = 0; i < table.GetNumRows(); i++) {
                        string uid = table.GetData(i, column);
                        for (int j = connectList.Count - 1; j >= 0; j--) {
                            Connect c = connectList[j];
                            if (uid.Equals(c.FromSheet.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID)))
                                connectList.Remove(c);
                        }
                    }

                    // make sure we don't have two links from the same state with the same label which are both needing repair
                    // very, very remote - but I wouldn't want to have to figure this out after the fact - stop it here
                    List<string> checkList = new List<string>();
                    for (int i = 0; i < table.GetNumRows(); i++) {
                        if (table.GetData(i, column).Equals(NeedsToBeRepairedString)) {
                            string label = Common.MakeLabelName(table.GetData(i, 0));
                            if (checkList.Contains(label)) {
                                // multiple links messed up with the same label
                                postUpgradeReportString += "Shape " + ShapeDescription(shape) + " has more than one transition with the same label of \"" +
                                    label + "\". This is a blocking error.  Please go back into CID and make sure all the links coming out of that state have labels. " +
                                    "Then PathMaker will be able to import it, after which you can put the labels back to the way you want them.\n\n";
                                catastrophicError = true;
                                return;
                            }
                            else
                                checkList.Add(label);
                        }
                    }

                    for (int i = table.GetNumRows() - 1; i >= 0; i--) {
                        if (table.GetData(i, column).Equals(NeedsToBeRepairedString)) {
                            bool repaired = false;
                            string connectorText = Common.MakeLabelName(table.GetData(i, 0)).Trim();

                            foreach (Connect c in connectList) {
                                if (c.FromSheet.Text.Trim().Equals(connectorText)) {
                                    table.SetData(i, column, c.FromSheet.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
                                    connectList.Remove(c);
                                    repaired = true;
                                    break;
                                }
                            }

                            if (!repaired) {
                                // Something's really messed up - there's a transition and no link...
                                // Sometimes visio renames a connector the same name as a previous but different connector
                                // which means we will have transition A matched up with transition B's connector
                                // Only way to try and fix is mark everything as needing repair and go at it again
                                if (!allNeedToBeRepaired) {
                                    for (int r = 0; r < table.GetNumRows(); r++)
                                        table.SetData(r, column, NeedsToBeRepairedString);
                                    Common.SetCellTable(shape, cellName, table);
                                    ReplaceStateIdsWithUniqueIds(shape, uniqueIdMap, cellName, column);
                                    return;
                                }
                                else {
                                    postUpgradeReportString += "Trouble upgrading shape " + ShapeDescription(shape) + ". " +
                                        "Deleting transition " + table.GetData(i, 0) + " without link from " + shape.Text + "." + 
                                        "Please review state to ensure change is correct.\n\n";
                                    table.DeleteRow(i);
                                }
                            }
                        }
                    }
                    if (connectList.Count > 0) {
                        foreach (Connect c in connectList) {
                            postUpgradeReportString += "Trouble upgrading shape " + ShapeDescription(shape) + ". " +
                                "Deleting connector " + c.FromSheet.Text.Trim() + " without transition from " + shape.Text + "." +
                                "Please review state to ensure change is correct.\n\n";
                            c.FromSheet.Delete();
                        }
                    }
                }

                Common.SetCellTable(shape, cellName, table);
            }
        }

        // Changes AA1000_FooBar to the UID for that state for a single cell
        private static void ReplaceStateIdWithUniqueIds(Shape shape, Dictionary<string, List<string>> uniqueIdMap, string cellName) {
            string gotoShapeName = Common.GetCellString(shape, cellName);

            if (gotoShapeName.Length != 0) {
                string uniqueId = TranslateGotoToUniqueIdForShapeOrKeyword(shape, gotoShapeName, uniqueIdMap, null);
                if (uniqueId.Equals(NeedsToBeRepairedString)) {
                    postUpgradeReportString += "Couldn't convert " + gotoShapeName + " from " + ShapeDescription(shape) + "\nChanging to hang up\n";
                    Common.SetCellString(shape, cellName, Strings.HangUpKeyword);
                }
                if (uniqueId != null)
                    Common.SetCellString(shape, cellName, uniqueId);
            }
        }

        // Does the actual translation from state name to UID - may also return a keyword (Return, Hang Up, etc.) for which no UID exists or 
        // null if no change is necessary
        // expectedLabel is used to check connectors - Visio has a habit of changing connector names so we need to ensure
        // we have the right one - it's ignored if the goto is not via a connector
        private static string TranslateGotoToUniqueIdForShapeOrKeyword(Shape shape, string gotoShapeName, Dictionary<string, List<string>> uniqueIdMap, string expectedLabel) {
            List<string> list = null;

            // Call SubDialogs don't have underscores in the SubDialog names
            ShapeTypes type = Common.GetShapeType(shape);
            if (type == ShapeTypes.CallSubDialog)
                gotoShapeName = StateShadow.StateIdForStorage(gotoShapeName);

            if (uniqueIdMap.TryGetValue(gotoShapeName, out list)) {
                // dynamic connectors must be on the same page as the shape
                // visio somtimes renames shapes when copying/pasting so we really want
                // to make sure we get the right connector from the right page
                if (list.Count == 1 && !gotoShapeName.StartsWith(Strings.DynamicConnectorShapeNameStart))
                    return list.ElementAt(0);
                else {
                    // There can be multiple shapes with same name, lovely - need to find one on this page or we're in trouble
                    foreach (string uid in list) {
                        try {
                            Shape s = shape.ContainingPage.Shapes.get_ItemU(uid);
                            if (s != null) {
                                // if it's a connector, make sure it's attached to the shape and matches the expected label
                                if (gotoShapeName.StartsWith(Strings.DynamicConnectorShapeNameStart)) {
                                    foreach (Connect c in s.Connects) {
                                        if (c.FromCell.Name.Equals(Strings.BeginConnectionPointCellName) &&
                                            c.ToSheet == shape &&
                                            Common.MakeLabelName(s.Text).Equals(expectedLabel))
                                            return uid;
                                    }
                                    return NeedsToBeRepairedString;
                                }
                                else 
                                    return uid;
                            }
                        }
                        catch {
                            // just ignore for now - when checking for one from a different page, it throws an exception
                        }
                    }
                    // if we get here, it means either we have a bogus connection (unlikely) or the connector we're looking for 
                    // has been changed when we output from CID
                    return NeedsToBeRepairedString;
                }
            }

            // some things are expected
            if (gotoShapeName.Length == 0)
                return null;

            if (gotoShapeName.Equals(Strings.CurrentStateKeyword))
                return null;

            // these will just be treated as terminals - let's do our best to convert them
            if (gotoShapeName.ToLower().Contains(Strings.ReturnKeyword.ToLower()))
                return Strings.ReturnKeyword;
            else if (gotoShapeName.ToLower().Contains(Strings.TransferKeyword.ToLower()))
                return Strings.TransferKeyword;
            else if (gotoShapeName.ToLower().Contains(Strings.HangUpKeywordShortForm.ToLower()))
                return Strings.HangUpKeywordShortForm;

            return NeedsToBeRepairedString;
        }

        private static void RemoveColumnsFromCellTable(Shape shape, string cellName, int[] columns) {
            Table table = Common.GetCellTable(shape, cellName);
            if (!table.IsEmpty()) {
                table.DeleteColumns(columns);
                Common.SetCellTable(shape, cellName, table);
            }
        }

        private static void SwapColumnsInCellTable(Shape shape, string cellName, int col1, int col2) {
            Table table = Common.GetCellTable(shape, cellName);
            if (!table.IsEmpty()) {
                table.SwapColumns(col1, col2);
                Common.SetCellTable(shape, cellName, table);
            }
        }

        private static void SwapRowsInCellTable(Shape shape, string cellName, int row1, int row2) {
            Table table = Common.GetCellTable(shape, cellName);
            if (!table.IsEmpty()) {
                table.SwapRows(row1, row2);
                Common.SetCellTable(shape, cellName, table);
            }
        }

        private static void UnencodeProperty(Shape shape, string property) {
            string encoded = Common.GetCellString(shape, property);
            if (encoded.Length == 0)
                return;
            // need to handle weird quotes, etc. - this does the job
            Encoding encoding = Encoding.GetEncoding(1252);
            string unencoded = encoding.GetString(System.Convert.FromBase64String(encoded));
            Common.SetCellString(shape, property, unencoded);
        }

        private static void DeleteOldPropertyCell(Shape shape, string cellName) {
            if (shape.get_CellExists(cellName, (short)VisExistsFlags.visExistsAnywhere) != 0) {
                short row = shape.get_CellsRowIndex(cellName);
                shape.DeleteRow((short)VisSectionIndices.visSectionProp, row);
            }
        }

        private static void RenameRow(Shape shape, string oldRowName, string newRowName, string newRowLabel) {
            if (shape.get_CellExists(oldRowName, (short)VisExistsFlags.visExistsAnywhere) != 0) {
                string tmp = shape.get_Cells(oldRowName).Formula;
                short row = shape.get_CellsRowIndex(oldRowName);
                shape.DeleteRow((short)VisSectionIndices.visSectionProp, row);
                Common.SetCellFormula(shape, newRowName, tmp);
                SetRowLabel(shape, newRowName, newRowLabel);
            }
        }

        private static void SetRowLabel(Shape shape, string rowName, string label) {
            if (shape.get_CellExists(rowName, (short)VisExistsFlags.visExistsAnywhere) != 0) {
                Cell c = shape.get_Cells(rowName);
                Cell cSRC = shape.get_CellsSRC((short)VisSectionIndices.visSectionProp, c.Row, (short)VisCellIndices.visCustPropsLabel);
                cSRC.FormulaU = Common.StringToFormulaForString(label);
            }
        }


        /// <summary>This method adds a right mouse action to a shape. The 
        /// right mouse action is added to the Actions section of the given
        /// shape.</summary>
        /// <param name="targetShape">Shape to add the action item to.</param>
        /// <param name="menuCaption">Caption for the newly created menu item.
        /// </param>
        /// <param name="menuAction">Action to be taken when the menu item is
        /// selected. This is a formula in universal syntax.</param>
        /// <param name="menuEnabled">Initial enabled state of the menu item.
        /// </param>
        /// <param name="menuChecked">Initial checked state of the menu item.
        /// </param>
        /// <param name="beginGroup">display a divider bar above the command 
        /// in the menu.</param>
        /// <param name="addToBottom">display the command at the bottom of the
        ///  menu.</param>
        private static void AddRightMouseAction(Shape targetShape, string menuCaption, string menuAction,
            bool menuEnabled, bool menuChecked, bool beginGroup, bool addToBottom) {

            const string DividerBarPrefix = "_";
            const string AddToBottomPrefix = "%";
            const string AcceleratorPrefix = "&";

            if (menuCaption == null || targetShape == null)
                return;

            short actionRow;
            short actionRows;
            string taggedMenuCaption;
            string rowCaption;
            string cleanMenuCaption;
            Cell actionCell;

            try {
                // the menuCaption string may need to be modified to include a
                // tag that indicates the menu should be at the bottom, or
                // should be preceeded by a separator line.
                taggedMenuCaption = menuCaption;
                if (taggedMenuCaption == null)
                    throw new ArgumentNullException("Menu caption is null");

                // strip modifier tokens from the caption
                cleanMenuCaption = menuCaption.Replace(AcceleratorPrefix, "");

                // Check if the right menu action item already exists.
                actionRows = targetShape.get_RowCount((short)VisSectionIndices.visSectionAction);

                bool actionExists = false;

                for (actionRow = 0; (actionExists == false) && (actionRow < actionRows); actionRow++) {
                    actionCell = targetShape.get_CellsSRC((short)VisSectionIndices.visSectionAction,
                        (short)(VisRowIndices.visRowAction + actionRow),
                        (short)VisCellIndices.visActionMenu);

                    rowCaption = Common.FormulaStringToString(actionCell.FormulaU);

                    // strip modifier tokens from the caption before compare
                    rowCaption = rowCaption.Replace(DividerBarPrefix, "");
                    rowCaption = rowCaption.Replace(AddToBottomPrefix, "");
                    rowCaption = rowCaption.Replace(AcceleratorPrefix, "");

                    if (rowCaption == cleanMenuCaption)
                        actionExists = true;
                }

                if (actionExists == false) {
                    // prefix underscore (_) to the caption to add a separator
                    // line above it.
                    if (beginGroup == true && taggedMenuCaption != null)
                        taggedMenuCaption = taggedMenuCaption.Insert(0, DividerBarPrefix);

                    // prefix percent (%) to the caption to add it to the 
                    // bottom of the menu.
                    if (addToBottom == true)
                        taggedMenuCaption = taggedMenuCaption.Insert(0, AddToBottomPrefix);

                    // Add a new action row to the shape.
                    actionRow = targetShape.AddRow((short)VisSectionIndices.visSectionAction,
                        (short)VisRowIndices.visRowLast,
                        (short)VisRowIndices.visRowAction);

                    // Set the menu caption.
                    actionCell = targetShape.get_CellsSRC((short)VisSectionIndices.visSectionAction,
                        actionRow,
                        (short)VisCellIndices.visActionMenu);
                    actionCell.FormulaU = Common.StringToFormulaForString(taggedMenuCaption);

                    // Set the action for the menu item.
                    actionCell = targetShape.get_CellsSRC((short)VisSectionIndices.visSectionAction,
                        actionRow,
                        (short)VisCellIndices.visActionAction);
                    actionCell.FormulaU = menuAction;

                    // Set the menu item's enabled/disabled state.
                    actionCell = targetShape.get_CellsSRC((short)VisSectionIndices.visSectionAction,
                        actionRow,
                        (short)VisCellIndices.visActionDisabled);
                    actionCell.set_ResultFromInt(VisUnitCodes.visNumber,
                        Convert.ToInt32(!menuEnabled, System.Globalization.CultureInfo.InvariantCulture));

                    // Set the menu item's checked state.
                    actionCell = targetShape.get_CellsSRC(
                        (short)VisSectionIndices.visSectionAction,
                        actionRow,
                        (short)VisCellIndices.visActionChecked);
                    actionCell.set_ResultFromInt(VisUnitCodes.visNumber,
                        Convert.ToInt32(menuChecked, System.Globalization.CultureInfo.InvariantCulture));
                }
            }
            catch (System.Runtime.InteropServices.COMException err) {
                System.Diagnostics.Debug.WriteLine(err.Message);
            }
        }

        // Removes the action associated with using the right mouse on a shape
        private static void RemoveRightMouseAction(Shape targetShape, string menuCaption) {
            const string DividerBarPrefix = "_";
            const string AddToBottomPrefix = "%";
            const string AcceleratorPrefix = "&";

            if (menuCaption == null || targetShape == null)
                return;

            short actionRow;
            short actionRows;
            string taggedMenuCaption;
            string rowCaption;
            string cleanMenuCaption;
            Cell actionCell;

            try {
                // the menuCaption string may need to be modified to include a
                // tag that indicates the menu should be at the bottom, or
                // should be preceeded by a separator line.
                taggedMenuCaption = menuCaption;
                if (taggedMenuCaption == null)
                    throw new ArgumentNullException("Menu caption is null");

                // strip modifier tokens from the caption
                cleanMenuCaption = menuCaption.Replace(AcceleratorPrefix, "");

                // Check if the right menu action item already exists.
                actionRows = targetShape.get_RowCount((short)VisSectionIndices.visSectionAction);

                bool actionExists = false;

                for (actionRow = 0; (actionExists == false) && (actionRow < actionRows); actionRow++) {
                    actionCell = targetShape.get_CellsSRC((short)VisSectionIndices.visSectionAction,
                        (short)(VisRowIndices.visRowAction + actionRow),
                        (short)VisCellIndices.visActionMenu);

                    rowCaption = Common.FormulaStringToString(actionCell.FormulaU);

                    // strip modifier tokens from the caption before compare
                    rowCaption = rowCaption.Replace(DividerBarPrefix, "");
                    rowCaption = rowCaption.Replace(AddToBottomPrefix, "");
                    rowCaption = rowCaption.Replace(AcceleratorPrefix, "");

                    if (rowCaption == cleanMenuCaption) {
                        targetShape.DeleteRow((short)VisSectionIndices.visSectionAction, (short)(VisRowIndices.visRowAction + actionRow));
                        actionExists = true;
                    }
                }
            }
            catch (System.Runtime.InteropServices.COMException err) {
                System.Diagnostics.Debug.WriteLine(err.Message);
            }
        }

        private static string ShapeDescription(Shape shape) {
            if (shape == null)
                return "Null";
            else
                return shape.ContainingPage.Name + " - " + shape.Text;
        }

        private static void EnsureTwoColumns(Shape shape, string cellName) {
            Table table = Common.GetCellTable(shape, cellName);
            if (!table.IsEmpty()) {
                while (table.GetNumColumns() < 2)
                    table.AddColumn();
                Common.SetCellTable(shape, cellName, table);
            }
        }
    }
}
