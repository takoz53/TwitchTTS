﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace TextToSpeechTTV
{
    class SpeechWordHandler
    {
        private static string badWordsLocation =
            Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "badwords.txt");

        private static string blockListLocation =
            Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "blocklist.txt");

        private static string options =
            Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "options.txt");

        public string defaultVoice;
        private List<string> badWords;

        public void LoadDefaultVoice()
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
            foreach (string s in badWords)
            {
                if (text.Contains(s))
                {
                    wordsFound.Add(s);
                }
            }

            return wordsFound;
        }

        public User ContainsJsonUsername(string username)
        {
            var jsonText = File.ReadAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config",
                "usernames.json"));
            var userList = JsonConvert.DeserializeObject<Userlist>(jsonText);
            foreach (var user in userList.Users)
            {
                if (user.Name == username)
                {
                    try
                    {
                        return CreateTempUser(username, user.Nick, user.Voice, user.SpeakingSpeed, user.SpeakingPitch);
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Couldn't get User Data. Speaking Rate or Pitch might not be set for user!\n" +
                                          "Please delete the Config Folder (or cut it somewhere else) and restart the Program.\n" +
                                          "This will create a new usernames.json file with the new json-properties.");
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                }
            }
            return CreateTempUser(username, username, defaultVoice, 1, 0);
        }

        public bool CheckBlocked(string username) //Check if user is blocked
        {
            var usernames = File.ReadAllLines(blockListLocation).ToList();
            return usernames.Contains(username);
        }

        public string GetBlockListLocation()
        {
            return blockListLocation;
        }

        public User CreateTempUser(string username, string nick, string voice, double speakingSpeed, double speakingPitch)
        {
            var tempUser = new User
            {
                Name = username,
                Nick = nick,
                Voice = voice,
                SpeakingSpeed = speakingSpeed,
                SpeakingPitch = speakingPitch
            };
            return tempUser;
        }
    }
}

public class User
{
    public string Name { get; set; }
    public string Nick { get; set; }
    public string Voice { get; set; }
    public double SpeakingSpeed { get; set; }
    public double SpeakingPitch { get; set; }
}

public class Userlist
{
    public List<User> Users { get; set; }
}

