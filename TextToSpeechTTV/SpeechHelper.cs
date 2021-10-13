using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using Google.Cloud.TextToSpeech.V1;
using System.IO;
using NAudio.Wave;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace TextToSpeechTTV {
    class SpeechHelper {
        private int rate;
        private bool randomVoice;
        private string ttsName;
        private double googleSpeakingRate;
        private double googlePitch;
        private SpeechSynthesizer speechSynthesizer;
        private TextToSpeechClient client;
        private List<string> voiceList;
        private List<string> voiceListCulture;

        private WaveOutEvent waveOut;
        //private List<string> voicelistWavenet;
        //private List<string> voicelistStandard;

        public SpeechHelper (string ttsName, int rate) {
            this.rate = rate;
            this.ttsName = ttsName;

            try {
                CreateSpeechSynthesizer();
                Config config = new Config();
                string gcpType = config.GetGCP();
                try {
                    if (!randomVoice) {
                        googleSpeakingRate = config.GetSpeakingRate();
                        googlePitch = config.GetSpeakingPitch();
                    }
                } catch (ArgumentException e) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.WriteLine("Press any Key to continue...");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                voiceList = config.voicelist;
                if (voiceList == null) {
                    voiceList = new List<string>();
                }

                if (gcpType != "false") {
                    client = config.GetGCPClient();
                    if (gcpType == "standard") {
                        voiceList = voiceList.FindAll(delegate (string s) { return s.Contains("Standard"); });
                    } else if (gcpType == "wavenet") {
                        voiceList = voiceList.FindAll(delegate (string s) { return s.Contains("Wavenet"); });
                    }
                }

                Console.WriteLine("Voicelist.txt is available in Config Folder.");

                //Returns: en-EN, we only want "en"
                string cultureFirst = CultureInfo.CurrentCulture.Name.Split('-')[0];
                voiceListCulture = new List<string>();

                foreach (string voiceListItem in voiceList) {
                    if (voiceListItem.StartsWith(cultureFirst)) {
                        voiceListCulture.Add(voiceListItem);
                    }
                }

            } catch {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("TTS Name does not exist. TTS will NOT work.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private void CreateSpeechSynthesizer () {
            speechSynthesizer = new SpeechSynthesizer {
                Rate = rate
            };
            speechSynthesizer.SetOutputToDefaultAudioDevice();
            speechSynthesizer.SelectVoice(ttsName);
        }

        public void StopReading () {
            //Reinstate speechSynthesizer
            speechSynthesizer.Dispose();
            CreateSpeechSynthesizer();
            waveOut.Stop();
            waveOut.Dispose();
        }

        public static List<string> GetAllInstalledVoices () {
            List<string> list = new List<string>();
            using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer()) {
                foreach (InstalledVoice installedVoice in speechSynthesizer.GetInstalledVoices()) {
                    VoiceInfo voiceInfo = installedVoice.VoiceInfo;
                    list.Add(voiceInfo.Name);
                }
            }

            return list;
        }

        public void Speak (User user, string text) {
            Prompt prompt = new Prompt($"{user.Nick} {text}");
            speechSynthesizer.Speak(prompt);
        }

        public void Speak_gcp (User user, string text, bool readOut) {
            string voiceName;
            var userData = user;
            if (!readOut) {
                user.Name = "";
                user.Nick = "";
            }

            //Fallback on Microsoft
            if (voiceList?.Any() != true || user.Voice.ToLower() == "microsoft") {
                Speak(user, text);
                return;
            }

            // Set the text input to be synthesized.
            SynthesisInput input = new SynthesisInput {
                Text = $"{user.Nick} {text}"
            };

            // Build the voice request, select the language code ("en-US"),
            // and the SSML voice gender ("neutral").

                Random r = new Random();
            int randName = r.Next(voiceList.Count);
            // Boundaries are -20, 20, but makes it sound awful.
            int randPitch = r.Next(-10, 10);
            switch (userData.Voice.ToLower()) {
                case "random":
                    voiceName = voiceList[randName];
                    randomVoice = true;
                    googleSpeakingRate = 1;
                    googlePitch = randPitch;
                    break;
                case "random-pc-language":
                    randomVoice = true;
                    int randNameCulture = r.Next(voiceListCulture.Count);
                    voiceName = voiceListCulture[randNameCulture];
                    googleSpeakingRate = 1;
                    googlePitch = randPitch;
                    break;
                default: {
                        if (voiceList.Any(n => n.ToLower() == user.Voice.ToLower())) {
                            voiceName = userData.Voice;
                        } else {
                            voiceName = "en-AU-Standard-B";
                        }
                        randomVoice = false;
                        break;
                    }
            }


            string[] voiceParts = voiceName.Split('-');
            var languageCode = ($"{voiceParts[0]}-{voiceParts[1]}");


            VoiceSelectionParams voice = new VoiceSelectionParams {
                Name = voiceName,
                LanguageCode = languageCode
            };

            AudioConfig config = new AudioConfig {
                AudioEncoding = AudioEncoding.Linear16,
                SpeakingRate = googleSpeakingRate,
                Pitch = googlePitch
            };

            //WIP
            var task = Task.Factory.StartNew(() => CreateResponse(input, voice, config));
            var isCompletedSuccessfully = task.Wait(TimeSpan.FromMilliseconds(5000));

            if (!isCompletedSuccessfully) {
                throw new TimeoutException("Can't read this piece of text! Exceeded Timeout limit! (5s)");
            }
            SynthesizeSpeechResponse response = task.Result;

            using (var ms = new MemoryStream()) {
                response.AudioContent.WriteTo(ms);
                byte[] buf = ms.GetBuffer();
                var source = new BufferedWaveProvider(new WaveFormat(24000, 16, 1)) {
                    ReadFully = false,
                    BufferLength = buf.Length
                };
                try {
                    source.AddSamples(buf, 0, buf.Length);
                } catch (Exception) {
                    Console.WriteLine("Can't read the given TTS input!");
                    return;
                }

                waveOut = new WaveOutEvent();
                waveOut.Init(source);

                AutoResetEvent stopped = new AutoResetEvent(false);
                waveOut.PlaybackStopped += (sender, e) => {
                    stopped.Set();
                    waveOut.Dispose();
                };
                waveOut.Play();
                stopped.WaitOne();
            }
        }

        private SynthesizeSpeechResponse CreateResponse (SynthesisInput input, VoiceSelectionParams voice, AudioConfig config) {
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });
            return response;
        }
    }
}
