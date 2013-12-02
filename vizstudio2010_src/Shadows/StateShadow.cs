using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace PathMaker {
    /**
     * Class from which all of the state shadows are derived from 
     */
    public class StateShadow : Shadow {
        public const string AllowedPrefixChars = "\bABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string AllowedNumberChars = "\b0123456789";
        private const string AllowedPrefixRegEx = "[ABCDEFGHIJKLMNOPQRSTUVWXYZ]{2}";
        private const string AllowedNumberRegEx = "[0123456789]{4}";

        public StateShadow(Shape shape) : base(shape) {
            StatePrefixAndNumberManager.Add(this, shape);
        }

        public override void OnShapeAdd() {
            base.OnShapeAdd();
            // when pasting we need to force this to be updated
            // Not using GetStateId here because we don't want a fake one if it's empty - we need to know
            // if we have one in the shape data or not
            string stateId = Common.GetCellString(shape, ShapeProperties.StateId);
            Common.ForcedSetShapeText(shape, StateIdForDisplay(stateId));
        }

        public override void OnBeforeShapeDelete() {
            StatePrefixAndNumberManager.Remove(this);

            // check to see if any max handlers use this - if so change to hang up
            List<Shadow> shadowList = PathMaker.LookupAllShadows();

            foreach (Shadow s in shadowList) {
                if (s.RemoveGotosThatDontUseConnectors(shape.get_UniqueID((short)VisUniqueIDArgs.visGetOrMakeGUID)))
                    Common.ErrorMessage("Shape was being used as a goto in " + s.GetGotoName() + " - changing to Hang up");
            }
        }

        public override void OnShapeExitTextEdit() {
            base.OnShapeExitTextEdit();
            string prefix, number, name, errorMessage;
            string newName = StateIdForDisplay(shape.Text);

            DisectStateIdIntoParts(newName, out prefix, out number, out name);

            // always use uppercase prefixes
            prefix = prefix.ToUpper();

            // They can enter anything in the shape text - here we make sure it's a good stateid
            // and, if not, we make it one.
            if (!ValidateStateIdTextBoxStrings(prefix, number, name, out errorMessage)) {
                if (GetStateId().Length > 0) {
                    string oldPrefix, oldNumber, oldName;
                    DisectStateIdIntoParts(GetStateId(), out oldPrefix, out oldNumber, out oldName);
                    newName = oldPrefix.ToUpper() + oldNumber + Strings.StateIdWordDisplayDelimiter + newName;
                }
                else {
                    newName = StatePrefixAndNumberManager.GetCurrentStateIdPrefix() + 
                        StatePrefixAndNumberManager.GetNextAvailableNumber() + Strings.StateIdWordDisplayDelimiter + newName;
                }
            }
            else {
                // make sure we pick up the uppercase prefix
                newName = prefix + number + Strings.StateIdWordDisplayDelimiter + name;
            }

            string oldStateId = GetStateId();
            string newStateId = StateIdForStorage(newName);
            SetStateId(newStateId);
            StartShadow shadowStart = PathMaker.LookupStartShadow();
            if (shadowStart != null && !oldStateId.Equals(newStateId)) {
                string promptIdFormat = shadowStart.GetDefaultSetting(Strings.DefaultSettingsPromptIDFormat);
                if (promptIdFormat.Equals(Strings.PromptIdFormatFull) || promptIdFormat.Equals(Strings.PromptIdFormatPartial))
                    RedoPromptIds(0, promptIdFormat);
            }
        }

        virtual internal void SetStateId(string stateId) {
            // remove the old prefix+number from the list
            StatePrefixAndNumberManager.Remove(this);

            Common.SetCellString(shape, ShapeProperties.Play.StateId, stateId);
            shape.Text = StateIdForDisplay(stateId);

            // add the new prefix+number to the list
            StatePrefixAndNumberManager.Add(this, shape);
        }


        // Gets a stateId but... if the shape doesn't have one, it will return
        // an automatically generated one
        internal string GetStateId() {
            string stateId = Common.GetCellString(shape, ShapeProperties.StateId);

            if (stateId.Length == 0)
                stateId = BuildStateIdForStorage(StatePrefixAndNumberManager.GetCurrentStateIdPrefix(), 
                    StatePrefixAndNumberManager.GetNextAvailableNumber(), String.Empty);
            return stateId;
        }

        public override string GetGotoName() {
            return GetStateId();
        }

        // Validates the individual components used by dialog boxes to represent a stateId
        public bool ValidateStateIdTextBoxStrings(string prefix, string number, string name, out string errorMessage) {
            string oldPrefix, oldNumber, oldName;
            DisectStateIdIntoParts(GetStateId(), out oldPrefix, out oldNumber, out oldName);

            if (prefix.Length != StatePrefixAndNumberManager.PrefixLength) {
                errorMessage = "Enter a valid 2-letter uppercase state prefix";
                return false;
            }
            else {
                if (!Regex.Match(prefix, AllowedPrefixRegEx).Success) {
                    errorMessage = "Prefix must be 2 uppercase alpha characters";
                    return false;
                }
            }

            if (number.Length != StatePrefixAndNumberManager.NumberLength) {
                errorMessage = "Enter a valid 4 digit state number";
                return false;
            }
            else {
                if (!Regex.Match(number, AllowedNumberRegEx).Success) {
                    errorMessage = "Number must consist of 4 numberical characters";
                    return false;
                }
            }

            if (StatePrefixAndNumberManager.ContainsPrefixAndNumber(prefix + number) && (!((prefix == oldPrefix) && (number == oldNumber)))) {
                errorMessage = "State prefix and number are already assigned to another state";
                return false;
            }
            else {
                errorMessage = String.Empty;
                return true;
            }
        }

        // breaks a stateId down into it's components
        internal static void DisectStateIdIntoParts(string stateId, out string statePrefix, out string stateNumber, out string stateName) {
            if (stateId.Length >= StatePrefixAndNumberManager.PrefixLength)
                statePrefix = stateId.Substring(0, StatePrefixAndNumberManager.PrefixLength).Trim();
            else
                statePrefix = String.Empty;

            if (stateId.Length >= (StatePrefixAndNumberManager.PrefixLength + StatePrefixAndNumberManager.NumberLength))
                stateNumber = stateId.Substring(StatePrefixAndNumberManager.PrefixLength, StatePrefixAndNumberManager.NumberLength).Trim();
            else
                stateNumber = String.Empty;

            if (stateId.Length >= (StatePrefixAndNumberManager.PrefixLength + StatePrefixAndNumberManager.NumberLength + 1))
                stateName = stateId.Substring(StatePrefixAndNumberManager.PrefixLength + StatePrefixAndNumberManager.NumberLength).Trim();
            else
                stateName = String.Empty;
        }

        // composes a stateId from it's component parts
        public static string BuildStateIdForStorage(string prefix, string number, string name) {
            string tmp = prefix + number + Strings.StateIdWordStorageDelimiter + name;
            tmp = tmp.Replace(Strings.StateIdWordDisplayDelimiter, Strings.StateIdWordStorageDelimiter);
            return tmp;
        }

        // Replaces underscores with spaces for better readability
        public static string StateIdForDisplay(string stateId) {
            return stateId.Replace(Strings.StateIdWordStorageDelimiter, Strings.StateIdWordDisplayDelimiter);
        }

        // Replaces spaces with underscores for storage
        public static string StateIdForStorage(string stateId) {
            return stateId.Replace(Strings.StateIdWordDisplayDelimiter, Strings.StateIdWordStorageDelimiter);
        }

        /**
         * Returns a list of all the nicknames of the input target shapes for
         * this one.  For most shadows, this is the NickName of the shape.  For 
         * things like connectors, on and off page refs, etc. this will work
         * backwards to get to the sources.
         */
        virtual public List<string> GetEnteringFromTargetNames() {
            List<Connect> connects = GetShapeInputs();
            List<string> list = new List<string>();

            foreach (Connect connect in connects) {
                Shape input = connect.FromSheet;
                Shadow shadow = PathMaker.LookupShadowByShape(input);
                foreach (Shadow s in shadow.GetSourceTargets())
                    list.Add(s.GetGotoName());
            }
            return list;
        }
    }
}
