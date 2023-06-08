
namespace Sip_EX
{
    partial class ExForm
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
            this.Reg_btn = new System.Windows.Forms.Button();
            this.Unreg_btn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.WAVopenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btn_hangup = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Reg_btn
            // 
            this.Reg_btn.Location = new System.Drawing.Point(22, 188);
            this.Reg_btn.Name = "Reg_btn";
            this.Reg_btn.Size = new System.Drawing.Size(138, 88);
            this.Reg_btn.TabIndex = 0;
            this.Reg_btn.Text = "Register";
            this.Reg_btn.UseVisualStyleBackColor = true;
            this.Reg_btn.Click += new System.EventHandler(this.Reg_btn_Click);
            // 
            // Unreg_btn
            // 
            this.Unreg_btn.Location = new System.Drawing.Point(520, 188);
            this.Unreg_btn.Name = "Unreg_btn";
            this.Unreg_btn.Size = new System.Drawing.Size(145, 88);
            this.Unreg_btn.TabIndex = 1;
            this.Unreg_btn.Text = "Unregister";
            this.Unreg_btn.UseVisualStyleBackColor = true;
            this.Unreg_btn.Click += new System.EventHandler(this.Unreg_btn_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(182, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 88);
            this.button1.TabIndex = 2;
            this.button1.Text = "Call VOIP 000";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // WAVopenFileDialog
            // 
            this.WAVopenFileDialog.FileName = "*.wav";
            // 
            // btn_hangup
            // 
            this.btn_hangup.Location = new System.Drawing.Point(351, 188);
            this.btn_hangup.Name = "btn_hangup";
            this.btn_hangup.Size = new System.Drawing.Size(141, 88);
            this.btn_hangup.TabIndex = 3;
            this.btn_hangup.Text = "HangUp";
            this.btn_hangup.UseVisualStyleBackColor = true;
            this.btn_hangup.Click += new System.EventHandler(this.btn_hangup_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(182, 188);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 88);
            this.button2.TabIndex = 4;
            this.button2.Text = "Call SoftPhone 005";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ExForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 409);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btn_hangup);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Unreg_btn);
            this.Controls.Add(this.Reg_btn);
            this.Name = "ExForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ExForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Reg_btn;
        private System.Windows.Forms.Button Unreg_btn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog WAVopenFileDialog;
        private System.Windows.Forms.Button btn_hangup;
        private System.Windows.Forms.Button button2;
    }
}

