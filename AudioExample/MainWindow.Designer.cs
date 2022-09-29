namespace AudioExample
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ToAudioBtn = new System.Windows.Forms.Button();
            this.MessageMorseTxt = new System.Windows.Forms.TextBox();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.MorseTxt = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // ToAudioBtn
            // 
            this.ToAudioBtn.Location = new System.Drawing.Point(304, 35);
            this.ToAudioBtn.Name = "ToAudioBtn";
            this.ToAudioBtn.Size = new System.Drawing.Size(142, 29);
            this.ToAudioBtn.TabIndex = 0;
            this.ToAudioBtn.Text = "ToAudioMorse";
            this.ToAudioBtn.UseVisualStyleBackColor = true;
            this.ToAudioBtn.Click += new System.EventHandler(this.ToAudioBtn_Click);
            // 
            // MessageMorseTxt
            // 
            this.MessageMorseTxt.Location = new System.Drawing.Point(12, 37);
            this.MessageMorseTxt.Name = "MessageMorseTxt";
            this.MessageMorseTxt.Size = new System.Drawing.Size(286, 27);
            this.MessageMorseTxt.TabIndex = 1;
            // 
            // PlayBtn
            // 
            this.PlayBtn.Location = new System.Drawing.Point(304, 69);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(142, 29);
            this.PlayBtn.TabIndex = 2;
            this.PlayBtn.Text = "Play";
            this.PlayBtn.UseVisualStyleBackColor = true;
            this.PlayBtn.Click += new System.EventHandler(this.PlayBtn_Click);
            // 
            // MorseTxt
            // 
            this.MorseTxt.Location = new System.Drawing.Point(12, 70);
            this.MorseTxt.Name = "MorseTxt";
            this.MorseTxt.Size = new System.Drawing.Size(286, 109);
            this.MorseTxt.TabIndex = 3;
            this.MorseTxt.Text = "";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 191);
            this.Controls.Add(this.MorseTxt);
            this.Controls.Add(this.PlayBtn);
            this.Controls.Add(this.MessageMorseTxt);
            this.Controls.Add(this.ToAudioBtn);
            this.Name = "MainWindow";
            this.Text = "MorseToAudio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button ToAudioBtn;
        private TextBox MessageMorseTxt;
        private Button PlayBtn;
        private RichTextBox MorseTxt;
    }
}