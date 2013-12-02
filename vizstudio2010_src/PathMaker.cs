using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    public partial class PathMaker : Form {
        private const short visEvtAdd = -32768;
        private EventSink eventHandler = null;
        private static Dictionary<string, Shadow> shadowShapeMap = null;
        private static bool OneTimeOnlyActivateHack = true;
        private static bool SuspendConnectHandlingToMoveAConnectionPoint = false;

        private static PathRunnerBackgroundWorker pathRunnerBackgroundWorker = null;

        public PathMaker() {
            InitializeComponent();

            // set up the application event handlers - these only need to be done once
            eventHandler = new EventSink();
            visioControl.Document.Application.EventList.AddAdvise((short)VisEventCodes.visEvtApp + (short)VisEventCodes.visEvtMarker, eventHandler, "", "");
            eventHandler.onApplicationMarker += new EventSink.VisioApplicationEventHandler(OnApplicationMarker);
            visioControl.Document.Application.EventList.AddAdvise((short)VisEventCodes.visEvtCodeExitScope, eventHandler, "", "");
            eventHandler.onExitScope += new EventSink.VisioScopeEventHandler(OnExitScope);
            visioControl.Document.Application.EventList.AddAdvise((short)VisEventCodes.visEvtCodeEnterScope, eventHandler, "", "");

            pathRunnerBackgroundWorker = new PathRunnerBackgroundWorker();
            pathRunnerBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnPathRunnerBackgroundWorkerCompleted);
            pathRunnerBackgroundWorker.WorkerSupportsCancellation = true;
        }

        private void PathMaker_Load(object sender, EventArgs e) {
            visioControl.Document.Application.Settings.EnableAutoConnect = false;

            string startupFileName = null;
            string[] startupArgs = null;
            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null)
                startupArgs = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
            if (startupArgs != null && startupArgs.Length > 0)
                startupFileName = Uri.UnescapeDataString(startupArgs[0]);

            if (startupFileName == null)
                visioControl.Src = System.Windows.Forms.Application.StartupPath + @"\\" + Strings.VisioTemplateFile;
            else
                visioControl.Src = startupFileName;

            // if I don't do this, the stencil window is active when we come up
            // this makes it so the document window is active
            visioControl.Document.Application.Windows[1].Activate();

            OnSrcDocumentChange();
        }

        private void SetupDocumentEventHandlersAndRemoveAccelerators() {
            try {
                visioControl.Document.EventList.AddAdvise((short)VisEventCodes.visEvtShape + (short)VisEventCodes.visEvtDel, eventHandler, "", "");
                eventHandler.onBeforeShapeDelete -= new EventSink.VisioShapeEventHandler(OnBeforeShapeDelete);
                eventHandler.onBeforeShapeDelete += new EventSink.VisioShapeEventHandler(OnBeforeShapeDelete);

                visioControl.Document.EventList.AddAdvise((short)VisEventCodes.visEvtShape + visEvtAdd, eventHandler, "", "");
                eventHandler.onShapeAdd -= new EventSink.VisioShapeEventHandler(OnShapeAdd);
                eventHandler.onShapeAdd += new EventSink.VisioShapeEventHandler(OnShapeAdd);

                visioControl.Document.EventList.AddAdvise((short)VisEventCodes.visEvtCodeShapeExitTextEdit, eventHandler, "", "");
                eventHandler.onShapeExitTextEdit -= new EventSink.VisioShapeEventHandler(OnShapeExitTextEdit);
                eventHandler.onShapeExitTextEdit += new EventSink.VisioShapeEventHandler(OnShapeExitTextEdit);

                visioControl.Document.EventList.AddAdvise((short)VisEventCodes.visEvtConnect + visEvtAdd, eventHandler, "", "");
                eventHandler.onConnectAdd -= new EventSink.VisioConnectEventHandler(OnConnectAdd);
                eventHandler.onConnectAdd += new EventSink.VisioConnectEventHandler(OnConnectAdd);

                visioControl.Document.EventList.AddAdvise((short)VisEventCodes.visEvtConnect + (short)VisEventCodes.visEvtDel, eventHandler, "", "");
                eventHandler.onConnectDelete -= new EventSink.VisioConnectEventHandler(OnConnectDelete);
                eventHandler.onConnectDelete += new EventSink.VisioConnectEventHandler(OnConnectDelete);

                visioControl.Document.EventList.AddAdvise((short)VisEventCodes.visEvtPage + (short)VisEventCodes.visEvtDel, eventHandler, "", "");
                eventHandler.onBeforePageDelete -= new EventSink.VisioPageEventHandler(OnBeforePageDelete);
                eventHandler.onBeforePageDelete += new EventSink.VisioPageEventHandler(OnBeforePageDelete);

                visioControl.Document.EventList.AddAdvise((short)VisEventCodes.visEvtPage + visEvtAdd, eventHandler, "", "");
                eventHandler.onPageAdd -= new EventSink.VisioPageEventHandler(OnPageAdd);
                eventHandler.onPageAdd += new EventSink.VisioPageEventHandler(OnPageAdd);

                visioControl.Document.EventList.AddAdvise((short)VisEventCodes.visEvtPage + (short)VisEventCodes.visEvtMod, eventHandler, "", "");
                eventHandler.onPageAdd -= new EventSink.VisioPageEventHandler(OnPageMod);
                eventHandler.onPageAdd += new EventSink.VisioPageEventHandler(OnPageMod);

                // delete accelerators in Visio we want to disable
                UIObject uiObject = visioControl.Document.Application.BuiltInMenus;
                AccelTable accelTable = uiObject.AccelTables.get_ItemAtID((int)VisUIObjSets.visUIObjSetInPlace);
                AccelItems accelItems = accelTable.AccelItems;

                for (int i = accelItems.Count - 1; i >= 0; i--) {
                    AccelItem item = accelItems[i];
                    if (item.CmdNum == (short)VisUICmds.visCmdToolsSpelling ||
                        item.CmdNum == (short)VisUICmds.visCmdSpellingOptionsDlg ||
                        item.CmdNum == (short)VisUICmds.visCmdToolsRunVBE)
                        item.Delete();
                }

                visioControl.Document.SetCustomMenus(uiObject);
            }
            catch (Exception err) {
                System.Diagnostics.Debug.WriteLine(err.Message);
            }
        }

        // needs to be called when the document changes and on startup but startup has to be after the 
        // window is visible - otherwise the oldStencil and document stencil stuff doesn't work
        private void StencilCleanup() {
            // unload any pre-existing pathmaker stencils
            try {
                Document oldStencil = visioControl.Document.Application.Documents[Strings.StencilFileName];
                oldStencil.Close();
            }
            catch {
                // means it didn't exist - ignore
            }

            // make sure we load the latest stencil if we need to
            string stencilFileName = System.Windows.Forms.Application.StartupPath + "\\" + Strings.StencilFileName;
            visioControl.Document.Application.Documents.OpenEx(stencilFileName, (short)VisOpenSaveArgs.visOpenDocked);

            // Hide the document stencil
            try {
                visioControl.Document.Application.DoCmd((short)VisUICmds.visCmdHideDocumentStencil);
            }
            catch {
                FixHourGlassAfterConfusingVisio();
            }

        }

        private void OnSrcDocumentChange() {
            int pathMakerVersion = Common.GetDocumentSchemaVersion(visioControl.Document);
            bool repaired = false;

            StatePrefixAndNumberManager.Initialize();

            if (pathMakerVersion < Common.GetResourceInt(Strings.PathMakerSchemaVersionRes))
                if (!Repair.UpgradeDocumentToCurrentSchemaVersion(visioControl.Document))
                    visioControl.Src = System.Windows.Forms.Application.StartupPath + @"\\" + Strings.VisioTemplateFile;
                else
                    repaired = true;

            if (pathMakerVersion > Common.GetResourceInt(Strings.PathMakerSchemaVersionRes)) {
                Common.ErrorMessage("This file was created using a newer schema than this version of\n"
                                  + "PathMaker supports.  Upgrade to a newer version to edit this file.");
                visioControl.Src = System.Windows.Forms.Application.StartupPath + @"\\" + Strings.VisioTemplateFile;
                repaired = false;
            }

            try {
                // For some reason our template will cause an exception if you add a new
                // page to it before you have put a shape on it.  My guess is something to 
                // do with it being a vst but it only happens in the activeX control, not
                // standard visio.  Either way, this little trick of putting a connector on and 
                // deleting it seems to solve the problem.  One other note, the shape you drop 
                // matters.  A comment shape didn't work...
                Page page = visioControl.Document.Application.ActivePage;
                Document stencil = visioControl.Document.Application.Documents[Strings.StencilFileName];
                if (page != null) {
                    Shape shape = page.Drop(stencil.Masters["Dynamic connector"], 1, 1);
                    shape.Delete();
                }
            }
            catch {
                // fails if the window isn't up yet - can be ignored
            }

            // when starting from the template, the doc title and change log cut copy paste temp
            // data isn't set - so let's set it here
            foreach (Page page in visioControl.Document.Pages)
                foreach (Shape shape in page.Shapes) {
                    string uid = shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);
                    Common.SetCellString(shape, Strings.CutCopyPasteTempCellName, uid);
                }

            // if we didn't do a repair, make it so the hack above doesn't mean we have to save
            if (!repaired)
                visioControl.Document.Saved = true;

            // set up our gotoPageComboBox on the toolbar
            gotoPageComboBox.Items.Clear();
            gotoPageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (Page p in visioControl.Document.Pages)
                gotoPageComboBox.Items.Add(p.Name);
            if (visioControl.Document.Application.ActivePage != null) {
                string pageName = visioControl.Document.Application.ActivePage.Name;
                int index = visioControl.Document.Pages[pageName].Index;
                gotoPageComboBox.SelectedIndex = index - 1;
            }

            StencilCleanup();

            // set titlebar
            string filename = System.IO.Path.GetFileName(visioControl.Src);
            if (filename.Contains(Strings.VisioTemplateFileSuffix))
                Text = Common.StripExtensionFileName(Strings.DefaultFileName) + Strings.TitleBarSuffix;
            else
                Text = Common.StripExtensionFileName(filename) + Strings.TitleBarSuffix;

            // we rebuild this every time we get a new document
            BuildShadowShapeMap();
            // and add the event handlers which are associated with the document (as opposed to the application)
            SetupDocumentEventHandlersAndRemoveAccelerators();
        }

        public static List<Shadow> LookupAllShadows() {
            List<Shadow> list = new List<Shadow>();

            list.AddRange(shadowShapeMap.Values);
            return list;
        }

        public static Shadow LookupShadowByShape(Shape shape) {
            string uid = shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);
            return LookupShadowByUID(uid);
        }

        public static Shadow LookupShadowByUID(string uid) {
            Shadow shadow;

            if (shadowShapeMap.TryGetValue(uid, out shadow))
                return shadow;
            else
                return null;
        }

        public static StartShadow LookupStartShadow() {
            foreach (Shadow shadow in shadowShapeMap.Values) {
                StartShadow startShadow = shadow as StartShadow;
                if (startShadow != null)
                    return shadow as StartShadow;
            }
            return null;
        }

        public static DocTitleShadow LookupDocTitleShadow() {
            foreach (Shadow shadow in shadowShapeMap.Values) {
                DocTitleShadow docTitleShadow = shadow as DocTitleShadow;
                if (docTitleShadow != null)
                    return shadow as DocTitleShadow;
            }
            return null;
        }

        public static ChangeLogShadow LookupChangeLogShadow() {
            foreach (Shadow shadow in shadowShapeMap.Values) {
                ChangeLogShadow changeLogShadow = shadow as ChangeLogShadow;
                if (changeLogShadow != null)
                    return shadow as ChangeLogShadow;
            }
            return null;
        }

        public static List<Shadow> LookupShadowsByShapeType(ShapeTypes shapeType) {
            List<Shadow> list = new List<Shadow>();
            foreach (Shadow shadow in shadowShapeMap.Values)
                if (shadow.GetShapeType() == shapeType)
                    list.Add(shadow);
            return list;
        }

        private void BuildShadowShapeMap() {
            // this map will be used to shadow what is going on in Visio
            // we'll use the map to get things done in memory instead of 
            // always going back to visio shape sheets to do things
            shadowShapeMap = new Dictionary<string, Shadow>();

            // when we go through creating the shadows, we may actually delete shapes
            // if there are problems - that messes up the foreach page/shape loops
            // so we stash them in all shapes and then loop off of that, which doesn't
            // get messed up
            List<Shape> allShapes = new List<Shape>();
            foreach (Page page in visioControl.Document.Pages)
                foreach (Shape shape in page.Shapes)
                    allShapes.Add(shape);

            foreach (Shape shape in allShapes) {
                string uid = shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);

                Shadow shadow = Common.MakeShapeShadow(shape);

                if (shadow != null)
                    shadowShapeMap.Add(uid, shadow);
            }
        }

        private void PathMaker_FormClosing(object sender, FormClosingEventArgs e) {
            DialogResult result = SaveDrawing(visioControl, true, false);
            if (DialogResult.Cancel == result)
                e.Cancel = true;
        }

        /**
         * Application Markers are how we get custom commands from Visio Shapes to us.  The operations are set in 
         * the shape to do things using the QueueMarker event and those eventually get here where we decipher them
         * and trigger the appropriate event handlers on our side
         */
        private void OnApplicationMarker(Microsoft.Office.Interop.Visio.Application application) {
            string arguments;
            Shape targetShape;

            // If the arguments include /PathMaker /cmd=1
            // then get a reference to the shape and raise the
            // OnShapeDoubleClick event
            arguments = visioControl.Document.Application.get_EventInfo((short)VisEventCodes.visEvtIdMostRecent);
            arguments = arguments.ToLower(System.Globalization.CultureInfo.InvariantCulture);

            // If this marker event was caused by double-clicking a 
            // shape from the PathMaker stencil then
            // raise an OnShapeDoubleClick event
            if ((arguments.IndexOf(Strings.PathMakerArgument) >= 0) && (arguments.IndexOf(Strings.DoubleClickCommand) >= 0)) {
                // Get a reference to this shape
                targetShape = Common.GetShapeFromArguments(visioControl.Document.Application, arguments);
                OnShapeDoubleClick(targetShape);
            }
            else if ((arguments.IndexOf(Strings.PathMakerArgument) >= 0) && (arguments.IndexOf(Strings.PropertiesCommand) >= 0)) {
                // Get a reference to this shape
                targetShape = Common.GetShapeFromArguments(visioControl.Document.Application, arguments);
                OnShapeProperties(targetShape);
            }
            else
                System.Diagnostics.Debug.WriteLine("Unhandled application marker - args = " + arguments);
        }

        // We need this to handle pasting - when objects are pasted the transitions have the old GUIDs but
        // visio may have assigned new ones to the pasted objects.  We stash the old GUIDs in OnWindowSelectionChange
        // and we make use of them here to get everyone in sync
        private void OnExitScope(Microsoft.Office.Interop.Visio.Application application, string moreInformation) {
            // moreInformation is a standard ;-delimited string from the scope events that contains what we need 

            // 1344 is the Microsoft Scope Id for GotoPage
            if (moreInformation.StartsWith("1344")) {
                string pageName = visioControl.Document.Application.ActivePage.Name;
                int index = visioControl.Document.Pages[pageName].Index;
                gotoPageComboBox.SelectedIndex = index - 1;
            }
            // 1022 is the Microsoft Scope Id for Paste
            // 1024 is the Microsoft Scope Id for Duplicate
            else if (moreInformation.StartsWith(((int)VisUICmds.visCmdUFEditPaste).ToString()) ||
                moreInformation.StartsWith(((int)VisUICmds.visCmdUFEditDuplicate).ToString())) {
                // first make a map of old GUIDs to new
                Dictionary<string, string> oldGUIDToNewGUIDMap = new Dictionary<string, string>();
                foreach (Shape shape in visioControl.Document.Application.ActiveWindow.Selection) {
                    string oldUID = Common.GetCellString(shape, Strings.CutCopyPasteTempCellName);
                    string newUID = shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);
                    oldGUIDToNewGUIDMap.Add(oldUID, newUID);
                }

                // now call each shadow to fix itself
                foreach (Shape shape in visioControl.Document.Application.ActiveWindow.Selection) {
                    Shadow s = PathMaker.LookupShadowByShape(shape);
                    if (s == null)
                        continue;
                    s.FixUIDReferencesAfterPaste(oldGUIDToNewGUIDMap);
                }

                // because we halt this when pasting, we now need to go fix all the ones that were pasted
                foreach (Shape s in visioControl.Document.Application.ActiveWindow.Selection) {
                    string uid = s.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);
                    Common.SetCellString(s, Strings.CutCopyPasteTempCellName, uid);
                }
            }
        }

        // NOTE - You can't just add an OnShapeProperties handler to a shadow and get the event
        // You also need to change the shapesheet (in .vss file and in upgrade) to use the
        // double click handler used by others "RUNADDONWARGS(\"QueueMarkerEvent\",\"/PathMaker /CMD=2\")"
        private void OnShapeProperties(Shape shape) {
            Shadow shadow = LookupShadowByShape(shape);
            if (shadow != null)
                shadow.OnShapeProperties();
            else
                Common.ErrorMessage("Error - properties on unexpected item " + shape.Name);
        }

        // NOTE - You can't just add an OnShapeDoubleClick handler to a shadow and get the event
        // You also need to change the shapesheet (in .vss file and in upgrade) to use the
        // double click handler used by others "RUNADDONWARGS(\"QueueMarkerEvent\",\"/PathMaker /CMD=1\")"
        private void OnShapeDoubleClick(Shape shape) {
            Shadow shadow = LookupShadowByShape(shape);
            if (shadow != null)
                shadow.OnShapeDoubleClick();
            else
                Common.ErrorMessage("Error - double clicking on unexpected item " + shape.Name);
        }

        void OnBeforeShapeDelete(Shape shape) {
            Shadow shadow = LookupShadowByShape(shape);
            if (shadow != null)
                shadow.OnBeforeShapeDelete();
            shadowShapeMap.Remove(shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID));
        }

        private void RebuildPageComboBox(Page page) {
            gotoPageComboBox.Items.Clear();
            foreach (Page p in visioControl.Document.Pages)
                gotoPageComboBox.Items.Add(p.Name);
            string pageName = visioControl.Document.Application.ActivePage.Name;
            int index = visioControl.Document.Pages[pageName].Index;
            gotoPageComboBox.SelectedIndex = index - 1;
        }

        void OnPageAdd(Page page) {
            RebuildPageComboBox(page);
            // when you undo a shape deletion, the individual shapes on it do not call OnShapeAdd...
            foreach (Shape shape in page.Shapes)
                OnShapeAdd(shape);
        }

        void OnPageMod(Page page) {
            RebuildPageComboBox(page);
        }

        void OnBeforePageDelete(Page page) {
            gotoPageComboBox.Items.Remove(page.Name);

            // when you delete an entire page, the individual shapes on it do not get deleted...
            foreach (Shape shape in page.Shapes)
                OnBeforeShapeDelete(shape);
        }

        void OnShapeAdd(Shape shape) {
            ShapeTypes type = Common.GetShapeType(shape);
            bool is1D = shape.OneD != 0;

            // Tricky - when pasting, Visio gives out new uids to the shapes if there are duplicates
            // in this document.  So, we are going to stash the original ones - unless we are pasting.
            // If we are pasting, the paste end will fix the ones that were added.
            if (!visioControl.Document.Application.get_IsInScope((int)VisUICmds.visCmdUFEditPaste) &&
                !visioControl.Document.Application.get_IsInScope((int)VisUICmds.visCmdUFEditDuplicate)) {
                string uid = shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID);
                string cached = Common.GetCellString(shape, Strings.CutCopyPasteTempCellName);

                // when undoing a delete page, you can't write this yet so this check will ignore it
                if (!uid.Equals(cached))
                    Common.SetCellString(shape, Strings.CutCopyPasteTempCellName, uid);
            }
            else if (Common.GetShapeType(shape) == ShapeTypes.OffPageRef) {
                Common.ErrorMessage("Pasted off-page reference needs to be connected.  Double click on it to repair.");
                // Because these can be cut and pasted from a single document, clearing these fields
                // allows us to avoid having more than one off page connector pointing to a single other one
                // which causes issues with tracking things in the shadows.  This way here, double clicking
                // on the connector will ask which page to connect it to.
                Common.SetCellString(shape, ShapeProperties.OffPageConnectorDestinationPageID, "");
                Common.SetCellString(shape, ShapeProperties.OffPageConnectorDestinationShapeID, "");
            }

            if (type == ShapeTypes.None && is1D) {
                // rogue connector - need to make it conform
                Common.SetCellString(shape, ShapeProperties.ShapeType, ((int)ShapeTypes.Connector).ToString());
                shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                    (short)VisRowIndices.visRowLine,
                    (short)VisCellIndices.visLineEndArrow).FormulaU = "13";
                shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                    (short)VisRowIndices.visRowLine,
                    (short)VisCellIndices.visLineRounding).FormulaU = "0.25 in";
                shape.get_CellsSRC((short)VisSectionIndices.visSectionObject,
                    (short)VisRowIndices.visRowLock,
                    (short)VisCellIndices.visLockTextEdit).FormulaU = "1";

                // just in case
                Common.FixConnectorTextControl(shape);

                // make every row in the shape data section invisible
                short row = (short)VisRowIndices.visRowFirst;
                while (shape.get_CellsSRCExists((short)VisSectionIndices.visSectionProp, row, (short)VisCellIndices.visCustPropsInvis, (short)VisExistsFlags.visExistsAnywhere) != 0)
                    shape.get_CellsSRC((short)VisSectionIndices.visSectionProp, row++, (short)VisCellIndices.visCustPropsInvis).FormulaU = "TRUE";
            }

            // when a shape is copied and pasted, it will be an exact copy of the previous shape
            // we need fix the duplicate name issue before we do anything else
            string oldPastedStateName = String.Empty;
            if (visioControl.Document.Application.get_IsInScope((int)VisUICmds.visCmdUFEditPaste) ||
                visioControl.Document.Application.get_IsInScope((int)VisUICmds.visCmdUFEditDuplicate)) {
                string stateId = Common.GetCellString(shape, ShapeProperties.StateId);
                if (stateId.Length > 0) {
                    if (!StatePrefixAndNumberManager.IsStateIdOkayForUse(stateId)) {
                        oldPastedStateName = stateId;
                        // NEVER, NEVER do this without going through the shadow except here, before the shadow is made
                        Common.SetCellString(shape, ShapeProperties.StateId, String.Empty);
                    }
                }
            }

            Shadow shadow = Common.MakeShapeShadow(shape);
            if (shadow != null) {
                // if we have a pasted name that conflicted, this will reuse the name portion 
                // but get us a new prefix and number and then renumber any prompts
                if (oldPastedStateName.Length > 0) {
                    string prefix, number, name;
                    StateShadow.DisectStateIdIntoParts(oldPastedStateName, out prefix, out number, out name);
                    shape.Text = StateShadow.StateIdForDisplay(name).Trim();
                    // this just pretends we just typed the name portion into the shape itself
                    shadow.OnShapeExitTextEdit();

                    // and now let's renumber any prompts if we're not using the "number" option
                    List<Shadow> shadowList = LookupShadowsByShapeType(ShapeTypes.Start);
                    if (shadowList.Count > 0) {
                        StartShadow startShadow = shadowList[0] as StartShadow;
                        string promptIdFormat = startShadow.GetDefaultSetting(Strings.DefaultSettingsPromptIDFormat);
                        if (promptIdFormat.Equals(Strings.PromptIdFormatFull) || promptIdFormat.Equals(Strings.PromptIdFormatPartial)) {
                            StateShadow stateShadow = shadow as StateShadow;
                            if (stateShadow != null)
                                stateShadow.RedoPromptIds(0, promptIdFormat);
                        }
                    }
                }
                shadowShapeMap.Add(shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID), shadow);
                shadow.OnShapeAdd();

                if (shadow.GetShapeType() == ShapeTypes.DocTitle ||
                    shadow.GetShapeType() == ShapeTypes.ChangeLog ||
                    shadow.GetShapeType() == ShapeTypes.Start) {
                    if (LookupShadowsByShapeType(shadow.GetShapeType()).Count > 1) {
                        Common.ErrorMessage("Cannot have two Start, Change Log, or Document Title shapes");
                        Common.ForcedSetShapeText(shape, Strings.ToBeDeletedLabel);
                    }
                }
            }
            else {
                Common.ErrorMessage("Invalid non-PathMaker shape added");
                try {
                    Common.ForcedSetShapeText(shape, Strings.ToBeDeletedLabel);
                }
                catch {
                    // it may be a shape with two subshapes (play/interaction) so try this too
                    try {
                        Common.ForcedSetShapeText(shape.Shapes[0], Strings.ToBeDeletedLabel);
                    }
                    catch {
                        // copying from non-PathMaker visios can cause this to fail depending on shape sheets, locks, etc.
                        // We did our best - we can ignore these
                    }
                }
            }
        }

        void OnConnectDelete(Connects connects) {
            if (SuspendConnectHandlingToMoveAConnectionPoint)
                return;

            foreach (Connect c in connects) {
                // each connection should be between a 1D shape (connector or comment) and a 2D shape (everything else)
                // seems like the fromsheet is always the 1D shape but this should be bulletproof if it changes
                Shape connector = null;
                Shape nonConnector = null;
                if (c.FromSheet.OneD != 0)
                    connector = c.FromSheet;
                else
                    nonConnector = c.FromSheet;

                if (connector == null && c.ToSheet.OneD != 0)
                    connector = c.ToSheet;
                else if (nonConnector == null && c.ToSheet.OneD == 0)
                    nonConnector = c.ToSheet;

                // not sure what it is but we don't care about it
                if (connector == null || nonConnector == null)
                    return;

                Shadow connectorShadow = LookupShadowByShape(connector);
                Shadow nonConnectorShadow = LookupShadowByShape(nonConnector);

                if (connectorShadow == null || nonConnectorShadow == null) {
                    Common.ErrorMessage("Deleting connection from invalid shapes...");
                    return;
                }

                // an ignored shadow (like a comment) can sometimes have links
                // but shouldn't be added as a transition - it's irrelevant
                IgnoredShadow ignored = connectorShadow as IgnoredShadow;
                if (ignored != null)
                    return;

                // determine which end of the connector is connected to the nonConnectorShadow
                bool arrowSide = false;
                if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                    arrowSide = true;

                if (arrowSide) {
                    connectorShadow.OnConnectDeleteOutput(nonConnectorShadow);
                    nonConnectorShadow.OnConnectDeleteInput(connectorShadow);
                }
                else {
                    connectorShadow.OnConnectDeleteInput(nonConnectorShadow);
                    nonConnectorShadow.OnConnectDeleteOutput(connectorShadow);
                }
            }
            FixHourGlassAfterConfusingVisio();
        }

        void OnConnectAdd(Connects connects) {
            if (SuspendConnectHandlingToMoveAConnectionPoint)
                return;

            foreach (Connect c in connects) {
                // each connection should be between a 1D shape (connector or comment) and a 2D shape (everything else)
                // The 1D connector is always the To and 2D shapes are always the From
                Shape connector = null;
                Shape nonConnector = null;
                if (c.FromSheet.OneD != 0)
                    connector = c.FromSheet;
                else
                    nonConnector = c.FromSheet;

                if (connector == null && c.ToSheet.OneD != 0)
                    connector = c.ToSheet;
                else if (nonConnector == null && c.ToSheet.OneD == 0)
                    nonConnector = c.ToSheet;

                // not sure what it is but we don't care about it
                if (connector == null || nonConnector == null)
                    return;

                Shadow connectorShadow = LookupShadowByShape(connector);
                Shadow nonConnectorShadow = LookupShadowByShape(nonConnector);

                if (connectorShadow == null || nonConnectorShadow == null) {
                    Common.ErrorMessage("Adding connector to bogus shapes...");
                    return;
                }

                // an ignored shadow (like a comment) can sometimes have links
                // but shouldn't be added as a transition - it's irrelevant
                IgnoredShadow ignored = connectorShadow as IgnoredShadow;
                if (ignored != null)
                    return;

                // determine which end of the connector is connected to the nonConnectorShadow
                bool arrowSide = false;
                if (c.FromCell.Name.Equals(Strings.EndConnectionPointCellName))
                    arrowSide = true;

                if (arrowSide) {
                    connectorShadow.OnConnectAddOutput(nonConnectorShadow);
                    nonConnectorShadow.OnConnectAddInput(connectorShadow);
                }
                else {
                    connectorShadow.OnConnectAddInput(nonConnectorShadow);
                    nonConnectorShadow.OnConnectAddOutput(connectorShadow);
                }
            }
        }

        void OnShapeExitTextEdit(Shape shape) {
            Shadow shadow = LookupShadowByShape(shape);
            if (shadow != null)
                shadow.OnShapeExitTextEdit();
            else {
                Shape parent = shape.Parent as Shape;

                if (parent != null) {
                    // for state shapes where there are multiple shapes in the stencil, we need the parent
                    shadow = LookupShadowByShape(parent);
                    if (shadow != null)
                        shadow.OnShapeExitTextEdit();
                    else
                        Common.ErrorMessage("Error - editing text on unexpected item " + shape.Name);
                }
                else {
                    // sometimes odd ball shapes get edited - just let them be
                    // for example, dropping a non-PathMaker shape or upgrading one could end up here
                }
            }
        }

        private void OnActivated(object sender, EventArgs e) {
            if (gotoPageComboBox.SelectedIndex == -1) {
                string pageName = visioControl.Document.Application.ActivePage.Name;
                int index = visioControl.Document.Pages[pageName].Index;
                gotoPageComboBox.SelectedIndex = index - 1;
            }

            // do this only on the initial load of the application
            if (OneTimeOnlyActivateHack) {
                OneTimeOnlyActivateHack = false;
                try {
                    // For some reason our template will cause an exception if you add a new
                    // page to it before you have put a shape on it.  My guess is something to 
                    // do with it being a vst but it only happens in the activeX control, not
                    // standard visio.  Either way, this little trick of putting a connector on and 
                    // deleting it seems to solve the problem.  One other note, the shape you drop 
                    // matters.  A comment shape didn't work...
                    Page page = visioControl.Document.Application.ActivePage;
                    Document stencil = visioControl.Document.Application.Documents[Strings.StencilFileName];
                    Shape shape = page.Drop(stencil.Masters["Dynamic connector"], 1, 1);
                    shape.Delete();
                    visioControl.Document.Saved = true;
                }
                catch {
                    // fails if the window isn't up yet - can be ignored
                }
            }
        }

        private void gotoPageComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            int pageIndex = gotoPageComboBox.SelectedIndex + 1;

            // this routine gets called even if we are the ones programmatically setting the value of the 
            // combo box to match up with the user changing pages using tabs or the find control (or whatever) 
            // so we should only change the page if it's not already correct
            if (visioControl.Document.Application.ActiveWindow.Page != visioControl.Document.Pages[pageIndex])
                visioControl.Document.Application.ActiveWindow.Page = visioControl.Document.Pages[pageIndex];
        }

        private void OnPathRunnerBackgroundWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            ResetValidateSpecOptions();
        }

        private void PathMaker_Shown(object sender, EventArgs e) {
            // this is needed to handle the case where someone double clicks
            // on a .vui file and opens it - it's the only way to clean up
            // the stencils in that case.  OnSrcDocumentChanged doesn't 
            // cleanup the stencils if the window isn't visible yet
            StencilCleanup();
        }

        public string getCurrentFileDirectory() {
            return getCurrentFileDirectory(visioControl);
        }

        static public string getCurrentFileDirectory(AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl) {
            if (visioControl.Src.Contains(Strings.VisioTemplateFileSuffix))
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            else
                return System.IO.Path.GetDirectoryName(visioControl.Src);
        }
    }
}