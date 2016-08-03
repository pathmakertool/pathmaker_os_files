﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    // StateShadows with Transitions will subclass from this
    public class StateWithTransitionShadow : StateShadow {

        public StateWithTransitionShadow(Shape shape)
            : base(shape) {
        }

        internal void SetDeveloperNotes(Table table) {
            Table tmp = GetDeveloperNotes();

            if ((table.GetData(0, 0) == null || table.GetData(0, 0).Length == 0) && tmp.IsEmpty())
                return;

            if (tmp.IsEmpty()) {
                //table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.DeveloperNotes, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.DeveloperNotes.Text).Equals(table.GetData(0, (int)TableColumns.DeveloperNotes.Text))) {
                //table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.DeveloperNotes, table);
            }
        }

        internal override void RedoHiddenDateMarkers(StateShadow stateShadow)
        {
            //use this to force hidden date fields to be version numbers
            Table table = GetTransitions();
            String lastVersionStamp = base.GetLastChangeVersion();
            String tempVersionStamp = "";
            Boolean labelsUpdated = false;

            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string actionDateString = table.GetData(r, (int)TableColumns.Transitions.ActionDateStamp);
                string conditionDateString = table.GetData(r, (int)TableColumns.Transitions.ConditionDateStamp);
                string gotoDateString = table.GetData(r, (int)TableColumns.Transitions.GotoDateStamp);

                if (!actionDateString.Equals("") && actionDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(actionDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.Transitions.ActionDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (!conditionDateString.Equals("") && conditionDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(conditionDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.Transitions.ConditionDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
                if (!gotoDateString.Equals("") && gotoDateString.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(gotoDateString, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.Transitions.GotoDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            
            if (labelsUpdated)
                SetTransitions(table);

            labelsUpdated = false;
            table = GetDeveloperNotes();
            for (int r = 0; r < table.GetNumRows(); r++)
            {
                string textDateStamp = table.GetData(r, (int)TableColumns.DeveloperNotes.TextDateStamp);

                if (textDateStamp.Contains("/"))
                {
                    DateTime revisionDate;
                    if (DateTime.TryParse(textDateStamp, out revisionDate))
                    {
                        tempVersionStamp = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(revisionDate);
                        table.SetData(r, (int)TableColumns.DeveloperNotes.TextDateStamp, tempVersionStamp);
                        labelsUpdated = true;
                    }
                }
            }
            if (labelsUpdated)
                SetDeveloperNotes(table);
            
        }


        internal void SetDesignNotes(Table table)
        {
            Table tmp = GetDesignNotes();

            if ((table.GetData(0, 0) == null || table.GetData(0, 0).Length == 0) && tmp.IsEmpty())
                return;

            if (tmp.IsEmpty())
            {
                //table.SetData(0, (int)TableColumns.DesignNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.DesignNotes.TextDateStamp, PathMaker.LookupChangeLogShadow().GetFirstChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.DesignNotes, table);
            }
            else if (!tmp.GetData(0, (int)TableColumns.DesignNotes.Text).Equals(table.GetData(0, (int)TableColumns.DesignNotes.Text)))
            {
                //table.SetData(0, (int)TableColumns.DesignNotes.TextDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                table.SetData(0, (int)TableColumns.DesignNotes.TextDateStamp, PathMaker.LookupChangeLogShadow().GetFirstChangeVersion());//JDK added
                
                Common.SetCellTable(shape, ShapeProperties.DesignNotes, table);
            }
        }

        /**
         * Because SetTransitions calls RemoveOutputsForDeletedTransitions, it can result in 
         * a shape delete.  When it's being called because of a connector delete, we can end
         * up with an error because we try to delete the same shape twice.  This avoids that.
         **/
        private void SetTransitionsWithoutRemovingOutputsForDeletedTransitions(Table table) {
            List<Connect> connects = GetShapeOutputs();

            for (int r = 0; r < table.GetNumRows(); r++) {
                string uid = table.GetData(r, (int)TableColumns.Transitions.Goto);
                ConnectorShadow shadow = PathMaker.LookupShadowByUID(uid) as ConnectorShadow;
                if (shadow != null)
                    shadow.SetLabelName(table.GetData(r, (int)TableColumns.Transitions.Condition));
            }

            Common.SetCellTable(shape, ShapeProperties.Transitions, table);
        }

        internal void SetTransitions(Table table) {
            SetTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
            RemoveOutputsIfNotInTableColumn(table, (int)TableColumns.Transitions.Goto);
        }

        internal Table GetDeveloperNotes() {
            return Common.GetCellTable(shape, ShapeProperties.DeveloperNotes);
        }

        internal Table GetDesignNotes()
        {
            return Common.GetCellTable(shape, ShapeProperties.DesignNotes);
        }

        internal Table GetTransitions() {
            return Common.GetCellTable(shape, ShapeProperties.Transitions);
        }

        public override void OnConnectAddOutput(Shadow shadow) {
            base.OnConnectAddOutput(shadow);
            Table table = GetTransitions();

            // make sure it's not already in there - this can happen with undo/redo
            for (int r = 0; r < table.GetNumRows(); r++)
                if (table.GetData(r, (int)TableColumns.Transitions.Goto).Equals(shadow.GetUniqueId()))
                    return;

            if (table.IsEmpty())
                table = new Table(1, Enum.GetNames(typeof(TableColumns.Transitions)).Length);
            else
                table.AddRow();

            ConnectorShadow connector = shadow as ConnectorShadow;
            if (connector != null) {
                string label = connector.GetLabelName();
                if (label.Length > 0) {
                    table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.Condition, CommonShadow.GetStringWithNewConnectorLabel("", label));
                    //table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.ConditionDateStamp, DateTime.Today.ToString(Strings.DateColumnFormatString));
                    table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.ConditionDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
                }
            }

            table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.Goto, shadow.GetUniqueId());
            //table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.GotoDateStamp, DateTime.Today.ToString(Strings.DateColumnFormatString));
            table.SetData(table.GetNumRows() - 1, (int)TableColumns.Transitions.GotoDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());
            SetTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
        }


        public override void OnConnectDeleteOutput(Shadow shadow) {
            base.OnConnectDeleteOutput(shadow);
            Table table = GetTransitions();
            for (int r = 0; r < table.GetNumRows(); r++) {
                string data = table.GetData(r, (int)TableColumns.Transitions.Goto);
                string uid = shadow.GetUniqueId();

                if (data.Equals(uid)) {
                    // cleanup the connector label
                    ConnectorShadow connectorShadow = PathMaker.LookupShadowByUID(uid) as ConnectorShadow;
                    if (connectorShadow != null)
                        connectorShadow.SetLabelName(String.Empty);

                    table.DeleteRow(r);
                    SetTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
                    // only one per link possible
                    return;
                }
            }
        }

        internal override DateTime GetLastChangeDate() {
            DateTime date = new DateTime(1965, 4, 1);
            Table table = GetTransitions();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.Transitions.ActionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.Transitions.ConditionDateStamp);
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.Transitions.GotoDateStamp);

            table = GetDeveloperNotes();
            date = Common.MaxDateWithDateColumn(date, table, (int)TableColumns.DeveloperNotes.TextDateStamp);

            return date;
        }

        //JDK added this to switch code to version stamp for highlighting
        internal override String GetLastChangeVersion()
        {
            String versionStamp = "0.0";//JDK was base.GetLastChangeVersion() - just setting a default string

            Table table = GetTransitions();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.Transitions.ActionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.Transitions.ConditionDateStamp);
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.Transitions.GotoDateStamp);
            SetTransitions(table);

            table = GetDeveloperNotes();
            versionStamp = Common.MaxVersionWithDateColumn(versionStamp, table, (int)TableColumns.DeveloperNotes.TextDateStamp);
            SetDeveloperNotes(table);

            return versionStamp;
        }

        public override void FixUIDReferencesAfterPaste(Dictionary<string, string> oldGUIDToNewGUIDMap) {
            Table table = GetTransitions();

            for (int row = table.GetNumRows() - 1; row >= 0; row--) {
                string oldUID = table.GetData(row, (int)TableColumns.Transitions.Goto);
                string newUID = CommonShadow.GetNewUIDAfterPaste(oldUID, oldGUIDToNewGUIDMap, false);
                if (newUID == null)
                    table.DeleteRow(row);
                else if (oldUID != newUID)
                    table.SetData(row, (int)TableColumns.Transitions.Goto, newUID);
            }
            SetTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
        }

        public override void OnBeforeShapeDelete() {
            base.OnBeforeShapeDelete();
            List<Connect> connects = GetShapeOutputs();

            Table table = GetTransitions();
            for (int r = 0; r < table.GetNumRows(); r++) {
                string uid = table.GetData(r, (int)TableColumns.Transitions.Goto);
                ConnectorShadow shadow = PathMaker.LookupShadowByUID(uid) as ConnectorShadow;
                if (shadow != null)
                    shadow.SetLabelName(string.Empty);
            }
        }

        public override void OnConnectorLabelChange(ConnectorShadow shadow) {
            Table table = GetTransitions();

            for (int r = 0; r < table.GetNumRows(); r++)
                if (table.GetData(r, (int)TableColumns.Transitions.Goto).Equals(shadow.GetUniqueId())) {

                    string condition = table.GetData(r, (int)TableColumns.Transitions.Condition);
                    string newCondition = CommonShadow.GetStringWithNewConnectorLabel(condition, shadow.GetLabelName());

                    if (!condition.Equals(newCondition)) {
                        table.SetData(r, (int)TableColumns.Transitions.Condition, newCondition);
                        //table.SetData(r, (int)TableColumns.Transitions.ConditionDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                        table.SetData(r, (int)TableColumns.Transitions.ConditionDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                        
                        SetTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
                    }
                    break;
                }
        }

        public override void OnConnectorChangeTarget(ConnectorShadow shadow) {
            Table table = GetTransitions();

            for (int r = 0; r < table.GetNumRows(); r++)
                if (table.GetData(r, (int)TableColumns.Transitions.Goto).Equals(shadow.GetUniqueId())) {
                    //table.SetData(r, (int)TableColumns.Transitions.GotoDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    table.SetData(r, (int)TableColumns.Transitions.GotoDateStamp, PathMaker.LookupChangeLogShadow().GetLastChangeVersion());//JDK added
                    
                    SetTransitionsWithoutRemovingOutputsForDeletedTransitions(table);
                    break;
                }
        }
    }
}
