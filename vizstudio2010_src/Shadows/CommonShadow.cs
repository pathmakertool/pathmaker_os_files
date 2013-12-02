using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    /**
     * Shared settings used by multiple shadows
     */
    class CommonShadow {
        public static string[] MaxHandlingConditions = {
                                                           Strings.MaxRetriesLabel,
                                                           Strings.MaxTimeoutsLabel,
                                                           Strings.MaxDisconfirmsLabel,
                                                           Strings.MaxTotalErrorsLabel
                                                       };

        public static string[] MaxHandlingDefaultCounts = {
                                                             "2",
                                                             "2",
                                                             "2",
                                                             "5"
                                                         };

        public static string[] MaxHandlingDefaultGotos = {
                                                            Strings.TransferKeyword,
                                                            Strings.TransferKeyword,
                                                            Strings.TransferKeyword,
                                                            Strings.TransferKeyword
                                                        };
        public static int MaxPromptShapeText = 70;
        public static string Ellipses = "...";

        public static int RedoPromptTypeIds(ref Table table, string stateId, int startNumber, string promptIdFormat) {
            if (promptIdFormat.Equals(Strings.PromptIdFormatFull) || promptIdFormat.Equals(Strings.PromptIdFormatPartial)) {
                string statePrefix = "";
                string stateNumber = ""; 
                string stateName = "";

                if (stateId != null)
                    StateShadow.DisectStateIdIntoParts(stateId, out statePrefix, out stateNumber, out stateName);

                int added = 0;
                int[] nextNumArray = new int[26];
                for (int i = 0; i < 26; i++)
                    nextNumArray[i] = 1;
                char letter = Strings.DefaultPromptType.ToLower().Substring(0, 1)[0];

                for (int row = 0; row < table.GetNumRows(); row++) {
                    string type = table.GetData(row, (int)TableColumns.PromptTypes.Type);

                    if (type != null && type.Trim().Length > 0)
                        letter = type.Trim().ToLower().Substring(0, 1)[0];

                    if (letter - 'a' < 0 || letter - 'a' > 25)
                        letter = Strings.DefaultPromptType.ToLower().Substring(0, 1)[0];

                    string wording = table.GetData(row, (int)TableColumns.PromptTypes.Wording);
                    if (wording == null || wording.Length == 0 || wording.Trim().StartsWith(Strings.CalculatedPromptStartString) || wording.Trim().StartsWith(Strings.PromptTypeMacroStartString))
                        continue;

                    string newPromptId;
                    if (stateId != null) {
                        if (promptIdFormat.Equals(Strings.PromptIdFormatFull))
                            newPromptId = stateId + Strings.PromptIdSeparationChar + letter + Strings.PromptIdSeparationChar + nextNumArray[letter-'a'].ToString();
                        else
                            newPromptId = statePrefix + stateNumber + Strings.PromptIdSeparationChar + letter + Strings.PromptIdSeparationChar + nextNumArray[letter - 'a'].ToString();
                    }
                    else
                        newPromptId = Strings.GlobalPromptPrefix + Strings.PromptIdSeparationChar + letter + Strings.PromptIdSeparationChar + nextNumArray[letter - 'a'].ToString();

                    if (!table.GetData(row, (int)TableColumns.PromptTypes.Id).Equals(newPromptId)) {
                        table.SetData(row, (int)TableColumns.PromptTypes.Id, newPromptId);
                        table.SetData(row, (int)TableColumns.PromptTypes.IdDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    }
                    nextNumArray[letter - 'a']++;
                    added++;
                }

                return added;
            }
            else if (promptIdFormat.Equals(Strings.PromptIdFormatNumeric)) {
                int nextNum = startNumber;

                for (int row = 0; row < table.GetNumRows(); row++) {
                    string wording = table.GetData(row, (int)TableColumns.PromptTypes.Wording);
                    if (wording == null || wording.Length == 0 || wording.Trim().StartsWith(Strings.CalculatedPromptStartString) || wording.Trim().StartsWith(Strings.PromptTypeMacroStartString))
                        continue;

                    table.SetData(row, (int)TableColumns.PromptTypes.Id, nextNum.ToString());
                    table.SetData(row, (int)TableColumns.PromptTypes.IdDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    nextNum++;
                }

                return nextNum - startNumber;
            }
            else
                return 0;
        }

        public static int RedoConfirmationPromptIds(ref Table table, string stateId, int startNumber, string promptIdFormat) {
            if (promptIdFormat.Equals(Strings.PromptIdFormatFull) || promptIdFormat.Equals(Strings.PromptIdFormatPartial)) {
                string statePrefix = "";
                string stateNumber = "";
                string stateName = "";

                if (stateId != null)
                    StateShadow.DisectStateIdIntoParts(stateId, out statePrefix, out stateNumber, out stateName);
                
                int nextNum = 1;

                for (int row = 0; row < table.GetNumRows(); row++) {
                    string wording = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Wording);
                    if (wording == null || wording.Length == 0 || wording.Trim().StartsWith(Strings.CalculatedPromptStartString) || wording.Trim().StartsWith(Strings.PromptTypeMacroStartString))
                        continue;

                    string newPromptId;
                    if (stateId != null) {
                        if (promptIdFormat.Equals(Strings.PromptIdFormatFull))
                            newPromptId = stateId + Strings.PromptIdSeparationChar + Strings.DefaultConfirmationPromptLetter + Strings.PromptIdSeparationChar + nextNum.ToString();
                        else
                            newPromptId = statePrefix + stateNumber + Strings.PromptIdSeparationChar + Strings.DefaultConfirmationPromptLetter + Strings.PromptIdSeparationChar + nextNum.ToString();
                    }
                    else
                        newPromptId = Strings.GlobalPromptPrefix.ToString () + Strings.PromptIdSeparationChar + 
                            Strings.DefaultConfirmationPromptLetter.ToString() + Strings.PromptIdSeparationChar + nextNum;

                    if (!table.GetData(row, (int)TableColumns.ConfirmationPrompts.Id).Equals(newPromptId)) {
                        table.SetData(row, (int)TableColumns.ConfirmationPrompts.Id, newPromptId);
                        table.SetData(row, (int)TableColumns.ConfirmationPrompts.IdDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    }
                    nextNum++;
                }

                return nextNum - 1;
            }
            else if (promptIdFormat.Equals(Strings.PromptIdFormatNumeric)) {
                int nextNum = startNumber;

                for (int row = 0; row < table.GetNumRows(); row++) {
                    string wording = table.GetData(row, (int)TableColumns.ConfirmationPrompts.Wording);
                    if (wording == null || wording.Length == 0 || wording.Trim().StartsWith(Strings.CalculatedPromptStartString) || wording.Trim().StartsWith(Strings.PromptTypeMacroStartString))
                        continue;

                    table.SetData(row, (int)TableColumns.ConfirmationPrompts.Id, nextNum.ToString());
                    table.SetData(row, (int)TableColumns.ConfirmationPrompts.IdDateStamp, DateTime.Now.ToString(Strings.DateColumnFormatString));
                    nextNum++;
                }

                return nextNum - startNumber;
            }
            else
                return 0;
        }

        internal static string GetNewUIDAfterPaste(string oldUID, Dictionary<string, string> oldGUIDToNewGUIDMap, bool checkWholeDocument) {
            // first try to find it amongst the pasted items
            string newUID;
            if (oldGUIDToNewGUIDMap.TryGetValue(oldUID, out newUID))
                return newUID;

            // for max handlers and call subdialogs, we can use older UIDs as long as that shape exists in this document
            if (checkWholeDocument) {
                // see if it is amongst the ones in this document, if it is, it can stay the same
                Shadow s = PathMaker.LookupShadowByUID(oldUID);
                if (s != null)
                    return oldUID;
            }

            return null;
        }

        internal static string PromptToShapeLabel(string prompt) {
            if (prompt.Length > CommonShadow.MaxPromptShapeText) {
                prompt = prompt.Substring(0, MaxPromptShapeText - Ellipses.Length);

                int i = prompt.Length - 1;
                while (i > 0 && prompt[i] != ' ')
                    i--;
                prompt = prompt.Substring(0, i + 1);
                prompt = prompt + Ellipses;
            }

            return Common.StripBracketLabels(prompt);
        }

        internal static string GetStringWithNewConnectorLabel(string fullText, string label) {
            string newText = Common.StripBracketLabels(fullText);
            if (newText.Length > 0)
                newText += " ";
            newText += Strings.LabelStartBracket + label + Strings.LabelEndBracket;
            return newText;
        }
    }
}
