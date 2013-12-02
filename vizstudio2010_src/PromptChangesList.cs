using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    class PromptChangesList : PromptRecordingList {
        Dictionary<string, string> promptIdToWordingMapToTrackFoundPrompts = new Dictionary<string, string>();

        public override void AddPromptRecording(string id, string wording) {
            base.AddPromptRecording(id, wording);
            promptIdToWordingMapToTrackFoundPrompts.Add(id, wording);
        }

        internal override string getWording(string id) {
            promptIdToWordingMapToTrackFoundPrompts.Remove(id);
            return base.getWording(id);
        }

        public Dictionary<string, string> getUnusedPromptChanges() {
            return promptIdToWordingMapToTrackFoundPrompts;
        }
    }
}
