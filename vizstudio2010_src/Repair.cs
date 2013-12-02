using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;
using System.Threading;

namespace PathMaker {
    partial class Repair {
        private static string postUpgradeReportString;
        private static bool catastrophicError = false;

        internal static string DecodeFrom64(string encodedData) {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }

        // This is where you plug in your upgrade if need be.  These should only be 
        // necessary for schema changes - releases where the shape data has changed.
        // Add an if statement for your version as described below
        internal static bool UpgradeDocumentToCurrentSchemaVersion(Document document) {
            int docSchemaVersion = Common.GetDocumentSchemaVersion(document);
            postUpgradeReportString = "";
            catastrophicError = false;

            /**
             * Add another if statement here NOT AN ELSE IF so that the document will go
             * through all upgrades that are less than it's schema version
             * For example, two upgrades would look like this
             * if (docSchemaVersion < 1) 
             *     progressBarForm = new ProgressBarForm(DoVersion1Upgrade);
             *     if (progressBarForm.ShowDialog() != System.Windows.Forms.DialogResult.OK) 
             *         return false;
             * if (docSchemaVersion < 2)
             *     progressBarForm = new ProgressBarForm(DoVersion1Upgrade);
             *     if (progressBarForm.ShowDialog() != System.Windows.Forms.DialogResult.OK) 
             *         return false;
             * etc.
             */
            if (docSchemaVersion < 1) {
                // initial release of PathRunner
                ProgressBarForm progressBarForm = new ProgressBarForm("Upgrading to schema version 1", DoVersion1Upgrade, document);

                if (progressBarForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return false;
            }
            if (docSchemaVersion < 2) {
                ProgressBarForm progressBarForm = new ProgressBarForm("Upgrading to schema version 2", DoVersion2Upgrade, document);

                if (progressBarForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return false;
            }
            if (docSchemaVersion < 3) {
                ProgressBarForm progressBarForm = new ProgressBarForm("Upgrading to schema version 3", DoVersion3Upgrade, document);

                if (progressBarForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return false;
            }
            if (docSchemaVersion < 4) {
                ProgressBarForm progressBarForm = new ProgressBarForm("Upgrading to schema version 4", DoVersion4Upgrade, document);

                if (progressBarForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return false;                
            }
            if (docSchemaVersion < 5) {
                ProgressBarForm progressBarForm = new ProgressBarForm("Upgrading to schema version 5", DoVersion5Upgrade, document);

                if (progressBarForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return false;
            }
            if (docSchemaVersion < 6) {
                ProgressBarForm progressBarForm = new ProgressBarForm("Upgrading to schema version 6", DoVersion6Upgrade, document);

                if (progressBarForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return false;
            }

            if (docSchemaVersion < Common.GetResourceInt(Strings.PathMakerSchemaVersionRes)) {
                if (postUpgradeReportString.Length > 0) {
                    if (catastrophicError)
                        postUpgradeReportString =
                            "The upgrade of this file resulted in at least one major error that requires you " +
                            "go back into CID and fix something.  You'll need to do that and export to PathMaker " +
                            "again before proceeding.\n\n" + postUpgradeReportString;
                    else
                        postUpgradeReportString =
                            "The upgrade of this file resulted in one or more minor issues. " +
                            "To cleanup errors, search for shapes with the label \"" + Strings.ToBeDeletedLabel + "\". " +
                            "Those shapes will need to be removed and replaced if necessary - do not use them.\n\n" +
                            postUpgradeReportString;
                    System.Windows.Forms.MessageBox.Show(postUpgradeReportString, "Upgrade Problem Report");
                }

                if (catastrophicError)
                    return false;

                Common.SetDocumentSchemaVersion(document);
            }

            return true;
        }

        internal static bool DoVersion2Upgrade(Object arg, ProgressBarForm progressBarForm) {
            // needed to track the previous uid for cut/copy/paste
            // cannot be done on selection because with no selection you can still
            // right click on a page and select copy drawing
            Document document = arg as Document;

            int total = 0;
            foreach (Page page in document.Pages)
                total += page.Shapes.Count;

            int count = 0;
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    string uid = shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);
                    Common.SetCellString(shape, Strings.CutCopyPasteTempCellName, uid);
                    count++;
                }
                progressBarForm.SetProgressPercentage(count, total);
            }
            return true;
        }

        internal static bool DoVersion3Upgrade(Object arg, ProgressBarForm progressBarForm) {
            // Took version and last modified date off of DocTitle
            Document document = arg as Document;

            int total = 0;
            foreach (Page page in document.Pages)
                total += page.Shapes.Count;

            int count = 0;
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    if (Common.GetShapeType(shape) == ShapeTypes.DocTitle) {
                        DeleteOldPropertyCell(shape, "Prop.LastModifiedDate");
                        DeleteOldPropertyCell(shape, "Prop.Version");
                    }
                    count++;
                }
                progressBarForm.SetProgressPercentage(count, total);
            }
            return true;
        }


        internal static bool DoVersion4Upgrade(Object arg, ProgressBarForm progressBarForm) {
            // Adding text control handles to connector text
            Document document = arg as Document;

            int total = 0;
            foreach (Page page in document.Pages)
                total += page.Shapes.Count;

            int count = 0;
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    Common.FixConnectorTextControl(shape);
                    count++;
                }
                progressBarForm.SetProgressPercentage(count, total);
            }
            return true;
        }

        internal static bool DoVersion5Upgrade(Object arg, ProgressBarForm progressBarForm) {
            // Removing restrictions on change connector label text
            Document document = arg as Document;

            int total = 0;
            foreach (Page page in document.Pages)
                total += page.Shapes.Count;

            int count = 0;
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    if (Common.GetShapeType(shape) == ShapeTypes.Connector) {
                        shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                            (short)VisRowIndices.visRowLock,
                            (short)VisCellIndices.visLockTextEdit).FormulaU = "0"; 
                        shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                            (short)VisRowIndices.visRowLock,
                            (short)VisCellIndices.visSLOSplittable).FormulaU = "1";
                    }
                    count++;
                }
                progressBarForm.SetProgressPercentage(count, total);
            }
            return true;
        }

        internal static bool DoVersion6Upgrade(Object arg, ProgressBarForm progressBarForm) {
            // Added State Sort Order to DefaultSettings
            Document document = arg as Document;

            int total = 0;
            foreach (Page page in document.Pages)
                total += page.Shapes.Count;

            int count = 0;
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    if (Common.GetShapeType(shape) == ShapeTypes.Start) {
                        Table table = Common.GetCellTable(shape, ShapeProperties.Start.DefaultSettings);
                        if (!table.IsEmpty()) {
                            int row = table.AddRow();
                            table.SetData(row, (int)TableColumns.NameValuePairs.Name, Strings.DefaultSettingsStateSortOrder);
                            table.SetData(row, (int)TableColumns.NameValuePairs.Value, Strings.StateSortOrderAlphaNumerical);
                            Common.SetCellTable(shape, ShapeProperties.Start.DefaultSettings, table);
                        }
                    }
                    count++;
                }
                progressBarForm.SetProgressPercentage(count, total);
            }
            return true;
        }

        // tried to do this as a BackgroundWorker - way too slow.  Need to use the ProgressBarForm this way for 
        // good performance.
        internal static bool DoVersion1Upgrade(Object arg, ProgressBarForm progressBarForm) {
            Document document = arg as Document;

            // these should only ever be used here...
            const string OffPageReferenceShapeName = "Off-page reference";
            const string DynamicConnectorShapeName = "Dynamic connector";
            const string OnPageReferenceIncomingShapeName = "On-page reference.Incoming";
            const string OnPageReferenceOutgoingShapeName = "On-page reference.Outgoing";
            const string SubDialogCallShapeName = "Call Sub-Dialog";
            const string CommentShapeName = "Comment";
            const string PageShapeName = "Sheet";
            const string ReturnShapeName = "Return";
            const string PlaceHolderShapeName = "Placeholder";
            const string DMTypeCellName = "Prop.DMType";
            const string DocumentTitleShapeName = "Document Title";
            const string ChangeLogShapeName = "Change Log";
            const string TransferShapeName = "Transfer";
            const string TransferShapeName2 = "Terminator";
            const string HangUpShapeName = "Hang up";

            // most transitions can point to terminals (hangup/transfer/return) but not 
            // maxhandlers - so we need a list without terminals to use for maxhandler conversions
            Dictionary<string, List<string>> uniqueIdMap = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> uniqueIdMapNoTerminals = new Dictionary<string, List<string>>();

            int total = 0;
            foreach (Page page in document.Pages)
                total += page.Shapes.Count;

            total = total * 4; // add some extra for the stuff done outside of this loop at the end

            int counter = 0;
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    ShapeTypes shapeType = ShapeTypes.None;

                    progressBarForm.SetProgressPercentage(counter++, total);
                    if (progressBarForm.Cancelled)
                        return false;

                    string dmType = Common.GetCellString(shape, DMTypeCellName);

                    if (dmType.Equals("IA", StringComparison.Ordinal))
                        shapeType = ShapeTypes.Interaction;
                    else if (dmType.Equals("PP", StringComparison.Ordinal))
                        shapeType = ShapeTypes.Play;
                    else if (dmType.Equals("DE", StringComparison.Ordinal))
                        shapeType = ShapeTypes.Decision;
                    else if (dmType.Equals("DR", StringComparison.Ordinal))
                        shapeType = ShapeTypes.Data;
                    else if (dmType.Equals("SD", StringComparison.Ordinal))
                        shapeType = ShapeTypes.SubDialog;
                    else if (dmType.Equals("ST", StringComparison.Ordinal))
                        shapeType = ShapeTypes.Start;

                    if (shapeType == ShapeTypes.None) {
                        // Figure out what else it could be...
                        if (shape.Name.StartsWith(DocumentTitleShapeName))
                            shapeType = ShapeTypes.DocTitle;
                        else if (shape.Name.StartsWith(ChangeLogShapeName))
                            shapeType = ShapeTypes.ChangeLog;
                        else if (shape.Name.StartsWith(TransferShapeName) || shape.Name.StartsWith(TransferShapeName2))
                            shapeType = ShapeTypes.Transfer;
                        else if (shape.Name.StartsWith(HangUpShapeName))
                            shapeType = ShapeTypes.HangUp;
                        else if (shape.Name.StartsWith(OffPageReferenceShapeName))
                            shapeType = ShapeTypes.OffPageRef;
                        else if (shape.Name.StartsWith(DynamicConnectorShapeName))
                            shapeType = ShapeTypes.Connector;
                        else if (shape.Name.StartsWith(OnPageReferenceIncomingShapeName))
                            shapeType = ShapeTypes.OnPageRefIn;
                        else if (shape.Name.StartsWith(OnPageReferenceOutgoingShapeName))
                            shapeType = ShapeTypes.OnPageRefOut;
                        else if (shape.Name.StartsWith(SubDialogCallShapeName))
                            shapeType = ShapeTypes.CallSubDialog;
                        else if (shape.Name.StartsWith(CommentShapeName))
                            shapeType = ShapeTypes.Comment;
                        else if (shape.Name.StartsWith(PageShapeName))
                            shapeType = ShapeTypes.Page;
                        else if (shape.Name.StartsWith(ReturnShapeName))
                            shapeType = ShapeTypes.Return;
                        else if (shape.Name.StartsWith(PlaceHolderShapeName))
                            shapeType = ShapeTypes.Placeholder;
                        else {
                            postUpgradeReportString += "Shape could not be upgraded properly - " + ShapeDescription(shape) + "\n\n";
                            shape.Text = Strings.ToBeDeletedLabel;
                            continue;
                        }
                    }

                    System.Diagnostics.Debug.Assert(shapeType != ShapeTypes.None);

                    // we really don't need to do anything with the page shapes
                    // this also includes the rectangles drawn for revision history
                    if (shapeType == ShapeTypes.Page)
                        continue;

                    // remove the action properties on all shapes
                    RemoveRightMouseAction(shape, "P&roperties");

                    // and fix the actions for the ones we care about
                    if (shapeType != ShapeTypes.OffPageRef && shapeType != ShapeTypes.HangUp &&
                        shapeType != ShapeTypes.OnPageRefIn && shapeType != ShapeTypes.OnPageRefOut &&
                        shapeType != ShapeTypes.Return && shapeType != ShapeTypes.Transfer &&
                        shapeType != ShapeTypes.Comment && shapeType != ShapeTypes.Placeholder &&
                        shapeType != ShapeTypes.Connector)
                        AddRightMouseAction(shape, "P&roperties", "RUNADDONWARGS(\"QueueMarkerEvent\",\"/PathMaker /CMD=2\")",
                            true, false, false, false);

                    // delete old rows no longer used
                    DeleteOldPropertyCell(shape, "Prop.TestPlanSetting");
                    DeleteOldPropertyCell(shape, "Prop.CheckPoints");
                    DeleteOldPropertyCell(shape, "Prop.UpdateData");
                    DeleteOldPropertyCell(shape, "Prop.LineName");
                    DeleteOldPropertyCell(shape, "Prop.Input");
                    DeleteOldPropertyCell(shape, "Prop.Output");
                    DeleteOldPropertyCell(shape, "Prop.OSDM");
                    DeleteOldPropertyCell(shape, "Prop.Destination");
                    DeleteOldPropertyCell(shape, "Prop.ColumnData");
                    DeleteOldPropertyCell(shape, "Prop.EnteringFrom");
                    DeleteOldPropertyCell(shape, "Prop.StartingDialogState");
                    DeleteOldPropertyCell(shape, "Prop.StartingDM");
                    DeleteOldPropertyCell(shape, "Prop.ChangeDate");
                    DeleteOldPropertyCell(shape, "Prop.InitialDate");
                    DeleteOldPropertyCell(shape, "Prop.LogoFilePath");
                    DeleteOldPropertyCell(shape, "Prop.LastUpdate");

                    if (shapeType == ShapeTypes.CallSubDialog) {
                        // none of these are used...
                        DeleteOldPropertyCell(shape, "Prop.DMName");
                        DeleteOldPropertyCell(shape, "Prop.EnteringFrom");
                        DeleteOldPropertyCell(shape, "Prop.StartingDialogState");
                        DeleteOldPropertyCell(shape, "Prop.Notes");
                        DeleteOldPropertyCell(shape, "Prop.LastUpdate");
                        DeleteOldPropertyCell(shape, "Prop.Transitions");
                    }
                    else if (shapeType == ShapeTypes.SubDialog)
                        DeleteOldPropertyCell(shape, "Prop.Transitions");

                    // Add a new cell containing the name of the subdialog being called
                    // Later it will be converted to a UID
                    if (shapeType == ShapeTypes.CallSubDialog)
                        Common.SetCellString(shape, ShapeProperties.CallSubDialog.SubDialogUID, shape.Text);

                    // rename rows that are confusing
                    RenameRow(shape, "Prop.DialogParameters", ShapeProperties.SpecialSettings, "Special Settings");
                    RenameRow(shape, "Prop.DMName", ShapeProperties.StateId, "State Name");
                    RenameRow(shape, "Prop.DMType", ShapeProperties.ShapeType, "Shape Type");
                    RenameRow(shape, "Prop.InputData", ShapeProperties.Start.Initialization, "Initialization");
                    RenameRow(shape, "Prop.GlobalCommands", ShapeProperties.CommandTransitions, "Command Transitions");
                    RenameRow(shape, "Prop.GlobalPrompts", ShapeProperties.PromptTypes, "Prompt Types");
                    RenameRow(shape, "Prop.GlobalConfirmations", ShapeProperties.ConfirmationPrompts, "Confirmation Prompts");
                    RenameRow(shape, "Prop.TitleLine1", ShapeProperties.DocTitle.ClientName, "Client Name");
                    RenameRow(shape, "Prop.TitleLine2", ShapeProperties.DocTitle.ProjectName, "Project Name");
                    if (shapeType == ShapeTypes.Interaction) {
                        RenameRow(shape, "Prop.Transitions", "Prop.CommandTransitions", "Command Transitions");
                        RenameRow(shape, "Prop.Prompts", "Prop.PromptTypes", "Prompt Types");
                    }

                    // undo encoding we'll never use
                    UnencodeProperty(shape, ShapeProperties.ChangeLog.Changes);
                    UnencodeProperty(shape, ShapeProperties.Start.DefaultSettings);
                    UnencodeProperty(shape, ShapeProperties.Start.Initialization);
                    UnencodeProperty(shape, ShapeProperties.MaxHandling);
                    UnencodeProperty(shape, ShapeProperties.Play.Prompts);
                    UnencodeProperty(shape, ShapeProperties.PromptTypes);
                    UnencodeProperty(shape, ShapeProperties.Transitions);
                    UnencodeProperty(shape, ShapeProperties.CommandTransitions);
                    UnencodeProperty(shape, ShapeProperties.SpecialSettings);
                    UnencodeProperty(shape, ShapeProperties.DeveloperNotes);
                    UnencodeProperty(shape, ShapeProperties.ConfirmationPrompts);
                    UnencodeProperty(shape, ShapeProperties.LastUpdate);

                    // make each shape get a GUID
                    string uniqueId = shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);
                    // off page refs have two shapes with the same name and we don't need them for this
                    if (shapeType != ShapeTypes.OffPageRef) {
                        List<string> list = null;

                        if (uniqueIdMap.TryGetValue(shape.Name, out list))
                            list.Add(uniqueId);
                        else {
                            list = new List<string>();
                            list.Add(uniqueId);
                            uniqueIdMap.Add(shape.Name, list);
                        }

                        if (shapeType != ShapeTypes.Placeholder && shapeType != ShapeTypes.HangUp &&
                            shapeType != ShapeTypes.Transfer && shapeType != ShapeTypes.Return) {
                            list = null;
                            if (uniqueIdMapNoTerminals.TryGetValue(shape.Name, out list))
                                list.Add(uniqueId);
                            else {
                                list = new List<string>();
                                list.Add(uniqueId);
                                uniqueIdMapNoTerminals.Add(shape.Name, list);
                            }
                        }
                    }

                    // change all double click handlers
                    if (shapeType == ShapeTypes.DocTitle || shapeType == ShapeTypes.ChangeLog ||
                        shapeType == ShapeTypes.Start || shapeType == ShapeTypes.Interaction ||
                        shapeType == ShapeTypes.Play || shapeType == ShapeTypes.Decision ||
                        shapeType == ShapeTypes.Data || shapeType == ShapeTypes.SubDialog ||
                        shapeType == ShapeTypes.CallSubDialog)
                        Common.SetCellFormula(shape, "EventDblClick", "RUNADDONWARGS(\"QueueMarkerEvent\",\"/PathMaker /CMD=1\")");

                    // global transitions need a new column to match the interaction transitions
                    if (shapeType == ShapeTypes.Start) {
                        Table table = Common.GetCellTable(shape, ShapeProperties.CommandTransitions);
                        if (!table.IsEmpty()) {
                            int newCol = table.AddColumn();
                            System.Diagnostics.Debug.Assert(newCol == 18);
                            // copies the goto column to the line column to match interactions
                            table.CopyColumn(7, 18);
                            Common.SetCellTable(shape, ShapeProperties.CommandTransitions, table);
                        }
                    }

                    // unlock most shape text edits - remember play and interaction are multiple shapes
                    if (shapeType == ShapeTypes.Data || shapeType == ShapeTypes.Decision ||
                        shapeType == ShapeTypes.Interaction || shapeType == ShapeTypes.Play || shapeType == ShapeTypes.SubDialog)
                        shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                            (short)VisRowIndices.visRowLock,
                            (short)VisCellIndices.visLockTextEdit).FormulaU = "0";
                    if (shapeType == ShapeTypes.Interaction || shapeType == ShapeTypes.Play) {
                        shape.Shapes[1].get_CellsSRC((short)VisSectionIndices.visSectionObject,
                            (short)VisRowIndices.visRowLock,
                            (short)VisCellIndices.visLockTextEdit).FormulaU = "0";
                        // and bring it to front so you are editing the shape name by default
                        shape.Shapes[1].BringToFront();
                    }
                    // the old stuff never locked call sub dialogs - we want to
                    if (shapeType == ShapeTypes.CallSubDialog)
                        shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                            (short)VisRowIndices.visRowLock,
                            (short)VisCellIndices.visLockTextEdit).FormulaU = "1";

                    // normalize/fix all the grid datastore stuff - get rid of unused columns, etc.
                    // remove plus, minus, goto, update_plus, update_minus, cp_idx
                    RemoveColumnsFromCellTable(shape, ShapeProperties.CommandTransitions, new int[6] { 3, 5, 7, 12, 14, 19 });
                    // remove plus, minus, update_plus, update_minus
                    RemoveColumnsFromCellTable(shape, ShapeProperties.ConfirmationPrompts, new int[4] { 1, 3, 7, 9 });
                    // remove plus, minus, updateplus, updateminus
                    RemoveColumnsFromCellTable(shape, ShapeProperties.PromptTypes, new int[4] { 1, 3, 7, 9 });
                    // remove plus, minus, updateplus, updateminus, cp_idx
                    RemoveColumnsFromCellTable(shape, ShapeProperties.Play.Prompts, new int[5] { 0, 2, 5, 7, 10 });
                    // remove plus, minus, goto, updateplus, updateminus
                    RemoveColumnsFromCellTable(shape, ShapeProperties.Transitions, new int[5] { 0, 2, 4, 5, 7 });
                    // remove highlight index column
                    RemoveColumnsFromCellTable(shape, ShapeProperties.ChangeLog.Changes, new int[1] { 5 });

                    // clear any old changelog descriptions from the page itself
                    if (shapeType == ShapeTypes.ChangeLog) {
                        Page changeLogPage = shape.ContainingPage;
                        for (int count = changeLogPage.Shapes.Count; count > 0; count--)
                            if (changeLogPage.Shapes[count] != shape)
                                changeLogPage.Shapes[count].Delete();
                    }

                    // Start has the max handlers in a different order than the interaction states - fix it here
                    if (shapeType == ShapeTypes.Start)
                        SwapRowsInCellTable(shape, ShapeProperties.MaxHandling, 0, 1);

                    // Get rid of expressware checkpoints default setting
                    if (shapeType == ShapeTypes.Start) {
                        Table table = Common.GetCellTable(shape, ShapeProperties.Start.DefaultSettings);
                        for (int r = 0; r < table.GetNumRows(); r++) {
                            if (table.GetData(r, 0).Equals("ExpressWare Checkpoints")) {
                                table.DeleteRow(r);
                                break;
                            }
                        }
                    }

                    // apparently subDialogs developer notes were not written out as a two column entry (one for date of change)
                    if (shapeType == ShapeTypes.SubDialog)
                        EnsureTwoColumns(shape, ShapeProperties.DeveloperNotes);

                    // set shape type - this will add it if necessary - make sure to use int value
                    Common.SetCellFormula(shape, ShapeProperties.ShapeType, ((int)shapeType).ToString());
                    SetRowLabel(shape, ShapeProperties.ShapeType, "Shape Type");
                }
            }

            // we remove change log stuff above - need to recount the remaining totals
            total = 0;
            foreach (Page page in document.Pages)
                total += page.Shapes.Count;
            total = counter + (total * 3);  // we'll go through 3 times below

            // The following stuff needed to wait until every state had a GUID

            // change all references from state ids/names to use GUIDs
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    // column 13 should now be the link column
                    ReplaceStateIdsWithUniqueIds(shape, uniqueIdMap, ShapeProperties.CommandTransitions, 13);
                    // column 5 should be the link column
                    ReplaceStateIdsWithUniqueIds(shape, uniqueIdMap, ShapeProperties.Transitions, 5);
                    // column 3 should be the goto column
                    ReplaceStateIdsWithUniqueIds(shape, uniqueIdMapNoTerminals, ShapeProperties.MaxHandling, 3);
                    // it's a singleton, not a table - only exists on call subdialogs
                    ReplaceStateIdWithUniqueIds(shape, uniqueIdMap, ShapeProperties.CallSubDialog.SubDialogUID);
                    progressBarForm.SetProgressPercentage(counter++, total);
                    if (progressBarForm.Cancelled)
                        return false;
                }
            }

            // fix off page connectors
            Dictionary<string, Shape> offPageShapeNameMap = new Dictionary<string, Shape>();
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    if (Common.GetShapeType(shape) == ShapeTypes.OffPageRef) {
                        // common fixes
                        // hyperlinks point to pages, not shapes, even if the shape is deleted - drop these
                        short row;
                        if (shape.get_CellExists(ShapeProperties.OffPageRef.HyperLink, (short)VisExistsFlags.visExistsAnywhere) != 0) {
                            row = shape.get_CellsRowIndex(ShapeProperties.OffPageRef.HyperLink);
                            shape.DeleteRow((short)VisSectionIndices.visSectionHyperlink, row);
                        }
                        if (shape.get_CellExists("Hyperlink.PageLinkDest", (short)VisExistsFlags.visExistsAnywhere) != 0) {
                            row = shape.get_CellsRowIndex("Hyperlink.PageLinkDest");
                            shape.DeleteRow((short)VisSectionIndices.visSectionHyperlink, row);
                        }
                        // the standard off-page connector dbl click handler works fine
                        Common.SetCellFormula(shape, ShapeProperties.EventDblClick, Strings.OffPageConnectorDblClickCommand);
                        // the standard off-page connector drop handler works fine
                        Common.SetCellFormula(shape, ShapeProperties.EventDrop, Strings.OffPageConnectorDropCommand);
                        // keep the text synchronized
                        Common.SetCellFormula(shape, ShapeProperties.TheText, Strings.OffPageConnectorTextCommand);
                        // delete actions to change shape to circle/arrow
                        row = shape.get_CellsRowIndex("Actions.Row_3");
                        shape.DeleteRow((short)VisSectionIndices.visSectionAction, row);
                        row = shape.get_CellsRowIndex("Actions.Row_4");
                        shape.DeleteRow((short)VisSectionIndices.visSectionAction, row);

                        Shape target;
                        if (offPageShapeNameMap.TryGetValue(shape.Text, out target)) {
                            // we have a pair...
                            Common.SetCellString(shape, ShapeProperties.OffPageConnectorShapeID,
                                shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
                            Common.SetCellString(shape, ShapeProperties.OffPageConnectorDestinationPageID,
                                target.ContainingPage.PageSheet.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
                            Common.SetCellString(shape, ShapeProperties.OffPageConnectorDestinationShapeID,
                                target.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));

                            Common.SetCellString(target, ShapeProperties.OffPageConnectorShapeID,
                                target.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
                            Common.SetCellString(target, ShapeProperties.OffPageConnectorDestinationPageID,
                                shape.ContainingPage.PageSheet.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
                            Common.SetCellString(target, ShapeProperties.OffPageConnectorDestinationShapeID,
                                shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));

                            offPageShapeNameMap.Remove(shape.Text);
                        }
                        else
                            offPageShapeNameMap.Add(shape.Text, shape);
                    }
                    progressBarForm.SetProgressPercentage(counter++, total);
                    if (progressBarForm.Cancelled)
                        return false;
                }
            }

            // In CID you could have on-page connectors backwards - check for them here
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    if (Common.GetShapeType(shape) == ShapeTypes.OnPageRefIn) {
                        bool arrowSide = false;
                        foreach (Connect c in shape.FromConnects) {
                            // determine which end of the connector is connected to the nonConnectorShadow
                            if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                                arrowSide = true;
                        }
                        // if we are on the arrow side of the connector, that means this is an input connector
                        if (arrowSide) {
                            postUpgradeReportString += "On page reference input can't have inputs - " + ShapeDescription(shape) + "\n\n";
                            catastrophicError = true;
                        }
                    }
                    else if (Common.GetShapeType(shape) == ShapeTypes.OnPageRefOut) {
                        bool arrowSide = true;
                        foreach (Connect c in shape.FromConnects) {
                            // determine which end of the connector is connected to the nonConnectorShadow
                            if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                                arrowSide = true;

                            // if we are on the non-arrow side of the connector, that means this is an output connector
                            if (!arrowSide) {
                                postUpgradeReportString += "On page reference output can't have outputs - " + ShapeDescription(shape) + "\n\n";
                                catastrophicError = true;
                            }
                        }
                    }
                }
            }

            // any left over are not hooked up - this is a problem...
            if (offPageShapeNameMap.Count > 0)
                for (int i = 0; i < offPageShapeNameMap.Count; i++) {
                    Shape noMatch = offPageShapeNameMap.Values.ElementAt(i);
                    postUpgradeReportString += "No matching off page connector found for " + ShapeDescription(noMatch) + "\n\n";
                    noMatch.Text = Strings.ToBeDeletedLabel;
                }

            // Last thing to do - now that we can use all the new column names, we want to find all table fields using $$ and fix them
            foreach (Page page in document.Pages) {
                foreach (Shape shape in page.Shapes) {
                    FixDollarDollarTableFields(shape, ShapeProperties.CommandTransitions);
                    FixDollarDollarTableFields(shape, ShapeProperties.PromptTypes);
                    FixDollarDollarTableFields(shape, ShapeProperties.ConfirmationPrompts);
                    FixDollarDollarTableFields(shape, ShapeProperties.MaxHandling);
                    FixDollarDollarTableFields(shape, ShapeProperties.DeveloperNotes);
                    FixDollarDollarTableFields(shape, ShapeProperties.SpecialSettings);
                    FixDollarDollarTableFields(shape, ShapeProperties.Transitions);
                    FixDollarDollarTableFields(shape, ShapeProperties.Play.Prompts);
                    FixDollarDollarTableFields(shape, ShapeProperties.Start.DefaultSettings);
                    FixDollarDollarTableFields(shape, ShapeProperties.Start.Initialization);
                    progressBarForm.SetProgressPercentage(counter++, total);
                    if (progressBarForm.Cancelled)
                        return false;
                }
            }
            progressBarForm.SetProgressPercentage(100, 100);

            progressBarForm.Hide();
            progressBarForm = null;
            return true;
        }
    }
}
