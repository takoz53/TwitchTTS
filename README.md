# TwitchTTS
A TTS reading out Twitch chat loud with multiple settings you'll be able to change :)


## Download TwitchTTS
Here you go (coming soon!)

## Donate via Paypal
I'd really appreciate Donations, so if you're extra-nice, you might want to give a donation to your hearts extent! Thank you!

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=XAJJTAVXE4P8C)


## How to set up as developer?
You'll need the [DLL](https://puu.sh/BNfss/1c444104df.dll) for the SpeechHelper for TTS and add it as Reference in VS.
You'll also need TwitchLib available on NuGet and you're good to go.

## How to set up as user? (Windows Only!)
1. Download the Program and extract
1. Go into Config and open up creds.txt
    1. Set up your Bot-Account ID and enter your ID below "ID:"(Just create a Twitch Account, **all lowercase!**)
    1. Create an [OAuth Key](https://twitchapps.com/tmi/) and enter your Key below "Oauth:"
    1. Set the channel, where it should connect to (**all lowercase!**)
1. Open up the .exe and check what voices your PC Supports (It'll be shown inside the Console)
    1. Copy a voice its name **(Care lower & uppercase)** and put it in your Config it's creds.txt its "voice" and set your locale like en-US
1. Set Max-Letters Length, which should be Displayed (After for example 40 Letters, the bot will say "to be continued")
    1. To read out everything until the end, leave the Setting at 0.
1. Run the program, type something in your Chat and it should be working.

### What's next?
1. Keep the program running while streaming and everyone will be able to hear the chat.
1. Use badwords.txt to create your own wordfilter, tip: There are many out in the internet, so maybe you want to download some!
1. Use blocklist.txt to block users (or bots) from TTS
    1. **Example:** Nightbot answers to uptime? Block him or time will be read out always
    1. Someone is being a bitch and abusing the Bot? Block him. This can be done while the Bot is running.
1. Use usernames.txt to give usernames other nicknames
    1. **Example:** Instead of takoz53, say "taco"
    1. This can be done while the Bot is running.

## FAQ

#### How do Install other voices?
You can just install for example Microsoft voices from your Region Settings by adding other Regions Voices
>![Here, installed is: English Voice, Japanese Voice and German Voice](https://puu.sh/BNfZS/4dacefdbff.png)

#### Noooo! I want other Voices besides Microsoft Voices

Well then, what you can do is for example download and install the IVANA Voices, which supports many Voices popular Streamers use.
What I really like is Justin and Ivy :P

#### The Bot doesn't say anything, but it says that it connected in Chat, what now?!

Then you fucked up at creds.txt, by probably typing in the Voicename wrongly, check over it!

#### I'm not getting a message that the Bot connected

Then I'd recommend you checking if your Twitch ID is lowercase, your channel is lowercase and the oauth key is right.


## That's it!
If something is to be added to the Readme, please create an Issue, describing what is needed! Thank you.
Also, I apologize for bad code. Sorry! :D
