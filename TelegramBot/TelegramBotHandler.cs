using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace AutoAccept_CSGO4.TelegramBot
{
    public class TelegramBotHandler
    {
        private TelegramBotClient botClient;
        private CancellationToken cancellationToken = new CancellationToken();
        private List<long> connectedChatIDs;
        public bool isInitialized;

        public async Task InitializeTelegramBot()
        {
            try
            {
                botClient = new TelegramBotClient(GetTelegramToken());
                var updates = await botClient.GetUpdatesAsync(cancellationToken: cancellationToken);
                connectedChatIDs = updates
                    .Select(update => update?.Message?.Chat.Id)
                    .Where(chatId => chatId.HasValue)
                    .Select(chatId => chatId.Value)
                    .Distinct()
                    .ToList();
                isInitialized = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Error initializing Telegram Bot, Check if token is correct and you have internet connection. " +
                    "If you have introduced an incorrect token, delete the token using the option 3 in the main menu and try again.");
                isInitialized = false;
                throw;
            }
        }


        public async Task SendMatchFoundToAllClients()
        {
            if (connectedChatIDs != null && connectedChatIDs.Count > 0)
            {
                foreach (var chatId in connectedChatIDs)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "Your csgo match is ready\\!",
                        parseMode: ParseMode.MarkdownV2,
                        disableNotification: false,
                        cancellationToken: cancellationToken
                    );
                }
            }
        }

        public static String GetTelegramToken()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonFilePath = Path.Combine(exeDirectory, "csgo-auto-accept-t-bot-token.json");

            if (File.Exists(jsonFilePath))
            {
                string jsonContent = File.ReadAllText(jsonFilePath);


                JObject jsonObject = JObject.Parse(jsonContent);
                JToken value;
                if (jsonObject.TryGetValue("token", out value))
                {
                    string tokenValue = value.ToString();
                    return tokenValue;
                }
            }

            return null;
        }

        public static void SetTelegramToken(String token)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonFilePath = Path.Combine(exeDirectory, "csgo-auto-accept-t-bot-token.json");

            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }

            File.Create(jsonFilePath).Dispose();

            JObject jsonObject = new JObject();
            jsonObject.Add("token", token);
            string jsonContent = jsonObject.ToString();
            File.WriteAllText(jsonFilePath, jsonContent);
        }

        public static void DeleteTelegramToken()
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string jsonFilePath = Path.Combine(exeDirectory, "csgo-auto-accept-t-bot-token.json");

            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }
        }

        public static bool HasTelegramToken()
        {
            return GetTelegramToken() != null;
        }
    }
}