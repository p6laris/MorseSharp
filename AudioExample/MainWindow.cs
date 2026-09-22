using MorseSharp;
using System.Media;

namespace AudioExample
{
    public partial class MainWindow : Form
    {
        // The frequency Convert() always renders at, so Decode can listen for the same tone.
        private const double Frequency = 600;

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
            DecodeBtn.Enabled = false;
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
                    .SetAudioOptions(25, 25, Frequency)
                    .GetBytes();

                language = selected;
                MorseTxt.Text = morse;
                PlayBtn.Enabled = true;
                DecodeBtn.Enabled = true;
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

        // Reads the last rendered WAV straight back into text, proving the round trip works without
        // relying on the Morse string Convert() already has.
        private void DecodeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                const int HeaderSize = 44;
                short[] pcm = new short[(wav.Length - HeaderSize) / 2];
                Buffer.BlockCopy(wav, HeaderSize, pcm, 0, pcm.Length * 2);

                string decoded = Morse.GetConverter()
                    .ForLanguage(language)
                    .FromAudio(pcm, sampleRate: 11025, frequency: Frequency, wordsPerMinute: 25);

                ResultsTxt.Text = $"Decoded from audio:\r\n{decoded}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Fills the message box with a procedural signal and encodes it, to show that a prosign is
        // keyed as one unbroken sequence rather than as separate letters.
        private void ProsignDemoBtn_Click(object sender, EventArgs e)
        {
            MessageMorseTxt.Text = "CQ CQ DE W1AW <AR>";
            Convert(Language.English);
        }

        private void KochBtn_Click(object sender, EventArgs e)
        {
            string lesson = Koch.Generate(level: 5, groups: 6);
            ResultsTxt.Text = $"Koch lesson (level 5):\r\n{lesson}";
        }

        private void QsoBtn_Click(object sender, EventArgs e)
        {
            ResultsTxt.Text = $"Callsign: {Callsign.Next()}\r\n\r\n" + string.Join("\r\n", Qso.Generate());
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
