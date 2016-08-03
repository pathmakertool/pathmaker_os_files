using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    class PromptChangesList : PromptRecordingList {
        Dictionary<string, string> promptIdToWordingMapToTrackFoundPrompts = new Dictionary<string, string>();
        

        public override void AddPromptRecording(string id, string wording)
        {
            //getting dupes for some VUI sprec inserts - not sure why - added the if() below to prevent conflicts
            //if (base.getWording(id.Trim()) == null)
            {
                base.AddPromptRecording(id.Trim(), wording);
                promptIdToWordingMapToTrackFoundPrompts.Add(id.Trim(), wording);
            }
        }


        internal override string getWording(string id) {
            promptIdToWordingMapToTrackFoundPrompts.Remove(id.Trim());
            return base.getWording(id.Trim());
        }

        public Dictionary<string, string> getUnusedPromptChanges() {
            return promptIdToWordingMapToTrackFoundPrompts;
        }
    }
}
