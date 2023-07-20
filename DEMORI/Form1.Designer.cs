namespace DEMORI
{
    partial class Form1
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
            this.locationLbl = new System.Windows.Forms.Label();
            this.browseBtn = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.targetLbl = new System.Windows.Forms.Label();
            this.outputBtn = new System.Windows.Forms.Button();
            this.decryptBtn = new System.Windows.Forms.Button();
            this.readmeBtn = new System.Windows.Forms.Button();
            this.srcLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // locationLbl
            // 
            this.locationLbl.Location = new System.Drawing.Point(12, 9);
            this.locationLbl.Name = "locationLbl";
            this.locationLbl.Size = new System.Drawing.Size(757, 23);
            this.locationLbl.TabIndex = 0;
            this.locationLbl.Text = "Xbox Game Location: ";
            // 
            // browseBtn
            // 
            this.browseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseBtn.Location = new System.Drawing.Point(12, 62);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(182, 42);
            this.browseBtn.TabIndex = 2;
            this.browseBtn.Text = "Select OMORI Directory";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.ForeColor = System.Drawing.SystemColors.Window;
            this.richTextBox1.Location = new System.Drawing.Point(12, 129);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(757, 278);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // targetLbl
            // 
            this.targetLbl.Location = new System.Drawing.Point(12, 36);
            this.targetLbl.Name = "targetLbl";
            this.targetLbl.Size = new System.Drawing.Size(757, 23);
            this.targetLbl.TabIndex = 4;
            this.targetLbl.Text = "Target Location: ";
            // 
            // outputBtn
            // 
            this.outputBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.outputBtn.Location = new System.Drawing.Point(204, 62);
            this.outputBtn.Name = "outputBtn";
            this.outputBtn.Size = new System.Drawing.Size(182, 42);
            this.outputBtn.TabIndex = 5;
            this.outputBtn.Text = "Select Output Directory";
            this.outputBtn.UseVisualStyleBackColor = true;
            this.outputBtn.Click += new System.EventHandler(this.outputBtn_Click);
            // 
            // decryptBtn
            // 
            this.decryptBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.decryptBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.decryptBtn.ForeColor = System.Drawing.Color.White;
            this.decryptBtn.Location = new System.Drawing.Point(394, 62);
            this.decryptBtn.Name = "decryptBtn";
            this.decryptBtn.Size = new System.Drawing.Size(182, 42);
            this.decryptBtn.TabIndex = 6;
            this.decryptBtn.Text = "Dump and Decrypt";
            this.decryptBtn.UseVisualStyleBackColor = false;
            this.decryptBtn.Click += new System.EventHandler(this.decryptBtn_Click);
            // 
            // readmeBtn
            // 
            this.readmeBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.readmeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.readmeBtn.ForeColor = System.Drawing.Color.White;
            this.readmeBtn.Location = new System.Drawing.Point(587, 62);
            this.readmeBtn.Name = "readmeBtn";
            this.readmeBtn.Size = new System.Drawing.Size(182, 42);
            this.readmeBtn.TabIndex = 7;
            this.readmeBtn.Text = "README";
            this.readmeBtn.UseVisualStyleBackColor = false;
            this.readmeBtn.Click += new System.EventHandler(this.readmeBtn_Click);
            // 
            // srcLink
            // 
            this.srcLink.Location = new System.Drawing.Point(12, 107);
            this.srcLink.Name = "srcLink";
            this.srcLink.Size = new System.Drawing.Size(100, 23);
            this.srcLink.TabIndex = 8;
            this.srcLink.TabStop = true;
            this.srcLink.Text = "Source code";
            this.srcLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.srcLink_LinkClicked);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(781, 419);
            this.Controls.Add(this.srcLink);
            this.Controls.Add(this.readmeBtn);
            this.Controls.Add(this.decryptBtn);
            this.Controls.Add(this.outputBtn);
            this.Controls.Add(this.targetLbl);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.browseBtn);
            this.Controls.Add(this.locationLbl);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Form1";
            this.Text = "OMORI decrypter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.LinkLabel srcLink;

        private System.Windows.Forms.Button readmeBtn;

        private System.Windows.Forms.Button decryptBtn;

        private System.Windows.Forms.Button outputBtn;

        private System.Windows.Forms.Label targetLbl;

        private System.Windows.Forms.RichTextBox richTextBox1;

        private System.Windows.Forms.Button browseBtn;

        private System.Windows.Forms.Label locationLbl;

        #endregion
    }
}