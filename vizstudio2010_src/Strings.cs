using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker
{
    // constants for use throughout the app
    class Strings
    {   
        public const string ResourceFileName = "PathMaker.Properties.Resources";

        public const string OpenDialogTitleRes = "OpenDialogTitle";
        public const string OpenDialogFilterRes = "OpenDialogFilter";
        public const string SaveDialogTitleRes = "SaveDialogTitle";
        public const string SaveDialogFilterRes = "SaveDialogFilter";
        public const string OpenLogoDialogTitleRes = "OpenLogoDialogTitle";
        public const string OpenLogoDialogFilterRes = "OpenLogoDialogFilter";
        public const string SaveFastPathXMLTitleRes = "SaveFastPathXMLTitle";
        public const string SaveFastPathXMLFilterRes = "SaveFastPathXMLFilter";
        public const string SavePromptRes = "SavePrompt";
        public const string SaveAsPromptRes = "SaveAsPrompt";
        public const string SavePromptsTitleRes = "SavePromptsTitle";
        public const string SavePromptsFilterRes = "SavePromptsFilter";
        public const string OpenPromptsTitleRes = "OpenPromptsTitle";
        public const string OpenPromptsFilterRes = "OpenPromptsFilter";
        public const string SaveUISpecTitleRes = "SaveUISpecTitle";
        public const string SaveUISpecFilterRes = "SaveUISpecFilter";
        public const string SaveHLDSpecTitleRes = "SaveHLDSpecTitle";
        public const string SaveHLDSpecFilterRes = "SaveHLDSpecFilter";
        public const string OpenUISpecTitleRes = "OpenUISpecTitle";
        public const string OpenUISpecFilterRes = "OpenUISpecFilter";

        public const string VisioTemplateFileSuffix = ".vst";
        public const string VisioTemplateFile = "PathMaker.vst";

        public const string PathMakerSchemaVersionRes = "PathMakerSchemaVersion";
        public const string PathMakerReleaseVersionRes = "PathMakerReleaseVersion";
        public const string DocumentSchemaVersionPrefixRes = "PathMakerSchemaVersion=";

        public const string StencilFileName = "PathMaker.vss";
        public const string VUITemplateFileNameRes = "VUITemplateFileName";
        public const string HLDTemplateFileNameRes = "HLDTemplateFileName";

        public const string DefaultSettingsMode = "Mode";
        public const string DefaultSettingsBargeIn = "Barge-in";
        public const string DefaultSettingsPromptIDFormat = "Prompt ID Format";
        public const string DefaultSettingsRetriesInTotalErrors = "Retries in Total Errors";
        public const string DefaultSettingsTimeoutsInTotalErrors = "Timeouts in Total Errors";
        public const string DefaultSettingsDisconfirmsInTotalErrors = "Disconfirms in Total Errors";
        public const string DefaultSettingsStateSortOrder = "State Sort Order";

        public const string DateStampColumnSuffix = "DateStamp";

        public const string PathMakerArgument = "/pathmaker";
        public const string DoubleClickCommand = "/cmd=1";
        public const string PropertiesCommand = "/cmd=2";

        public const string ReturnKeyword = "Return";
        public const string TransferKeyword = "Transfer";
        public const string HangUpKeyword = "Hang up";
        public const string PlaceHolderKeyword = "PlaceHolder";
        public const string HangUpKeywordShortForm = "Hang"; // better for comparisons against old stuff
        public const string CurrentStateKeyword = "CURRENT STATE";

        public const string ConfirmIfNecessary = "If necessary";
        public const string ConfirmAlways = "Always";
        public const string ConfirmNever = "Never";

        public const string PromptIdFormatPartial = "Partial (aa1000_i_00)";
        public const string PromptIdFormatFull = "Full (aa1000_state_i_00)";
        public const string PromptIdFormatNumeric = "Numeric (0)";
        public const string PromptIdFormatDisabled = "Disabled";

        public const string StateSortOrderAlphaNumerical = "Alpha-Numerical";
        public const string StateSortOrderNumericalOnly = "Numerical-Alpha";
        public const string StateSortOrderVisioHeuristic = "Visio Heuristic";

        public const string ModeSpeech = "Speech";
        public const string ModeDTMF = "DTMF";

        public const string ValueYes = "Y";
        public const string ValueNo = "N";

        public const string IndentColumnName = "->";
        public const string OutdentColumnName = "<-";
        public const string IndentButtonName = "+";
        public const string OutdentButtonName = "-";

        public const string NamelessNickName = "- Nameless -";

        public const string HighlightColorNone = "None";
        public const string HighlightColorYellow = "Yellow";
        public const string HighlightColorGreen = "Green";
        public const string HighlightColorPink = "Pink";
        public const string HighlightColorAqua = "Aqua";
        public const string HighlightColorBlue = "Blue";

        public const string BeginConnectionPointCellName = "BeginX";
        public const string EndConnectionPointCellName = "EndX";

        public const string OffPageConnectorDropCommand = "RUNADDONWARGS(\"OPC\",\"/CMD=1\")";
        public const string OffPageConnectorDblClickCommand = "RUNADDONWARGS(\"OPC\",\"/CMD=2\")";
        public const string OffPageConnectorTextCommand = "RUNADDONWARGS(\"OPC\",\"/CMD=3\")";

        public const string DisconnectedConnectorTarget = "Disconnected";

        public const string LabelStartBracket = "[";
        public const string LabelEndBracket = "]";
        public const string DynamicOptionKeyword = "<dynamic>";  // should be case insensitive

        public const string PromptTypeSeparator = "/";

        public const string IndentCharacterString = "»";

        public const string DefaultPromptType = "Initial";

        public const string MaxRetriesLabel = "Max Retries";
        public const string MaxTimeoutsLabel = "Max Timeouts";
        public const string MaxDisconfirmsLabel = "Max Disconfirms";
        public const string MaxTotalErrorsLabel = "Max Total Errors";

        public const string DateColumnFormatString = "MM/dd/yyyy";

        public const string StatePrefixTextBoxName = "statePrefixTextBox";
        public const string StateNumberTextBoxName = "stateNumberTextBox";
        public const string StateNameTextBoxName = "stateNameTextBox";

        public const string StateIdWordStorageDelimiter = "_";
        public const string StateIdWordDisplayDelimiter = " ";

        public const string CalculatedPromptStartString = "<";
        public const string CalculatedPromptEndString = ">";

        public const string PromptTypeMacroStartString = "{";
        public const string PromptTypeMacroEndString = "}";

        public const char DefaultPromptLetter = 'i';
        public const char DefaultConfirmationPromptLetter = 'c';

        public const char PromptIdSeparationChar = '_';

        public const string DynamicConnectorShapeNameStart = "Dynamic connector";

        public const string PageShapesToIngoreShapeNameStart = "Sheet";

        public const string ToBeDeletedLabel = "MUST BE DELETED";

        public const string PromptRecordingLocationRes = "PromptRecordingLocation";

        public const string StartTargetName = "Start";

        public const string GlobalPromptPrefix = "Global";

        public const string CutCopyPasteTempCellName = "Prop.CutCopyPasteTemp";
        
        public const string MoveRowUpText = "Move &Up";
        public const string MoveRowDownText = "Move &Down";
        public const string TextEditorText = "&Text Editor";
        public const string InsertRowText = "&Insert Row";
        public const string DuplicateRowText = "D&uplicate";
        public const string DeleteRowText = "D&elete Row";

        public const string DefaultFileName = "Drawing.vui";
        public const string DefaultCopyFileNameSuffix = "_copy.vui";
        public const string TitleBarSuffix = " - PathMaker";

        public const string SERVERNAME = "http://fastpathserver.intervoice.int:8080/PathRunner/";
        public const string SAVEJSPNAME = "SaveFile.jsp?";
        public const string VALIDATEJSPNAME = "ValidateSpecWorker.jsp";
        public const string FILENAME = "filename=";
        public const string UISPECRESULTSFORM = "UISpecResultsForm";
        public const string UISPECTIMPORTRESULTS = "UISPECTIMPORTRESULTS";
        public const string UISPECVALIDATE = "UI Spec Validate";
        public const string UISPECCANCEL = "UI Spec Validate Cancel";
        public const string PlayPrompt = "Play Prompt";
        public const string Interaction = "Interaction";
        public const string PlayShadow = "PlayShadow";
        public const string InteractionShadow = "InteractionShadow";

        public const string MaxHandlerGlobalReferenceNote = "See Defaults";
    }
}
