using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextToSpeechTTV
{
    class SpeechWordHandler
    {
        private static string badWordsLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "badwords.txt");
        private static string blockListLocation = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "blocklist.txt");
        private static string options = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "options.txt");
        public Userlist userlist;
        public Userlist tempuserlist = new Userlist { Users = new List<User>() };
        public string defaultVoice;
        private List<string> badWords;

        public void loadDefaultVoice()
        {
            defaultVoice = File.ReadAllLines(options)[13];
        }
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

        public User ContainsJSONUsername(string username)
        {
            User tempUser = new User();
            //List<User> listOfUsers = new List<User
            if (!File.Exists(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "usernames.json")))
            {
                tempUser.Name = username;
                tempUser.Nick = username;
                //tempUser.Voice = defaultvoice;
                return tempUser;
            }
            String JSONtxt = File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "usernames.json"));
            userlist = JsonConvert.DeserializeObject<Userlist>(JSONtxt);
            tempUser = userlist.Users.FirstOrDefault(User => User.Name == username);
            if (tempUser != null)
            {
                return tempUser;
            }
            tempUser = tempuserlist.Users.FirstOrDefault(User => User.Name == username);
            if (tempUser != null)
            {
                return tempUser;
            }
            if (tempUser?.Name == null)
            {
                tempUser = createTempUser(username, defaultVoice);
                tempuserlist.Users.Add(tempUser);
                return tempUser;
            }
            return tempUser;
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

        public User createTempUser(string username, string voice)
        {
            User tempUser = new User();
            tempUser.Name = username;
            tempUser.Nick = username;
            tempUser.Voice = voice;
            tempuserlist.Users.Add(tempUser);
            return tempUser;
         }
    }
    }
public class User
    {
        public string Name { get; set; }
        public string Nick { get; set; }
        public string Voice { get; set; }
    }

public class Userlist
    {
        public List<User> Users { get; set; }
    }

