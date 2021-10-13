using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Google.Cloud.TextToSpeech.V1;


namespace TextToSpeechTTV {


    class Config {
        //Path for every Config
        private readonly string creds = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "creds.txt");
        private readonly string options = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "options.txt");
        private readonly string blocklist = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "blocklist.txt");
        private readonly string badwords = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "badwords.txt");
        private readonly string new_usernames = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "usernames.json");
        private readonly string voicelistfile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "voicelist.txt");
        private readonly string foldername = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config");
        public List<string> voicelist;
        public TextToSpeechClient client;
        public string[] optionsList;
        public string[] credsList;

        public Config () {
            if (Directory.Exists(foldername)) {
                optionsList = File.ReadAllLines(options);
                credsList = File.ReadAllLines(creds);
            }
            CreateConfig();
        }

        public object AuthExplicit () {
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Config", "gcp.json"));
            try {
                client = TextToSpeechClient.Create();
            } catch (Exception e) {
                Console.WriteLine($"Failed to initialise GCP TTS Client. Set GCP to 'false' in options.txt");
                Console.WriteLine("Press any key to see the exception...");
                Console.ReadKey();
                Console.WriteLine(e);
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
            var response = client.ListVoices("");

            List<string> voices = new List<string>();

            foreach (var voice in response.Voices) {
                Console.WriteLine($"{voice.Name} ({voice.SsmlGender}); Rate:{voice.NaturalSampleRateHertz} Language codes: {string.Join(", ", voice.LanguageCodes)}");
                voices.Add(voice.Name);
            }
            voicelist = voices;

            File.WriteAllText(voicelistfile, string.Empty);
            File.WriteAllLines(voicelistfile, voices);
            return null;
        }

        private void CreateConfig () {
            if (Directory.Exists(foldername)) {
                if (GetGCP() != "false") {
                    AuthExplicit();
                }
                return;
            }


            if (!Directory.Exists(foldername))
                Directory.CreateDirectory(foldername);
            if (!File.Exists(badwords))
                File.Create(badwords).Dispose();
            if (!File.Exists(options))
                FillOptionsFile();
            if (!File.Exists(blocklist))
                FillBlocklistExamples();
            if (!File.Exists(new_usernames))
                FillNewUsernamesExamples();
            if (!File.Exists(creds))
                FillCredsFile();
        }

        private void FillNewUsernamesExamples () {
            File.WriteAllLines(new_usernames, new string[]
            {
            @"{
  ""users"": [
    {
      ""name"": ""youraccount"",
      ""nick"": ""you"",
      ""voice"": ""random"",
      ""speakingSpeed:"" ""1"",
      ""speakingPitch:"" ""0""
    },
    {
      ""name"": ""myaccount"",
      ""nick"": ""me"",
      ""voice"": ""fr-CA-Wavenet-B"",
      ""speakingSpeed:"" ""2"",
      ""speakingPitch:"" ""10""
    }
  ]
}"
            });
        }
        private void FillBlocklistExamples () {
            File.WriteAllLines(blocklist, new string[]
            {
                "nightbot",
                "moobot",
                "phantombot",
                "coebot",
                "deepbot"
            });
        }
        private void FillCredsFile () {
            Console.Write("Please enter your Botname (or Username, if no Bot Account): ");
            string twitchId = Console.ReadLine();
            Console.Write("Please enter your oauth key. URL in Readme (oauth:...): ");
            string oAuth = Console.ReadLine();
            Console.Write("Please enter your Channel Name (lowercase): ");
            string channelName = Console.ReadLine();
            Console.Write("Please enter your Access Token. URL in Readme: ");
            string accessToken = Console.ReadLine();
            Console.Write("Lastly I need your ChannelID. You can get the ID from the URL in the Readme\n" +
                          "Please Enter your Channel ID: ");
            string channelId = Console.ReadLine();
            File.WriteAllLines(creds, new string[] {
                "Twitch ID (lowercase):",
                twitchId,
                "OAUTH (Twitch TMI):",
                oAuth,
                "Channel(lowercase):",
                channelName,
                "Access token:",
                accessToken,
                "ChannelID:",
                channelId
            });
            Console.WriteLine("If anything doesn't seem to work out or changes, you can find this file in Config/creds.txt and edit it." +
                "\nOptions can be changed in options.txt as well. Pitch & Speed are only available for Google TTS.");
            Console.WriteLine("--------------------------------------------------------");
        }


        private void FillOptionsFile () {
            Console.WriteLine("I've automatically set up the default options for the TTS.\n" +
                              "You can change those Settings in the options.txt file.");
            string rewardName = "";
            while (true) {
                Console.Write("Do you want to bind TTS to a Channel Reward? Y / N: ");
                string result = Console.ReadLine().ToLower();
                if (result == "y") {
                    Console.Write("Please enter the Reward Title (e.g. TTS-Reward), case sensitive: ");
                    rewardName = Console.ReadLine();
                    break;
                } else if (result == "n") {
                    Console.WriteLine("Alright! TTS will run on every message sent!");
                    rewardName = "RewardType.None";
                    break;
                }
            }

            string readOut = "";
            while (true) {
                Console.Write("Should the TTS read out usernames? Y/N:");
                readOut = Console.ReadLine().ToLower();
                if (readOut == "y" || readOut == "n")
                    break;
            }

            double googleSpeakingRate;
            double googlePitch;

            while (true) {
                Console.Write("Please select Speaking Speed (0.25 to 4.0; 1 = Default Speed):");
                string speakingRate = Console.ReadLine();
                Console.Write("Please select the voice pitch (-20 to 20; 0 = Default Pitch):");
                string speakingPitch = Console.ReadLine();
                speakingRate = speakingRate.Replace(",", ".");
                speakingPitch = speakingPitch.Replace(",", ".");
                double.TryParse(speakingRate, NumberStyles.Any, CultureInfo.InvariantCulture, out googleSpeakingRate);
                double.TryParse(speakingPitch, NumberStyles.Any, CultureInfo.InvariantCulture, out googlePitch);
                if ((googleSpeakingRate <= 4 || googleSpeakingRate >= 0.25)
                    && (googlePitch <= 20 || googlePitch >= -20)) {
                    break;
                }
                Console.WriteLine("The entered values are either too high or too low! Please recheck the values.");
            }

            File.WriteAllLines(options, new string[] {
                "Set TTS Voice:",
                "Microsoft David Desktop",
                "Set Message Connector:",
                "said",
                "Maximum allowed Characters, 0 for no limit:",
                "100",
                "Replace swear word with:",
                "beep",
                "Say this, if long Sentence:",
                "to be continued",
                "GCP TTS? (true, wavenet, standard, false)",
                "false",
                "Default GCP Voice: (Select from voicelist.txt, random, random-pc-language:)",
                "Random-pc-language",
                "Bound Reward Name:",
                 rewardName,
                 "Read Names?:",
                 readOut == "y" ? "true" : "false",
                 "Google Speaking Speed:",
                 googleSpeakingRate.ToString(),
                 "Google Speaking Pitch:",
                 googlePitch.ToString()
            });
            Console.WriteLine("--------------------------------------------------------");
        }

        public string GetUsername () {
            string id = credsList[1];
            return id;
        }

        public string GetOAuth () {
            string password = credsList[3];
            return password;
        }

        public string GetChannel () {
            string channel = credsList[5];
            return channel;
        }

        public string GetAccessToken () {
            string accessToken = credsList[7];
            return accessToken;
        }

        public string GetChannelId () {
            string channelId = credsList[9];
            return channelId;
        }

        public bool ReadOutNames () {
            bool readOut = bool.Parse(optionsList[17]);
            return readOut;
        }

        public double GetSpeakingRate () {
            string strRate = optionsList[19].Replace(",", ".");
            bool parsable = double.TryParse(strRate, NumberStyles.Any, CultureInfo.InvariantCulture, out double speakingRate);
            if (speakingRate > 4 || speakingRate < 0.25) {
                throw new ArgumentException("Speaking Rate can't be higher than 4 or lower than 0.25!");
            }
            if (!parsable) {
                throw new ArgumentException("Can't parse Speaking Rate!");
            }
            return speakingRate;
        }
        public double GetSpeakingPitch () {
            string strPitch = optionsList[21].Replace(",", ".");
            bool parsable = double.TryParse(strPitch, NumberStyles.Any, CultureInfo.InvariantCulture, out double speakingPitch);

            if (speakingPitch > 20 || speakingPitch < -20) {
                throw new ArgumentException("Speaking Pitch can't be higher than 20 or lower than -20!");
            }
            if (!parsable) {
                throw new ArgumentException("Can't parse Speaking Pitch!");
            }
            return speakingPitch;
        }

        public string SetVoice () {
            string voice = optionsList[1];
            return voice;
        }
        public string SetMessageConnector () {
            string say = optionsList[3];
            return say;
        }

        public int GetMaxCharacterLength () {
            string wordLength = optionsList[5];
            int.TryParse(wordLength, out int result);
            return result;
        }

        public string ReplaceSwearWord () {
            string antiswear = optionsList[7];
            return antiswear;
        }

        public string GetLongMessage () {
            string longMessage = optionsList[9];
            return longMessage;
        }

        public string GetGCP () {
            string gcp = optionsList[11].ToLower();
            string[] settings = { "true", "wavenet", "standard", "false" };
            if (settings.Contains(gcp))
                return gcp;
            Console.WriteLine("Couldn't get GCP Settings in Settings file!");
            throw new ArgumentException("Must be either true, false, wavenet, or standard");
        }
        public string GetRewardName () {
            string rewardName = optionsList[15];
            return rewardName;
        }

        public TextToSpeechClient GetGCPClient () {
            return client;
        }
    }
}
