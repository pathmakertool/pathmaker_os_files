using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    partial class XmlActions {
        private class xmlStrings {
            public const string Header = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<!DOCTYPE CALLFLOW SYSTEM \"VUI.dtd\">\r\n<CALLFLOW />";
            public const string Project = "PROJECT";
            public const string Client = "CLIENT";
            public const string LastModified = "LASTMODIFIED";
            public const string Version = "VERSION";
            public const string Start = "START";
            public const string Interaction = "INTERACTION";
            public const string Play = "PLAY";
            public const string Decision = "DECISION";
            public const string Data = "DATA";
            public const string SubDialog = "SUBDIALOG";
            public const string StateId = "STATENAME";
            public const string PreviousStateList = "PREVIOUSSTATELIST";
            public const string PromptTypeList = "PROMPTTYPELIST";
            public const string PromptType = "PROMPTTYPE";
            public const string Prompt = "PROMPT";
            public const string PromptList = "PROMPTLIST";
            public const string Type = "TYPE";
            public const string Condition = "CONDITION";
            public const string Wording = "WORDING";
            public const string PromptId = "PROMPTID";
            public const string Level = "LEVEL";
            public const string CommandList = "COMMANDLIST";
            public const string Command = "COMMAND";
            public const string Action = "ACTION";
            public const string Option = "OPTION";
            public const string DTMF = "DTMF";
            public const string Vocabulary = "VOCABULARY";
            public const string Goto = "GOTO";
            public const string Confirm = "CONFIRM";
            public const string ConfirmationList = "CONFIRMATIONLIST";
            public const string Confirmation = "CONFIRMATION";
            public const string MaxHandling = "MAXHANDLING";
            public const string MaxRetries = "MAXRETRIES";
            public const string MaxTimeouts = "MAXTIMEOUTS";
            public const string MaxDisconfirms = "MAXDISCONFIRMS";
            public const string MaxTotalErrors = "MAXTOTALERRORS";
            public const string Count = "COUNT";
            public const string DeveloperNotes = "DEVELOPERNOTES";
            public const string SpecialSettings = "SPECIALSETTINGS";
            public const string GlobalBehavior = "GLOBALBEHAVIOR";
            public const string Initialization = "INPUTDATA";
            public const string NameValuePair = "NAMEVALUEPAIR";
            public const string FirstState = "FIRSTSTATE";
            public const string DefaultSettings = "DEFAULTSETTINGS";
            public const string Name = "NAME";
            public const string Value = "VALUE";
            public const string TransitionList = "TRANSITIONLIST";
            public const string Transition = "TRANSITION";
            public const string ReturnList = "RETURNLIST";
            public const string Return = "RETURN";
            public const string PreviousState = "PREVIOUSSTATE";
            public const string ReturnState = "RETURNSTATE";
            public const string StartingState = "STARTINGSTATE";
            public const string ConfirmAlways = "ALWAYS";
            public const string ConfirmNever = "NEVER";
            public const string ConfirmIfNecessary = "IFNECESSARY";

            // must match order in the dialogs
            public static string[] MaxHandlingRows = {
                MaxRetries,
                MaxTimeouts,
                MaxDisconfirms,
                MaxTotalErrors,
            };
        }
    }
}
