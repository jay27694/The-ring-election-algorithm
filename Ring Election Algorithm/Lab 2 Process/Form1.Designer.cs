namespace Lab_2_Process
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
            this.txt_id = new System.Windows.Forms.TextBox();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.listbox = new System.Windows.Forms.ListBox();
            this.btnGenEle = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_id
            // 
            this.txt_id.Location = new System.Drawing.Point(12, 50);
            this.txt_id.Name = "txt_id";
            this.txt_id.Size = new System.Drawing.Size(115, 20);
            this.txt_id.TabIndex = 0;
            // 
            // btnRegister
            // 
            this.btnRegister.Enabled = false;
            this.btnRegister.Location = new System.Drawing.Point(133, 49);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(85, 23);
            this.btnRegister.TabIndex = 1;
            this.btnRegister.Text = "Register ID";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(12, 12);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(206, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // listbox
            // 
            this.listbox.FormattingEnabled = true;
            this.listbox.Location = new System.Drawing.Point(13, 77);
            this.listbox.Name = "listbox";
            this.listbox.Size = new System.Drawing.Size(205, 134);
            this.listbox.TabIndex = 3;
            // 
            // btnGenEle
            // 
            this.btnGenEle.Location = new System.Drawing.Point(13, 215);
            this.btnGenEle.Name = "btnGenEle";
            this.btnGenEle.Size = new System.Drawing.Size(205, 23);
            this.btnGenEle.TabIndex = 4;
            this.btnGenEle.Text = "Generate Election";
            this.btnGenEle.UseVisualStyleBackColor = true;
            this.btnGenEle.Click += new System.EventHandler(this.btnGenEle_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(230, 250);
            this.Controls.Add(this.btnGenEle);
            this.Controls.Add(this.listbox);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.txt_id);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_id;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ListBox listbox;
        private System.Windows.Forms.Button btnGenEle;
    }
}

