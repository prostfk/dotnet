using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

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
                switch (msg.Text)
                {
                    
//                    case "/count"
                    case "/randomNews":
                        int max = Int32.Parse(Db("SELECT id FROM Articles WHERE id=(SELECT MAX(id) FROM Articles)"));
                        int id = new Random().Next(1, max + 1);
                        SendMessage(msg, Db("SELECT title FROM Articles WHERE id=" + id));
                        SendMessage(msg, Db("SELECT content FROM Articles WHERE id=" + id));
                        SendPicture(msg, Db("SELECT pathToFile FROM Articles WHERE id=" + id));
                        break;
                    case "/news":
                        SendMessage(msg, Db("SELECT title FROM Articles WHERE id=(SELECT MAX(id) FROM Articles)"));
                        SendMessage(msg, Db("SELECT content FROM Articles WHERE id=(SELECT MAX(id) FROM Articles)"));
                        SendPicture(msg, Db("SELECT pathToFile FROM Articles WHERE id=(SELECT MAX(id) FROM Articles)"));
                        break;
                    case "/pic":
                        string filename = Db("SELECT pathToFile FROM Articles ORDER BY RAND() LIMIT 1;");
                        SendPicture(msg, filename);
                        break;
                    default:
                        SendMessage(msg, "No command");
                        break;
                }
            }
        }

        private static string Db(String sqlCode)
        {
            MySqlConnection connection =
                new MySqlConnection("server=localhost;user=root;database=ItNews;password=0;SslMode=none;");
            connection.Open();
            MySqlCommand command = new MySqlCommand(sqlCode, connection);
            var result = command.ExecuteScalar().ToString();
            connection.Close();
            return result;
        }

        private static async void SendPicture(Message msg, string path)
        {
            string systemPath =
                "/home/prostrmk/Documents/Programs/Java/Java EE/Spring/SpringTutorials/ItNews/src/main/webapp/";
            await bot.SendPhotoAsync(msg.Chat.Id,
                new InputOnlineFile(new FileStream(systemPath + path, FileMode.Open), path));
        }

        private static async void SendMessage(Message msg, string text)
        {
            try
            {
                await bot.SendTextMessageAsync(msg.Chat.Id, text);
            }
            catch (Exception e)
            {
                SendMessage(msg, e.ToString());
            }
            
        }
    }
}