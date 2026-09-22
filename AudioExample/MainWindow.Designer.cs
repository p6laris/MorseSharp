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
            ToMorseEnglishBtn = new Button();
            MessageMorseTxt = new TextBox();
            PlayBtn = new Button();
            MorseTxt = new RichTextBox();
            ToMorseKurdishBtn = new Button();
            blinkerPl = new Panel();
            blinkBtn = new Button();
            DecodeBtn = new Button();
            ProsignDemoBtn = new Button();
            KochBtn = new Button();
            QsoBtn = new Button();
            ResultsTxt = new RichTextBox();
            SuspendLayout();
            //
            // ToMorseEnglishBtn
            //
            ToMorseEnglishBtn.Location = new Point(304, 35);
            ToMorseEnglishBtn.Name = "ToMorseEnglishBtn";
            ToMorseEnglishBtn.Size = new Size(173, 29);
            ToMorseEnglishBtn.TabIndex = 0;
            ToMorseEnglishBtn.Text = "To Morse English";
            ToMorseEnglishBtn.UseVisualStyleBackColor = true;
            ToMorseEnglishBtn.Click += ToAudioBtn_Click;
            //
            // MessageMorseTxt
            //
            MessageMorseTxt.Location = new Point(12, 37);
            MessageMorseTxt.Name = "MessageMorseTxt";
            MessageMorseTxt.Size = new Size(286, 27);
            MessageMorseTxt.TabIndex = 1;
            //
            // PlayBtn
            //
            PlayBtn.Location = new Point(304, 105);
            PlayBtn.Name = "PlayBtn";
            PlayBtn.Size = new Size(173, 29);
            PlayBtn.TabIndex = 2;
            PlayBtn.Text = "Play";
            PlayBtn.UseVisualStyleBackColor = true;
            PlayBtn.Click += PlayBtn_Click;
            //
            // MorseTxt
            //
            MorseTxt.Location = new Point(12, 70);
            MorseTxt.Name = "MorseTxt";
            MorseTxt.Size = new Size(286, 60);
            MorseTxt.TabIndex = 3;
            MorseTxt.Text = "";
            //
            // ToMorseKurdishBtn
            //
            ToMorseKurdishBtn.Location = new Point(304, 70);
            ToMorseKurdishBtn.Name = "ToMorseKurdishBtn";
            ToMorseKurdishBtn.Size = new Size(173, 29);
            ToMorseKurdishBtn.TabIndex = 4;
            ToMorseKurdishBtn.Text = "To Morse Kurdish";
            ToMorseKurdishBtn.UseVisualStyleBackColor = true;
            ToMorseKurdishBtn.Click += ToMorseKurdish_Click;
            //
            // blinkerPl
            //
            blinkerPl.Location = new Point(12, 295);
            blinkerPl.Name = "blinkerPl";
            blinkerPl.Size = new Size(465, 140);
            blinkerPl.TabIndex = 5;
            //
            // blinkBtn
            //
            blinkBtn.Location = new Point(304, 140);
            blinkBtn.Name = "blinkBtn";
            blinkBtn.Size = new Size(173, 29);
            blinkBtn.TabIndex = 6;
            blinkBtn.Text = "Light";
            blinkBtn.UseVisualStyleBackColor = true;
            blinkBtn.Click += blinkBtn_Click;
            //
            // DecodeBtn
            //
            DecodeBtn.Location = new Point(304, 175);
            DecodeBtn.Name = "DecodeBtn";
            DecodeBtn.Size = new Size(173, 29);
            DecodeBtn.TabIndex = 7;
            DecodeBtn.Text = "Decode Audio";
            DecodeBtn.UseVisualStyleBackColor = true;
            DecodeBtn.Click += DecodeBtn_Click;
            //
            // ProsignDemoBtn
            //
            ProsignDemoBtn.Location = new Point(304, 210);
            ProsignDemoBtn.Name = "ProsignDemoBtn";
            ProsignDemoBtn.Size = new Size(173, 29);
            ProsignDemoBtn.TabIndex = 8;
            ProsignDemoBtn.Text = "Prosign Demo";
            ProsignDemoBtn.UseVisualStyleBackColor = true;
            ProsignDemoBtn.Click += ProsignDemoBtn_Click;
            //
            // KochBtn
            //
            KochBtn.Location = new Point(304, 245);
            KochBtn.Name = "KochBtn";
            KochBtn.Size = new Size(84, 29);
            KochBtn.TabIndex = 9;
            KochBtn.Text = "Koch";
            KochBtn.UseVisualStyleBackColor = true;
            KochBtn.Click += KochBtn_Click;
            //
            // QsoBtn
            //
            QsoBtn.Location = new Point(393, 245);
            QsoBtn.Name = "QsoBtn";
            QsoBtn.Size = new Size(84, 29);
            QsoBtn.TabIndex = 10;
            QsoBtn.Text = "QSO";
            QsoBtn.UseVisualStyleBackColor = true;
            QsoBtn.Click += QsoBtn_Click;
            //
            // ResultsTxt
            //
            ResultsTxt.Location = new Point(12, 135);
            ResultsTxt.Name = "ResultsTxt";
            ResultsTxt.ReadOnly = true;
            ResultsTxt.Size = new Size(286, 150);
            ResultsTxt.TabIndex = 11;
            ResultsTxt.Text = "";
            //
            // MainWindow
            //
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 460);
            Controls.Add(ResultsTxt);
            Controls.Add(QsoBtn);
            Controls.Add(KochBtn);
            Controls.Add(ProsignDemoBtn);
            Controls.Add(DecodeBtn);
            Controls.Add(blinkBtn);
            Controls.Add(blinkerPl);
            Controls.Add(ToMorseKurdishBtn);
            Controls.Add(MorseTxt);
            Controls.Add(PlayBtn);
            Controls.Add(MessageMorseTxt);
            Controls.Add(ToMorseEnglishBtn);
            Name = "MainWindow";
            Text = "MorseToAudio";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ToMorseEnglishBtn;
        private TextBox MessageMorseTxt;
        private Button PlayBtn;
        private RichTextBox MorseTxt;
        private Button ToMorseKurdishBtn;
        private Panel blinkerPl;
        private Button blinkBtn;
        private Button DecodeBtn;
        private Button ProsignDemoBtn;
        private Button KochBtn;
        private Button QsoBtn;
        private RichTextBox ResultsTxt;
    }
}
