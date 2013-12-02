using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    class TableColumns {
        public enum NameValuePairs {
            Name=0,
            Value=1,
            NameDateStamp=2,
            ValueDateStamp=3,
        }

        public enum PromptTypes {
            Type=0,
            Condition=1,
            Wording=2,
            Id=3,
            TypeDateStamp=4,
            ConditionDateStamp=5,
            WordingDateStamp=6,
            IdDateStamp=7,
        }

        public enum CommandTransitions {
            Option=0,
            Vocab=1,
            DTMF=2,
            Condition=3,
            Action=4,
            Confirm=5,
            OptionDateStamp=6,
            VocabDateStamp=7,
            DTMFDateStamp=8,
            ConditionDateStamp=9,
            ActionDateStamp=10,
            GotoDateStamp=11,
            ConfirmDateStamp=12,
            Goto=13,
        }

        public enum ConfirmationPrompts {
            Option=0,
            Condition=1,
            Wording=2,
            Id=3,
            OptionDateStamp=4,
            ConditionDateStamp=5,
            WordingDateStamp=6,
            IdDateStamp=7,
        }

        public enum MaxHandling {
            Condition=0,
            Count=1,
            Action=2,
            Goto=3,
            CountDateStamp=4,
            ActionDateStamp=5,
            GotoDateStamp=6,
        }

        public enum SpecialSettings {
            Text=0,
            TextDateStamp=1,
        }

        public enum DeveloperNotes {
            Text=0,
            TextDateStamp=1,
        }

        public enum Transitions {
            Condition=0,
            Action=1,
            ConditionDateStamp=2,
            ActionDateStamp=3,
            GotoDateStamp=4,
            Goto = 5,
        }

        public enum Prompts {
            Condition=0,
            Wording=1,
            Id=2,
            ConditionDateStamp=3,
            WordingDateStamp=4,
            IdDateStamp=5,
        }

        public enum ChangeLog {
            Date=0,
            Version=1,
            Details=2,
            Author=3,
            Highlight=4,
        }
    }
}
