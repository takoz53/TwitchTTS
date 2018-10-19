using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using System.Globalization;
using System.Collections.ObjectModel;

namespace TextToSpeechTTV
{
    class SpeechHelper
    {
        SpeechSynthesizer speechSynthesizer;

        public SpeechHelper(string ttsName, int rate)
        {
            speechSynthesizer = new SpeechSynthesizer();

            speechSynthesizer.Rate = rate;
            try
            {
                speechSynthesizer.SelectVoice(ttsName);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("TTS Name does not exist. TTS will NOT work.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            

            speechSynthesizer.SpeakCompleted += SpeechSynthesizer_SpeakCompleted;
            
        }

        private void SpeechSynthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            Console.WriteLine("Speech has ended!");
        }

        public static List<string> GetAllInstalledVoices()
        {
            List<string> list = new List<string>();
            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
            {
                foreach (InstalledVoice installedVoice in speechSynthesizer.GetInstalledVoices())
                {
                    VoiceInfo voiceInfo = installedVoice.VoiceInfo;
                    list.Add(voiceInfo.Name);
                }
            }
            return list;
        }

        public void Speak(string text)
        {
                speechSynthesizer.Speak(text);
        }
    }
}
