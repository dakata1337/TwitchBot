using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Twitch
{
    class Program
    {
        static TwitchClient client;
        static void Main(string[] args)
        {
            LoggingService.Load();
            
            ConnectionCredentials credentials = new ConnectionCredentials("xdenkata_", File.ReadAllText(@"dev/token.txt"));
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
                
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, "xdenkata_");

            client.OnMessageReceived += Client_OnMessageReceived;

            client.Connect();
            Thread.Sleep(-1);
        }

        private static void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //If message Author is the bot
            if(e.ChatMessage.IsMe)
                return;

            string prefix = "!";
            //If the message doesn't start with the prefix return
            if(!e.ChatMessage.Message.StartsWith(prefix))
                return;

            //Remove prefix
            string message = e.ChatMessage.Message.Remove(0, prefix.Length).Trim();
            
            //Get Space Index (if none set spaceIndex to -1)
            int spaceIndex = message.IndexOf(" ");

            //If spaceIndex == -1 (no space) use whole message as command 
            //else get everything before the first space
            string command = spaceIndex == -1 ? message : message.Remove(spaceIndex, message.Length - spaceIndex).Trim();

            //If spaceIndex == -1 (no space) set the string to empty
            //else get everything after the first space
            string param = spaceIndex == -1 ? string.Empty : message.Remove(0, spaceIndex).Trim();

            var result = RunCommand(e.ChatMessage, command, param);

            if(result.isSuccessful)
                return;

            LoggingService.Log($"{e.ChatMessage.Username} => {e.ChatMessage.Message} => {result.exception.StackTrace}", "CommandHandler", ConsoleColor.Red, result.exception);
        }

        public static Result RunCommand(ChatMessage messageContext, string command, string param)
        {
            try
            {
                switch (command)
                {
                    case "os":
                        client.SendMessage(messageContext.Channel, $"I'm running on {Environment.OSVersion}");
                        break;
                    default:
                        return new Result { isSuccessful = false };
                }
                return new Result { isSuccessful = true };
            } catch (Exception e)
            {
                return new Result { isSuccessful = false, exception = e };
            }
        }

    }
}

