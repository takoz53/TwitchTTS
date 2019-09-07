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
        private static string badWordsLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "badwords.txt");
        private static string usernameRecognitionLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "usernames.txt");
        private static string blockListLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "blocklist.txt");

        private List<string> badWords;

        public SpeechWordHandler()
        {
            //Load bad words only once, because there are many and might cause performance issues, rather save them on a List.
            LoadBadWords();
        }

        private void LoadBadWords() //Get all bad words and add them to the badWords list.
        {
            badWords = new List<string>();
            string[] badwords = File.ReadAllLines(badWordsLocation);
            badWords.AddRange(badwords);
        }

        public List<string> ContainsBadWord(string text) //Check if sentence contains a bad word
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

        public string ContainsUsername(string username) //Check if username is in usernames list and return new nickname
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

        public bool CheckBlocked(string username) //Check if user is blocked
        {
            List<string> usernames = File.ReadAllLines(blockListLocation).ToList();
            if (usernames.Contains(username))
                return true;
            return false;
        }

        public string GetBlockListLocation()
        {
            return blockListLocation;
        }
    }
}
