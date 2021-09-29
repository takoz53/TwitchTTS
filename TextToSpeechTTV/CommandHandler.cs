using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextToSpeechTTV {
    class CommandHandler {
        public CommandHandler () {

        }

        public bool BlockUser (string message) {
            SpeechWordHandler speechWordHandler = new SpeechWordHandler();
            string[] messages;

            //First try splitting the message to check whether there is an username available
            try { messages = message.Split(' '); } catch { return false; }

            if (messages.Length > 2)
                return false;

            string user = messages[1];
            if (speechWordHandler.CheckUserBlocked(user))
                return false;

            File.AppendAllLines(speechWordHandler.GetBlockListLocation(), new string[] { "\n", user });
            return true;
        }

        public bool UnblockUser (string message) {
            SpeechWordHandler speechWordHandler = new SpeechWordHandler();
            string[] messages;

            //First try splitting the message to check whether there is an username available
            try { messages = message.Split(' '); } catch { return false; }
            if (messages.Length > 2)
                return false;

            string user = messages[1];
            //If user is not in blocked list, return.
            if (!speechWordHandler.CheckUserBlocked(user))
                return false;
            var lines = File.ReadAllLines(speechWordHandler.GetBlockListLocation());

            //Remove user
            for (int i = 0; i < lines.Length; i++) {
                if (lines[i] == user) {
                    lines[i] = lines[i].Replace(user, "");
                    break;
                }
            }
            //firstly, remove WhiteSpace and then replace old file with new file
            var newLines = lines.Where(arg => !string.IsNullOrWhiteSpace(arg));
            File.WriteAllLines(speechWordHandler.GetBlockListLocation(), newLines);
            return true;
        }
    }
}
