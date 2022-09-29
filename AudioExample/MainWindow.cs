using MorseSharp;
using MorseSharp.MorseConverter;
using System.IO;
using System.Media;
namespace AudioExample
{
    public partial class MainWindow : Form
    {
        //Declare memory of bytes
        Memory<byte> bytes = default;

        //Declare SoundPlayer object to play the sound form the stream
        SoundPlayer soundPlayer;
        public MainWindow()
        {
            InitializeComponent();

            //Init members
            PlayBtn.Enabled = false;
            soundPlayer = new();
        }

        private async void ToAudioBtn_Click(object sender, EventArgs e)
        {
            //Delcare and init MorseSharp MorseAudioConverter object to convert morse to audio of dash and dots.
            MorseAudioConverter converter = new MorseAudioConverter(25,25);

            //Delcare and init MorseSharp MorseTextConverter object to convert sentence to morse dash and dots.
            MorseTextConverter textConverter = new MorseTextConverter();

            try
            {
                //Check if the message length is greater than zero.
                if(MessageMorseTxt.Text.Length > 0)
                {
                    //Get wav bytes from the ConvertMorseAudio method and then assigned to memory of bytes
                    bytes = await converter.ConvertMorseToAudio(MessageMorseTxt.Text);

                    //Update the richtextbox text to morse dash and dots.
                    MorseTxt.Text = await textConverter.ConvertToMorseEnglish(MessageMorseTxt.Text);

                    //Enable the play button to play
                    PlayBtn.Enabled = true;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            //Read the memory stream from the memory of bytes
            using(Stream stream = new MemoryStream(bytes.ToArray()))
            {
                //Assign the stream to soundPlayer Stream
                soundPlayer.Stream = stream;

                //Play the sound
                soundPlayer.PlaySync();
            }
        }
    }
}