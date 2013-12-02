// EventSink.cs
// <copyright>Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This file contains the implementation of EventSink class.</summary>

using System;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    [System.Runtime.InteropServices.ComVisible(true)]
    public sealed class EventSink : Microsoft.Office.Interop.Visio.IVisEventProc {
        public delegate void VisioDocumentEventHandler(Document document);
        public delegate void VisioPageEventHandler(Page page);
        public delegate void VisioMasterEventHandler(Master master);
        public delegate void VisioSelectionEventHandler(Selection selection);
        public delegate void VisioShapeEventHandler(Shape shape);
        public delegate void VisioCellEventHandler(Cell cell);
        public delegate void VisioConnectEventHandler(Connects connect);
        public delegate void VisioStyleEventHandler(Style style);
        public delegate void VisioWindowEventHandler(Window window);
        public delegate void VisioApplicationEventHandler(Application application);
        public delegate void VisioScopeEventHandler(Application application, string moreInformation);
        public delegate void VisioKeyboardEventHandler(KeyboardEvent keyboardEvent);
        public delegate void VisioMouseEventHandler(MouseEvent mouseEvent);
        public delegate void VisioDataRecordsetEventHandler(DataRecordset dataRecordset);

        public event VisioDocumentEventHandler onDocumentDelete;
        public event VisioDocumentEventHandler onBeforeDocumentSave;
        public event VisioDocumentEventHandler onBeforeDocumentSaveAs;
        public event VisioDocumentEventHandler onDocumentDesign;
        public event VisioDocumentEventHandler onDocumentAdd;
        public event VisioDocumentEventHandler onDocumentMod;
        public event VisioDocumentEventHandler onCancelDocumentClose;
        public event VisioDocumentEventHandler onDocumentCreate;
        public event VisioDocumentEventHandler onDocumentOpen;
        public event VisioDocumentEventHandler onDocumentSave;
        public event VisioDocumentEventHandler onDocumentSaveAs;
        public event VisioDocumentEventHandler onDocumentRunning;
        public event VisioDocumentEventHandler onQueryCancelDocumentClose;

        public event VisioPageEventHandler onBeforePageDelete;
        public event VisioPageEventHandler onPageAdd;
        public event VisioPageEventHandler onPageMod;
        public event VisioPageEventHandler onCancelPageDelete;
        public event VisioPageEventHandler onQueryCancelPageDelete;

        public event VisioMasterEventHandler onBeforeMasterDelete;
        public event VisioMasterEventHandler onMasterMod;
        public event VisioMasterEventHandler onCancelMasterDelete;
        public event VisioMasterEventHandler onMasterAdd;
        public event VisioMasterEventHandler onQueryCancelMasterDelete;

        public event VisioSelectionEventHandler onBeforeSelectionDelete;
        public event VisioSelectionEventHandler onSelectionAdded;
        public event VisioSelectionEventHandler onCancelSelectionDelete;
        public event VisioSelectionEventHandler onCancelConvertToGroup;
        public event VisioSelectionEventHandler onQueryCancelUngroup;
        public event VisioSelectionEventHandler onQueryCancelConvertToGroup;
        public event VisioSelectionEventHandler onQueryCancelSelectionDelete;
        public event VisioSelectionEventHandler onCancelUngroup;
        public event VisioSelectionEventHandler onCancelSelectGroup;
        public event VisioSelectionEventHandler onShapesDelete;

        public event VisioShapeEventHandler onBeforeShapeDelete;
        public event VisioShapeEventHandler onShapeBeforeTextEdit;
        public event VisioShapeEventHandler onShapeAdd;
        public event VisioShapeEventHandler onShapeMod;
        public event VisioShapeEventHandler onShapeExitTextEdit;
        public event VisioShapeEventHandler onShapeParentChange;
        public event VisioShapeEventHandler onShapeTextMod;
        public event VisioShapeEventHandler onShapeLinkAdded;
        public event VisioShapeEventHandler onShapeLinkDeleted;
        public event VisioShapeEventHandler onShapeDataGraphicChanged;

        public event VisioCellEventHandler onCellMod;
        public event VisioCellEventHandler onFormulaMod;

        public event VisioConnectEventHandler onConnectAdd;
        public event VisioConnectEventHandler onConnectDelete;

        public event VisioStyleEventHandler onBeforeStyleDelete;
        public event VisioStyleEventHandler onStyleAdd;
        public event VisioStyleEventHandler onStyleMod;
        public event VisioStyleEventHandler onCancelStyleDel;
        public event VisioStyleEventHandler onQueryCancelStyleDel;

        public event VisioWindowEventHandler onBeforeWindowClose;
        public event VisioWindowEventHandler onBeforeWindowPageTurn;
        public event VisioWindowEventHandler onWindowAdd;
        public event VisioWindowEventHandler onWindowMod;
        public event VisioWindowEventHandler onWindowPageTurn;
        public event VisioWindowEventHandler onBeforeWindowSelectionDelete;
        public event VisioWindowEventHandler onCancelWindowClose;
        public event VisioWindowEventHandler onWindowActivate;
        public event VisioWindowEventHandler onWindowSelectionChange;
        public event VisioWindowEventHandler onViewChanged;
        public event VisioWindowEventHandler onQueryCancelWindowClose;

        public event VisioApplicationEventHandler onApplicationAfterModal;
        public event VisioApplicationEventHandler onAfterResume;
        public event VisioApplicationEventHandler onApplicationActivate;
        public event VisioApplicationEventHandler onApplicationDeactivate;
        public event VisioApplicationEventHandler onApplicationObjectActivate;
        public event VisioApplicationEventHandler onApplicationObjectDeactivate;
        public event VisioApplicationEventHandler onApplicationBeforeModal;
        public event VisioApplicationEventHandler onApplicationBeforeQuit;
        public event VisioApplicationEventHandler onBeforeSuspend;
        public event VisioApplicationEventHandler onApplicationMarker;
        public event VisioApplicationEventHandler onBeforeForcedFlush;
        public event VisioApplicationEventHandler onAfterForcedFlush;
        public event VisioApplicationEventHandler onApplicationNonePending;
        public event VisioApplicationEventHandler onWindowOnAddonKeyMSG;
        public event VisioApplicationEventHandler onQueryCancelQuit;
        public event VisioApplicationEventHandler onQueryCancelSuspend;
        public event VisioApplicationEventHandler onCancelQuit;
        public event VisioApplicationEventHandler onCancelSuspend;
        public event VisioApplicationEventHandler onApplicationIdle;

        public event VisioScopeEventHandler onEnterScope;
        public event VisioScopeEventHandler onExitScope;

        public event VisioKeyboardEventHandler onKeyDown;
        public event VisioKeyboardEventHandler onKeyPress;
        public event VisioKeyboardEventHandler onKeyUp;

        public event VisioMouseEventHandler onMouseDown;
        public event VisioMouseEventHandler onMouseMove;
        public event VisioMouseEventHandler onMouseUp;

        public event VisioDataRecordsetEventHandler onDataRecordsetAdd;
        public event VisioDataRecordsetEventHandler onDataRecordsetDel;
        public event VisioDataRecordsetEventHandler onDataRecordsetMod;

        private const short visEvtAdd = -32768;
        private const string eventSinkCaption = "Event Sink";
        private const string tab = "\t";
        private System.Collections.Specialized.StringDictionary eventDescriptions;

        public EventSink() {
            InitializeStrings();
        }

        /// <summary>This method is called by Visio when an event in the
        /// EventList collection has been triggered. This method is an
        /// implementation of IVisEventProc.VisEventProc method.</summary>
        /// <param name="eventCode">Event code of the event that fired</param>
        /// <param name="source">Reference to source of the event</param>
        /// <param name="eventId">Unique identifier of the event object that 
        /// raised the event</param>
        /// <param name="eventSequenceNumber">Relative position of the event in 
        /// the event list</param>
        /// <param name="subject">Reference to the subject of the event</param>
        /// <param name="moreInformation">Additional information for the event
        /// </param>
        /// <returns>False to allow a QueryCancel operation or True to cancel 
        /// a QueryCancel operation. The return value is ignored by Visio unless 
        /// the event is a QueryCancel event.</returns>
        /// <seealso cref="Microsoft.Office.Interop.Visio.IVisEventProc"></seealso>
        public object VisEventProc(short eventCode, object source, int eventId, int eventSequenceNumber, object subject, object moreInformation) {
            string message = "";
            string name = "";
            string eventInformation = "";
            object returnValue = true;

            Microsoft.Office.Interop.Visio.Application subjectApplication = null;
            Microsoft.Office.Interop.Visio.Document subjectDocument = null;
            Microsoft.Office.Interop.Visio.Page subjectPage = null;
            Microsoft.Office.Interop.Visio.Master subjectMaster = null;
            Microsoft.Office.Interop.Visio.Selection subjectSelection = null;
            Microsoft.Office.Interop.Visio.Shape subjectShape = null;
            Microsoft.Office.Interop.Visio.Cell subjectCell = null;
            Microsoft.Office.Interop.Visio.Connects subjectConnects = null;
            Microsoft.Office.Interop.Visio.Style subjectStyle = null;
            Microsoft.Office.Interop.Visio.Window subjectWindow = null;
            Microsoft.Office.Interop.Visio.MouseEvent subjectMouseEvent = null;
            Microsoft.Office.Interop.Visio.KeyboardEvent subjectKeyboardEvent = null;
            Microsoft.Office.Interop.Visio.DataRecordset subjectDataRecordset = null;
            Microsoft.Office.Interop.Visio.DataRecordsetChangedEvent subjectDataRecordsetChangedEvent = null;

            switch (eventCode) {
                // Document event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    if (onDocumentDelete != null) onDocumentDelete((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefDocSave:
                    if (onBeforeDocumentSave != null) onBeforeDocumentSave((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefDocSaveAs:
                    if (onBeforeDocumentSaveAs != null) onBeforeDocumentSaveAs((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocDesign:
                    if (onDocumentDesign != null) onDocumentDesign((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + visEvtAdd:
                    if (onDocumentAdd != null) onDocumentAdd((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onDocumentMod != null) onDocumentMod((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelDocClose:
                    if (onCancelDocumentClose != null) onCancelDocumentClose((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocCreate:
                    if (onDocumentCreate != null) onDocumentCreate((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocOpen:
                    if (onDocumentOpen != null) onDocumentOpen((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocSave:
                    if (onDocumentSave != null) onDocumentSave((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocSaveAs:
                    if (onDocumentSaveAs != null) onDocumentSaveAs((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocRunning:
                    if (onDocumentRunning != null) onDocumentRunning((Document)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelDocClose:
                    if (onQueryCancelDocumentClose != null) onQueryCancelDocumentClose((Document)subject); break;

                // Page event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    if (onBeforePageDelete != null) onBeforePageDelete((Page)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + visEvtAdd:
                    if (onPageAdd != null) onPageAdd((Page)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onPageMod != null) onPageMod((Page)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelPageDel:
                    if (onCancelPageDelete != null) onCancelPageDelete((Page)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelPageDel:
                    if (onQueryCancelPageDelete != null) onQueryCancelPageDelete((Page)subject); break;

                // Master event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    if (onBeforeMasterDelete != null) onBeforeMasterDelete((Master)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onMasterMod != null) onMasterMod((Master)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelMasterDel:
                    if (onCancelMasterDelete != null) onCancelMasterDelete((Master)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + visEvtAdd:
                    if (onMasterAdd != null) onMasterAdd((Master)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelMasterDel:
                    if (onQueryCancelMasterDelete != null) onQueryCancelMasterDelete((Master)subject); break;

                // Selection event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefSelDel:
                    if (onBeforeSelectionDelete != null) onBeforeSelectionDelete((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeSelAdded:
                    if (onSelectionAdded != null) onSelectionAdded((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSelDel:
                    if (onCancelSelectionDelete != null) onCancelSelectionDelete((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelConvertToGroup:
                    if (onCancelConvertToGroup != null) onCancelConvertToGroup((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelUngroup:
                    if (onQueryCancelUngroup != null) onQueryCancelUngroup((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelConvertToGroup:
                    if (onQueryCancelConvertToGroup != null) onQueryCancelConvertToGroup((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSelDel:
                    if (onQueryCancelSelectionDelete != null) onQueryCancelSelectionDelete((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelUngroup:
                    if (onCancelUngroup != null) onCancelUngroup((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSelGroup:
                    if (onCancelSelectGroup != null) onCancelSelectGroup((Selection)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeDelete:
                    if (onShapesDelete != null) onShapesDelete((Selection)subject); break;

                // Shape event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    if (onBeforeShapeDelete != null) onBeforeShapeDelete((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeBeforeTextEdit:
                    if (onShapeBeforeTextEdit != null) onShapeBeforeTextEdit((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + visEvtAdd:
                    if (onShapeAdd != null) onShapeAdd((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onShapeMod != null) onShapeMod((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeExitTextEdit:
                    if (onShapeExitTextEdit != null) onShapeExitTextEdit((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeParentChange:
                    if (onShapeParentChange != null) onShapeParentChange((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtText + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onShapeTextMod != null) onShapeTextMod((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeLinkAdded:
                    if (onShapeLinkAdded != null) onShapeLinkAdded((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeLinkDeleted:
                    if (onShapeLinkDeleted != null) onShapeLinkDeleted((Shape)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeDataGraphicChanged:
                    if (onShapeDataGraphicChanged != null) onShapeDataGraphicChanged((Shape)subject); break;

                // Cell event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCell + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onCellMod != null) onCellMod((Cell)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtFormula + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onFormulaMod != null) onFormulaMod((Cell)subject); break;

                // Connects event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtConnect + visEvtAdd:
                    if (onConnectAdd != null) onConnectAdd((Connects)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtConnect + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    if (onConnectDelete != null) onConnectDelete((Connects)subject); break;

                // Style event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    if (onBeforeStyleDelete != null) onBeforeStyleDelete((Style)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + visEvtAdd:
                    if (onStyleAdd != null) onStyleAdd((Style)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onStyleMod != null) onStyleMod((Style)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelStyleDel:
                    if (onCancelStyleDel != null) onCancelStyleDel((Style)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelStyleDel:
                    if (onQueryCancelStyleDel != null) onQueryCancelStyleDel((Style)subject); break;

                // Window event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    if (onBeforeWindowClose != null) onBeforeWindowClose((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefWinPageTurn:
                    if (onBeforeWindowPageTurn != null) onBeforeWindowPageTurn((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + visEvtAdd:
                    if (onWindowAdd != null) onWindowAdd((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onWindowMod != null) onWindowMod((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinPageTurn:
                    if (onWindowPageTurn != null) onWindowPageTurn((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefWinSelDel:
                    if (onBeforeWindowSelectionDelete != null) onBeforeWindowSelectionDelete((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelWinClose:
                    if (onCancelWindowClose != null) onCancelWindowClose((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWinActivate:
                    if (onWindowActivate != null) onWindowActivate((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinSelChange:
                    if (onWindowSelectionChange != null) onWindowSelectionChange((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeViewChanged:
                    if (onViewChanged != null) onViewChanged((Window)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelWinClose:
                    if (onQueryCancelWindowClose != null) onQueryCancelWindowClose((Window)subject); break;

                // Application event codes
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAfterModal:
                    if (onApplicationAfterModal != null) onApplicationAfterModal((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeAfterResume:
                    if (onAfterResume != null) onAfterResume((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAppActivate:
                    if (onApplicationActivate != null) onApplicationActivate((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAppDeactivate:
                    if (onApplicationDeactivate != null) onApplicationDeactivate((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtObjActivate:
                    if (onApplicationObjectActivate != null) onApplicationObjectActivate((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtObjDeactivate:
                    if (onApplicationObjectDeactivate != null) onApplicationObjectDeactivate((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtBeforeModal:
                    if (onApplicationBeforeModal != null) onApplicationObjectDeactivate((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtBeforeQuit:
                    if (onApplicationBeforeQuit != null) onApplicationBeforeQuit((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBeforeSuspend:
                    if (onBeforeSuspend != null) onBeforeSuspend((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMarker:
                    if (onApplicationMarker != null) onApplicationMarker((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefForcedFlush:
                    if (onBeforeForcedFlush != null) onBeforeForcedFlush((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeAfterForcedFlush:
                    if (onAfterForcedFlush != null) onAfterForcedFlush((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtNonePending:
                    if (onApplicationNonePending != null) onApplicationNonePending((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinOnAddonKeyMSG:
                    if (onWindowOnAddonKeyMSG != null) onWindowOnAddonKeyMSG((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelQuit:
                    if (onQueryCancelQuit != null) onQueryCancelQuit((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSuspend:
                    if (onQueryCancelSuspend != null) onQueryCancelSuspend((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelQuit:
                    if (onCancelQuit != null) onCancelQuit((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSuspend:
                    if (onCancelSuspend != null) onCancelSuspend((Application)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtIdle:
                    if (onApplicationIdle != null) onApplicationIdle((Application)subject); break;

                // scope events
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeEnterScope:
                    if (onEnterScope != null) onEnterScope((Application)subject, (string)moreInformation); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeExitScope:
                    if (onExitScope != null) onExitScope((Application)subject, (string)moreInformation); break;

                // Keyboard events
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyDown:
                    if (onKeyDown != null) onKeyDown((KeyboardEvent)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyPress:
                    if (onKeyPress != null) onKeyPress((KeyboardEvent)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyUp:
                    if (onKeyUp != null) onKeyUp((KeyboardEvent)subject); break;

                // Mouse Events
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseDown:
                    if (onMouseDown != null) onMouseDown((MouseEvent)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseMove:
                    if (onMouseMove != null) onMouseDown((MouseEvent)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseUp:
                    if (onMouseUp != null) onMouseDown((MouseEvent)subject); break;

                // DataRecordset events
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + visEvtAdd:
                    if (onDataRecordsetAdd != null) onDataRecordsetAdd((DataRecordset)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    if (onDataRecordsetDel != null) onDataRecordsetDel((DataRecordset)subject); break;
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    if (onDataRecordsetMod != null) onDataRecordsetMod((DataRecordset)subject); break;

                default:
                    break;
            }


            try {
                switch (eventCode) {
                    // Document event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefDocSave:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefDocSaveAs:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocDesign:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + visEvtAdd:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelDocClose:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocCreate:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocOpen:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocSave:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocSaveAs:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocRunning:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelDocClose:

                        // Subject object is a Document
                        //   Eventinfo may be non empty. 
                        //   (1) For DocumentChanged Event it may indicate what 
                        //   changed, e.g.  /pagereordered, etc. 
                        //   (2) For the save, saveas events the eventinfo is 
                        //   typically empty. However, starting with Visio
                        //   2000 SR1 it is the name of the recover file if 
                        //   save occured for autorecovery.  In general expect
                        //   non-empty eventinfo only for SaveAs.
                        //   (3) For RemoveHiddenInformation the eventinfo
                        //   includes the data that was removed. The various types 
                        //   are represented by the following strings: 
                        //   /visRHIPersonalInfo, /visRHIMasters, /visRHIStyles,
                        //   /visRHIDataRecordsets.

                        subjectDocument = (Document)subject;
                        subjectApplication = subjectDocument.Application;
                        name = subjectDocument.Name;
                        break;

                    // Page event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + visEvtAdd:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelPageDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelPageDel:

                        // Subject object is a Page
                        subjectPage = (Page)subject;
                        subjectApplication = subjectPage.Application;
                        name = subjectPage.Name;
                        break;

                    // Master event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelMasterDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + visEvtAdd:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelMasterDel:

                        // Subject object is a Master
                        subjectMaster = (Master)subject;
                        subjectApplication = subjectMaster.Application;
                        name = subjectMaster.Name;
                        break;

                    // Selection event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefSelDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeSelAdded:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSelDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelConvertToGroup:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelUngroup:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelConvertToGroup:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSelDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelUngroup:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSelGroup:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeDelete:

                        // Subject object is a Selection
                        subjectSelection = (Selection)subject;
                        subjectApplication = subjectSelection.Application;
                        break;

                    // Shape event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeBeforeTextEdit:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + visEvtAdd:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeExitTextEdit:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeParentChange:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtText + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeLinkAdded:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeLinkDeleted:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeDataGraphicChanged:

                        // Subject object is a Shape
                        //  Eventinfo may be non-empty.
                        //  (1) For ShapeChanged Event it may indicate 
                        //  what changed, e.g. /data1, /name, etc.
                        //  (2) For the ShapeLinkAdded and ShapelinkDeleted events, 
                        //  the eventinfo provides the recordset ID and rowID 
                        //  participating in the link as  
                        //  /DataRecordsetID=<ID> and /DataRowID=<ID2>

                        subjectShape = (Shape)subject;
                        subjectApplication = subjectShape.Application;
                        name = subjectShape.Name;
                        break;

                    // Cell event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCell + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtFormula + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:

                        // Subject object is a Cell
                        subjectCell = (Cell)subject;
                        subjectShape = subjectCell.Shape;
                        subjectApplication = subjectCell.Application;
                        name = subjectShape.Name + "!" + subjectCell.Name;
                        break;

                    // Connects event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtConnect + visEvtAdd:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtConnect + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:

                        // Subject object is a Connects collection
                        subjectConnects = (Connects)subject;
                        subjectApplication = subjectConnects.Application;
                        break;

                    // Style event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + visEvtAdd:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelStyleDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelStyleDel:

                        // Subject object is a Style
                        subjectStyle = (Style)subject;
                        subjectApplication = subjectStyle.Application;
                        name = subjectStyle.Name;
                        break;

                    // Window event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefWinPageTurn:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + visEvtAdd:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinPageTurn:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefWinSelDel:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelWinClose:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWinActivate:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinSelChange:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeViewChanged:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelWinClose:

                        // Subject object is a Window
                        subjectWindow = (Window)subject;
                        subjectApplication = subjectWindow.Application;
                        name = subjectWindow.Caption;
                        break;

                    // Application event codes
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAfterModal:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeAfterResume:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAppActivate:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAppDeactivate:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtObjActivate:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtObjDeactivate:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtBeforeModal:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtBeforeQuit:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBeforeSuspend:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeEnterScope:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeExitScope:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMarker:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefForcedFlush:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeAfterForcedFlush:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtNonePending:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinOnAddonKeyMSG:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelQuit:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSuspend:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelQuit:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSuspend:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtIdle:

                        // Subject object is an Application
                        // EventInfo is empty for most of these events.  However for
                        // the Marker event, the EnterScope event and the ExitScope 
                        // event eventinfo contains the context string. 
                        subjectApplication = (Application)subject;
                        break;

                    // Keyboard events
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyDown:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyPress:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyUp:

                        // Subject object is KeyboardEvent
                        // Note, keyboard events can be canceled.
                        subjectKeyboardEvent = (KeyboardEvent)subject;
                        break;


                    // Mouse Events
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseDown:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseMove:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseUp:

                        // Subject object is MouseEvent Object. 
                        // Eventinfo may be non-empty for mouse move events.
                        // In that cases it indicates the drag state which is 
                        // also exposed in the DragState property of the
                        // MouseEvent object. 
                        // Note, mouse events can be canceled. 
                        subjectMouseEvent = (MouseEvent)subject;
                        break;


                    // DataRecordset events
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + visEvtAdd:
                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel:

                        // Subject object is DataRecordset
                        subjectDataRecordset = (DataRecordset)subject;
                        break;

                    case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod:

                        // Subject object is DataRecordsetChangedEvent object
                        subjectDataRecordsetChangedEvent = (DataRecordsetChangedEvent)subject;
                        break;

                    default:
                        name = "Unknown";
                        break;
                }

                // get a description for this event code
                message = GetEventDescription(eventCode);

                // append the name of the subject object
                if (name.Length > 0)
                    message += ": " + name;

                // append event info when it is available
                if (subjectApplication != null) {
                    eventInformation = subjectApplication.get_EventInfo((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtIdMostRecent);

                    if (eventInformation != null)
                        message += tab + eventInformation;
                }

                // append moreInformation when it is available
                if (moreInformation != null)
                    message += tab + moreInformation.ToString();

                // get the targetArgs string from the event object. targetArgs
                // are added to the event object in the AddAdvise method
                Microsoft.Office.Interop.Visio.EventList events = null;
                Microsoft.Office.Interop.Visio.Event thisEvent = null;
                string sourceType;
                string targetArgs = "";

                sourceType = source.GetType().FullName;
                if (sourceType == "Microsoft.Office.Interop.Visio.ApplicationClass")
                    events = ((Microsoft.Office.Interop.Visio.Application)source).EventList;
                else if (sourceType == "Microsoft.Office.Interop.Visio.DocumentClass")
                    events = ((Microsoft.Office.Interop.Visio.Document)source).EventList;
                else if (sourceType == "Microsoft.Office.Interop.Visio.PageClass")
                    events = ((Microsoft.Office.Interop.Visio.Page)source).EventList;

                if (events != null) {
                    thisEvent = events.get_ItemFromID(eventId);
                    targetArgs = thisEvent.TargetArgs;

                    // append targetArgs when it is available
                    if (targetArgs.Length > 0)
                        message += " " + targetArgs;
                }

                // Write the event info to the output window
                System.Diagnostics.Debug.WriteLine(message);

                // if this is a QueryCancel event then prompt the user
                returnValue = GetQueryCancelResponse(eventCode, subject);
            }
            catch (Exception err) {
                System.Diagnostics.Debug.WriteLine(err.Message);
                throw err;
            }

            return returnValue;
        }

        /// <summary>
        /// This method prompts the user to continue or cancel. If the
        /// alertResponse value is set in this Visio instance then its value 
        /// will be used and the dialog will be suppressed.</summary>
        /// <param name="eventCode">Event code of the event that fired</param>
        /// <param name="subject">Reference to subject of the event</param>
        /// <returns>False to allow the QueryCancel operation or True to cancel 
        /// the QueryCancel operation.</returns>
        private static object GetQueryCancelResponse(short eventCode, object subject) {
            const string docCloseCancelPrompt = "Are you sure you want to close the document?";
            const string pageDeleteCancelPrompt = "Are you sure you want to delete the page?";
            const string masterDeleteCancelPrompt = "Are you sure you want to delete the master?";
            const string ungroupCancelPrompt = "Are you sure you want to ungroup the selected shapes?";
            const string convertToGroupCancelPrompt = "Are you sure you want to convert the selected shapes to a group?";
            const string selectionDeleteCancelPrompt = "Are you sure you want to delete the selected shapes?";
            const string styleDeleteCancelPrompt = "Are you sure you want to delete the style?";
            const string windowCloseCancelPrompt = "Are you sure you want to close the window?";
            const string quitCancelPrompt = "Are you sure you want to quit Visio?";
            const string suspendCancelPrompt = "Are you sure you want suspend Visio?";
            const string groupCancelPrompt = "Are you sure you want to group the selected shapes?";

            Microsoft.Office.Interop.Visio.Application subjectApplication = null;
            Microsoft.Office.Interop.Visio.Document subjectDocument = null;
            Microsoft.Office.Interop.Visio.Page subjectPage = null;
            Microsoft.Office.Interop.Visio.Master subjectMaster = null;
            Microsoft.Office.Interop.Visio.Selection subjectSelection = null;
            Microsoft.Office.Interop.Visio.Style subjectStyle = null;
            Microsoft.Office.Interop.Visio.Window subjectWindow = null;
            string prompt = "";
            string subjectName = "";
            short alertResponse = 0;
            bool isQueryCancelEvent = true;
            object returnValue = false;

            switch (eventCode) {
                // Query Document Close
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelDocClose:
                    subjectDocument = ((Microsoft.Office.Interop.Visio.Document)subject);
                    subjectName = subjectDocument.Name;
                    subjectApplication = subjectDocument.Application;
                    prompt = docCloseCancelPrompt + System.Environment.NewLine + subjectName;
                    break;

                // Query Cancel Page Delete
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelPageDel:
                    subjectPage = ((Microsoft.Office.Interop.Visio.Page)subject);
                    subjectName = subjectPage.NameU;
                    subjectApplication = subjectPage.Application;
                    prompt = pageDeleteCancelPrompt + System.Environment.NewLine + subjectName;
                    break;

                // Query Cancel Master Delete
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelMasterDel:
                    subjectMaster = ((Microsoft.Office.Interop.Visio.Master)subject);
                    subjectName = subjectMaster.NameU;
                    subjectApplication = subjectMaster.Application;
                    prompt = masterDeleteCancelPrompt + System.Environment.NewLine + subjectName;
                    break;

                // Query Cancel Ungroup
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelUngroup:
                    subjectSelection = ((Microsoft.Office.Interop.Visio.Selection)subject);
                    subjectApplication = subjectSelection.Application;
                    prompt = ungroupCancelPrompt;
                    break;

                // Query Cancel Convert To Group
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelConvertToGroup:
                    subjectSelection = ((Microsoft.Office.Interop.Visio.Selection)subject);
                    subjectApplication = subjectSelection.Application;
                    prompt = convertToGroupCancelPrompt;
                    break;

                // Query Cancel Selection Delete
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSelDel:
                    subjectSelection = ((Microsoft.Office.Interop.Visio.Selection)subject);
                    subjectApplication = subjectSelection.Application;
                    prompt = selectionDeleteCancelPrompt;
                    break;

                // Query Cancel Style Delete
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelStyleDel:
                    subjectStyle = ((Microsoft.Office.Interop.Visio.Style)subject);
                    subjectName = subjectStyle.NameU;
                    subjectApplication = subjectStyle.Application;
                    prompt = styleDeleteCancelPrompt + System.Environment.NewLine + subjectName;
                    break;

                // Query Cancel Window Close
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelWinClose:
                    subjectWindow = ((Microsoft.Office.Interop.Visio.Window)subject);
                    subjectName = subjectWindow.Caption;
                    subjectApplication = subjectWindow.Application;
                    prompt = windowCloseCancelPrompt + System.Environment.NewLine + subjectName;
                    break;

                // Query Cancel Quit
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelQuit:
                    subjectApplication = (Microsoft.Office.Interop.Visio.Application)subject;
                    prompt = quitCancelPrompt;
                    break;

                // Query Cancel Suspend
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSuspend:
                    subjectApplication = (Microsoft.Office.Interop.Visio.Application)subject;
                    prompt = suspendCancelPrompt;
                    break;

                // Query Cancel Group
                case (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSelGroup:
                    subjectSelection = ((Microsoft.Office.Interop.Visio.Selection)subject);
                    subjectApplication = subjectSelection.Application;
                    prompt = groupCancelPrompt;
                    break;

                default:
                    // This event is not cancelable.
                    isQueryCancelEvent = false;
                    break;
            }

            if (isQueryCancelEvent == true) {
                // check for an alertResponse setting in Visio
                if (subjectApplication != null)
                    alertResponse = subjectApplication.AlertResponse;

                if (alertResponse != 0) {
                    // if alertResponse is No or Cancel then cancel this event
                    // by returning true
                    if ((alertResponse == (int)System.Windows.Forms.DialogResult.No) ||
                        (alertResponse == (int)System.Windows.Forms.DialogResult.Cancel))
                        returnValue = true;
                }
                else {
                    // alertResponse is not set so prompt the user
                    System.Windows.Forms.DialogResult result;
                    result = System.Windows.Forms.MessageBox.Show(prompt, eventSinkCaption, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);

                    if (result == System.Windows.Forms.DialogResult.No)
                        returnValue = true;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// This method adds an event description to the eventDescriptions 
        /// dictionary.</summary>
        /// <param name="eventCode">Event code of the event</param>
        /// <param name="description">Short description of the event</param>
        private void AddEventDescription(short eventCode, string description) {

            string key = Convert.ToString(eventCode, System.Globalization.CultureInfo.InvariantCulture);
            eventDescriptions.Add(key, description);
        }

        /// <summary>
        /// This method returns a short description for the given eventCode.
        /// </summary>
        /// <param name="eventCode">Event code</param>
        /// <returns>Short description of the eventCode</returns>
        private string GetEventDescription(short eventCode) {
            string description;
            string key;

            key = Convert.ToString(eventCode, System.Globalization.CultureInfo.InvariantCulture);
            description = eventDescriptions[key];

            if (description == null)
                description = "NoEventDescription";

            return description;
        }

        /// <summary>
        /// This method populates the eventDescriptions dictionary with a short 
        /// // description of each Visio event code.</summary>
        private void InitializeStrings() {
            eventDescriptions = new System.Collections.Specialized.StringDictionary();

            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAfterModal, "AfterModal");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeAfterResume, "AfterResume");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAppActivate, "AppActivated");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtAppDeactivate, "AppDeactivated");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtObjActivate, "AppObjActivated");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtObjDeactivate, "AppObjDeactivated");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel, "BeforeDocumentClose");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefDocSave, "BeforeDocumentSave");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefDocSaveAs, "BeforeDocumentSaveAs");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel, "BeforeMasterDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtBeforeModal, "BeforeModal");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel, "BeforePageDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtBeforeQuit, "BeforeQuit");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefSelDel, "BeforeSelectionDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel, "BeforeShapeDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeBeforeTextEdit, "BeforeShapeTextEdit");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel, "BeforeStyleDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBeforeSuspend, "BeforeSuspend");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel, "BeforeWindowClose");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefWinPageTurn, "BeforeWindowPageTurn");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefWinSelDel, "BeforeWindowSelDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCell + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "CellChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtConnect + visEvtAdd, "ConnectionsAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtConnect + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel, "ConnectionsDeleted");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelConvertToGroup, "ConvertToGroupCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocDesign, "DesignModeEntered");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + visEvtAdd, "DocumentAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDoc + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "DocumentChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelDocClose, "DocumentCloseCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocCreate, "DocumentCreated");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocOpen, "DocumentOpened");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocSave, "DocumentSaved");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocSaveAs, "DocumentSavedAs");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeEnterScope, "EnterScope");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeExitScope, "ExitScope");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtFormula + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "FormulaChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyDown, "KeyDown");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyPress, "KeyPress");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeKeyUp, "KeyUp");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + visEvtAdd, "MasterAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMarker, "MarkerEvent");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMaster + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "MasterChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelMasterDel, "MasterDeleteCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseDown, "MouseDown");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseMove, "MouseMove");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeMouseUp, "MouseUp");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeBefForcedFlush, "MustFlushScopeBeginning");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeAfterForcedFlush, "MustFlushScopeEnded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtNonePending, "NoEventsPending");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinOnAddonKeyMSG, "OnKeystrokeMessageForAddon");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + visEvtAdd, "PageAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtPage + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "PageChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelPageDel, "PageDeleteCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelConvertToGroup, "QueryCancelConvertToGroup");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelDocClose, "QueryCancelDocumentClose");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelMasterDel, "QueryCancelMasterDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelPageDel, "QueryCancelPageDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelQuit, "QuerCancelQuit");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSelDel, "QueryCancelSelectionDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelStyleDel, "QueryCancelStyleDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSuspend, "QueryCancelSuspend");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelUngroup, "QueryCancelUngroup");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelWinClose, "QueryCancelWindowClose");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelQuit, "QuitCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeDocRunning, "RunModeEntered");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeSelAdded, "SelectionAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinSelChange, "SelectionChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSelDel, "SelectionDeleteCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + visEvtAdd, "ShapeAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShape + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "ShapeChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeExitTextEdit, "ShapeExitedTextEdit");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeParentChange, "ShapeParentChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeShapeDelete, "ShapesDeleted");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + visEvtAdd, "StyleAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtStyle + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "StyleChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelStyleDel, "StyleDeleteCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSuspend, "SuspendCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtText + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "TextChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelUngroup, "UngroupCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeViewChanged, "ViewChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtIdle, "VisioIsIdle");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtApp + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWinActivate, "WindowActivated");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelWinClose, "WindowCloseCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + visEvtAdd, "WindowOpened");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtWindow + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "WindowChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeWinPageTurn, "WindowTurnedToPage");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeDataGraphicChanged, "ShapeDataGraphicChanged");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeLinkAdded, "ShapeLinkAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtShapeLinkDeleted, "ShapeLinkDeleted");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtRemoveHiddenInformation, "RemoveHiddenInformation");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeCancelSelGroup, "GroupCanceled");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtCodeQueryCancelSelGroup, "QueryCancelGroup");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + visEvtAdd, "DataRecordsetAdded");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDel, "BeforeDataRecordsetDelete");
            AddEventDescription((short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtDataRecordset + (short)Microsoft.Office.Interop.Visio.VisEventCodes.visEvtMod, "DataRecordsetChanged");
        }
    }
}

