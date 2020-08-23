using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using Google.Cloud.TextToSpeech.V1;
using System.IO;
using NAudio.Wave;
using System.Threading;


namespace TextToSpeechTTV
{
    class SpeechHelper
    {
        private SpeechSynthesizer speechSynthesizer;
        private TextToSpeechClient client;
        private List<string> voicelist;
        //private List<string> voicelistWavenet;
        //private List<string> voicelistStandard;

        public SpeechHelper(string ttsName, int rate)
        {
            speechSynthesizer = new SpeechSynthesizer
            {
                Rate = rate
            };

            speechSynthesizer.SetOutputToDefaultAudioDevice();

            try
            {
                speechSynthesizer.SelectVoice(ttsName);
                //Need to add this in an if gcp = yes
                Config config = new Config();
                string gcptype = config.GetGCP();
                voicelist = config.voicelist;
                if (gcptype != "false")
                {
                    client = config.GetGCPClient();
                    if (gcptype == "standard")
                    {
                        voicelist = voicelist.FindAll(delegate (string s) { return s.Contains("Standard"); });
                    }
                    else if (gcptype == "wavenet")
                    {
                        voicelist = voicelist.FindAll(delegate (string s) { return s.Contains("Wavenet"); });
                    }
                    else
                    {
                        //voicelist = voicelist;
                    }
                }
                Console.WriteLine(string.Join("\n",voicelist));
                Console.WriteLine("Voicelist");

            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("TTS Name does not exist. TTS will NOT work.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
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

        public void Speak(User user, string text)
        {
            Prompt prompt = new Prompt($"{ user.Nick } { text}");
            speechSynthesizer.Speak(prompt);
        }

        public string GetCurrentVoice()
        {
            return speechSynthesizer.Voice.Description;
        }

        public void Speak_gcp(User user, string text)
        {
            string voicename;
            string languageCode;

            //Fallback on Microsoft
            if ( (voicelist?.Any() != true) || (user.Voice.ToLower() == "microsoft") )
            {
                Speak(user, text);
                return;
            }

           // Set the text input to be synthesized.
           SynthesisInput input = new SynthesisInput
            {
                Text = $"{user.Nick} {text}"
            };

            // Build the voice request, select the language code ("en-US"),
            // and the SSML voice gender ("neutral").

            Random r = new Random();
            int randName = r.Next(voicelist.Count);
            if  (user.Voice.ToLower() == "random-per-user")
            {
                user.Voice = voicelist[randName];
                voicename = voicelist[randName];
            }
            else if (user.Voice.ToLower() == "random")
            {

                voicename = voicelist[randName];
            }
            else if (voicelist.Any(n => n.ToLower() == user.Voice.ToLower()))
            {
                voicename = user.Voice;
            }
            else
            {
                voicename = "en-AU-Standard-B";
            }

            string[] voiceParts = voicename.Split('-');
            languageCode = ($"{voiceParts[0]}-{voiceParts[1]}");


            VoiceSelectionParams voice = new VoiceSelectionParams
            { 
                Name = voicename,
                LanguageCode = languageCode
            };

            // Select the type of audio file you want returned.
            AudioConfig config = new AudioConfig
            {
                //AudioEncoding = AudioEncoding.Mp3
                AudioEncoding = AudioEncoding.Linear16
            };

            // Perform the Text-to-Speech request, passing the text input
            // with the selected voice parameters and audio file type
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });

            /*using (Stream output = File.Create("sample.mp3"))
            {
                response.AudioContent.WriteTo(output);
                Console.WriteLine($"Audio content written to file 'sample.mp3'");
            }*/


            //https://ryanclouser.com/csharp/2017/11/09/C-AWS-Polly-Speech-Synthesizer.html

            using (var ms = new MemoryStream())
            {
                response.AudioContent.WriteTo(ms);
                byte[] buf = ms.GetBuffer();
                var source = new BufferedWaveProvider(new WaveFormat(24000, 16, 1));
                //var source = new BufferedWaveProvider(new WaveFormat(44100, 1, 0));

                TimeSpan timespan = TimeSpan.FromSeconds(20);
                source.ReadFully = false;
                source.BufferDuration = timespan;
                try { 
                source.AddSamples(buf, 0, buf.Length);
                }
                catch (Exception e)
                {}
                using (WaveOutEvent waveOut = new WaveOutEvent())
                {
                    waveOut.Init(source);

                    AutoResetEvent stopped = new AutoResetEvent(false);
                    waveOut.PlaybackStopped += (object sender, StoppedEventArgs e) => { stopped.Set(); };
                    waveOut.Play();
                    stopped.WaitOne();
                }

            }

        }
        
    }

}
