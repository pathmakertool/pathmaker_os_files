using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;
using System.Text.RegularExpressions;

namespace PathMaker {
    public class ChangeLogShadow : Shadow {

        public ChangeLogShadow(Shape shape)
            : base(shape) {
        }

        override public void OnShapeProperties() {
            OnShapeDoubleClick();
        }

        override public void OnShapeDoubleClick() {
            ChangeLogForm form = new ChangeLogForm();
            form.ShowDialog(this);
            form.Dispose();
        }

        internal Table GetChangeLog() {
            return Common.GetCellTable(shape, ShapeProperties.ChangeLog.Changes);
        }

        internal string GetLastChangeVersion() {
            Table table = GetChangeLog();
            if (table.IsEmpty())
                return string.Empty;
            return table.GetData(table.GetNumRows()-1, (int)TableColumns.ChangeLog.Version);
        }

        internal string GetFirstChangeVersion()
        {
            Table table = GetChangeLog();
            if (table.IsEmpty())
                return string.Empty;
            return table.GetData(0, (int)TableColumns.ChangeLog.Version);
        }

        public DateTime GetLastChangeDate()
        {
            Table table = GetChangeLog();
            DateTime tempDateTime;

            if (table.IsEmpty())
                return new DateTime(1965, 4, 1);

            DateTime.TryParse(table.GetData(table.GetNumRows() - 1, (int)TableColumns.ChangeLog.Date), out tempDateTime);
            return tempDateTime;
        }

        public string GetLastLogChangeDate()
        {
            Table table = GetChangeLog();
            if (table.IsEmpty())
                return string.Empty;
            return table.GetData(table.GetNumRows() - 1, (int)TableColumns.ChangeLog.Date);
        }

        internal void SetChangeLog(Table table) {
            Common.SetCellTable(shape, ShapeProperties.ChangeLog.Changes, table);
        }

        /**
        * Utility method to build an array of valid versions in the current revTable returns the array
        */
        internal String GetValidVersionString(Table currentRevTable, String targetVersionMarker)
        {
            String validVersionString = "0.0";
            String tempVersionString;
            if (!currentRevTable.IsEmpty())
            {
                for (int i = 0; i < currentRevTable.GetNumRows(); i++)
                {
                    tempVersionString = currentRevTable.GetData(i, (int)TableColumns.ChangeLog.Version);
                    if (tempVersionString.Trim() == targetVersionMarker.Trim())
                    {
                        validVersionString = targetVersionMarker;
                        return validVersionString;
                    }
                }
            }
            return validVersionString;
        }

        /**
        * Utility method to return the version for the passed in change date
        */
        internal string GetVersionStringForChange(DateTime date)
        {
            Table table = GetChangeLog();
            //string color = Strings.HighlightColorNone;
            //string versionMarker = GetLastChangeVersion();//defaults to latest version
            string versionMarker = GetFirstChangeVersion();//defaults to latest version
            //string pattern = "[a-zA-Z-]+";
            //string replacement = "";
            //Regex rgx = new Regex(pattern);
            //string result = rgx.Replace(versionInfo, replacement);

            if (date == null)
                return versionMarker;

            for (int r = table.GetNumRows()-1; r >= 0; r--)
            {
                DateTime revisionDate;
                if (DateTime.TryParse(table.GetData(r, (int)TableColumns.ChangeLog.Date), out revisionDate))
                {
                    if (revisionDate <= date)
                    {
                        versionMarker = table.GetData(r, (int)TableColumns.ChangeLog.Version);
                        //return rgx.Replace(versionMarker, replacement);
                        //return versionMarker;//JDK Only want to return valid versions here - don't float to highest version!
                        return GetValidVersionString(table, versionMarker);
                    }
                }
            }

            //if (rgx.IsMatch(versionMarker))
            //{
              //  versionMarker = rgx.Replace(versionMarker, replacement);
            //}
            return versionMarker;
        }


        /**
         * Utility method to return the highlight color given a change date
         */
        internal string GetColorStringForChange(DateTime date) {
            Table table = GetChangeLog();
            string color = Strings.HighlightColorNone;

            if (date == null)
                return color;

            for (int r = 0; r < table.GetNumRows(); r++) {
                DateTime revisionDate;
                if (DateTime.TryParse(table.GetData(r, (int)TableColumns.ChangeLog.Date), out revisionDate)) {
                    if (revisionDate > date)
                        return color;
                    else
                        color = table.GetData(r, (int)TableColumns.ChangeLog.Highlight);
                }
            }
            return color;
        }

        /**
        * Utility method to return the highlight color given a change version label
        */
        internal string GetColorStringForChange(string versionLabel) {
            Table table = GetChangeLog();
            string color = Strings.HighlightColorNone;
            string tempVersionStampFix;
            //string pattern = "[a-zA-Z-]+";
            //string replacement = "";
            //Regex rgx = new Regex(pattern);

            //DateTime revisionDate;//from old style highlight fomratting
            DateTime date;
                        
            if (versionLabel == null || versionLabel == "")
                return color;

            versionLabel = Common.CleanupVersionLabel(versionLabel);//JDK ORIG
            //versionLabel = GetValidVersionString(table, Common.CleanupVersionLabel(versionLabel));//JDK added some extra validation;

            if (Common.ForcedStringVersionToDouble(versionLabel) > Common.ForcedStringVersionToDouble(GetLastChangeVersion()))
            {
                return color;
            }

            //JDK This check will catch any fields with dates still in the hidden stamp fields
            if (versionLabel.Contains("/") && DateTime.TryParse(versionLabel, out date))
            {
                tempVersionStampFix = PathMaker.LookupChangeLogShadow().GetVersionStringForChange(date);
                //versionLabel = tempVersionStampFix;
                versionLabel = GetValidVersionString(table, tempVersionStampFix);//JDK added some extra validation
                //versionLabel = rgx.Replace(tempVersionStampFix, replacement);
                //return GetColorStringForChange(date);//will only do this for backward compatibility support
            }

            for (int r = 0; r < table.GetNumRows(); r++) {
                double decimalVal = 0;
                String revisionVersion = table.GetData(r, (int)TableColumns.ChangeLog.Version);
                decimalVal = Common.ForcedStringVersionToDouble(revisionVersion);
                
                if (decimalVal != 0)
                {
                    if (decimalVal > Common.ForcedStringVersionToDouble(versionLabel))
                        return color;
                    else
                        color = table.GetData(r, (int)TableColumns.ChangeLog.Highlight);
                }                
            }
            return color;
        }
    }
}


