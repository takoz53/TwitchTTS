using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using DarrenLee.SpeechSynthesis;
using System.Text.RegularExpressions;

namespace TextToSpeechTTV
{
    class TwitchBot
    {
        TwitchClient client;
        Config config;
        SpeechWordHandler speechWordHandler;
        int maxWordLength = 0;
        string locale = "";
        string voice = "";
        public TwitchBot()
        {

            //Set up Config Informations
            config = new Config();
            maxWordLength = config.GetMaxWordLength();
            locale = config.SetLocale();
            voice = config.SetVoice();

            //Set up Speech Helper
            SpeechHelper.Rate = 0;
            speechWordHandler = new SpeechWordHandler();
            //Show all available voices to users
            List<string> voices = SpeechHelper.GetAllInstalledVoices();
            foreach (string s in voices)
                Console.WriteLine(s);

            //Set up Twitch Info
            ConnectionCredentials credentials = new ConnectionCredentials(config.GetUsername(), config.GetOAuth());
            
            client = new TwitchClient();
            client.Initialize(credentials, config.GetChannel());
            client.OnConnected += OnConnected;
            client.OnJoinedChannel += OnJoinedChannel;
            client.OnMessageReceived += OnMessageReceived;
            client.OnNewSubscriber += OnNewSubscriber;

            //Log in Twitch
            client.Connect();
        }
        private void OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }
        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine($"Successfully joined {e.Channel} Channel.");
            client.SendMessage(e.Channel, "TTS by takoz5334 successfully joined channel");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            Console.WriteLine(e.ChatMessage.Username + " said " + e.ChatMessage.Message);

            string newUsername = speechWordHandler.ContainsUsername(e.ChatMessage.Username);

            if (e.ChatMessage.Username == e.ChatMessage.BotUsername) //Ignore TTS-Bot.
                return;
            if (speechWordHandler.CheckBlocked(e.ChatMessage.Username)) //Ignore blocked users
                return;
            if (e.ChatMessage.Message.StartsWith("!")) //Ignore Commands starting with !
                return;

            //Check if URL is in Message
            Regex UrlMatch = new Regex(@"(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?");
            Match url = UrlMatch.Match(e.ChatMessage.Message);

            //Create a List for multiple bad Words in sentence
            //Add first replaced sentence
            //Get first replaced sentence and replace it and continue this loop for each bad word.
            List<string> badWords = new List<string>();

            badWords = speechWordHandler.ContainsBadWord(e.ChatMessage.Message);


            string newMessageEdited = e.ChatMessage.Message;

            if(url.Success) //Check if contains URL
                newMessageEdited = e.ChatMessage.Message.Replace(url.Value, "url");
            if (badWords.Count != 0) //Check if containing bad words
            {
                for (int i = 0; i < badWords.Count; i++)
                    newMessageEdited = newMessageEdited.Replace(badWords.ElementAt(i), "beep");
            }
            if (maxWordLength <= newMessageEdited.Length && maxWordLength != 0) //Check if Sentence is too long
            {
                newMessageEdited = newMessageEdited.Substring(0, Math.Min(newMessageEdited.Length, maxWordLength)) + "....... to be continued.";
                Speak(newUsername + " said " + newMessageEdited);
            }
            else
                Speak(newUsername + " said " + newMessageEdited);
        }

        private void OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            client.SendMessage(e.Channel, $"{e.Subscriber.DisplayName} thank you for subbing! Much love <3 PogChamp");
            Speak($"{e.Subscriber.DisplayName} thank you for Subscribing, I love you!");
        }

        private void Speak(string text) //Shorten SpeechHelper Function
        {
            SpeechHelper.Speak(locale, voice, text);
        }
    }
}
