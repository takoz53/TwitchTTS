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
            string voice = File.ReadAllLines(oauth)[7];
            return voice;
        }
        public string SetLocale()
        {
            string locale = File.ReadAllLines(oauth)[9];
            return locale;
        }

        public int GetMaxWordLength()
        {
            int result = 0;
            string wordLength = File.ReadAllLines(oauth)[11];
            int.TryParse(wordLength, out result);
            return result;
        }
    }
}
