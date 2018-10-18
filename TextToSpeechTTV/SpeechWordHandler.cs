using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeechTTV
{
    class SpeechWordHandler
    {
        private string badWordsLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "badwords.txt");
        private string usernameRecognitionLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "usernames.txt");
        private string blockListLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "blocklist.txt");
        private string prefWordsLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "prefWords.txt");
        private List<string> badWords;

        public SpeechWordHandler()
        {
            LoadBadWords();
        }

        private void LoadBadWords()
        {
            badWords = new List<string>();
            string[] badwords = File.ReadAllLines(badWordsLocation);
            foreach (string s in badwords)
                badWords.Add(s);
        }

        public List<string> ContainsBadWord(string text)
        {
            List<string> wordsFound = new List<string>();
            foreach(string s in badWords)
            {
                if (text.Contains(s))
                {
                    wordsFound.Add(s);
                }
            }
            return wordsFound;
        }

        public string ContainsUsername(string username)
        {
            List<string> usernames = File.ReadAllLines(usernameRecognitionLocation).ToList();
            List<string> availableNames = new List<string>();
            foreach(string s in usernames) //Example: hello=hi
            {
                availableNames.Add(s.Split('=')[0]);
            }
            for (int i = 0; i < availableNames.Count; i++)
            {
                if(availableNames.ElementAt(i) == username)
                {
                    return usernames.ElementAt(i).Split('=')[1];
                }
            }
            return username;
        }

        public bool CheckBlocked(string username)
        {
            List<string> usernames = File.ReadAllLines(blockListLocation).ToList();
            if (usernames.Contains(username))
                return true;
            return false;
        }
    }
}
