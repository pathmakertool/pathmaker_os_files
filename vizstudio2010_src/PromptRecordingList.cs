using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    // used to gather and manage a list of prompts for exporting
    class PromptRecordingList {
        Dictionary<string, PromptRecording> wordingToRecordingMap = new Dictionary<string, PromptRecording>();
        Dictionary<string, string> promptIdToWordingMap = new Dictionary<string, string>();
        List<string> duplicateIdList = new List<string>();

        public class PromptRecording {
            public List<string> duplicateIds = null;

            public PromptRecording(string id, string wording) {
                PromptId = id.Trim();
                Wording = wording.Trim();
            }
            public void AddDuplicateId(string id) {
                if (duplicateIds == null)
                    duplicateIds = new List<string>();
                duplicateIds.Add(id.Trim());
            }
            public string PromptId { get; set; }
            public string Wording { get; set; }
            public List<string> GetDuplicateIds() { return duplicateIds; }
        }

        public PromptRecordingList() {

        }

        virtual public void AddPromptRecording(string id, string wording) {
            PromptRecording recording;
            if (promptIdToWordingMap.ContainsKey(id.Trim()))
            {
                if (!duplicateIdList.Contains(id.Trim()))
                    duplicateIdList.Add(id.Trim());
                return;
            }
            promptIdToWordingMap.Add(id.Trim(), wording.Trim());
            if (wordingToRecordingMap.TryGetValue(wording.Trim(), out recording))
            {
                recording.AddDuplicateId(id.Trim());
            }
            else {
                recording = new PromptRecording(id.Trim(), wording.Trim());
                wordingToRecordingMap.Add(wording.Trim(), recording);
            }
        }

        public List<string> GetDuplicatePromptIds() {
            return duplicateIdList;
        }

        public List<PromptRecording> GetPromptRecordings() {
            return wordingToRecordingMap.Values.ToList();
        }

        virtual internal string getWording(string id) {
            string wording;
            if (promptIdToWordingMap.TryGetValue(id.Trim(), out wording))
                return wording;
            return null;
        }
    }
}
