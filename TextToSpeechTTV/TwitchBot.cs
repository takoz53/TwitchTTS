using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using System.Text.RegularExpressions;

namespace TextToSpeechTTV
{
    class TwitchBot
    {
        private TwitchClient client;
        private Config config;
        private SpeechWordHandler speechWordHandler;
        private SpeechHelper speechHelper;
        
        //Set some defaults
        private int maxWordLength = 100;
        private string messageConnector = "said";
        private string voice = "Microsoft David Desktop";
        private string antiswear = "beep";
        private string longMessage = "to be continued";
        //private string gcp = "false";

        public TwitchBot()
        {

            //Set up Config Informations
            config = new Config();
            maxWordLength = config.GetMaxCharacterLength();
            messageConnector = config.SetMessageConnector();
            antiswear = config.ReplaceSwearWord();
            voice = config.SetVoice();
            longMessage = config.GetLongMessage();

            //Set up Speech Helper
            speechHelper = new SpeechHelper(voice, 0);
            speechWordHandler = new SpeechWordHandler();
            //Show all available voices to users
            List<string> voices = SpeechHelper.GetAllInstalledVoices();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All available voices: ");
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (string s in voices)
                Console.WriteLine(s);
            Console.WriteLine("----------------------------------------------------------------");
            //Set up Twitch Info
            ConnectionCredentials credentials = new ConnectionCredentials(config.GetUsername(), config.GetOAuth());
            
            client = new TwitchClient();
            client.Initialize(credentials, config.GetChannel());
            client.OnConnected += OnConnected;
            client.OnJoinedChannel += OnJoinedChannel;
            client.OnMessageReceived += OnMessageReceived;


            //Log in Twitch
            client.Connect();
        }
        private void OnConnected(object sender, OnConnectedArgs e)
        {
            Console.Write($"Connected to ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(e.AutoJoinChannel);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Currently using voice: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(speechHelper.GetCurrentVoice());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Successfully joined Channel: {e.Channel}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            speechWordHandler.loadDefaultVoice();
            CommandHandler commandHandler = new CommandHandler();

            Console.WriteLine($"{e.ChatMessage.Username}:{e.ChatMessage.Message}");
            //string newUsername = speechWordHandler.ContainsUsername(e.ChatMessage.Username);
            //Console.WriteLine($"{speechWordHandler.ContainsJSONUsername(e.ChatMessage.Username)}");
            
            if (e.ChatMessage.Username == e.ChatMessage.BotUsername) //Ignore TTS-Bot.
                return;

            if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
            {
                if (e.ChatMessage.Message.StartsWith("!block"))
                {
                    bool blocked = commandHandler.BlockUser(e.ChatMessage.Message);
                    if (blocked)
                        client.SendMessage(config.GetChannel(), "The user has been successfully blocked.");
                    else
                        client.SendMessage(config.GetChannel(), "The user is already blocked or the input was wrong.");
                }
                else if (e.ChatMessage.Message.StartsWith("!unblock"))
                {
                    bool unblocked = commandHandler.UnblockUser(e.ChatMessage.Message);
                    if (unblocked)
                        client.SendMessage(config.GetChannel(), "The user has been successfully unblocked.");
                    else
                        client.SendMessage(config.GetChannel(), "The user isn't blocked or the input was wrong.");
                }
            }

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

            if (url.Success) //Check if contains URL
            {
                newMessageEdited = e.ChatMessage.Message.Replace(url.Value, "url");
            }

            if (badWords.Count != 0) //Check if containing bad words
            {
                for (int i = 0; i < badWords.Count; i++)
                    newMessageEdited = newMessageEdited.Replace(badWords.ElementAt(i), antiswear);
            }
            if ((maxWordLength + longMessage.Length) <= newMessageEdited.Length && maxWordLength != 0) //Check if Sentence is too long
            {
                newMessageEdited = newMessageEdited.Substring(0, Math.Min(newMessageEdited.Length, maxWordLength)) + "....... " + longMessage;
                //speechHelper.Speak($"{newUsername} {messageConnector} {newMessageEdited}");
                //return;
            }
            
            speechHelper.Speak_gcp(speechWordHandler.ContainsJSONUsername(e.ChatMessage.Username), $"{messageConnector} {newMessageEdited}");
            
        }
    }
}
