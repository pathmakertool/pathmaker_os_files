using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Visio;

namespace PathMaker {
    class StatePrefixAndNumberManager {
        // This appeared to be a simple task until we started doing cut/copy/past and then UNDO.  When we paste
        // a copy of a shape that has the same name as another one (usually the one it was copied from), we 
        // change the name of the shape in OnShapeAdd.  But, if that shape paste is undone, Visio undoes our naming
        // and then removes the shape that was pasted with the original shape name, shape text and stateid.
        // So, we need to track the state id information along with the shadow to make sure we don't remove one
        // that's in use by the original object.  The dual maps are used to double check on removal.
        private static Dictionary<string, StateShadow> prefixNumberToShadow;
        private static Dictionary<StateShadow, string> shadowToPrefixNumber;

        // State ID assignment and verification will all happen here
        // For this to work, all shape id changes MUST come through here

        // current prefix to use
        private static string currentStateIdPrefix;
        // next available number 
        private static int currentStateNumber;
        // amount to bump between numbers
        private static int nextStateBump;
        public const int PrefixLength = 2;
        public const int NumberLength = 4;

        static StatePrefixAndNumberManager() {
            Initialize();
        }

        public static void Initialize() {
            currentStateIdPrefix = "AA";
            currentStateNumber = 1000;
            nextStateBump = 50;

            prefixNumberToShadow = new Dictionary<string, StateShadow>();
            shadowToPrefixNumber = new Dictionary<StateShadow, string>();
        }

        public static void Add(StateShadow stateShadow, Shape shape) {
            // Not using GetShapeId here because we don't want a fake one - we need to know
            // if we have one in the shape data or not
            string stateId = Common.GetCellString(shape, ShapeProperties.Play.StateId);

            // if we have a stateid already - we need to update our prefix and number tracking
            if (stateId.Length > 0) {
                string prefix, number, name;
                StateShadow.DisectStateIdIntoParts(stateId, out prefix, out number, out name);

                prefixNumberToShadow.Add(prefix + number, stateShadow);
                shadowToPrefixNumber.Add(stateShadow, prefix + number);
                
                if (prefix.Length == PrefixLength)
                    currentStateIdPrefix = prefix.ToUpper();
                if (number.Length == NumberLength) {
                    int numInt;

                    if (int.TryParse(number, out numInt)) {
                        while (numInt > currentStateNumber) {
                            currentStateNumber = currentStateNumber + nextStateBump;
                            // if we are over 9999 we need to restart at 1000 but use increments of 25
                            if (currentStateNumber > 9999) {
                                currentStateNumber = 1000;
                                if (nextStateBump == 50)
                                    nextStateBump = 25;
                                else if (nextStateBump == 25)
                                    nextStateBump = 10;
                                else if (nextStateBump == 10)
                                    nextStateBump = 5;
                                else
                                    nextStateBump = 1;
                            }
                        }
                    }
                }
            }
        }

        public static void Remove(StateShadow stateShadow) {
            string prefix, number, name;
            StateShadow.DisectStateIdIntoParts(stateShadow.GetStateId(), out prefix, out number, out name);
            int backupToNumber = currentStateNumber;

            // let's make sure this isn't an undo of a paste of a duplicate
            string prefixPlusNumber;
            StateShadow shadow;
            if (prefixNumberToShadow.TryGetValue(prefix + number, out shadow)) {
                if (shadow == stateShadow) {
                    // good match - remove
                    prefixNumberToShadow.Remove(prefix + number);
                    shadowToPrefixNumber.Remove(shadow);

                    int numberInt;
                    if (int.TryParse(number, out numberInt))
                        backupToNumber = numberInt;
                }
                else {
                    if (shadowToPrefixNumber.TryGetValue(stateShadow, out prefixPlusNumber)) {
                        // so we are registered with a different prefix + number, remove that one
                        prefixNumberToShadow.Remove(prefixPlusNumber);
                        shadowToPrefixNumber.Remove(stateShadow);

                        int numberInt;
                        if (int.TryParse(number, out numberInt))
                            backupToNumber = numberInt;
                    }
                    else {
                        // should never get here...
                        Common.ErrorMessage("Removing a state which isn't in the State Id Map");
                    }
                }
                if (backupToNumber < currentStateNumber)
                    currentStateNumber = backupToNumber;
            }
        }

        // Keeps trying new numbers until we find one that works
        private static void BumpToNextPrefixAndNumber() {
            while (prefixNumberToShadow.ContainsKey(currentStateIdPrefix + currentStateNumber.ToString("0000"))) {
                currentStateNumber += nextStateBump;
                // if we are over 9999 we need to restart at 1000 but use increments of 25
                if (currentStateNumber > 9999) {
                    currentStateNumber = 1000;
                    if (nextStateBump == 50)
                        nextStateBump = 25;
                    else if (nextStateBump == 25)
                        nextStateBump = 10;
                    else if (nextStateBump == 10)
                        nextStateBump = 5;
                    else
                        nextStateBump = 1;
                    // TODO bump letters if necessary
                }
            }
        }

        internal static string GetCurrentStateIdPrefix() {
            return currentStateIdPrefix;
        }

        internal static string GetNextAvailableNumber() {
            BumpToNextPrefixAndNumber();
            return currentStateNumber.ToString("0000");
        }

        internal static bool ContainsPrefixAndNumber(string prefixPlusNumber) {
            return prefixNumberToShadow.ContainsKey(prefixPlusNumber);
        }

        // Given a stateId, tells if it's okay to use or not - used when pasting 
        // to make sure we don't end up with duplicate ids in use
        internal static bool IsStateIdOkayForUse(string stateId) {
            string prefix, number, name;
            StateShadow.DisectStateIdIntoParts(stateId, out prefix, out number, out name);
            if (prefixNumberToShadow.ContainsKey(prefix + number))
                return false;
            else
                return true;
        }
    }
}
