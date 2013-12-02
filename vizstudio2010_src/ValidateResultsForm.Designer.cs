namespace PathMaker {
    partial class ValidateResultsForm {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValidateResultsForm));
            this.OkayBtn = new System.Windows.Forms.Button();
            this.UISpecResultsGridView = new System.Windows.Forms.DataGridView();
            this.SaveResultsBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.UISpecResultsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // OkayBtn
            // 
            this.OkayBtn.Location = new System.Drawing.Point(895, 673);
            this.OkayBtn.Name = "OkayBtn";
            this.OkayBtn.Size = new System.Drawing.Size(99, 23);
            this.OkayBtn.TabIndex = 3;
            this.OkayBtn.Text = "OK";
            this.OkayBtn.UseVisualStyleBackColor = true;
            this.OkayBtn.Click += new System.EventHandler(this.OkayBtn_Click);
            // 
            // UISpecResultsGridView
            // 
            this.UISpecResultsGridView.AllowUserToAddRows = false;
            this.UISpecResultsGridView.AllowUserToOrderColumns = true;
            this.UISpecResultsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.UISpecResultsGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.UISpecResultsGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.UISpecResultsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.UISpecResultsGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.UISpecResultsGridView.Location = new System.Drawing.Point(12, 12);
            this.UISpecResultsGridView.MultiSelect = false;
            this.UISpecResultsGridView.Name = "UISpecResultsGridView";
            this.UISpecResultsGridView.ReadOnly = true;
            this.UISpecResultsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.UISpecResultsGridView.Size = new System.Drawing.Size(982, 655);
            this.UISpecResultsGridView.TabIndex = 2;
            this.UISpecResultsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.UISpecResultsGridView_CellContentClick);
            this.UISpecResultsGridView.SelectionChanged += new System.EventHandler(this.UISpecResultsGridView_SelectionChanged);
            // 
            // SaveResultsBtn
            // 
            this.SaveResultsBtn.Location = new System.Drawing.Point(12, 673);
            this.SaveResultsBtn.Name = "SaveResultsBtn";
            this.SaveResultsBtn.Size = new System.Drawing.Size(99, 23);
            this.SaveResultsBtn.TabIndex = 4;
            this.SaveResultsBtn.Text = "Save...";
            this.SaveResultsBtn.UseVisualStyleBackColor = true;
            this.SaveResultsBtn.Click += new System.EventHandler(this.SaveResultsBtn_Click);
            // 
            // UISpecResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 704);
            this.Controls.Add(this.SaveResultsBtn);
            this.Controls.Add(this.OkayBtn);
            this.Controls.Add(this.UISpecResultsGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(400, 150);
            this.MaximumSize = new System.Drawing.Size(1012, 736);
            this.MinimumSize = new System.Drawing.Size(1012, 736);
            this.Name = "UISpecResultsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PathRunner Results";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.UISpecResultsForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.UISpecResultsGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OkayBtn;
        private System.Windows.Forms.DataGridView UISpecResultsGridView;
        private System.Windows.Forms.Button SaveResultsBtn;

    }
}