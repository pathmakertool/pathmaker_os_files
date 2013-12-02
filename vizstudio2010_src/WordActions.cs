using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;

namespace PathMaker {
    class WordActions {
        // by keeping it static like this, we can reuse one - which means it will stay
        // in the old directory each time you do it
        private static Dictionary<Shadow, string> gotoNameCache = null;
        private static SaveFileDialog saveFileDialog = null;
        private static ChangeLogShadow changeLogShadow = null;
        private static Dictionary<string, List<string>> gotoMaxHandlerCache = null;
        private static OpenFileDialog openFileDialog = null;

        private class ParamCache {
            public DocTitleShadow docTitleShadow = null;
            public StartShadow startShadow = null;
            public AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl;
            public string targetFilename;
            public string currentFileName;

        }

        private class Templates {
            public static int ChangeLog = 3;
            public static int GlobalCommands = 13;
            public static int GlobalPromptTypes = 14;
            public static int DefaultSettings = 15;
            public static int GlobalMaxHandler = 16;
            public static int Decision = 17;
            public static int Data = 18;
            public static int Interaction = 19;
            public static int Play = 20;
            public static int SubDialog = 21;
            public static int Start = 22;
        }

        internal static void ExportUserInterfaceSpec(AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl) {
            ParamCache paramCache = new ParamCache();

            paramCache.visioControl = visioControl;
            paramCache.docTitleShadow = PathMaker.LookupDocTitleShadow();
            if (paramCache.docTitleShadow == null) {
                Common.ErrorMessage("Missing Document Title shape");
                return;
            }

            paramCache.startShadow = PathMaker.LookupStartShadow();
            if (paramCache.startShadow == null) {
                Common.ErrorMessage("Missing Start shape");
                return;
            }

            //Get changeLogShawdow to get version and date information
            changeLogShadow = PathMaker.LookupChangeLogShadow();
            if (changeLogShadow == null)
            {
                Common.ErrorMessage("Missing Change Log shape");
                return;
            }

            if (saveFileDialog == null)
                saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = PathMaker.getCurrentFileDirectory(visioControl);
            saveFileDialog.Title = Common.GetResourceString(Strings.SaveUISpecTitleRes);
            saveFileDialog.Filter = Common.GetResourceString(Strings.SaveUISpecFilterRes);
            saveFileDialog.FilterIndex = 1;
    
            paramCache.targetFilename = paramCache.visioControl.Src;
            paramCache.currentFileName = System.IO.Path.GetFileName(paramCache.targetFilename);
            saveFileDialog.FileName = Common.StripExtensionFileName(paramCache.currentFileName) + ".docx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                paramCache.targetFilename = saveFileDialog.FileName;
            else
                return;

            ProgressBarForm progressBarForm = new ProgressBarForm("Exporting UI", ExportUserInterfaceSpecWorker, paramCache);
            progressBarForm.ShowDialog();
        }

        private static bool ExportUserInterfaceSpecWorker(Object arg, ProgressBarForm progressBarForm) {
            ParamCache paramCache = arg as ParamCache;

            gotoNameCache = new Dictionary<Shadow, string>();

            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = false;

            string templateFilename = System.Windows.Forms.Application.StartupPath + @"\" + Common.GetResourceString(Strings.VUITemplateFileNameRes);
            Document doc = wordApp.Documents.Add(templateFilename);

            doc.BuiltInDocumentProperties["Author"] = "Convergys PathMaker";

            // output visio
            Selection content = wordApp.Selection;
            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "CallFlowDiagram");
            content.ClearFormatting();
            content.set_Style("Normal");
            content.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            foreach (Microsoft.Office.Interop.Visio.Page page in paramCache.visioControl.Document.Pages) {
                if (!page.Name.StartsWith("Background-") &&
                    !page.Name.Equals("Title") &&
                    !page.Name.Equals("Revision History")) {
                    string tmpFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".jpg";
                    page.Export(tmpFileName);
                    content.InlineShapes.AddPicture(tmpFileName);
                    content.InsertBreak(WdBreakType.wdPageBreak);
                }
            }

            // just show we're moving a little earlier
            progressBarForm.SetProgressPercentage(1, 100); 

            AddTitleAndLogo(doc, paramCache.docTitleShadow);
            AddChangeLog(doc, changeLogShadow);
            AddStartTables(doc, paramCache.startShadow);

            List<Shadow> shadowList = PathMaker.LookupAllShadows();

            for (int i = shadowList.Count - 1; i >= 0; i--) {
                StateShadow tmpShadow = shadowList[i] as StateShadow;
                if (tmpShadow == null)
                    shadowList.RemoveAt(i);
            }

            SetUpGotoMaxHandlerCache();

            string stateSortOrder = paramCache.startShadow.GetDefaultSetting(Strings.DefaultSettingsStateSortOrder);
            if (stateSortOrder.Equals(Strings.StateSortOrderAlphaNumerical))
                shadowList.Sort(Common.StateIdShadowSorterAlphaNumerical);
            else if (stateSortOrder.Equals(Strings.StateSortOrderNumericalOnly))
                shadowList.Sort(Common.StateIdShadowSorterNumericalAlpha);
            else
                Common.StateIdShadowSorterVisioHeuristic(shadowList, paramCache.visioControl.Document, paramCache.startShadow);

            int total = shadowList.Count;
            int progress = 0;
            foreach (Shadow shadow in shadowList) {
                progress++;
                switch (shadow.GetShapeType()) {
                    case ShapeTypes.Interaction:
                        AddInteractionTable(doc, shadow as InteractionShadow);
                        break; 
                    case ShapeTypes.Play:
                        AddPlayTable(doc, shadow as PlayShadow);
                        break;
                    case ShapeTypes.Decision:
                        AddDecisionTable(doc, shadow as DecisionShadow);
                        break;
                    case ShapeTypes.Data:
                        AddDataTable(doc, shadow as DataShadow);
                        break;
                    case ShapeTypes.SubDialog:
                        AddSubDialogTable(doc, shadow as SubDialogShadow);
                        break;
                    default:
                        break;
                }

                progressBarForm.SetProgressPercentage(progress, total);
                if (progressBarForm.Cancelled) {
                    ((_Application)wordApp).Quit(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                    gotoNameCache = null;
                    gotoMaxHandlerCache = null;
                    wordApp = null;
                    return false;
                }
            }

            doc.Tables[Templates.GlobalCommands].Delete();
            doc.Tables[Templates.GlobalPromptTypes].Delete();
            doc.Tables[Templates.DefaultSettings].Delete();
            doc.Tables[Templates.GlobalMaxHandler].Delete();

            if (paramCache.startShadow.GetDefaultSetting(Strings.DefaultSettingsMode).Equals(Strings.ModeSpeech))
                doc.Bookmarks["TouchTone"].Range.Delete();
            else
                doc.Bookmarks["Speech"].Range.Delete();

            doc.Bookmarks["TempPages"].Range.Delete();
            doc.Fields.Update();

            gotoNameCache = null;
            gotoMaxHandlerCache = null;

            progressBarForm.SetProgressPercentage(total, total);
            System.Windows.Forms.Application.DoEvents();
            doc.SaveAs(paramCache.targetFilename);

            ((_Application)wordApp).Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp); 
            wordApp = null;
            return true;
        }

        private static void SetUpGotoMaxHandlerCache() {
            gotoMaxHandlerCache = new Dictionary<string, List<string>>();

            StartShadow startShadow;

            List<Shadow> shadowList = PathMaker.LookupShadowsByShapeType(ShapeTypes.Start);
            if (shadowList.Count > 0) {
                startShadow = shadowList[0] as StartShadow;
                Table table = startShadow.GetMaxHandling();
                AddMaxHandlingToCache("Start", table);
            }

            shadowList = PathMaker.LookupShadowsByShapeType(ShapeTypes.Interaction);
            foreach (Shadow s in shadowList) {
                InteractionShadow interactionShadow = s as InteractionShadow;
                Table table = interactionShadow.GetMaxHandling();
                AddMaxHandlingToCache(interactionShadow.GetStateId(), table);
            }
        }

        private static void AddMaxHandlingToCache(string source, Table table) {
            for (int row = 0; row < table.GetNumRows(); row++) {
                string gotoData = table.GetData(row, (int)TableColumns.MaxHandling.Goto);
                if (gotoData == null || gotoData.Length == 0)
                    continue;
                Shadow targetShadow = Common.GetGotoTargetFromData(gotoData);
                string targetName;

                if (targetShadow != null) {
                    targetName = CachedGetGotoName(targetShadow);

                    List<string> fromStates;
                    if (gotoMaxHandlerCache.TryGetValue(targetName, out fromStates))
                        fromStates.Add(source);
                    else {
                        fromStates = new List<string>();
                        fromStates.Add(source);
                        gotoMaxHandlerCache.Add(targetName, fromStates);
                    }
                }
            }
        }

        private static string CachedGetGotoName(Shadow shadow) {
            string name;
            if (gotoNameCache.TryGetValue(shadow, out name))
                return name;
            name = shadow.GetGotoName();
            gotoNameCache.Add(shadow, name);
            return name;
        }

        private static void AddTitleAndLogo(Document doc, DocTitleShadow docTitleShadow) {
            Selection content = doc.Application.Selection;
            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "TitleLine1");
            content.TypeText(docTitleShadow.GetClientName());

            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "TitleLine2");
            content.TypeText(docTitleShadow.GetProjectName());

            content.Move(Unit: WdUnits.wdStory);

            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "TitleLogo");
            string logoData = docTitleShadow.GetLogoData();
            if (logoData.Length > 0) {
                string tmpFileName = System.IO.Path.GetTempFileName();
                FileStream fs = new FileStream(tmpFileName, FileMode.Create, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);
                byte[] imageBytes = Convert.FromBase64String(logoData);
                bw.Write(imageBytes);
                bw.Close();
                fs.Close();

                content.InlineShapes.AddPicture(tmpFileName);
            }
            content.Move(Unit: WdUnits.wdStory);
        }

        private static void AddChangeLog(Document doc, ChangeLogShadow changeLogShadow) {
            Microsoft.Office.Interop.Word.Table changeLogTable = doc.Tables[Templates.ChangeLog];
            Table table = changeLogShadow.GetChangeLog();
            InsertWordTableRows(changeLogTable, 2, table.GetNumRows() - 1);

            int wordRow = 2;
            for (int row = 0; row < table.GetNumRows(); row++) {
                changeLogTable.Cell(wordRow, 1).Range.InsertAfter(table.GetData(row, (int)TableColumns.ChangeLog.Date));
                changeLogTable.Cell(wordRow, 2).Range.InsertAfter(table.GetData(row, (int)TableColumns.ChangeLog.Version));
                changeLogTable.Cell(wordRow, 3).Range.InsertAfter(table.GetData(row, (int)TableColumns.ChangeLog.Details));
                changeLogTable.Cell(wordRow, 4).Range.InsertAfter(table.GetData(row, (int)TableColumns.ChangeLog.Author));

                string color = table.GetData(row, (int)TableColumns.ChangeLog.Highlight);
                WdColorIndex colorIndex = ConvertStringToColorIndex(color);
                for (int i = 1; i <= 4; i++) 
                    changeLogTable.Cell(wordRow, i).Range.Font.Shading.BackgroundPatternColorIndex = colorIndex;
                wordRow++;
            }

            Selection content = doc.Application.Selection;
            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "TitleVersion");
            content.TypeText("Version " + changeLogShadow.GetLastChangeVersion());
            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "TitleLastModifiedDate");
            DateTime date;
            if (DateTime.TryParse(changeLogShadow.GetLastLogChangeDate(), out date))
                content.TypeText(date.ToString("MMMM dd, yyyy"));
        }

        private static WdColorIndex ConvertStringToColorIndex(string color) {
            if (color.Equals(Strings.HighlightColorAqua))
                return WdColorIndex.wdTurquoise;
            else if (color.Equals(Strings.HighlightColorBlue))
                return WdColorIndex.wdBlue;
            else if (color.Equals(Strings.HighlightColorGreen))
                return WdColorIndex.wdBrightGreen;
            else if (color.Equals(Strings.HighlightColorNone))
                return WdColorIndex.wdNoHighlight;
            else if (color.Equals(Strings.HighlightColorPink))
                return WdColorIndex.wdPink;
            else if (color.Equals(Strings.HighlightColorYellow))
                return WdColorIndex.wdYellow;
            else
                return WdColorIndex.wdNoHighlight;
        }

        private static void InsertWordTableRows(Microsoft.Office.Interop.Word.Table wordTable, int rowNumberToStartInsert, int numberOfNewRows) {
            Range range = wordTable.Cell(rowNumberToStartInsert, 1).Range;
            range.MoveStart(WdUnits.wdCell, -1);
            range.Collapse(WdCollapseDirection.wdCollapseEnd);
            if (numberOfNewRows > 0) {
                for (int i = 0; i < numberOfNewRows; i++) {
                    wordTable.Rows.Add(range);
                }
            }
        }

        private static void AddStartTables(Document doc, StartShadow startShadow) {
            Selection content = doc.Application.Selection;

            // always work bottom of table to top so we can add rows and not move the row starts for later actions
            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "GlobalCommands");
            doc.Tables[Templates.GlobalCommands].Range.Copy();
            content.Paste();
            Table table = startShadow.GetConfirmationPrompts();
            Microsoft.Office.Interop.Word.Table globalCommandTable = doc.Tables[doc.Tables.Count - 4]; 
            FillConfirmationPromptTable(globalCommandTable, 5, table);
            table = startShadow.GetCommandTransitions();
            FillCommandTransitionTable(globalCommandTable, 2, table);
            
            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "GlobalPrompts");
            doc.Tables[Templates.GlobalPromptTypes].Range.Copy();
            content.Paste();
            table = startShadow.GetPromptTypes();
            Microsoft.Office.Interop.Word.Table globalPromptTypesTable = doc.Tables[doc.Tables.Count - 3];
            FillPromptTypesTable(globalPromptTypesTable, 3, table);

            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "DefaultSettings");
            doc.Tables[Templates.DefaultSettings].Range.Copy();
            content.Paste();
            table = startShadow.GetDefaultSettings();
            Microsoft.Office.Interop.Word.Table defaultSettingsTable = doc.Tables[doc.Tables.Count - 2];
            FillNameValuePairs(defaultSettingsTable, 2, table);

            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "MaxHandling");
            doc.Tables[Templates.GlobalMaxHandler].Range.Copy();
            content.Paste();
            table = startShadow.GetMaxHandling();
            Microsoft.Office.Interop.Word.Table maxHandlingTable = doc.Tables[doc.Tables.Count - 1];
            FillMaxHandling(maxHandlingTable, 2, table);

            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "DialogStates");
            doc.Tables[Templates.Start].Range.Copy();
            content.Bookmarks.Add("bmApplicationStart");
            content.Paste();
            Microsoft.Office.Interop.Word.Table startTable = doc.Tables[doc.Tables.Count];
            Shadow firstShadow = startShadow.GetFirstStateGotoTarget();
            if (firstShadow != null) {
                Selection sel = startTable.Cell(3, 1).Application.Selection;
                sel.InsertAfter(CachedGetGotoName(firstShadow));
                sel.set_Style("HyperLink");
                string link = "bm" + Left(AlphaNumericCharsOnly(CachedGetGotoName(firstShadow)), 38);
                sel.Document.Hyperlinks.Add(Anchor: sel.Range, SubAddress: link);
                sel.set_Style("TableNormalCell");
                sel.MoveStart(WdUnits.wdWord, -1);
                sel.Cut();
                Range range = startTable.Cell(3, 1).Range;
                range.Paste();
            }

            table = startShadow.GetInitialization();
            FillNameValuePairs(startTable, 6, table);

            startTable.Range.ParagraphFormat.KeepTogether = -1; // = true 

            SetCellBackgroundColorIfNecessary(startTable.Cell(1, 1), startShadow.GetLastChangeDate());

            content.Move(WdUnits.wdStory);
            content.set_Style("Normal");
            content.TypeParagraph();
            content.Move(WdUnits.wdStory);
        }

        private static void SetCellBackgroundColorIfNecessary(Cell cell, DateTime dateTime) {
            WdColorIndex index = GetHighlightColorIndex(dateTime);
            if (index != WdColorIndex.wdNoHighlight)
                cell.Range.Font.Shading.BackgroundPatternColorIndex = index;
        }

        private static void SetCellBackgroundColorIfNecessary(Cell cell, string dateTime) {
            WdColorIndex index = GetHighlightColorIndex(dateTime);
            if (index != WdColorIndex.wdNoHighlight)
                cell.Range.Font.Shading.BackgroundPatternColorIndex = index;
        }
        
        private static void FillMaxHandling(Microsoft.Office.Interop.Word.Table wordTable, int wordTableBeginRow, Table table) {
            InsertWordTableRows(wordTable, wordTableBeginRow, table.GetNumRows() - 1);

            for (int row = 0; row < table.GetNumRows(); row++) {
                string condition = table.GetData(row, (int)TableColumns.MaxHandling.Condition);
                string count = table.GetData(row, (int)TableColumns.MaxHandling.Count);
                string action = table.GetData(row, (int)TableColumns.MaxHandling.Action);
                string goingTo = table.GetData(row, (int)TableColumns.MaxHandling.Goto);
                string countDate = table.GetData(row, (int)TableColumns.MaxHandling.CountDateStamp);
                string actionDate = table.GetData(row, (int)TableColumns.MaxHandling.ActionDateStamp);
                string goingToDate = table.GetData(row, (int)TableColumns.MaxHandling.GotoDateStamp);

                if (count.Trim().Length == 0)
                    count = Strings.MaxHandlerGlobalReferenceNote;
                if (action.Trim().Length == 0 && goingTo.Trim().Length == 0)
                    action = Strings.MaxHandlerGlobalReferenceNote;

                Cell cell = wordTable.Cell(wordTableBeginRow + row, 1);
                cell.Range.InsertAfter(condition);

                cell = wordTable.Cell(wordTableBeginRow + row, 2);
                cell.Range.InsertAfter(count);
                SetCellBackgroundColorIfNecessary(cell, countDate);

                BuildConditionActionGotoCell(wordTable.Cell(wordTableBeginRow + row, 3), "", "", action, actionDate, goingTo, goingToDate);
            }
        }

        private static void FillNameValuePairs(Microsoft.Office.Interop.Word.Table wordTable, int wordTableBeginRow, Table table) {
            InsertWordTableRows(wordTable, wordTableBeginRow, table.GetNumRows() - 1);

            for (int row = 0; row < table.GetNumRows(); row++) {
                string name = table.GetData(row, (int)TableColumns.NameValuePairs.Name);
                string value = table.GetData(row, (int)TableColumns.NameValuePairs.Value);
                string nameDate = table.GetData(row, (int)TableColumns.NameValuePairs.NameDateStamp);
                string valueDate = table.GetData(row, (int)TableColumns.NameValuePairs.ValueDateStamp);

                Cell cell = wordTable.Cell(wordTableBeginRow + row, 1);
                cell.Range.InsertAfter(name);
                SetCellBackgroundColorIfNecessary(cell, nameDate);

                cell = wordTable.Cell(wordTableBeginRow + row, 2);
                cell.Range.InsertAfter(value);
                SetCellBackgroundColorIfNecessary(cell, valueDate);
            }
        }

        private static void FillPromptTypesTable(Microsoft.Office.Interop.Word.Table wordTable, int wordTableBeginRow, Table table) {
            // prompts with conditions get split over 2 rows - need to add extra for them
            int conditionsWithPrompts = CountConditionsWithPrompts(table, (int)TableColumns.ConfirmationPrompts.Condition, (int)TableColumns.ConfirmationPrompts.Wording);
            // There's already one row in the template, so always subtract 1
            InsertWordTableRows(wordTable, wordTableBeginRow, table.GetNumRows() + conditionsWithPrompts - 1);

            int level = 0;
            int wordingLevel = 0;
            int currentWordTableRow = wordTableBeginRow;
            for (int row = 0; row < table.GetNumRows(); row++) {
                string type = table.GetData(row, (int)TableColumns.PromptTypes.Type);
                string condition = table.GetData(row, (int)TableColumns.PromptTypes.Condition);
                string wording = Common.StripBracketLabels(table.GetData(row, (int)TableColumns.PromptTypes.Wording));
                string id = table.GetData(row, (int)TableColumns.PromptTypes.Id);
                string typeDate = table.GetData(row, (int)TableColumns.PromptTypes.TypeDateStamp);
                string conditionDate = table.GetData(row, (int)TableColumns.PromptTypes.ConditionDateStamp);
                string wordingDate = table.GetData(row, (int)TableColumns.PromptTypes.WordingDateStamp);
                string idDate = table.GetData(row, (int)TableColumns.PromptTypes.IdDateStamp);

                Cell cell = wordTable.Cell(currentWordTableRow, 1);
                if (OptionOrPromptTypeEntriesSameAsPrevious(table, row, (int)TableColumns.ConfirmationPrompts.Option))
                    cell.Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;
                else {
                    cell.Range.InsertAfter(Common.StripBracketLabels(type));
                    level = 0;
                    wordingLevel = 0;
                }
                SetCellBackgroundColorIfNecessary(cell, typeDate);

                cell = wordTable.Cell(currentWordTableRow, 2);
                if (condition.Length > 0) {
                    level = Common.GetConditionLevel(condition);
                    wordingLevel = level + 1;
                }
                condition = condition.Replace(Strings.IndentCharacterString, "").Trim();
                if (condition.Length > 0) {
                    cell.Range.ParagraphFormat.IndentCharWidth((short)level);
                    cell.Range.InsertAfter(condition);
                    cell.Range.Font.Italic = 1;
                    SetCellBackgroundColorIfNecessary(cell, conditionDate);
                }

                if (wording.Length > 0) {
                    if (condition.Length > 0) {
                        // goes on a separate line after the condition row and indented
                        currentWordTableRow++;
                        // clear the border in column 1 so it looks like part of the one above
                        wordTable.Cell(currentWordTableRow, 1).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;
                    }
                    cell = wordTable.Cell(currentWordTableRow, 2);
                    cell.Range.ParagraphFormat.IndentCharWidth((short)wordingLevel);
                    cell.Range.InsertAfter(wording);
                    SetCellBackgroundColorIfNecessary(cell, wordingDate);
                }

                if (id.Length > 0) {
                    cell = wordTable.Cell(currentWordTableRow, 3);
                    cell.Range.InsertAfter(id);
                    SetCellBackgroundColorIfNecessary(cell, idDate);
                }

                currentWordTableRow++;
            }
        }

        private static void FillCommandTransitionTable(Microsoft.Office.Interop.Word.Table wordTable, int wordTableBeginRow, Table table) {
            InsertWordTableRows(wordTable, wordTableBeginRow, table.GetNumRows() - 1);

            for (int row = 0; row < table.GetNumRows(); row++) {
                string option = table.GetData(row, (int)TableColumns.CommandTransitions.Option);
                string vocab = table.GetData(row, (int)TableColumns.CommandTransitions.Vocab);
                string dtmf = table.GetData(row, (int)TableColumns.CommandTransitions.DTMF);
                string condition = table.GetData(row, (int)TableColumns.CommandTransitions.Condition);
                string action = table.GetData(row, (int)TableColumns.CommandTransitions.Action);
                string goingTo = table.GetData(row, (int)TableColumns.CommandTransitions.Goto);
                string confirm = table.GetData(row, (int)TableColumns.CommandTransitions.Confirm);
                string optionDate = table.GetData(row, (int)TableColumns.CommandTransitions.OptionDateStamp);
                string vocabDate = table.GetData(row, (int)TableColumns.CommandTransitions.VocabDateStamp);
                string dtmfDate = table.GetData(row, (int)TableColumns.CommandTransitions.DTMFDateStamp);
                string conditionDate = table.GetData(row, (int)TableColumns.CommandTransitions.ConditionDateStamp);
                string actionDate = table.GetData(row, (int)TableColumns.CommandTransitions.ActionDateStamp);
                string goingToDate = table.GetData(row, (int)TableColumns.CommandTransitions.GotoDateStamp);
                string confirmDate = table.GetData(row, (int)TableColumns.CommandTransitions.ConfirmDateStamp);

                Cell cell = wordTable.Cell(wordTableBeginRow + row, 1);
                if (OptionOrPromptTypeEntriesSameAsPrevious(table, row, (int)TableColumns.CommandTransitions.Option))
                    cell.Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;
                else
                    cell.Range.InsertAfter(Common.StripBracketLabels(option));
                SetCellBackgroundColorIfNecessary(cell, optionDate);

                cell = wordTable.Cell(wordTableBeginRow + row, 2);
                cell.Range.InsertAfter(vocab);
                SetCellBackgroundColorIfNecessary(cell, vocabDate);

                cell = wordTable.Cell(wordTableBeginRow + row, 3);
                cell.Range.InsertAfter(dtmf);
                SetCellBackgroundColorIfNecessary(cell, dtmfDate);

                cell = wordTable.Cell(wordTableBeginRow + row, 4);
                BuildConditionActionGotoCell(cell, condition, conditionDate, action, actionDate, goingTo, goingToDate);

                cell = wordTable.Cell(wordTableBeginRow + row, 5);
                cell.Range.InsertAfter(confirm);
                SetCellBackgroundColorIfNecessary(cell, confirmDate);
            }
        }

        private static void BuildConditionActionGotoCell(Cell cell, string condition, string conditionDate, string action, string actionDate, string goingTo, string goingToDate) {
            if (condition.Length > 0) {
                cell.Range.InsertAfter(condition);
                WdColorIndex ci = GetHighlightColorIndex(conditionDate);
                for (int i = 0; i < cell.Range.Characters.Count; i++) {
                    if (ci != WdColorIndex.wdNoHighlight)
                        cell.Range.Characters[i + 1].Font.Shading.BackgroundPatternColorIndex = ci;
                    cell.Range.Font.Italic = 1;
                }
            }

            if (action.Length > 0) {
                WdColorIndex ci = GetHighlightColorIndex(actionDate);
                int beforeCount = cell.Range.Characters.Count;
                if (condition.Length > 0) {
                    cell.Range.InsertAfter("\r\n    ");
                    action = action.Replace("\r\n", "\r\n    ");
                }
                cell.Range.InsertAfter(action);
                for (int i = beforeCount; i < cell.Range.Characters.Count; i++)
                    if (ci != WdColorIndex.wdNoHighlight)
                        cell.Range.Characters[i+1].Font.Shading.BackgroundPatternColorIndex = ci;
            }

            if (goingTo.Length > 0) {
                Shadow targetShadow = Common.GetGotoTargetFromData(goingTo);
                string targetName;

                if (targetShadow == null)
                    targetName = goingTo;
                else
                    targetName = CachedGetGotoName(targetShadow);

                WdColorIndex ci = GetHighlightColorIndex(goingToDate);
                if (GoingToNonState(goingTo)) {
                    int beforeCount = cell.Range.Characters.Count;
                    if (condition.Length > 0)
                        cell.Range.InsertAfter("\r\n    ");
                    else if (action.Length > 0)
                        cell.Range.InsertAfter("\r\n");
                    cell.Range.InsertAfter("Go To: ");
                    cell.Range.InsertAfter(targetName);
                    if (ci != WdColorIndex.wdNoHighlight)
                        for (int i = beforeCount; i < cell.Range.Characters.Count; i++)
                            cell.Range.Characters[i + 1].Font.Shading.BackgroundPatternColorIndex = ci;
                }
                else {
                    int beforeCount = cell.Range.Characters.Count;
                    if (condition.Length > 0)
                        cell.Range.InsertAfter("\r\n    ");
                    else if (action.Length > 0)
                        cell.Range.InsertAfter("\r\n");
                    cell.Range.InsertAfter("Go To: ");
                    if (ci != WdColorIndex.wdNoHighlight)
                        for (int i = beforeCount; i < cell.Range.Characters.Count; i++)
                            cell.Range.Characters[i + 1].Font.Shading.BackgroundPatternColorIndex = ci;
                    Selection sel = cell.Application.Selection;
                    sel.InsertAfter(targetName);
                    sel.set_Style("HyperLink");
                    string link = "bm" + Left(AlphaNumericCharsOnly(targetName), 38);
                    sel.Document.Hyperlinks.Add(Anchor: sel.Range, SubAddress: link);
                    int count = cell.Range.Characters.Count;
                    sel.set_Style("TableNormalCell");
                    sel.MoveStart(WdUnits.wdWord, -1);
                    sel.Cut();
                    Range range = cell.Range.Characters[count - 1];
                    range.Paste();
                }
            }
        }

        private static string AlphaNumericCharsOnly(string goingTo) {
            const string alphaNumericString = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string tmp = "";
            foreach (char c in goingTo)
                if (alphaNumericString.Contains(c))
                    tmp += c;
            return tmp;
        }

        private static bool GoingToNonState(string goingTo) {
            Shadow shadow = PathMaker.LookupShadowByUID(goingTo);
            if (shadow == null)
                return true;
            Shadow target = shadow.GetDestinationTarget();
            StateShadow stateShadow = target as StateShadow;
            if (stateShadow == null)
                return true;
            return false;
        }

        private static void FillConfirmationPromptTable(Microsoft.Office.Interop.Word.Table wordTable, int wordTableBeginRow, Table table) {
            // prompts with conditions get split over 2 rows - need to add extra for them
            int conditionsWithPrompts = CountConditionsWithPrompts(table, (int)TableColumns.ConfirmationPrompts.Condition, (int)TableColumns.ConfirmationPrompts.Wording);
            // There's already one row in the template, so always subtract 1
            InsertWordTableRows(wordTable, wordTableBeginRow, table.GetNumRows() + conditionsWithPrompts - 1);

            int level = 0;
            int wordingLevel = 0;
            int currentWordTableRow = wordTableBeginRow;
            for (int row = 0; row < table.GetNumRows(); row++) {
                string option = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Option);
                string condition = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Condition);
                string wording = Common.StripBracketLabels(table.GetData(row, (int)TableColumns.ConfirmationPrompts.Wording));
                string id = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Id);
                string optionDate = table.GetData(row, (int)TableColumns.ConfirmationPrompts.OptionDateStamp);
                string conditionDate = table.GetData(row, (int)TableColumns.ConfirmationPrompts.ConditionDateStamp);
                string wordingDate = table.GetData(row, (int)TableColumns.ConfirmationPrompts.WordingDateStamp);
                string idDate = table.GetData(row, (int)TableColumns.ConfirmationPrompts.IdDateStamp);

                Cell cell = wordTable.Cell(currentWordTableRow, 1);
                if (OptionOrPromptTypeEntriesSameAsPrevious(table, row, (int)TableColumns.ConfirmationPrompts.Option))
                    cell.Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;
                else {
                    cell.Range.InsertAfter(Common.StripBracketLabels(option));
                    level = 0;
                    wordingLevel = 0;
                }
                SetCellBackgroundColorIfNecessary(cell, optionDate);

                cell = wordTable.Cell(currentWordTableRow, 2);
                if (condition.Length > 0) {
                    level = Common.GetConditionLevel(condition);
                    wordingLevel = level + 1;
                }
                condition = condition.Replace(Strings.IndentCharacterString, "").Trim();
                if (condition.Length > 0) {
                    cell.Range.ParagraphFormat.IndentCharWidth((short)level);
                    cell.Range.InsertAfter(condition);
                    cell.Range.Font.Italic = 1;
                    SetCellBackgroundColorIfNecessary(cell, conditionDate);
                }

                if (wording.Length > 0) {
                    if (condition.Length > 0) {
                        // goes on a separate line after the condition row and indented
                        currentWordTableRow++;
                        // clear the highlight in column 1 so it looks like part of the one above
                        wordTable.Cell(currentWordTableRow, 1).Range.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleNone;
                    }
                    cell = wordTable.Cell(currentWordTableRow, 2);
                    cell.Range.ParagraphFormat.IndentCharWidth((short)wordingLevel);
                    cell.Range.InsertAfter(wording);
                    SetCellBackgroundColorIfNecessary(cell, wordingDate);
                }

                if (id.Length > 0) {
                    cell = wordTable.Cell(currentWordTableRow, 3);
                    cell.Range.InsertAfter(id);
                    SetCellBackgroundColorIfNecessary(cell, idDate);
                }

                currentWordTableRow++;
            }
        }

        private static int CountConditionsWithPrompts(Table table, int conditionColumn, int wordingColumn) {
            int count = 0;
            for (int r = 0; r < table.GetNumRows(); r++) {
                string condition = table.GetData(r, conditionColumn);
                string wording = table.GetData(r, wordingColumn);

                condition = condition.Replace(Strings.IndentCharacterString, "").Trim();

                if (condition.Length > 0 && wording.Length > 0)
                    count++;
            }
            return count;
        }

        private static bool OptionOrPromptTypeEntriesSameAsPrevious(Table table, int row, int column) {
            if (row == 0)
                return false;

            string data = table.GetData(row, column);
            string previousData = table.GetData(row - 1, column);

            if (data.Length == 0)
                return true;

            if (Common.StripBracketLabels(data).Equals(Common.StripBracketLabels(previousData)))
                return true;

            return false;
        }

        public static WdColorIndex GetHighlightColorIndex(string dateString) {
            DateTime date;
            if (DateTime.TryParse(dateString, out date))
                return GetHighlightColorIndex(date);
            else
                return WdColorIndex.wdNoHighlight;
        }

        public static WdColorIndex GetHighlightColorIndex(DateTime date) {
            if (changeLogShadow == null)
                return WdColorIndex.wdNoHighlight;

            string color = changeLogShadow.GetColorStringForChange(date);

            return ConvertStringToColorIndex(color);
        }

        private static string Left(string input, int count) {
            if (input == null || input.Length <= count)
                return input;
            else
                return input.Substring(0, count);
        }

        private static void AddInteractionTable(Document doc, InteractionShadow interactionShadow) {
            Selection content = doc.Application.Selection;

            doc.Tables[Templates.Interaction].Range.Copy();
            content.Bookmarks.Add("bm" + Left(AlphaNumericCharsOnly(interactionShadow.GetStateId()), 38));
            content.Move(WdUnits.wdStory);
            content.Paste();

            Microsoft.Office.Interop.Word.Table wordTable = doc.Tables[doc.Tables.Count];

            wordTable.Range.ParagraphFormat.KeepWithNext = -1;
            wordTable.Cell(1, 1).Range.InsertAfter(interactionShadow.GetStateId());
            wordTable.Cell(1, 2).Range.InsertAfter("Interaction");
            FillEnteringFrom(wordTable.Cell(3, 1), interactionShadow);

            Cell cell = wordTable.Cell(17, 1);
            Table table = interactionShadow.GetDeveloperNotes();
            if (!table.IsEmpty()) {
                string notes = table.GetData(0, (int)TableColumns.DeveloperNotes.Text);
                string notesDate = table.GetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp);
                cell.Range.InsertAfter(notes);
                SetCellBackgroundColorIfNecessary(cell, notesDate);
            }

            cell = wordTable.Cell(15, 1);
            table = interactionShadow.GetSpecialSettings();
            if (!table.IsEmpty()) {
                string settings = table.GetData(0, (int)TableColumns.SpecialSettings.Text);
                string settingsDate = table.GetData(0, (int)TableColumns.SpecialSettings.TextDateStamp);
                cell.Range.InsertAfter(settings);
                SetCellBackgroundColorIfNecessary(cell, settingsDate);
            }

            table = interactionShadow.GetMaxHandling();
            bool hasOverride = false;
            for (int r = 0; r < table.GetNumRows() && hasOverride == false; r++)
                for (int c = 1; c < table.GetNumColumns() && hasOverride == false; c++)
                    if (table.GetData(r, c).Length > 0)
                        hasOverride = true;
            if (hasOverride)
                FillMaxHandling(wordTable, 13, table);
            else {
                wordTable.Range.Rows[12].Delete();
                wordTable.Range.Rows[12].Delete();
            }

            table = interactionShadow.GetConfirmationPrompts();
            FillConfirmationPromptTable(wordTable, 11, table);
            table = interactionShadow.GetCommandTransitions();
            FillCommandTransitionTable(wordTable, 8, table);
            table = interactionShadow.GetPromptTypes();
            FillPromptTypesTable(wordTable, 6, table);

            SetCellBackgroundColorIfNecessary(wordTable.Cell(1, 1), interactionShadow.GetLastChangeDate());

            content.Move(WdUnits.wdStory);
            content.set_Style("Normal");
            content.TypeParagraph();
            content.Move(WdUnits.wdStory);
        }

        private static void FillEnteringFrom(Cell cell, StateShadow shadow) {
            List<string> list = shadow.GetEnteringFromTargetNames();
            List<string> maxList;

            HashSet<string> uniqueList = new HashSet<string>();
            foreach (string s in list)
                uniqueList.Add(s);
            if (gotoMaxHandlerCache.TryGetValue(shadow.GetStateId(), out maxList))
                foreach (string s in maxList)
                    uniqueList.Add(s);

            list = uniqueList.ToList();

            bool first = true;
            cell.Range.InsertAfter(" ");
            foreach (string enteringFrom in list) {
                if (!first)
                    cell.Range.InsertAfter(", ");
                first = false;

                Selection sel = cell.Application.Selection; 
                sel.InsertAfter(enteringFrom);
                sel.set_Style("HyperLink");
                string link = "bm" + Left(AlphaNumericCharsOnly(enteringFrom), 38);
                sel.Document.Hyperlinks.Add(Anchor: sel.Range, SubAddress: link);
                int count = cell.Range.Characters.Count;
                sel.set_Style("TableNormalCell");
                sel.MoveStart(WdUnits.wdWord, -1);
                sel.Cut();
                Range range = cell.Range.Characters[count - 1];
                range.Paste();
                
            }

        }

        private static void AddPlayTable(Document doc, PlayShadow playShadow) {
            Selection content = doc.Application.Selection;

            doc.Tables[Templates.Play].Range.Copy();
            content.Bookmarks.Add("bm" + Left(AlphaNumericCharsOnly(playShadow.GetStateId()), 38));
            content.Move(WdUnits.wdStory);
            content.Paste();

            Microsoft.Office.Interop.Word.Table wordTable = doc.Tables[doc.Tables.Count];

            wordTable.Range.ParagraphFormat.KeepWithNext = -1;
            wordTable.Cell(1, 1).Range.InsertAfter(playShadow.GetStateId());
            FillEnteringFrom(wordTable.Cell(3, 1), playShadow);

            Cell cell = wordTable.Cell(12, 1);
            Table table = playShadow.GetDeveloperNotes();
            if (!table.IsEmpty()) {
                string notes = table.GetData(0, (int)TableColumns.DeveloperNotes.Text);
                string notesDate = table.GetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp);
                cell.Range.InsertAfter(notes);
                SetCellBackgroundColorIfNecessary(cell, notesDate);
            }

            cell = wordTable.Cell(10, 1);
            table = playShadow.GetSpecialSettings();
            if (!table.IsEmpty()) {
                string settings = table.GetData(0, (int)TableColumns.SpecialSettings.Text);
                string settingsDate = table.GetData(0, (int)TableColumns.SpecialSettings.TextDateStamp);
                cell.Range.InsertAfter(settings);
                SetCellBackgroundColorIfNecessary(cell, settingsDate);
            }

            table = playShadow.GetTransitions();
            FillTransitionTable(wordTable, 8, table);
            table = playShadow.GetPrompts();
            FillPromptTable(wordTable, 6, table);

            SetCellBackgroundColorIfNecessary(wordTable.Cell(1, 1), playShadow.GetLastChangeDate());

            content.Move(WdUnits.wdStory);
            content.set_Style("Normal");
            content.TypeParagraph();
            content.Move(WdUnits.wdStory);            
        }

        private static void FillTransitionTable(Microsoft.Office.Interop.Word.Table wordTable, int wordTableBeginRow, Table table) {
            InsertWordTableRows(wordTable, wordTableBeginRow, table.GetNumRows() - 1);

            for (int row = 0; row < table.GetNumRows(); row++) {
                string condition = table.GetData(row, (int)TableColumns.Transitions.Condition);
                string action = table.GetData(row, (int)TableColumns.Transitions.Action);
                string goingTo = table.GetData(row, (int)TableColumns.Transitions.Goto);
                string conditionDate = table.GetData(row, (int)TableColumns.Transitions.ConditionDateStamp);
                string actionDate = table.GetData(row, (int)TableColumns.Transitions.ActionDateStamp);
                string goingToDate = table.GetData(row, (int)TableColumns.Transitions.GotoDateStamp);

                Cell cell = wordTable.Cell(wordTableBeginRow + row, 1);
                cell.Range.InsertAfter(Common.StripBracketLabels(condition));
                SetCellBackgroundColorIfNecessary(cell, conditionDate);

                cell = wordTable.Cell(wordTableBeginRow + row, 2);
                BuildConditionActionGotoCell(cell, "", "", action, actionDate, goingTo, goingToDate);
            }
        }

        private static void FillPromptTable(Microsoft.Office.Interop.Word.Table wordTable, int wordTableBeginRow, Table table) {
            // prompts with conditions get split over 2 rows - need to add extra for them
            int conditionsWithPrompts = CountConditionsWithPrompts(table, (int)TableColumns.Prompts.Condition, (int)TableColumns.Prompts.Wording);
            // There's already one row in the template, so always subtract 1
            InsertWordTableRows(wordTable, wordTableBeginRow, table.GetNumRows() + conditionsWithPrompts - 1);

            int level = 0;
            int wordingLevel = 0;
            int currentWordTableRow = wordTableBeginRow;
            for (int row = 0; row < table.GetNumRows(); row++) {
                string condition = table.GetData(row, (int)TableColumns.Prompts.Condition);
                string wording = Common.StripBracketLabels(table.GetData(row, (int)TableColumns.Prompts.Wording));
                string id = table.GetData(row, (int)TableColumns.Prompts.Id);
                string conditionDate = table.GetData(row, (int)TableColumns.Prompts.ConditionDateStamp);
                string wordingDate = table.GetData(row, (int)TableColumns.Prompts.WordingDateStamp);
                string idDate = table.GetData(row, (int)TableColumns.Prompts.IdDateStamp);

                Cell cell = wordTable.Cell(currentWordTableRow, 1);

                if (condition.Length > 0) {
                    level = Common.GetConditionLevel(condition);
                    wordingLevel = level + 1;
                }
                condition = condition.Replace(Strings.IndentCharacterString, "").Trim();
                if (condition.Length > 0) {
                    cell.Range.ParagraphFormat.IndentCharWidth((short)level);
                    cell.Range.InsertAfter(condition);
                    cell.Range.Font.Italic = 1;
                    SetCellBackgroundColorIfNecessary(cell, conditionDate);
                }

                if (wording.Length > 0) {
                    if (condition.Length > 0) {
                        // goes on a separate line after the condition row and indented
                        currentWordTableRow++;
                    }
                    cell = wordTable.Cell(currentWordTableRow, 1);
                    cell.Range.ParagraphFormat.IndentCharWidth((short)wordingLevel);
                    cell.Range.InsertAfter(wording);
                    SetCellBackgroundColorIfNecessary(cell, wordingDate);
                }

                if (id.Length > 0) {
                    cell = wordTable.Cell(currentWordTableRow, 2);
                    cell.Range.InsertAfter(id);
                    SetCellBackgroundColorIfNecessary(cell, idDate);
                }

                currentWordTableRow++;
            }
        }

        private static void AddDecisionTable(Document doc, DecisionShadow decisionShadow) {
            Selection content = doc.Application.Selection;

            doc.Tables[Templates.Decision].Range.Copy();
            content.Bookmarks.Add("bm" + Left(AlphaNumericCharsOnly(decisionShadow.GetStateId()), 38));
            content.Move(WdUnits.wdStory);
            content.Paste();

            Microsoft.Office.Interop.Word.Table wordTable = doc.Tables[doc.Tables.Count];

            wordTable.Range.ParagraphFormat.KeepWithNext = -1;
            wordTable.Cell(1, 1).Range.InsertAfter(decisionShadow.GetStateId());
            FillEnteringFrom(wordTable.Cell(3, 1), decisionShadow);

            Cell cell = wordTable.Cell(7, 1);
            Table table = decisionShadow.GetDeveloperNotes();
            if (!table.IsEmpty()) {
                string notes = table.GetData(0, (int)TableColumns.DeveloperNotes.Text);
                string notesDate = table.GetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp);
                cell.Range.InsertAfter(notes);
                SetCellBackgroundColorIfNecessary(cell, notesDate);
            }

            table = decisionShadow.GetTransitions();
            FillTransitionTable(wordTable, 5, table);

            SetCellBackgroundColorIfNecessary(wordTable.Cell(1, 1), decisionShadow.GetLastChangeDate());

            content.Move(WdUnits.wdStory);
            content.set_Style("Normal");
            content.TypeParagraph();
            content.Move(WdUnits.wdStory);            
        }

        private static void AddDataTable(Document doc, DataShadow dataShadow) {
            Selection content = doc.Application.Selection;

            doc.Tables[Templates.Data].Range.Copy();
            content.Bookmarks.Add("bm" + Left(AlphaNumericCharsOnly(dataShadow.GetStateId()), 38));
            content.Move(WdUnits.wdStory);
            content.Paste();

            Microsoft.Office.Interop.Word.Table wordTable = doc.Tables[doc.Tables.Count];

            wordTable.Range.ParagraphFormat.KeepWithNext = -1;
            wordTable.Cell(1, 1).Range.InsertAfter(dataShadow.GetStateId());
            FillEnteringFrom(wordTable.Cell(3, 1), dataShadow);

            Cell cell = wordTable.Cell(7, 1); // was 11 but I think that's wrong...
            Table table = dataShadow.GetDeveloperNotes();
            if (!table.IsEmpty()) {
                string notes = table.GetData(0, (int)TableColumns.DeveloperNotes.Text);
                string notesDate = table.GetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp);
                cell.Range.InsertAfter(notes);
                SetCellBackgroundColorIfNecessary(cell, notesDate);
            }

            table = dataShadow.GetTransitions();
            FillTransitionTable(wordTable, 5, table);

            SetCellBackgroundColorIfNecessary(wordTable.Cell(1, 1), dataShadow.GetLastChangeDate());

            content.Move(WdUnits.wdStory);
            content.set_Style("Normal");
            content.TypeParagraph();
            content.Move(WdUnits.wdStory);                        
        }

        private static void AddSubDialogTable(Document doc, SubDialogShadow subDialogShadow) {
            Selection content = doc.Application.Selection;

            doc.Tables[Templates.SubDialog].Range.Copy();
            content.Bookmarks.Add("bm" + Left(AlphaNumericCharsOnly(subDialogShadow.GetStateId()), 38));
            content.Move(WdUnits.wdStory);
            content.Paste();

            Microsoft.Office.Interop.Word.Table wordTable = doc.Tables[doc.Tables.Count];

            wordTable.Range.ParagraphFormat.KeepWithNext = -1;
            wordTable.Cell(1, 1).Range.InsertAfter(subDialogShadow.GetStateId());

            string tmp = CachedGetGotoName(subDialogShadow);

            Cell cell = wordTable.Cell(7, 1);
            Table table = subDialogShadow.GetDeveloperNotes();
            if (!table.IsEmpty()) {
                string notes = table.GetData(0, (int)TableColumns.DeveloperNotes.Text);
                string notesDate = table.GetData(0, (int)TableColumns.DeveloperNotes.TextDateStamp);
                cell.Range.InsertAfter(notes);
                SetCellBackgroundColorIfNecessary(cell, notesDate);
            }

            cell = wordTable.Cell(5, 1);
            string startState = CachedGetGotoName(subDialogShadow.GetFirstStateTarget());
            content.InsertAfter(startState);
            content.set_Style("Hyperlink");
            content.Hyperlinks.Add(Anchor: content.Range, SubAddress: "bm" + Left(AlphaNumericCharsOnly(startState), 38));
            content.Move(WdUnits.wdStory);
            content.MoveStart(WdUnits.wdParagraph, -1);
            content.Cut();
            cell.Range.Paste();

            List<SubDialogShadow.ReturnPair> pairs = subDialogShadow.GetReturnPairs();
            InsertWordTableRows(wordTable, 3, pairs.Count - 1);
            for (int i = 0; i < pairs.Count; i++) {
                cell = wordTable.Cell(i + 3, 1);
                string prev = CachedGetGotoName(pairs[i].PreviousState);
                content.InsertAfter(prev);
                content.set_Style("Hyperlink");
                content.Hyperlinks.Add(Anchor: content.Range, SubAddress: "bm" + Left(AlphaNumericCharsOnly(prev), 38));
                content.Move(WdUnits.wdStory);
                content.MoveStart(WdUnits.wdParagraph, -1);
                content.Cut();
                cell.Range.Paste();

                cell = wordTable.Cell(i + 3, 2);
                cell.Range.InsertAfter(CachedGetGotoName(pairs[i].Return));
                string next = CachedGetGotoName(pairs[i].Return);
                content.InsertAfter(next);
                content.set_Style("Hyperlink");
                content.Hyperlinks.Add(Anchor: content.Range, SubAddress: "bm" + Left(AlphaNumericCharsOnly(next), 38));
                content.Move(WdUnits.wdStory);
                content.MoveStart(WdUnits.wdParagraph, -1);
                content.Cut();
                cell.Range.Paste();
            }

            SetCellBackgroundColorIfNecessary(wordTable.Cell(1, 1), subDialogShadow.GetLastChangeDate());

            content.Move(WdUnits.wdStory);
            content.set_Style("Normal");
            content.TypeParagraph();
            content.Move(WdUnits.wdStory);                        
           
        }

        internal static void ExportHighLevelDesignDoc(AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl) {
            DocTitleShadow docTitleShadow = PathMaker.LookupDocTitleShadow();
            if (docTitleShadow == null) {
                Common.ErrorMessage("Missing Document Title shape");
                return;
            }
            changeLogShadow = PathMaker.LookupChangeLogShadow();
            if (changeLogShadow == null) {
                Common.ErrorMessage("Missing Change Log shape");
                return;
            }
            StartShadow startShadow = PathMaker.LookupStartShadow();
            if (startShadow == null) {
                Common.ErrorMessage("Missing Start shape");
                return;
            }

            string targetFilename;
            string currentFileName;

            if (saveFileDialog == null)
                saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = PathMaker.getCurrentFileDirectory(visioControl);
            saveFileDialog.Title = Common.GetResourceString(Strings.SaveHLDSpecTitleRes);
            saveFileDialog.Filter = Common.GetResourceString(Strings.SaveHLDSpecFilterRes);
            saveFileDialog.FilterIndex = 1;
            
            targetFilename = visioControl.Src;
            currentFileName = System.IO.Path.GetFileName(targetFilename);
            saveFileDialog.FileName = Common.StripExtensionFileName(currentFileName) + "_hld.docx";
  
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                targetFilename = saveFileDialog.FileName;
            else
                return;

            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            wordApp.Visible = false;

            string templateFilename = System.Windows.Forms.Application.StartupPath + @"\" + Common.GetResourceString(Strings.HLDTemplateFileNameRes);
            Document doc = wordApp.Documents.Add(templateFilename);

            doc.BuiltInDocumentProperties["Author"] = "Convergys PathMaker";

            bool saved = visioControl.Document.Saved;

            // output visio
            Selection content = wordApp.Selection;
            content.GoTo(What: WdGoToItem.wdGoToBookmark, Name: "CallFlowDiagram");
            content.ClearFormatting();
            content.set_Style("Normal");
            content.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

            foreach (Microsoft.Office.Interop.Visio.Page page in visioControl.Document.Pages) {
                if (!page.Name.StartsWith("Background-") &&
                    !page.Name.Equals("Title") &&
                    !page.Name.Equals("Revision History")) {
                    string tmpFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".jpg";
                    page.Export(tmpFileName);
                    content.InlineShapes.AddPicture(tmpFileName);
                    content.InsertBreak(WdBreakType.wdPageBreak);
                }
            }

            visioControl.Document.Saved = saved;

            AddTitleAndLogo(doc, docTitleShadow);
            AddChangeLog(doc, changeLogShadow);

            doc.Fields.Update();

            doc.SaveAs(targetFilename);
            ((_Application)wordApp).Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
            wordApp = null;            
        }

        internal static void ImportUISpec(AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControlIn)
        {
            ParamCache paramCache = new ParamCache();
            paramCache.visioControl = visioControlIn;

            ProgressBarForm progressBarForm = new ProgressBarForm("Import User Interface Spec Changes", ImportUserInterfaceSpecWorker, paramCache);
            progressBarForm.ShowDialog();
        }

        private static bool ImportUserInterfaceSpecWorker(Object arg, ProgressBarForm progressBarForm)
        {
            ParamCache paramCache = arg as ParamCache;
            int total = 0;
            int progress = 0;
            int WordingCol = 0;
            int PromptCol = 0;
            Rows x;
            List<string> notProcessedList = new List<string>();
            string notFoundText = string.Empty;
            string STATE = "State: ";
            string shapeName = string.Empty;
            string fieldSeparator = "[\r\a\t]";
            object saveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            object originalFormat = Type.Missing;
            object routeDocument = Type.Missing;

            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            try
            {
                // Set up the open file dialog and let the user select the file to open.
                if (openFileDialog == null)
                {
                    openFileDialog = new OpenFileDialog();
                    openFileDialog.Title = Common.GetResourceString(Strings.OpenUISpecTitleRes);
                    openFileDialog.Filter = Common.GetResourceString(Strings.OpenUISpecFilterRes);
                    openFileDialog.FilterIndex = 1;
                }

                openFileDialog.InitialDirectory = PathMaker.getCurrentFileDirectory(paramCache.visioControl);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // The user selected a valid file name and hit OK. Get the
                    // file name from the dialog and open the file.
                    paramCache.currentFileName = openFileDialog.FileName;

                    if (wordApp == null)
                    {
                        Common.ErrorMessage("Couldn't start Word - make sure it's installed");
                        return false;
                    }

                    Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(paramCache.currentFileName, ReadOnly: true, Visible: false);
                    doc.Activate();

                    // just show we're moving a little earlier
                    progressBarForm.SetProgressPercentage(1, 100);

                    total = doc.Revisions.Count;

                    //turn off show changes, otherwise it be included in revised text
                    doc.ActiveWindow.View.ShowInsertionsAndDeletions = false;
                    notProcessedList.Clear();
                    PromptChangesList recordingList = new PromptChangesList();

                    foreach (Revision arev in doc.Revisions)
                    {

                        progress++;
                        try
                        {
                            if (arev.Range.Information[WdInformation.wdWithInTable])
                            {

                                x = arev.Range.Rows;
                                notFoundText = Regex.Replace((STATE + arev.Range.Tables[1].Cell(1, 1).Range.Text + "\n" + x.First.Range.Text), fieldSeparator, " ").Trim() + "\r\n\n";

                                //TODO Make sure import and export consistant in the cols.
                                shapeName = arev.Range.Tables[1].Cell(1, 2).Range.Text.Replace("\r\a", string.Empty).Trim(); ;
                                if (shapeName.Equals(Strings.Interaction))
                                {
                                    WordingCol = 1;
                                    PromptCol = 2;
                                }
                                else if (shapeName.Equals(Strings.PlayPrompt))
                                {
                                    WordingCol = 0;
                                    PromptCol = 1;
                                }
                                else
                                {
                                    WordingCol = 0;
                                    PromptCol = 0;
                                    notProcessedList.Add(notFoundText);
                                    continue;
                                }

                                string[] lines = Regex.Split(x.First.Range.Text, "\r\a");

                                if (lines[WordingCol] != "" && lines[PromptCol] != "")
                                {
                                    recordingList.AddPromptRecording(lines[PromptCol], lines[WordingCol]);
                                }
                                else
                                    if (!notProcessedList.Contains(notFoundText))
                                        notProcessedList.Add(notFoundText);
                            }
                            else
                            {
                                if (!notProcessedList.Contains(arev.Range.Sentences.First.Text.Trim() + "\r\n\n"))
                                    notProcessedList.Add(arev.Range.Sentences.First.Text.Trim() + "\r\n\n");
                            }

                            progressBarForm.SetProgressPercentage(progress, total);
                            if (progressBarForm.Cancelled)
                            {
                                ((_Application)wordApp).Quit(false);
                                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                                wordApp = null;
                                return false;
                            }
                        }
                        catch
                        {
                            if (!notProcessedList.Contains(notFoundText))
                                notProcessedList.Add(notFoundText);
                        }
                    }

                    Common.ApplyPromptRecordingList(recordingList);

                    //Process not process records to display
                    string unprocessed = string.Empty;
                    for (int y = 0; y < notProcessedList.Count; y++)
                    {
                        unprocessed += notProcessedList[y].ToString();
                    }

                    Dictionary<string, string> unprocessChangeList = recordingList.getUnusedPromptChanges();
                    if (unprocessChangeList.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> pair in unprocessChangeList)
                        {
                            unprocessed += pair.Value + " " + pair.Key + "\r\n\n";
                        }
                    }

                    if (total == 0)
                        unprocessed += "No changes found \r\n\n";

                    if (unprocessed.Length > 0)
                    {
                        ValidateResultsForm frm = new ValidateResultsForm(unprocessed, paramCache.visioControl, Strings.UISPECTIMPORTRESULTS);
                        frm.Text = "Changes Requiring Manual Updates";
                        frm.Name = Strings.UISPECTIMPORTRESULTS;
                        frm.Show();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (wordApp != null)
                {
                    // Quit without saving changes.
                    try {
                        wordApp.Documents.Close(ref saveChanges, ref originalFormat, ref routeDocument);
                    }catch {
                        //do nothing not open
                    }

                    ((_Application)wordApp).Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                    wordApp = null;
                }
            }

            progressBarForm.SetProgressPercentage(total, total);
            System.Windows.Forms.Application.DoEvents();
            return true;
        }
    }
}
