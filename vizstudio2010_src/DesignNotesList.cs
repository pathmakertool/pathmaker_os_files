using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PathMaker {
    // used to gather and manage a list of prompts for exporting
    class DesignNotesList {
        Dictionary<string, DesignNoteContent> stateIdToNotesMap = new Dictionary<string, DesignNoteContent>();

        public class DesignNoteContent {
            
            public DesignNoteContent(string id, string wording) {
                StateId = id;
                Wording = wording;
            }
                        
            public string StateId { get; set; }
            public string Wording { get; set; }  
        }

        public DesignNotesList() {

        }

        virtual public void AddDesignNoteContent(string id, string wording) {
            DesignNoteContent designNote;

            designNote = new DesignNoteContent(id, wording);
            stateIdToNotesMap.Add(id, designNote);
            
        }


        public List<DesignNoteContent> GetDesignNotes()
        {
            return stateIdToNotesMap.Values.ToList();
        }

        virtual internal string getWording(string id) {
            //string wording;
            DesignNoteContent designNote;
            if (stateIdToNotesMap.TryGetValue(id, out designNote))
                return designNote.Wording;
            return null;
        }
    }
}
