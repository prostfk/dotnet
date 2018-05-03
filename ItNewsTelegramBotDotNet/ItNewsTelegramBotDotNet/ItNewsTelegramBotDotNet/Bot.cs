using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace ItNewsTelegramBotDotNet
{
    public class Bot
    {
        static TelegramBotClient bot = new TelegramBotClient("581433051:AAHyiVPjjY99jYWF_zOiCLqrxuQy5FT0p_U");

        public static async Task TestApiAsync()
        {
            bot.OnMessage += Bot_OnMessage;
//            bot.SetWebhookAsync();
            var me = bot.GetMeAsync().Result;
            Console.Title = me.Username;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            Message msg = e.Message;
            if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
//                await bot.SendTextMessageAsync(msg.Chat.Id, "checking dotnet bot, " + msg.From.FirstName);
                switch (msg.Text)
                {
                    case "/news":
                        SendMessage(msg, Db("SELECT content FROM Articles WHERE id=(SELECT MAX(id) FROM Articles)"));
                        break;
                    default:
                        SendMessage(msg, "No command");
                        break;
                }
            }
        }

        private static string Db(String sqlCode)
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=ItNews;password=0;SslMode=none;");
            connection.Open();
            MySqlCommand command = new MySqlCommand(sqlCode, connection);
            var result = command.ExecuteScalar().ToString();
            connection.Close();
            return result;
        }

        private static async void SendMessage(Message msg, string text)
        {
            await bot.SendTextMessageAsync(msg.Chat.Id, text);
        }
    }
}