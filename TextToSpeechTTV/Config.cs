using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeechTTV
{
    class Config
    {
        private string oauth = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "creds.txt");
        private string options = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "options.txt");

        public string GetUsername()
        {
            string id = File.ReadAllLines(oauth)[1];
            return id;
        }

        public string GetOAuth()
        {
            string password = File.ReadAllLines(oauth)[3];
            return password;
        }

        public string GetChannel()
        {
            string channel = File.ReadAllLines(oauth)[5];
            return channel;
        }

        public string SetVoice()
        {
            string voice = File.ReadAllLines(options)[1];
            return voice;
        }
        public string SetMessageConnector()
        {
            string say = File.ReadAllLines(options)[3];
            return say;
        }

        public int GetMaxCharacterLength()
        {
            int result = 0;
            string wordLength = File.ReadAllLines(options)[5];
            int.TryParse(wordLength, out result);
            return result;
        }

        public string ReplaceSwearWord()
        {
            string antiswear = File.ReadAllLines(options)[7];
            return antiswear;
        }

        public string GetLongMessage()
        {
            string longMessage = File.ReadAllLines(options)[9];
            return longMessage;
        }
    }
}
