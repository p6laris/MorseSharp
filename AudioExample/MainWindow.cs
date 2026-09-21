using MorseSharp;
using System.Media;

namespace AudioExample
{
    public partial class MainWindow : Form
    {
        // The last rendered WAV file.
        private byte[] wav = [];

        // The last encoded Morse string and the language it was encoded with.
        private string morse = "";
        private Language language = Language.English;

        // Plays the WAV from a memory stream.
        private readonly SoundPlayer soundPlayer = new();

        private CancellationTokenSource? blinkCancellation;

        public MainWindow()
        {
            InitializeComponent();
            PlayBtn.Enabled = false;
        }

        private void ToAudioBtn_Click(object sender, EventArgs e) => Convert(Language.English);

        private void ToMorseKurdish_Click(object sender, EventArgs e) => Convert(Language.Kurdish);

        private void Convert(Language selected)
        {
            try
            {
                if (MessageMorseTxt.Text.Length == 0)
                    return;

                var encoded = Morse.GetConverter()
                    .ForLanguage(selected)
                    .ToMorse(MessageMorseTxt.Text);

                morse = encoded.Encode();
                wav = encoded.ToAudio()
                    .SetAudioOptions(25, 25, 600)
                    .GetBytes();

                language = selected;
                MorseTxt.Text = morse;
                PlayBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            using MemoryStream stream = new(wav, writable: false);
            soundPlayer.Stream = stream;
            soundPlayer.PlaySync();
        }

        private async void blinkBtn_Click(object sender, EventArgs e)
        {
            if (morse.Length == 0)
                return;

            // A second click stops the running sequence.
            if (blinkCancellation is not null)
            {
                blinkCancellation.Cancel();
                return;
            }

            blinkCancellation = new CancellationTokenSource();
            try
            {
                await Morse.GetConverter()
                    .ForLanguage(language)
                    .ToLight(morse)
                    .SetBlinkerOptions(25, 25)
                    .DoBlinks(on => blinkerPl.BackColor = on ? Color.Black : Color.White, blinkCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                // Stopped by the user.
            }
            finally
            {
                blinkCancellation.Dispose();
                blinkCancellation = null;
            }
        }
    }
}
