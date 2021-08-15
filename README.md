# TwitchTTS
A TTS client for Twitch chat, integrated with GCP Text-to-speech (500 voices from 40+ languages). 

### Updated on 23.08.2020
## Download [Here](https://mega.nz/file/6x9wWQhI#CqcPYCyqCbKkeZqFY4LHorvKMlNO5fVMdFqFhhquRNg) or at Releases
## How to set up as developer?
You'll (maybe?) need to add System.Speech available on your System as Reference

You'll also need TwitchLib and Google.Cloud.TextToSpeech.V1 available on NuGet and you're good to go.


## How to set up as user? (Windows Only!)
1. Download the latest release and extract
1. Run once to generate a Config folder
1. Go into Config and open up creds.txt
    1. Set up your Bot-Account ID and enter your ID below "ID:"(Just create a Twitch Account, **all lowercase!**)
    1. Create an [OAuth Key](https://twitchapps.com/tmi/) and enter your Key below "Oauth:"
    1. Set the channel, where it should connect to (**all lowercase!**)
1. Open up the .exe and check what voices your PC Supports (It'll be shown inside the Console)
    1. Copy a voice its name **(Care lower & uppercase)** and put it in your Config it's options.txt its "voice"
1. Set Max-Letters Length, which should be Displayed (After for example 40 Letters, the bot will say "to be continued")
    1. To read out everything until the end, leave the Setting at 0.
1. Run the program, type something in your Chat and it should be working.

## How to set up with GCP Text-To-Speech
In order for the bot to be able to access the google API, a valid service account key with access to the text-to-speech API must be available in the gcp.json file in the config folder.

1. Register for a free account at https://cloud.google.com/free (1 million wavenet characters per month are free, and 4 million free standard characters)
Try out the TTS here: https://cloud.google.com/text-to-speech

I'd recommend setting up a budget of $0 just in case.

### Create credentials

1. In the menu on the left: APIs & Services -> Select login data
1. Create a service account key with the button 'Create login data'.
1. Enter service account name, select role project-> owner, key type: JSON
1. Save the file as gcp.json

### Activate the text-to-speech API

1. In the menu on the left: APIs & Services -> Select Dashboard
1. Activate the Cloud Text-to-Speech API with the button '+ APIS and services'

### What's next?
1. Keep the program running while streaming and everyone will be able to hear the chat.
1. Use badwords.txt to create your own wordfilter, tip: There are many out in the internet, so maybe you want to download some!
1. Use blocklist.txt to block users (or bots) from TTS
    1. **Example:** Nightbot answers to uptime? Block him or time will be read out always
    1. Someone is being a bitch and abusing the Bot? Block him. This can be done while the Bot is running.
1. Use usernames.json to give usernames other nicknames
    1. **Example:** Instead of takoz53, say "taco"
    1. This can be done while the Bot is running.
1. The app should fall back on local Microsoft TTS if connectivity to GCP fails for whatever reason.
## FAQ

#### The Bot doesn't say anything, but it says that it connected in Chat, what now?!

Could be a number of things. If chat is not appearing in the console, it could be an issue with creds.txt. If gcp voices aren't working, it could be an issue with gcp.json, or maybe your account is out of requests (haven't tested this).

#### I'm not getting a message that the Bot connected

Then I'd recommend you checking if your Twitch ID is lowercase, your channel is lowercase and the oauth key is right.

### What can I customize?

1. Maximum allowed characters
1. Sentence, if maximum allowed characters are **exceeded**
1. Message connector
    1. **Example:** "takoz5334 said hello" can be changed to "takoz5334 speaks hello" or into any other language etc.
1. Swearword replacing word, default is "beep" when something bad is written.
1. The TTS Voice - 
    1. Set GCP to True or False to enable/disable GCP voices
    1. Default Voice Options (For options.txt 'Default' or usernames.json 'voice' attribute per user)
        - "Microsoft" - Uses local Microsoft TTS
        - "Random" - Uses Random GCP Voice for all messages for all users not defined by usernames.json
        - "Random-per-user" - Selects and stores a Random GCP Voice per user while the app is open
        - Anything from voicelist.txt

### How do I block users?

Just write their name down in the blocklist, press enter and write another name in. Simple? Yes.

### How do I give users nicknames

Just like in blocklist, go to usernames and assign each user a name, e.g. takoz53=taco xxswordmasterxx=swordmaster.
Just note down, that the names **have to** be written in lowercase.

### Can I block and unblock users from chat?
Yeah, definitely. You can by typing !block username and !unblock username. It'll do the checks whether the user is blocked and input is correct, so don't worry about typing something wrong.

