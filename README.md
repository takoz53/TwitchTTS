# TwitchTTS with Channel Awards
A TTS client for Twitch chat, integrated with GCP Text-to-speech (500 voices from 40+ languages).

### Updated on 16.08.2021
## Download at [Releases](https://github.com/takoz53/TwitchTTS/releases)
## How to set up as developer?
You'll (maybe?) need to add System.Speech available on your System as Reference

You'll also need TwitchLib and Google.Cloud.TextToSpeech.V1 available on NuGet and you're good to go.


## How to set up as user? (Windows Only!)
1. Download the latest release and extract
1. Run the Program and you'll have to fill out multiple things
    1. Firstly, whether you want to bind it on a specific Channel Award. If yes, type Y, else N.
        1. If Y, type the Channel Award its Title, e.g. "TTS" without the "".
    1. If you want the TTS to read out usernames (e.g. X said ...), hit Y, else N.
    2. Enter your botname / username
    4. Create an [OAuth Key](https://twitchapps.com/tmi/) and enter your Key (oauth:xxxxxx..)
    5. Set the channel, where it should connect to (**all lowercase!**)
    6. Create an [Access Token](https://twitchtokengenerator.com/) with Custom Scope Token. Tick following:
        1. chat:read
        2. chat:edit
        3. channel:read:redemptions
        4. Hit GENERATE TOKEN.
    7. Insert your Access Token.
    8. Get your [Channel ID](https://www.streamweasels.com/support/convert-twitch-username-to-user-id/)
    9. Insert your Twitch/Channel ID.
1. Everything should be working now.
2. Don't forget to set up a bad words filter or you might get banned if somebody wants to be funny.
3. I don't take responsibility for whatever happens in your stream.

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

##Feel free to [donate](https://streamelements.com/takoz5334/tip)
If you liked the project and want to give me a bit of support, you can drop a few pennies here c:

