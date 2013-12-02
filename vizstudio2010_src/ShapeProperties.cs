using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    /**
     * List of all the properties for each kind of shape
     */
    class ShapeProperties {
        // Shared Properties
        public const string ShapeType = "Prop.ShapeType";
        public const string CommandTransitions = "Prop.CommandTransitions";
        public const string PromptTypes = "Prop.PromptTypes";
        public const string ConfirmationPrompts = "Prop.ConfirmationPrompts";
        public const string LastUpdate = "Prop.LastUpdate";
        public const string MaxHandling = "Prop.MaxHandling";
        public const string StateId = "Prop.StateId";
        public const string DeveloperNotes = "Prop.Notes";
        public const string SpecialSettings = "Prop.SpecialSettings";
        public const string Transitions = "Prop.Transitions";
        public const string TheText = "TheText";
        public const string EventDrop = "EventDrop";
        public const string EventDblClick = "EventDblClick";

        public const string OffPageConnectorShapeID = "User.OPCShapeID";
        public const string OffPageConnectorDestinationPageID = "User.OPCDPageID";
        public const string OffPageConnectorDestinationShapeID = "User.OPCDShapeID";


        // Document Title
        public class DocTitle {
            public const string ClientName = "Prop.ClientName";
            public const string ProjectName = "Prop.ProjectName";
            public const string ShapeType = ShapeProperties.ShapeType;
            public const string LogoData = "Prop.LogoData";
        }

        // Change Log
        public class ChangeLog {
            public const string Changes = "Prop.Changes";
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // Start
        public class Start {
            public const string DefaultSettings = "Prop.DefaultSettings";
            public const string CommandTransitions = ShapeProperties.CommandTransitions;
            public const string PromptTypes = ShapeProperties.PromptTypes;
            public const string ConfirmationPrompts = ShapeProperties.ConfirmationPrompts;
            public const string Initialization = "Prop.Initialization";
            public const string LastUpdate = ShapeProperties.LastUpdate;
            public const string ShapeType = ShapeProperties.ShapeType;
            public const string MaxHandling = ShapeProperties.MaxHandling;
        }

        // Interaction
        public class Interaction {
            public const string StateId = ShapeProperties.StateId;
            public const string PromptTypes = ShapeProperties.PromptTypes;
            public const string CommandTransitions = ShapeProperties.CommandTransitions;
            public const string ConfirmationPrompts = ShapeProperties.ConfirmationPrompts;
            public const string DeveloperNotes = ShapeProperties.DeveloperNotes;
            public const string SpecialSettings = ShapeProperties.SpecialSettings;
            public const string LastUpdate = ShapeProperties.LastUpdate;
            public const string ShapeType = ShapeProperties.ShapeType;
            public const string MaxHandling = ShapeProperties.MaxHandling;
        }

        // Play
        public class Play {
            public const string StateId = ShapeProperties.StateId;
            public const string Prompts = "Prop.Prompts";
            public const string Transitions = ShapeProperties.Transitions;
            public const string DeveloperNotes = ShapeProperties.DeveloperNotes;
            public const string SpecialSettings = ShapeProperties.SpecialSettings;
            public const string LastUpdate = ShapeProperties.LastUpdate;
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // Decision
        public class Decision {
            public const string StateId = ShapeProperties.StateId;
            public const string Transitions = ShapeProperties.Transitions;
            public const string DeveloperNotes = ShapeProperties.DeveloperNotes;
            public const string LastUpdate = ShapeProperties.LastUpdate;
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // Data
        public class Data {
            public const string StateId = ShapeProperties.StateId;
            public const string Transitions = ShapeProperties.Transitions;
            public const string DeveloperNotes = ShapeProperties.DeveloperNotes;
            public const string LastUpdate = ShapeProperties.LastUpdate;
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // Transfer
        public class Transfer {
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // Hangup
        public class Hangup {
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // SubDialog
        public class SubDialog {
            public const string StateId = ShapeProperties.StateId;
            public const string DeveloperNotes = ShapeProperties.DeveloperNotes;
            public const string LastUpdate = ShapeProperties.LastUpdate;
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // Return
        public class Return {
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // CallSubDialog
        public class CallSubDialog {
            public const string SubDialogUID = "Prop.SubDialogUID";
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // Placeholder
        public class PlaceHolder {
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // OffPageRef
        public class OffPageRef {
            public const string ShapeType = ShapeProperties.ShapeType;
            public const string HyperLink = "Hyperlink.OffPageConnector";
        }

        // Connector
        public class Connector {
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // OnPageIn
        public class OnPageIn {
            public const string ShapeType = ShapeProperties.ShapeType;
        }

        // OnPageOut
        public class OnPageOut {
            public const string ShapeType = ShapeProperties.ShapeType;
        }
    }
}
