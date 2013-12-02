using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.ComponentModel;

namespace PathMaker {
    partial class XmlActions {
        // by keeping it static like this, we can reuse one - which means it will stay
        // in the old directory each time you do it
        private static SaveFileDialog saveFileDialog = null;
        private static ChangeLogShadow changeLogShadow = null;
        private static string targetFilename;
        private static string currentFileName;

        internal static string ExportFastPathXML(AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl) {
            return ExportFastPathXML(visioControl, false);
        }

        internal static string ExportFastPathXML(AxMicrosoft.Office.Interop.VisOcx.AxDrawingControl visioControl, bool useTmpFile) {
            DocTitleShadow docTitleShadow = PathMaker.LookupDocTitleShadow();

            if (docTitleShadow == null) {
                Common.ErrorMessage("Missing Document Title shape");
                return null;
            }
            StartShadow startShadow = PathMaker.LookupStartShadow();
            if (startShadow == null) {
                Common.ErrorMessage("Missing Start shape");
                return null;
            }

            changeLogShadow = PathMaker.LookupChangeLogShadow();
            if (changeLogShadow == null) {
                Common.ErrorMessage("Missing Change Log shape");
                return null;
            }

            if (saveFileDialog == null) {
                saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = Common.GetResourceString(Strings.SaveFastPathXMLTitleRes);
                saveFileDialog.Filter = Common.GetResourceString(Strings.SaveFastPathXMLFilterRes);
                saveFileDialog.FilterIndex = 1;
            }

            saveFileDialog.InitialDirectory = PathMaker.getCurrentFileDirectory(visioControl);
            saveFileDialog.RestoreDirectory = true;

            targetFilename = visioControl.Src;
            currentFileName = System.IO.Path.GetFileName(targetFilename);
            saveFileDialog.FileName = Common.StripExtensionFileName(currentFileName) + ".xml";

            if (!useTmpFile) {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    targetFilename = saveFileDialog.FileName;
                else
                    return null;
            }
            else
                targetFilename = saveFileDialog.FileName + ".tmp";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.LoadXml(xmlStrings.Header);
            xmlDoc.DocumentElement.SetAttribute(xmlStrings.Project, docTitleShadow.GetProjectName());
            xmlDoc.DocumentElement.SetAttribute(xmlStrings.Client, docTitleShadow.GetClientName());
            xmlDoc.DocumentElement.SetAttribute(xmlStrings.LastModified, changeLogShadow.GetLastLogChangeDate());
            xmlDoc.DocumentElement.SetAttribute(xmlStrings.Version, changeLogShadow.GetLastChangeVersion());

            AddStartElement(xmlDoc, startShadow);

            List<Shadow> shadowList = PathMaker.LookupAllShadows();
            // sorting them here helps the Missed statements in PathRunner come out in order
            string stateSortOrder = startShadow.GetDefaultSetting(Strings.DefaultSettingsStateSortOrder);
            if (stateSortOrder.Equals(Strings.StateSortOrderAlphaNumerical))
                shadowList.Sort(Common.StateIdShadowSorterAlphaNumerical);
            else if (stateSortOrder.Equals(Strings.StateSortOrderNumericalOnly))
                shadowList.Sort(Common.StateIdShadowSorterNumericalAlpha);
            else
                Common.StateIdShadowSorterVisioHeuristic(shadowList, visioControl.Document, startShadow);

            foreach (Shadow shadow in shadowList) {
                switch (shadow.GetShapeType()) {
                    case ShapeTypes.Interaction:
                        AddInteractionElement(xmlDoc, shadow as InteractionShadow);
                        break;
                    case ShapeTypes.Play:
                        AddPlayElement(xmlDoc, shadow as PlayShadow);
                        break;
                    case ShapeTypes.Decision:
                        AddDecisionElement(xmlDoc, shadow as DecisionShadow);
                        break;
                    case ShapeTypes.Data:
                        AddDataElement(xmlDoc, shadow as DataShadow);
                        break;
                    case ShapeTypes.SubDialog:
                        AddSubDialogElement(xmlDoc, shadow as SubDialogShadow);
                        break;
                    default:
                        break;
                }
            }

            xmlDoc.Save(targetFilename);
            return targetFilename;
        }

        private static void AddSubDialogElement(XmlDocument xmlDoc, SubDialogShadow shadow) {
            XmlElement stateElement = CreateElement(xmlDoc.DocumentElement, xmlStrings.SubDialog);
            stateElement.SetAttribute(xmlStrings.StateId, shadow.GetStateId());

            XmlElement startingStateElement = CreateElement(stateElement, xmlStrings.StartingState);

            Shadow targetShadow = shadow.GetFirstStateTarget();
            if (targetShadow == null)
                startingStateElement.InnerText = Strings.DisconnectedConnectorTarget;
            else
                startingStateElement.InnerText = targetShadow.GetGotoName();

            AddReturnList(shadow, stateElement);
            AddDeveloperNotes(shadow.GetDeveloperNotes(), stateElement);
        }

        private static void AddReturnList(SubDialogShadow shadow, XmlElement stateElement) {
            List<SubDialogShadow.ReturnPair> list = shadow.GetReturnPairs();

            if (list.Count == 0)
                return;

            XmlElement returnListElement = CreateElement(stateElement, xmlStrings.ReturnList);

            foreach (SubDialogShadow.ReturnPair pair in list) {
                XmlElement returnElement = CreateElement(returnListElement, xmlStrings.Return);
                returnElement.SetAttribute(xmlStrings.PreviousState, pair.PreviousState.GetGotoName());
                returnElement.SetAttribute(xmlStrings.ReturnState, GetXmlGotoFromData(pair.Return.GetGotoName()));
            }
        }

        private static void AddDataElement(XmlDocument xmlDoc, DataShadow shadow) {
            XmlElement stateElement = CreateElement(xmlDoc.DocumentElement, xmlStrings.Data);
            stateElement.SetAttribute(xmlStrings.StateId, shadow.GetStateId());
            AddTransitions(shadow, stateElement);
            AddDeveloperNotes(shadow.GetDeveloperNotes(), stateElement);
        }

        private static void AddDecisionElement(XmlDocument xmlDoc, DecisionShadow shadow) {
            XmlElement stateElement = CreateElement(xmlDoc.DocumentElement, xmlStrings.Decision);
            stateElement.SetAttribute(xmlStrings.StateId, shadow.GetStateId());
            AddTransitions(shadow, stateElement);
            AddDeveloperNotes(shadow.GetDeveloperNotes(), stateElement);
        }

        private static void AddPlayElement(XmlDocument xmlDoc, PlayShadow shadow) {
            XmlElement stateElement = CreateElement(xmlDoc.DocumentElement, xmlStrings.Play);
            stateElement.SetAttribute(xmlStrings.StateId, shadow.GetStateId());
            AddEnteringFrom(shadow, stateElement);
            AddPrompts(shadow, stateElement);
            AddTransitions(shadow, stateElement);
            AddSpecialSettings(shadow.GetSpecialSettings(), stateElement);
            AddDeveloperNotes(shadow.GetDeveloperNotes(), stateElement);
        }

        private static void AddTransitions(StateWithTransitionShadow shadow, XmlElement parentElement) {
            Table table = shadow.GetTransitions();
            if (table.IsEmpty())
                return;

            XmlElement transitionListElement = CreateElement(parentElement, xmlStrings.TransitionList);

            for (int r = 0; r < table.GetNumRows(); r++) {
                XmlElement transitionElement = CreateElement(transitionListElement, xmlStrings.Transition);
                XmlElement conditionElement = CreateElement(transitionElement, xmlStrings.Condition);
                XmlElement actionElement = CreateElement(transitionElement, xmlStrings.Action);
                XmlElement gotoElement = CreateElement(transitionElement, xmlStrings.Goto);

                conditionElement.InnerText = table.GetData(r, (int)TableColumns.Transitions.Condition);
                conditionElement.SetAttribute(xmlStrings.Level, "0");
                actionElement.InnerText = table.GetData(r, (int)TableColumns.Transitions.Action);
                string gotoString = table.GetData(r, (int)TableColumns.Transitions.Goto);
                gotoString = GetXmlGotoFromData(gotoString);
                gotoElement.InnerText = gotoString;
            }
        }

        private static void AddPrompts(PlayShadow shadow, XmlElement parentElement) {
            Table table = shadow.GetPrompts();
            if (table.IsEmpty())
                return;

            XmlElement promptListElement = CreateElement(parentElement, xmlStrings.PromptList);

            for (int r = 0; r < table.GetNumRows(); r++) {
                string condition = table.GetData(r, (int)TableColumns.Prompts.Condition);

                XmlElement promptElement = CreateElement(promptListElement, xmlStrings.Prompt);
                if (condition.Length > 0) {
                    XmlElement conditionElement = CreateElement(promptElement, xmlStrings.Condition);
                    int level = Common.GetConditionLevel(condition);
                    condition = condition.Replace(Strings.IndentCharacterString, "");
                    CreateCDataSection(conditionElement, condition);
                    conditionElement.SetAttribute(xmlStrings.Level, level.ToString());
                }
                XmlElement wordingElement = CreateElement(promptElement, xmlStrings.Wording);
                XmlElement promptIdElement = CreateElement(promptElement, xmlStrings.PromptId);

                string wording = Common.StripBracketLabels(table.GetData(r, (int)TableColumns.Prompts.Wording));
                CreateCDataSection(wordingElement, wording);

                string promptId = table.GetData(r, (int)TableColumns.Prompts.Id);
                promptIdElement.InnerText = promptId;
            }

        }

        private static void AddInteractionElement(XmlDocument xmlDoc, InteractionShadow shadow) {
            XmlElement stateElement = CreateElement(xmlDoc.DocumentElement, xmlStrings.Interaction);
            stateElement.SetAttribute(xmlStrings.StateId, shadow.GetStateId());
            AddEnteringFrom(shadow, stateElement);
            AddPromptTypes(shadow.GetPromptTypes(), stateElement);
            AddCommandTransitions(shadow, shadow.GetCommandTransitions(), stateElement);
            AddConfirmationList(shadow.GetConfirmationPrompts(), stateElement);
            AddMaxHandling(shadow, shadow.GetMaxHandling(), stateElement);
            AddSpecialSettings(shadow.GetSpecialSettings(), stateElement);
            AddDeveloperNotes(shadow.GetDeveloperNotes(), stateElement);
        }

        private static void AddStartElement(XmlDocument xmlDoc, StartShadow shadow) {
            XmlElement startElement = CreateElement(xmlDoc.DocumentElement, xmlStrings.Start);
            AddNameValuePairs(shadow.GetDefaultSettings(), startElement, xmlStrings.DefaultSettings);
            XmlElement globalBehaviorElement = CreateElement(startElement, xmlStrings.GlobalBehavior);
            AddPromptTypes(shadow.GetPromptTypes(), globalBehaviorElement);
            AddCommandTransitions(shadow, shadow.GetCommandTransitions(), globalBehaviorElement);
            AddConfirmationList(shadow.GetConfirmationPrompts(), globalBehaviorElement);
            AddNameValuePairs(shadow.GetInitialization(), startElement, xmlStrings.Initialization);
            XmlElement firstStateElement = CreateElement(startElement, xmlStrings.FirstState);
            firstStateElement.InnerText = GetXmlGotoFromShadow(shadow.GetFirstStateGotoTarget());
            AddMaxHandling(shadow, shadow.GetMaxHandling(), startElement);
        }

        private static void AddNameValuePairs(Table table, XmlElement parentElement, string elementName) {
            if (table == null)
                return;

            XmlElement defaultSettingsElement = CreateElement(parentElement, elementName);

            for (int r = 0; r < table.GetNumRows(); r++) {
                XmlElement nvPairElement = CreateElement(defaultSettingsElement, xmlStrings.NameValuePair);
                nvPairElement.SetAttribute(xmlStrings.Name, table.GetData(r, (int)TableColumns.NameValuePairs.Name));
                nvPairElement.SetAttribute(xmlStrings.Value, table.GetData(r, (int)TableColumns.NameValuePairs.Value));
            }
        }

        private static void AddEnteringFrom(StateShadow shadow, XmlElement stateElement) {
            List<string> inputList = shadow.GetEnteringFromTargetNames();
            if (inputList.Count == 0)
                return;

            XmlElement listElement = CreateElement(stateElement, xmlStrings.PreviousStateList);

            foreach (string s in inputList) {
                XmlElement element = CreateElement(listElement, xmlStrings.StateId);
                element.InnerText = s;
            }
        }

        private static void AddDeveloperNotes(Table table, XmlElement stateElement) {
            if (table.IsEmpty())
                return;

            XmlElement developerNotesElement = CreateElement(stateElement, xmlStrings.DeveloperNotes);
            developerNotesElement.InnerText = table.GetData(0, (int)TableColumns.DeveloperNotes.Text);
        }

        private static void AddSpecialSettings(Table table, XmlElement stateElement) {
            if (table.IsEmpty())
                return;

            XmlElement specialSettingsElement = CreateElement(stateElement, xmlStrings.SpecialSettings);
            specialSettingsElement.InnerText = table.GetData(0, (int)TableColumns.SpecialSettings.Text);
        }

        // parent could be an Interaction or a Start element
        private static void AddConfirmationList(Table table, XmlElement parentElement) {
            if (table.IsEmpty())
                return;

            XmlElement confirmationListElement = CreateElement(parentElement, xmlStrings.ConfirmationList);

            for (int r = 0; r < table.GetNumRows(); r++) {
                XmlElement confirmationElement = CreateElement(confirmationListElement, xmlStrings.Confirmation);
                XmlElement optionElement = CreateElement(confirmationElement, xmlStrings.Option);
                XmlElement promptListElement = CreateElement(confirmationElement, xmlStrings.PromptList);
                XmlElement promptElement = CreateElement(promptListElement, xmlStrings.Prompt);
                XmlElement conditionElement = CreateElement(promptElement, xmlStrings.Condition);
                XmlElement wordingElement = CreateElement(promptElement, xmlStrings.Wording);
                XmlElement promptIdElement = CreateElement(promptElement, xmlStrings.PromptId);

                optionElement.InnerText = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Option);

                string condition = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Condition);
                int level = Common.GetConditionLevel(condition);
                condition = condition.Replace(Strings.IndentCharacterString, "");
                CreateCDataSection(conditionElement, condition);
                conditionElement.SetAttribute(xmlStrings.Level, level.ToString());

                string wording = Common.StripBracketLabels(table.GetData(r, (int)TableColumns.ConfirmationPrompts.Wording));
                CreateCDataSection(wordingElement, wording);

                promptIdElement.InnerText = table.GetData(r, (int)TableColumns.ConfirmationPrompts.Id);
            }
        }

        // parent could be an Interaction or a Start element
        private static void AddMaxHandling(Shadow shadow, Table table, XmlElement parentElement) {
            if (table.IsEmpty())
                return;

            XmlElement maxHandlingElement = CreateElement(parentElement, xmlStrings.MaxHandling);

            for (int r = 0; r < 4; r++) {
                XmlElement rowElement = CreateElement(maxHandlingElement, xmlStrings.MaxHandlingRows[r]);
                XmlElement countElement = CreateElement(rowElement, xmlStrings.Count);
                XmlElement actionElement = CreateElement(rowElement, xmlStrings.Action);
                XmlElement gotoElement = CreateElement(rowElement, xmlStrings.Goto);

                countElement.InnerText = table.GetData(r, (int)TableColumns.MaxHandling.Count);
                actionElement.InnerText = table.GetData(r, (int)TableColumns.MaxHandling.Action);
                string gotoString = table.GetData(r, (int)TableColumns.MaxHandling.Goto);
                gotoString = GetXmlGotoFromData(gotoString);
                gotoElement.InnerText = gotoString;
            }
        }

        // parent could be an Interaction or a Start element
        private static void AddCommandTransitions(Shadow shadow, Table table, XmlElement parentElement) {
            if (table.IsEmpty())
                return;

            XmlElement commandListElement = CreateElement(parentElement, xmlStrings.CommandList);

            for (int r = 0; r < table.GetNumRows(); r++) {
                XmlElement commandElement = CreateElement(commandListElement, xmlStrings.Command);
                XmlElement optionElement = CreateElement(commandElement, xmlStrings.Option);
                XmlElement vocabularyElement = CreateElement(commandElement, xmlStrings.Vocabulary);
                XmlElement dtmfElement = CreateElement(commandElement, xmlStrings.DTMF);
                XmlElement conditionElement = CreateElement(commandElement, xmlStrings.Condition);
                XmlElement actionElement = CreateElement(commandElement, xmlStrings.Action);
                XmlElement gotoElement = CreateElement(commandElement, xmlStrings.Goto);

                optionElement.InnerText = table.GetData(r, (int)TableColumns.CommandTransitions.Option);

                string confirm = table.GetData(r, (int)TableColumns.CommandTransitions.Confirm);
                if (confirm.Equals(Strings.ConfirmAlways))
                    commandElement.SetAttribute(xmlStrings.Confirm, xmlStrings.ConfirmAlways);
                else if (confirm.Equals(Strings.ConfirmIfNecessary))
                    commandElement.SetAttribute(xmlStrings.Confirm, xmlStrings.ConfirmIfNecessary);
                else
                    commandElement.SetAttribute(xmlStrings.Confirm, xmlStrings.ConfirmNever);

                dtmfElement.InnerText = table.GetData(r, (int)TableColumns.CommandTransitions.DTMF);
                vocabularyElement.InnerText = table.GetData(r, (int)TableColumns.CommandTransitions.Vocab);
                conditionElement.InnerText = table.GetData(r, (int)TableColumns.CommandTransitions.Condition);
                conditionElement.SetAttribute(xmlStrings.Level, "0");
                actionElement.InnerText = table.GetData(r, (int)TableColumns.CommandTransitions.Action);
                string gotoString = table.GetData(r, (int)TableColumns.CommandTransitions.Goto);
                gotoString = GetXmlGotoFromData(gotoString);
                gotoElement.InnerText = gotoString;
            }
        }

        // parent could be an Interaction or a Start element
        private static void AddPromptTypes(Table table, XmlElement parentElement) {
            if (table.IsEmpty())
                return;

            XmlElement promptTypeListElement = CreateElement(parentElement, xmlStrings.PromptTypeList);

            for (int r = 0; r < table.GetNumRows(); r++) {
                XmlElement promptTypeElement = CreateElement(promptTypeListElement, xmlStrings.PromptType);
                XmlElement typeElement = CreateElement(promptTypeElement, xmlStrings.Type);
                XmlElement promptListElement = CreateElement(promptTypeElement, xmlStrings.PromptList);
                XmlElement promptElement = CreateElement(promptListElement, xmlStrings.Prompt);
                XmlElement conditionElement = CreateElement(promptElement, xmlStrings.Condition);
                XmlElement wordingElement = CreateElement(promptElement, xmlStrings.Wording);
                XmlElement promptIdElement = CreateElement(promptElement, xmlStrings.PromptId);

                string type = table.GetData(r, (int)TableColumns.PromptTypes.Type);
                typeElement.InnerText = type;

                string condition = table.GetData(r, (int)TableColumns.PromptTypes.Condition);
                int level = Common.GetConditionLevel(condition);
                condition = condition.Replace(Strings.IndentCharacterString, "");
                CreateCDataSection(conditionElement, condition);
                conditionElement.SetAttribute(xmlStrings.Level, level.ToString());

                string wording = Common.StripBracketLabels(table.GetData(r, (int)TableColumns.PromptTypes.Wording));
                CreateCDataSection(wordingElement, wording);

                string promptId = table.GetData(r, (int)TableColumns.PromptTypes.Id);
                promptIdElement.InnerText = promptId;
            }

        }

        private static XmlCDataSection CreateCDataSection(XmlElement parent, string data) {
            XmlCDataSection section = parent.OwnerDocument.CreateCDataSection(data);
            parent.AppendChild(section);
            return section;
        }

        private static XmlElement CreateElement(XmlElement parent, string name) {
            XmlElement element = parent.OwnerDocument.CreateElement(name);
            parent.AppendChild(element);
            return element;
        }

        // returns are special - we need to make sure the keyword gets in there
        private static string GetXmlGotoFromShadow(Shadow targetShadow) {
            if (targetShadow == null)
                return Strings.DisconnectedConnectorTarget;

            ReturnShadow returnShadow = targetShadow as ReturnShadow;
            if (returnShadow != null)
                return Strings.ReturnKeyword;
            else
                return targetShadow.GetGotoName();
        }

        // returns are special - we need to make sure the keyword gets in there
        private static string GetXmlGotoFromData(string data) {
            if (data == null || data.Length == 0)
                return "";

            Shadow targetShadow = Common.GetGotoTargetFromData(data);

            if (targetShadow == null)
                return data;
            else {
                ReturnShadow returnShadow = targetShadow as ReturnShadow;
                if (returnShadow != null)
                    return Strings.ReturnKeyword;
                else
                    return targetShadow.GetGotoName();
            }
        }
    }
}
