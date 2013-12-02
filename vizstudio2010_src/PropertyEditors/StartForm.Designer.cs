namespace PathMaker {
    partial class StartForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.defaultSettingsTab = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.bargeInComboBox = new System.Windows.Forms.ComboBox();
            this.retriesComboBox = new System.Windows.Forms.ComboBox();
            this.promptIdFormatComboBox = new System.Windows.Forms.ComboBox();
            this.disconfirmsComboBox = new System.Windows.Forms.ComboBox();
            this.timeoutsComboBox = new System.Windows.Forms.ComboBox();
            this.initializationTab = new System.Windows.Forms.TabPage();
            this.initializationDataGridView = new System.Windows.Forms.DataGridView();
            this.globalBehaviorTab = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.confirmationsDataGridView = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.commandsDataGridView = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.promptsDataGridView = new System.Windows.Forms.DataGridView();
            this.maxHandlingTab = new System.Windows.Forms.TabPage();
            this.maxHandlingDataGridView = new System.Windows.Forms.DataGridView();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.sortOrderComboBox = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.defaultSettingsTab.SuspendLayout();
            this.initializationTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.initializationDataGridView)).BeginInit();
            this.globalBehaviorTab.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.confirmationsDataGridView)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.commandsDataGridView)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.promptsDataGridView)).BeginInit();
            this.maxHandlingTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxHandlingDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.defaultSettingsTab);
            this.tabControl1.Controls.Add(this.initializationTab);
            this.tabControl1.Controls.Add(this.globalBehaviorTab);
            this.tabControl1.Controls.Add(this.maxHandlingTab);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(984, 655);
            this.tabControl1.TabIndex = 0;
            // 
            // defaultSettingsTab
            // 
            this.defaultSettingsTab.Controls.Add(this.sortOrderComboBox);
            this.defaultSettingsTab.Controls.Add(this.label1);
            this.defaultSettingsTab.Controls.Add(this.label7);
            this.defaultSettingsTab.Controls.Add(this.label6);
            this.defaultSettingsTab.Controls.Add(this.label5);
            this.defaultSettingsTab.Controls.Add(this.label4);
            this.defaultSettingsTab.Controls.Add(this.label3);
            this.defaultSettingsTab.Controls.Add(this.label2);
            this.defaultSettingsTab.Controls.Add(this.modeComboBox);
            this.defaultSettingsTab.Controls.Add(this.bargeInComboBox);
            this.defaultSettingsTab.Controls.Add(this.retriesComboBox);
            this.defaultSettingsTab.Controls.Add(this.promptIdFormatComboBox);
            this.defaultSettingsTab.Controls.Add(this.disconfirmsComboBox);
            this.defaultSettingsTab.Controls.Add(this.timeoutsComboBox);
            this.defaultSettingsTab.Location = new System.Drawing.Point(4, 22);
            this.defaultSettingsTab.Name = "defaultSettingsTab";
            this.defaultSettingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.defaultSettingsTab.Size = new System.Drawing.Size(976, 629);
            this.defaultSettingsTab.TabIndex = 3;
            this.defaultSettingsTab.Text = "Default Settings";
            this.defaultSettingsTab.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 157);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(129, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Disconfirms in Total Errors";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Timeouts in Total Errors";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Retries in Total Errors";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Prompt Id Format";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Barge-in";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Mode";
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Location = new System.Drawing.Point(159, 19);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(180, 21);
            this.modeComboBox.TabIndex = 1;
            // 
            // bargeInComboBox
            // 
            this.bargeInComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bargeInComboBox.FormattingEnabled = true;
            this.bargeInComboBox.Location = new System.Drawing.Point(159, 46);
            this.bargeInComboBox.Name = "bargeInComboBox";
            this.bargeInComboBox.Size = new System.Drawing.Size(180, 21);
            this.bargeInComboBox.TabIndex = 3;
            // 
            // retriesComboBox
            // 
            this.retriesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.retriesComboBox.FormattingEnabled = true;
            this.retriesComboBox.Location = new System.Drawing.Point(159, 100);
            this.retriesComboBox.Name = "retriesComboBox";
            this.retriesComboBox.Size = new System.Drawing.Size(180, 21);
            this.retriesComboBox.TabIndex = 7;
            // 
            // promptIdFormatComboBox
            // 
            this.promptIdFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.promptIdFormatComboBox.FormattingEnabled = true;
            this.promptIdFormatComboBox.Location = new System.Drawing.Point(159, 73);
            this.promptIdFormatComboBox.Name = "promptIdFormatComboBox";
            this.promptIdFormatComboBox.Size = new System.Drawing.Size(180, 21);
            this.promptIdFormatComboBox.TabIndex = 5;
            // 
            // disconfirmsComboBox
            // 
            this.disconfirmsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.disconfirmsComboBox.FormattingEnabled = true;
            this.disconfirmsComboBox.Location = new System.Drawing.Point(159, 154);
            this.disconfirmsComboBox.Name = "disconfirmsComboBox";
            this.disconfirmsComboBox.Size = new System.Drawing.Size(180, 21);
            this.disconfirmsComboBox.TabIndex = 11;
            // 
            // timeoutsComboBox
            // 
            this.timeoutsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.timeoutsComboBox.FormattingEnabled = true;
            this.timeoutsComboBox.Location = new System.Drawing.Point(159, 127);
            this.timeoutsComboBox.Name = "timeoutsComboBox";
            this.timeoutsComboBox.Size = new System.Drawing.Size(180, 21);
            this.timeoutsComboBox.TabIndex = 9;
            // 
            // initializationTab
            // 
            this.initializationTab.Controls.Add(this.initializationDataGridView);
            this.initializationTab.Location = new System.Drawing.Point(4, 22);
            this.initializationTab.Name = "initializationTab";
            this.initializationTab.Padding = new System.Windows.Forms.Padding(3);
            this.initializationTab.Size = new System.Drawing.Size(976, 629);
            this.initializationTab.TabIndex = 4;
            this.initializationTab.Text = "Initialization";
            this.initializationTab.UseVisualStyleBackColor = true;
            // 
            // initializationDataGridView
            // 
            this.initializationDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.initializationDataGridView.Location = new System.Drawing.Point(7, 7);
            this.initializationDataGridView.Name = "initializationDataGridView";
            this.initializationDataGridView.Size = new System.Drawing.Size(963, 616);
            this.initializationDataGridView.TabIndex = 0;
            // 
            // globalBehaviorTab
            // 
            this.globalBehaviorTab.Controls.Add(this.groupBox3);
            this.globalBehaviorTab.Controls.Add(this.groupBox2);
            this.globalBehaviorTab.Controls.Add(this.groupBox1);
            this.globalBehaviorTab.Location = new System.Drawing.Point(4, 22);
            this.globalBehaviorTab.Name = "globalBehaviorTab";
            this.globalBehaviorTab.Padding = new System.Windows.Forms.Padding(3);
            this.globalBehaviorTab.Size = new System.Drawing.Size(976, 629);
            this.globalBehaviorTab.TabIndex = 5;
            this.globalBehaviorTab.Text = "Global Behavior";
            this.globalBehaviorTab.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.confirmationsDataGridView);
            this.groupBox3.Location = new System.Drawing.Point(7, 423);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(957, 200);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Confirmations";
            // 
            // confirmationsDataGridView
            // 
            this.confirmationsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.confirmationsDataGridView.Location = new System.Drawing.Point(7, 19);
            this.confirmationsDataGridView.Name = "confirmationsDataGridView";
            this.confirmationsDataGridView.Size = new System.Drawing.Size(944, 176);
            this.confirmationsDataGridView.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.commandsDataGridView);
            this.groupBox2.Location = new System.Drawing.Point(6, 215);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(958, 202);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Command Transitions";
            // 
            // commandsDataGridView
            // 
            this.commandsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.commandsDataGridView.Location = new System.Drawing.Point(7, 19);
            this.commandsDataGridView.Name = "commandsDataGridView";
            this.commandsDataGridView.Size = new System.Drawing.Size(945, 176);
            this.commandsDataGridView.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.promptsDataGridView);
            this.groupBox1.Location = new System.Drawing.Point(7, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(963, 202);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Prompts";
            // 
            // promptsDataGridView
            // 
            this.promptsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.promptsDataGridView.Location = new System.Drawing.Point(7, 20);
            this.promptsDataGridView.Name = "promptsDataGridView";
            this.promptsDataGridView.Size = new System.Drawing.Size(950, 176);
            this.promptsDataGridView.TabIndex = 0;
            // 
            // maxHandlingTab
            // 
            this.maxHandlingTab.Controls.Add(this.maxHandlingDataGridView);
            this.maxHandlingTab.Location = new System.Drawing.Point(4, 22);
            this.maxHandlingTab.Name = "maxHandlingTab";
            this.maxHandlingTab.Padding = new System.Windows.Forms.Padding(3);
            this.maxHandlingTab.Size = new System.Drawing.Size(976, 629);
            this.maxHandlingTab.TabIndex = 6;
            this.maxHandlingTab.Text = "Max Handling";
            this.maxHandlingTab.UseVisualStyleBackColor = true;
            // 
            // maxHandlingDataGridView
            // 
            this.maxHandlingDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.maxHandlingDataGridView.Location = new System.Drawing.Point(7, 7);
            this.maxHandlingDataGridView.Name = "maxHandlingDataGridView";
            this.maxHandlingDataGridView.Size = new System.Drawing.Size(963, 616);
            this.maxHandlingDataGridView.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(840, 673);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OK_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(921, 673);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 185);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "State Output Sort Order";
            // 
            // sortOrderComboBox
            // 
            this.sortOrderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sortOrderComboBox.FormattingEnabled = true;
            this.sortOrderComboBox.Location = new System.Drawing.Point(159, 181);
            this.sortOrderComboBox.Name = "sortOrderComboBox";
            this.sortOrderComboBox.Size = new System.Drawing.Size(180, 21);
            this.sortOrderComboBox.TabIndex = 13;
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 708);
            this.ControlBox = false;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximumSize = new System.Drawing.Size(1014, 736);
            this.MinimumSize = new System.Drawing.Size(1014, 736);
            this.Name = "StartForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Start";
            this.Load += new System.EventHandler(this.Start_Load);
            this.tabControl1.ResumeLayout(false);
            this.defaultSettingsTab.ResumeLayout(false);
            this.defaultSettingsTab.PerformLayout();
            this.initializationTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.initializationDataGridView)).EndInit();
            this.globalBehaviorTab.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.confirmationsDataGridView)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.commandsDataGridView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.promptsDataGridView)).EndInit();
            this.maxHandlingTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.maxHandlingDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage defaultSettingsTab;
        private System.Windows.Forms.TabPage initializationTab;
        private System.Windows.Forms.TabPage globalBehaviorTab;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridView initializationDataGridView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView confirmationsDataGridView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView commandsDataGridView;
        private System.Windows.Forms.DataGridView promptsDataGridView;
        private System.Windows.Forms.TabPage maxHandlingTab;
        private System.Windows.Forms.DataGridView maxHandlingDataGridView;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.ComboBox bargeInComboBox;
        private System.Windows.Forms.ComboBox retriesComboBox;
        private System.Windows.Forms.ComboBox promptIdFormatComboBox;
        private System.Windows.Forms.ComboBox disconfirmsComboBox;
        private System.Windows.Forms.ComboBox timeoutsComboBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox sortOrderComboBox;
        private System.Windows.Forms.Label label1;
    }
}