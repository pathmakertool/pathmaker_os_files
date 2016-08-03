namespace PathMaker
{
    partial class ModifyServerIp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.ServerIpText = new System.Windows.Forms.TextBox();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.extraDetails = new System.Windows.Forms.Label();
            this.useDefaultBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter the IP Address for the Validation Server:";
            // 
            // ServerIpText
            // 
            this.ServerIpText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerIpText.Location = new System.Drawing.Point(14, 33);
            this.ServerIpText.MaxLength = 15;
            this.ServerIpText.Name = "ServerIpText";
            this.ServerIpText.Size = new System.Drawing.Size(251, 20);
            this.ServerIpText.TabIndex = 1;
            this.ServerIpText.Text = PathMaker.GetValidationServerIP();
            this.ServerIpText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ServerIpText.WordWrap = false;
            // 
            // OKBtn
            // 
            this.OKBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKBtn.Location = new System.Drawing.Point(14, 87);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(72, 30);
            this.OKBtn.TabIndex = 2;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBtn.Location = new System.Drawing.Point(193, 88);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(72, 29);
            this.CancelBtn.TabIndex = 4;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // extraDetails
            // 
            this.extraDetails.AutoSize = true;
            this.extraDetails.Location = new System.Drawing.Point(40, 62);
            this.extraDetails.Name = "extraDetails";
            this.extraDetails.Size = new System.Drawing.Size(203, 13);
            this.extraDetails.TabIndex = 5;
            this.extraDetails.Text = "Other servers include .202, .173 and .240";
            // 
            // useDefaultBtn
            // 
            this.useDefaultBtn.Location = new System.Drawing.Point(104, 87);
            this.useDefaultBtn.Name = "useDefaultBtn";
            this.useDefaultBtn.Size = new System.Drawing.Size(72, 29);
            this.useDefaultBtn.TabIndex = 3;
            this.useDefaultBtn.Text = "Use Default";
            this.useDefaultBtn.UseVisualStyleBackColor = true;
            this.useDefaultBtn.Click += new System.EventHandler(this.useDefaultBtn_Click);
            // 
            // ModifyServerIp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 125);
            this.Controls.Add(this.useDefaultBtn);
            this.Controls.Add(this.extraDetails);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.ServerIpText);
            this.Controls.Add(this.label1);
            this.Name = "ModifyServerIp";
            this.Text = "Modify Server IP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox ServerIpText;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label extraDetails;
        private System.Windows.Forms.Button useDefaultBtn;
    }
}