using MorseSharp;
using System.IO;
using System.Media;
namespace AudioExample
{
    public partial class MainWindow : Form
    {
        //Declare memory of bytes
        byte[] bytes = default;

        //Language
        Language language;

        //Declare SoundPlayer object to play the sound form the stream
        SoundPlayer soundPlayer;
        public MainWindow()
        {
            InitializeComponent();

            //Init members
            PlayBtn.Enabled = false;
            soundPlayer = new();
        }

        private void ToAudioBtn_Click(object sender, EventArgs e)
        {

            try
            {
                //Check if the message length is greater than zero.
                if (MessageMorseTxt.Text.Length > 0)
                {
                    //Get wav bytes from the ConvertMorseAudio method and then assigned to memory of bytes
                    Morse.GetConverter()
                        .ForLanguage(Language.English)
                        .ToMorse(MessageMorseTxt.Text)
                        .ToAudio()
                        .SetAudioOptions(25, 25, 600)
                        .GetBytes(out Span<byte> bytes);

                    this.bytes = bytes.ToArray();


                    //Update the richtextbox text to morse dash and dots.
                    MorseTxt.Text = Morse.GetConverter()
                        .ForLanguage(Language.English)
                        .ToMorse(MessageMorseTxt.Text)
                        .Encode();

                    language = Language.English;
                    //Enable the play button to play
                    PlayBtn.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            //Read the memory stream from the memory of bytes
            using (Stream stream = new MemoryStream(bytes.ToArray()))
            {
                //Assign the stream to soundPlayer Stream
                soundPlayer.Stream = stream;

                //Play the sound
                soundPlayer.PlaySync();
            }
        }

        private void ToMorseKurdish_Click(object sender, EventArgs e)
        {
            ////Delcare and init MorseSharp MorseAudioConverter object to convert morse to audio of dash and dots.
            //MorseAudioConverter converter = new MorseAudioConverter(Language.Kurdish);

            ////Delcare and init MorseSharp MorseTextConverter object to convert sentence to morse dash and dots.
            //TextMorseConverter textConverter = new TextMorseConverter(Language.Kurdish);

            try
            {
                //Check if the message length is greater than zero.
                if (MessageMorseTxt.Text.Length > 0)
                {
                    //Get wav bytes from the ConvertMorseAudio method and then assigned to memory of bytes
                    Morse.GetConverter()
                        .ForLanguage(Language.Kurdish)
                        .ToMorse(MessageMorseTxt.Text)
                        .ToAudio()
                        .SetAudioOptions(25, 25, 600)
                        .GetBytes(out Span<byte> bytes);

                    this.bytes = bytes.ToArray();


                    //Update the richtextbox text to morse dash and dots.
                    MorseTxt.Text = Morse.GetConverter()
                        .ForLanguage(Language.Kurdish)
                        .ToMorse(MessageMorseTxt.Text)
                        .Encode();

                    language = Language.Kurdish;
                    //Enable the play button to play
                    PlayBtn.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void blinkBtn_Click(object sender, EventArgs e)
        {
            if (MessageMorseTxt.Text.Length > 0)
            {
                Morse.GetConverter()
                    .ForLanguage(this.language)
                    .ToMorse(MessageMorseTxt.Text)
                    .ToLight()
                    .SetBlinkerOptions(25, 25)
                    .DoBlinks((hasToBlink) =>
                    {
                        if (hasToBlink)
                        {
                            blinkerPl.BackColor = Color.Black;
                        }
                        else
                        {
                            blinkerPl.BackColor = Color.White;
                        }
                    });
            }
        }
    }
}
