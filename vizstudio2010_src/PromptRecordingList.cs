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
                PromptId = id;
                Wording = wording;
            }
            public void AddDuplicateId(string id) {
                if (duplicateIds == null)
                    duplicateIds = new List<string>();
                duplicateIds.Add(id);
            }
            public string PromptId { get; set; }
            public string Wording { get; set; }
            public List<string> GetDuplicateIds() { return duplicateIds; }
        }

        public PromptRecordingList() {

        }

        virtual public void AddPromptRecording(string id, string wording) {
            PromptRecording recording;
            if (promptIdToWordingMap.ContainsKey(id)) {
                if (!duplicateIdList.Contains(id))
                    duplicateIdList.Add(id);
                return;
            }
            promptIdToWordingMap.Add(id, wording);
            if (wordingToRecordingMap.TryGetValue(wording, out recording)) {
                recording.AddDuplicateId(id);
            }
            else {
                recording = new PromptRecording(id, wording);
                wordingToRecordingMap.Add(wording, recording);
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
            if (promptIdToWordingMap.TryGetValue(id, out wording))
                return wording;
            return null;
        }
    }
}
