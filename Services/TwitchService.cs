using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Twitch.DataStrucs;
using Twitch.Modules;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Twitch.Services
{
    class TwitchService
    {
        private TwitchClient _client;
        private Commands _commands;

        public TwitchService(IServiceProvider serviceProvider)
        {
            _commands = serviceProvider.GetRequiredService<Commands>();
        }

        public void Initialize()
        {
            ConnectionCredentials credentials = new("xdenkata_", File.ReadAllText(@"dev/token.txt"));
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };

            WebSocketClient customClient = new(clientOptions);
            _client = new TwitchClient(customClient);
            SubscribeEvents();
            _client.Initialize(credentials, "xdenkata_");
            _client.Connect();

            if (_client.IsConnected)
                LoggingService.Log("Connected", "Twitch");
        }

        private void SubscribeEvents()
        {
            _client.OnMessageReceived += Client_OnMessageReceived;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //If message Author is the bot
            if (e.ChatMessage.IsMe)
                return;

            string prefix = "!";
            //If the message doesn't start with the prefix return
            if (!e.ChatMessage.Message.StartsWith(prefix))
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

            //Execute Command
            var result = ExecuteCommand(e.ChatMessage, command, param);

            //If task was successful OR the reason is CommandNotFound
            if (result.isSuccessful || result.reason == Reason.CommandNotFound)
                return;

            //Log Exception
            LoggingService.Log($"{e.ChatMessage.Username} => {e.ChatMessage.Message} => {result.exception.StackTrace}", "CommandHandler", ConsoleColor.Red, result.exception);
        }

        public CommandExecutionResult ExecuteCommand(ChatMessage messageContext, string command, string param)
        {
            try
            {
                switch (command.ToLower())
                {
                    // For now commands can be added here in cases
                    case "os":
                        Reply(messageContext.Channel, _commands.GetOS());
                        break;
                    default:
                        return new CommandExecutionResult { isSuccessful = false, reason = Reason.CommandNotFound };
                }
                return new CommandExecutionResult { isSuccessful = true };
            }
            catch (Exception e)
            {
                //Return Exception
                return new CommandExecutionResult { isSuccessful = false, exception = e, reason = Reason.ExecutionError };
            }
        }

        /// <summary>
        /// Sends message to specified channel
        /// </summary>
        /// <param name="channel">The channel in which the message will be sent</param>
        /// <param name="message">The message</param>
        private void Reply(string channel, string message)
            => _client.SendMessage(channel, message);
    }
}
