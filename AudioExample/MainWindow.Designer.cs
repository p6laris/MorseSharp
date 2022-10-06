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
            this.ToMorseEnglishBtn = new System.Windows.Forms.Button();
            this.MessageMorseTxt = new System.Windows.Forms.TextBox();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.MorseTxt = new System.Windows.Forms.RichTextBox();
            this.ToMorseKurdishBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ToMorseEnglishBtn
            // 
            this.ToMorseEnglishBtn.Location = new System.Drawing.Point(304, 35);
            this.ToMorseEnglishBtn.Name = "ToMorseEnglishBtn";
            this.ToMorseEnglishBtn.Size = new System.Drawing.Size(173, 29);
            this.ToMorseEnglishBtn.TabIndex = 0;
            this.ToMorseEnglishBtn.Text = "To Morse English";
            this.ToMorseEnglishBtn.UseVisualStyleBackColor = true;
            this.ToMorseEnglishBtn.Click += new System.EventHandler(this.ToAudioBtn_Click);
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
            this.PlayBtn.Location = new System.Drawing.Point(304, 105);
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
            // ToMorseKurdishBtn
            // 
            this.ToMorseKurdishBtn.Location = new System.Drawing.Point(304, 70);
            this.ToMorseKurdishBtn.Name = "ToMorseKurdishBtn";
            this.ToMorseKurdishBtn.Size = new System.Drawing.Size(173, 29);
            this.ToMorseKurdishBtn.TabIndex = 4;
            this.ToMorseKurdishBtn.Text = "To Morse Kurdish";
            this.ToMorseKurdishBtn.UseVisualStyleBackColor = true;
            this.ToMorseKurdishBtn.Click += new System.EventHandler(this.ToMorseKurdish_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 191);
            this.Controls.Add(this.ToMorseKurdishBtn);
            this.Controls.Add(this.MorseTxt);
            this.Controls.Add(this.PlayBtn);
            this.Controls.Add(this.MessageMorseTxt);
            this.Controls.Add(this.ToMorseEnglishBtn);
            this.Name = "MainWindow";
            this.Text = "MorseToAudio";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button ToMorseEnglishBtn;
        private TextBox MessageMorseTxt;
        private Button PlayBtn;
        private RichTextBox MorseTxt;
        private Button ToMorseKurdishBtn;
    }
}