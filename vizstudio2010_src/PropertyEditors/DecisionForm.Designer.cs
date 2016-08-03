﻿namespace PathMaker {
    partial class DecisionForm {
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.statePrefixTextBox = new System.Windows.Forms.TextBox();
            this.stateNumberTextBox = new System.Windows.Forms.TextBox();
            this.stateNameTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.transitionsDataGridView = new System.Windows.Forms.DataGridView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.developerNotesTextBox = new System.Windows.Forms.TextBox();
            this.designNotesTextBox = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transitionsDataGridView)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(921, 673);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(840, 673);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "State Name";
            // 
            // statePrefixTextBox
            // 
            this.statePrefixTextBox.Location = new System.Drawing.Point(81, 12);
            this.statePrefixTextBox.MaxLength = 2;
            this.statePrefixTextBox.Name = "statePrefixTextBox";
            this.statePrefixTextBox.Size = new System.Drawing.Size(28, 20);
            this.statePrefixTextBox.TabIndex = 1;
            // 
            // stateNumberTextBox
            // 
            this.stateNumberTextBox.Location = new System.Drawing.Point(115, 12);
            this.stateNumberTextBox.MaxLength = 4;
            this.stateNumberTextBox.Name = "stateNumberTextBox";
            this.stateNumberTextBox.Size = new System.Drawing.Size(37, 20);
            this.stateNumberTextBox.TabIndex = 2;
            // 
            // stateNameTextBox
            // 
            this.stateNameTextBox.Location = new System.Drawing.Point(158, 12);
            this.stateNameTextBox.Name = "stateNameTextBox";
            this.stateNameTextBox.Size = new System.Drawing.Size(838, 20);
            this.stateNameTextBox.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.transitionsDataGridView);
            this.groupBox2.Location = new System.Drawing.Point(12, 38);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(984, 559);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Transitions";
            // 
            // transitionsDataGridView
            // 
            this.transitionsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.transitionsDataGridView.Location = new System.Drawing.Point(7, 20);
            this.transitionsDataGridView.Name = "transitionsDataGridView";
            this.transitionsDataGridView.Size = new System.Drawing.Size(969, 533);
            this.transitionsDataGridView.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.developerNotesTextBox);
            this.groupBox4.Location = new System.Drawing.Point(12, 603);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(502, 64);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Developer Notes";
            // 
            // developerNotesTextBox
            // 
            this.developerNotesTextBox.Location = new System.Drawing.Point(6, 19);
            this.developerNotesTextBox.Multiline = true;
            this.developerNotesTextBox.Name = "developerNotesTextBox";
            this.developerNotesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.developerNotesTextBox.Size = new System.Drawing.Size(490, 33);
            this.developerNotesTextBox.TabIndex = 0;
            this.developerNotesTextBox.TextChanged += new System.EventHandler(this.developerNotesTextBox_TextChanged);
            // 
            // designNotesTextBox
            // 
            this.designNotesTextBox.Location = new System.Drawing.Point(6, 19);
            this.designNotesTextBox.Multiline = true;
            this.designNotesTextBox.Name = "designNotesTextBox";
            this.designNotesTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.designNotesTextBox.Size = new System.Drawing.Size(470, 33);
            this.designNotesTextBox.TabIndex = 0;
            this.designNotesTextBox.TextChanged += new System.EventHandler(this.designNotesTextBox_TextChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.designNotesTextBox);
            this.groupBox5.Location = new System.Drawing.Point(514, 603);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(482, 64);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Design Notes";
            // 
            // DecisionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 711);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.stateNameTextBox);
            this.Controls.Add(this.stateNumberTextBox);
            this.Controls.Add(this.statePrefixTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximumSize = new System.Drawing.Size(1014, 736);
            this.MinimumSize = new System.Drawing.Size(1014, 736);
            this.Name = "DecisionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Decision";
            this.Load += new System.EventHandler(this.PlayForm_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.transitionsDataGridView)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox statePrefixTextBox;
        private System.Windows.Forms.TextBox stateNumberTextBox;
        private System.Windows.Forms.TextBox stateNameTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView transitionsDataGridView;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox developerNotesTextBox;
        private System.Windows.Forms.TextBox designNotesTextBox;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}