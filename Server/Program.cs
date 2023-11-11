using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Server.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types;


namespace TelegramBotExperiments
{

    class Program
    {
        private static InlineKeyboardButton[][] buttonsRooms = null;
        private static InlineKeyboardButton[][] buttonsLocation = null;
        private static InlineKeyboardButton[][] buttonsPrice = null;
        private static IQueryable<Advertisement> query = null;
        private static List<int> roomsFilter =new ();
        private static List<string> locationFilter = new();
        private static List<int> priceFilter = new();
        private static InlineKeyboardMarkup createButtonsRooms()
        {
            var button1 = InlineKeyboardButton.WithCallbackData("👉 \u0031\u20E3", "rooms1");
            var button2 = InlineKeyboardButton.WithCallbackData("👉 \u0032\u20E3", "rooms2");
            var button3 = InlineKeyboardButton.WithCallbackData("👉 \u0033\u20E3", "rooms3");
            var button4 = InlineKeyboardButton.WithCallbackData("👉 \u0034\u20E3 +", "rooms4");

            var buttons = new InlineKeyboardButton[] { };
            buttons = buttons.Concat(new InlineKeyboardButton[] { button1, button2, button3, button4 }).ToArray();
            buttonsRooms = buttons
                .Select((button, index) => new { Button = button, Index = index })
                .GroupBy(item => item.Index / 1)
                .Select(group => group.Select(item => item.Button).ToArray())
                .ToArray();
            return new InlineKeyboardMarkup(buttonsRooms);
        }
        private static InlineKeyboardMarkup createButtonsLocation()
        {
            var buttons = new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData("👉 Лук'янівка", "rblocation1"),
                InlineKeyboardButton.WithCallbackData("👉 Поділ/Воздвиженка", "rblocation2"),
                InlineKeyboardButton.WithCallbackData("👉 Центр", "rblocation3"),
                InlineKeyboardButton.WithCallbackData("👉 Печерськ", "rblocation4"),
                InlineKeyboardButton.WithCallbackData("👉 Сирець/Дорогожичі", "rblocation5"),
                InlineKeyboardButton.WithCallbackData("👉 Шулявка/КПІ", "rblocation6"),
                InlineKeyboardButton.WithCallbackData("👉 Відрадний/Караваєві дачі", "rblocation7"),
                InlineKeyboardButton.WithCallbackData("👉 Нивки", "rblocation8"),
                InlineKeyboardButton.WithCallbackData("👉 Солом'янка/Чоколівка", "rblocation9"),
                InlineKeyboardButton.WithCallbackData("👉 Голосієво/Деміївка", "rblocation10"),
                InlineKeyboardButton.WithCallbackData("👉 Столичне шосе", "rblocation11"),
                InlineKeyboardButton.WithCallbackData("👉 Теремки/ВДНГ", "rblocation12"),
                InlineKeyboardButton.WithCallbackData("👉 Борщагівка", "rblocation13"),
                InlineKeyboardButton.WithCallbackData("👉 Академмістечко", "rblocation14"),
                InlineKeyboardButton.WithCallbackData("👉 Куренівка/Пріорка", "rblocation15"),
                InlineKeyboardButton.WithCallbackData("👉 Виноградар/Мінський Масив", "rblocation16"),
                InlineKeyboardButton.WithCallbackData("👉 Оболонь", "rblocation17"),
                InlineKeyboardButton.WithCallbackData("👉 Троєщина", "lblocation18"),
                InlineKeyboardButton.WithCallbackData("👉 Ліс.Масив/Воскресенка", "lblocation19"),
                InlineKeyboardButton.WithCallbackData("👉 Дарниця/Русанівка", "lblocation20"),
                InlineKeyboardButton.WithCallbackData("👉 СоцМісто/Березняки", "lblocation21"),
                InlineKeyboardButton.WithCallbackData("👉 Позняки/Осокорки", "lblocation22"),
            };

            var buttonsPerRow = 2; // Кількість кнопок у кожному рядку
            buttonsLocation = buttons
                .Select((button, index) => new { Button = button, Index = index })
                .GroupBy(item => item.Index / buttonsPerRow)
                .Select(group => group.Select(item => item.Button).ToArray())
                .ToArray();
            var newRow1 = new InlineKeyboardButton[1];
            newRow1[0] = InlineKeyboardButton.WithCallbackData("👉 Харківське шосе", "lblocation23");
            var newRow2 = new InlineKeyboardButton[1];
            newRow2[0] = InlineKeyboardButton.WithCallbackData("Обрати все", "SelectAll");
            var newRow5 = new InlineKeyboardButton[1];
            newRow5[0] = InlineKeyboardButton.WithCallbackData("Обнулити вибір 🔄", "DeleteAll");
            var newRow3 = new InlineKeyboardButton[1];
            newRow3[0] = InlineKeyboardButton.WithCallbackData("🚣 За берегом Дніпра 👇");
            var newRow4 = new InlineKeyboardButton[2];
            newRow4[0] = InlineKeyboardButton.WithCallbackData("◀ Правий Берег", "right_bank");
            newRow4[1] = InlineKeyboardButton.WithCallbackData("Лівий Берег ▶", "left_bank");
            buttonsLocation = buttonsLocation.Concat(new[] { newRow1, newRow2,newRow5,newRow3,newRow4 }).ToArray();
            return new InlineKeyboardMarkup(buttonsLocation);
        }
        private static InlineKeyboardMarkup createButtonsPrice()
        {
            var button1 = InlineKeyboardButton.WithCallbackData("👉 До 13 тис.", "price12999");
            var button2 = InlineKeyboardButton.WithCallbackData("👉 13-16 тис.", "price16000");
            var button3 = InlineKeyboardButton.WithCallbackData("👉 16-20 тис.", "price20000");
            var button4 = InlineKeyboardButton.WithCallbackData("👉 20-25 тис.", "price25000");
            var button5 = InlineKeyboardButton.WithCallbackData("👉 25-30 тис.", "price30000");
            var button6 = InlineKeyboardButton.WithCallbackData("👉 30-35 тис.", "price35000");
            var button7 = InlineKeyboardButton.WithCallbackData("👉 35-45 тис.", "price45000");
            var button8 = InlineKeyboardButton.WithCallbackData("👉 45-60 тис.", "price59999");


            var buttons = new InlineKeyboardButton[] { };
            buttons = buttons.Concat(new InlineKeyboardButton[] { button1, button2, button3, button4, button5, button6, button7, button8 }).ToArray();
            buttonsPrice = buttons
                .Select((button, index) => new { Button = button, Index = index })
                .GroupBy(item => item.Index / 2)
                .Select(group => group.Select(item => item.Button).ToArray())
                .ToArray();
            var newRow1 = new InlineKeyboardButton[1];
            newRow1[0] = InlineKeyboardButton.WithCallbackData("👉 60 та більше", "price60000");
            buttonsPrice = buttonsPrice.Concat(new[] { newRow1}).ToArray();
            return new InlineKeyboardMarkup(buttonsPrice);
        }
        static ITelegramBotClient bot = new TelegramBotClient("6452887240:AAGj_JGsh1y83gTSwbo26-FQNo_kRiuTfyg");
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var keyBoardStart = new InlineKeyboardMarkup(new[]
                    {
                         new []
                         {
                         InlineKeyboardButton.WithCallbackData("Орендувати квартиру 🏠"),
                         },
                         new []
                         {
                             InlineKeyboardButton.WithCallbackData("Купити квартиру 💵"),
                         },
                         new []
                         {
                             InlineKeyboardButton.WithCallbackData("Здати/Продати квартиру 💸"),
                         }
                    });

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                var name = update.Message.From.FirstName;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, $"{update.Message.From.FirstName}, ти хочеш ...", replyMarkup: keyBoardStart);
                    resetFields();
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Я не розумію вашого повідомлення.");
                }
            }
            else if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                var callbackData = update.CallbackQuery.Data;
                var chatId = update.CallbackQuery.Message.Chat;
                var message = update.CallbackQuery.Message;
                if (callbackData == "Орендувати квартиру 🏠")
                {
                    await botClient.EditMessageTextAsync(chatId, message.MessageId, $"Скільки повинно бути кімнат в квартирі?", replyMarkup: createButtonsRooms());
                }
                else if (callbackData == "Купити квартиру 💵")
                {
                    await botClient.SendTextMessageAsync(chatId, "Ви вибрали опцію купівлі квартири.");
                }
                else if (callbackData == "Здати/Продати квартиру 💸")
                {
                    await botClient.SendTextMessageAsync(chatId, "Ви вибрали опцію здачі/продажу квартири.");
                }
                else if (callbackData.StartsWith("rooms"))
                {
                    if (buttonsRooms != null)
                    {
                        var roomNumber = int.Parse(callbackData.Split("rooms")[1]);
                        var button = buttonsRooms[roomNumber - 1][0];
                        if (button.Text.Contains("👉"))
                        {
                            button.Text = button.Text.Replace("👉", "✅");
                        }
                        else
                        {
                            button.Text = button.Text.Replace("✅", "👉");
                        }
                        if(roomsFilter.Contains(roomNumber))
                        {
                            roomsFilter.Remove(roomNumber);
                        }
                        else
                        {
                            roomsFilter.Add(roomNumber);
                        }
                        checkNext("NextToLocation",ref buttonsRooms);
                        await botClient.EditMessageReplyMarkupAsync(chatId, message.MessageId, replyMarkup: new InlineKeyboardMarkup(buttonsRooms));
                    }
                }
                else if (callbackData == "NextToLocation")
                {
                    query = query.Where(item => roomsFilter.Contains(item.RoomCount));
                    await botClient.EditMessageTextAsync(chatId, message.MessageId, "Супер! У яких районах шукаєш квартиру? Можна вибрати декілька варіантів.", replyMarkup: createButtonsLocation());
                }
                else if (callbackData.Contains("location"))
                {
                    if (buttonsLocation != null)
                    {
                        var button = buttonsLocation
                             .SelectMany(row => row)
                             .FirstOrDefault(b => b.CallbackData == callbackData);
                        var btnText= button.Text.Split(" ")[1];
                        if (button.Text.Contains("👉"))
                        {
                            button.Text = button.Text.Replace("👉", "✅");
                        }
                        else
                        {
                            button.Text = button.Text.Replace("✅", "👉");
                        }
                        if (locationFilter.Any(item => item.Equals(btnText)))
                        {
                            locationFilter.Remove(btnText);
                        }
                        else
                        {
                            locationFilter.Add(btnText);
                        }
                        checkNext("NextToPrice", ref buttonsLocation);
                        await botClient.EditMessageReplyMarkupAsync(chatId, message.MessageId, replyMarkup: new InlineKeyboardMarkup(buttonsLocation));
                    }
                }
                else if(callbackData== "SelectAll")
                {
                    if (buttonsLocation != null)
                    {

                        foreach (var row in buttonsLocation)
                        {
                            foreach (var button in row)
                            {
                                button.Text = button.Text.Replace("👉", "✅");
                                button.Text = button.Text.Replace("◀", "✅");
                                button.Text = button.Text.Replace("▶", "✅");
                                var btnText = button.Text.Split(" ")[1];
                                if (btnText != "Обрати" && btnText != "все" && btnText != "Обнулити" && btnText != "вибір" && btnText != "За" && btnText != "берегом" &&
                                    btnText != "Дніпра" && btnText != "Правий" && btnText != "Берег" && btnText != "Лівий" && btnText != "Далі")
                                { 
                                    if (locationFilter.Any(item => item.Equals(btnText)))
                                    {
                                    }
                                    else
                                    {
                                        locationFilter.Add(btnText);
                                    }
                                }
                            }
                        }
                        checkNext("NextToPrice", ref buttonsLocation);
                        await botClient.EditMessageReplyMarkupAsync(chatId, message.MessageId, replyMarkup: new InlineKeyboardMarkup(buttonsLocation));
                    }
                }
                else if(callbackData== "DeleteAll")
                {
                    if (buttonsLocation != null)
                    {
                        foreach (var row in buttonsLocation)
                        {
                            foreach (var button in row)
                            {
                                 button.Text = button.Text.Replace("✅","👉");
                                var btnText = button.Text.Split(" ")[1];
                                if (locationFilter.Any(item => item.Equals(btnText)))
                                {
                                    locationFilter.Remove(btnText);
                                }
                                else
                                {
                                }
                            }
                        }
                        buttonsLocation[15][0].Text = buttonsLocation[15][0].Text.Replace("👉", "◀");
                        buttonsLocation[15][1].Text = buttonsLocation[15][1].Text.Replace("👉", "▶");
                        buttonsLocation = buttonsLocation.Take(buttonsLocation.Length - 1).ToArray();
                        await botClient.EditMessageReplyMarkupAsync(chatId, message.MessageId, replyMarkup: new InlineKeyboardMarkup(buttonsLocation));
                    }
                }
                else if (callbackData == "right_bank")
                {
                    if (buttonsLocation != null)
                    {
                        bool check = true;
                        if (buttonsLocation[15][0].Text.Contains("◀"))
                        {
                            buttonsLocation[15][0].Text = buttonsLocation[15][0].Text.Replace("◀", "✅");
                        }
                        else if (buttonsLocation[15][0].Text.Contains("✅"))
                        {
                            buttonsLocation[15][0].Text = buttonsLocation[15][0].Text.Replace("✅","◀");
                            check = false;
                        }
                        foreach (var row in buttonsLocation)
                        {
                            foreach (var button in row)
                            {
                                if (button.CallbackData.StartsWith("rb"))
                                {
                                    var btnText = button.Text.Split(" ")[1];
                                    if (check)
                                    {
                                        button.Text = button.Text.Replace("👉", "✅");
                                        if (!locationFilter.Any(item => item.Equals(btnText)))
                                        {
                                            locationFilter.Add(btnText);
                                        }
                                    }
                                    else
                                    {
                                        button.Text = button.Text.Replace("✅", "👉");
                                        if (locationFilter.Any(item => item.Equals(btnText)))
                                        {
                                            locationFilter.Remove(btnText);
                                        }
                                    }
                                }
                            }
                        }
                        checkNext("NextToPrice", ref buttonsLocation);
                        await botClient.EditMessageReplyMarkupAsync(chatId, message.MessageId, replyMarkup: new InlineKeyboardMarkup(buttonsLocation));
                    }
                }
                else if (callbackData == "left_bank")
                {
                    if (buttonsLocation != null)
                    {
                        bool check = true;
                        if (buttonsLocation[15][1].Text.Contains("▶"))
                        {
                            buttonsLocation[15][1].Text = buttonsLocation[15][1].Text.Replace("▶", "✅");
                        }
                        else if (buttonsLocation[15][1].Text.Contains("✅"))
                        {
                            buttonsLocation[15][1].Text = buttonsLocation[15][1].Text.Replace("✅", "▶");
                            check = false;
                        }
                        foreach (var row in buttonsLocation)
                        {
                            foreach (var button in row)
                            {
                                if (button.CallbackData.StartsWith("lb"))
                                {
                                    var btnText = button.Text.Split(" ")[1];
                                    if (check)
                                    {
                                        button.Text = button.Text.Replace("👉", "✅");
                                        if (!locationFilter.Any(item => item.Equals(btnText)))
                                        {
                                            locationFilter.Add(btnText);
                                        }
                                    }
                                    else
                                    {
                                        button.Text = button.Text.Replace("✅", "👉");
                                        if (locationFilter.Any(item => item.Equals(btnText)))
                                        {
                                            locationFilter.Remove(btnText);
                                        }
                                    }
                                }
                            }
                        }
                        checkNext("NextToPrice", ref buttonsLocation);
                        await botClient.EditMessageReplyMarkupAsync(chatId, message.MessageId, replyMarkup: new InlineKeyboardMarkup(buttonsLocation));
                    }
                }
                else if (callbackData == "NextToPrice")
                {
                    query = query.Where(item => locationFilter.Contains(item.Location));
                    await botClient.EditMessageTextAsync(chatId, message.MessageId, "На яку суму ти розраховуєш? 😼", replyMarkup: createButtonsPrice());
                }
                else if(callbackData.Contains("price"))
                {
                    if (buttonsPrice != null)
                    {
                        var button = buttonsPrice
                             .SelectMany(row => row)
                             .FirstOrDefault(b => b.CallbackData == callbackData);
                        var btnPrice = int.Parse(button.CallbackData.Split("price")[1]) ;
                        if (button.Text.Contains("👉"))
                        {
                            button.Text = button.Text.Replace("👉", "✅");
                        }
                        else
                        {
                            button.Text = button.Text.Replace("✅", "👉");
                        }
                        if (priceFilter.Contains(btnPrice))
                        {
                            priceFilter.Remove(btnPrice);
                        }
                        else
                        {
                            priceFilter.Add(btnPrice);
                        }
                        checkNext("NextToShow", ref buttonsPrice, "Готово ✅");
                        await botClient.EditMessageReplyMarkupAsync(chatId, message.MessageId, replyMarkup: new InlineKeyboardMarkup(buttonsPrice));
                    }
                }
                else if( callbackData == "NextToShow")
                {
                    var min = priceFilter.Min();
                    var max = priceFilter.Max();
                    if(min==12999&& max == 12999)
                        query=query.Where(item=>item.Price<=min);
                    else if(max==60000 && min == 60000)
                        query=query.Where(item=>item.Price>=max);
                    else if(min==12999 && (max>min))
                        query=query.Where(item=>item.Price<=max); 
                    else    
                        query=query.Where(item=>item.Price>=min&&item.Price<=max);
                    var advertisements = query.ToList();
                    if(advertisements.Count>0)
                        foreach (var advertisement in advertisements)
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            stringBuilder.AppendLine("🔑" + advertisement.RoomCount.ToString() + 'к');
                            stringBuilder.AppendLine("✏️" + advertisement.Area + " м2");
                            stringBuilder.AppendLine("🔺Поверх: " + advertisement.Floor + "/" + advertisement.TotalFloors);
                            stringBuilder.AppendLine("🚣" + advertisement.Bank);
                            stringBuilder.AppendLine("📍" +advertisement.Location+", "+ advertisement.Address);
                            stringBuilder.AppendLine("〽️" + advertisement.MetroStation);
                            stringBuilder.AppendLine("💵" + advertisement.Price.ToString() + "грн");
                            stringBuilder.AppendLine("ℹ️" + advertisement.Description);
                            var listPhoto = _context.Photos.Where(item => item.AdvertisementId == advertisement.Id).ToList();
                            if (listPhoto.Any())
                            {
                                var media = new List<IAlbumInputMedia>();
                                for (int i = 0; i < listPhoto.Count; i++)
                                {
                                    var p1 = new MemoryStream(listPhoto[i].Img);
                                    var inputMediaPhoto = new InputMediaPhoto(InputFile.FromStream(p1, $"photo{i}"));
                                    if (i == 0)
                                        inputMediaPhoto.Caption = stringBuilder.ToString();
                                    media.Add(inputMediaPhoto);
                                }
                                await botClient.SendMediaGroupAsync(message.Chat, media);
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(message.Chat, stringBuilder.ToString());
                            }
                        }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat, "За вашим запитом нічого не знайшлось");
                        await botClient.SendTextMessageAsync(message.Chat, $"Скільки повинно бути кімнат в квартирі?", replyMarkup: createButtonsRooms());
                        resetFields();
                    }
                }
            }
        }
        private static void checkNext(string callbackData,ref InlineKeyboardButton[][] buttons,string text= "Далі \u2705")
        {
            var buttonNext = InlineKeyboardButton.WithCallbackData(text, callbackData);
            bool hasCheckedButton = buttons.Any(row => row.Any(button => button.Text.Contains("✅ ")));

            if (hasCheckedButton && !buttons.Any(row => row.Any(button => button.Text == text)))
            {
                var newRow = new InlineKeyboardButton[1];
                newRow[0] = buttonNext;
                buttons = buttons.Concat(new[] { newRow }).ToArray();
            }
            else if (!hasCheckedButton && buttons.Any(row => row.Any(button => button.Text == text)))
            {
                buttons = buttons.Take(buttons.Length - 1).ToArray();
            }
        }
        private static void resetFields()
        {
            query = null;
            query = _context.Advertisements.AsQueryable();
            roomsFilter = null;
            roomsFilter = new List<int>();
            locationFilter = null;
            locationFilter = new List<string>();
            priceFilter = null;
            priceFilter = new List<int>();
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
       static byte[] ConvertImageToByteArray(string imagePath)
        {
            byte[] byteArray = null;
            using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byteArray = reader.ReadBytes((int)stream.Length);
                }
            }
            return byteArray;
        }
        private static ApplicationDbContext _context;
        static void Main(string[] args)
        {
            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite("Data Source=helloapp.db")
                    .Options;
                _context = new ApplicationDbContext(options);
                query = _context.Advertisements.AsQueryable();
                //Advertisement advertisement1 = new Advertisement
                //{
                //    RoomCount = 1,
                //    Area = 40,
                //    Floor = 19,
                //    TotalFloors = 25,
                //    Location = "Лівий Берег"
                //    ,
                //    Address = "Позняки/Осокорки, ЖК Seven, вул. Дніпровська набережна 20а",
                //    MetroStation = "м. Осокорки",
                //    Price = 230000,
                //    Description = " Новобудова! ",
                //    //Photos = new List<Photo> { photo1, photo2, photo3, photo4 }
                //};
                //_context.Advertisements.Add(advertisement1);
                //_context.SaveChanges();
                //var advertisement = _context.Advertisements.FirstOrDefault(item => item.Id == 3); // Отримайте оголошення з бази даних
                //var photo1 = new Photo { Img = ConvertImageToByteArray("1.jpg"), Advertisement = advertisement };
                //var photo2 = new Photo { Img = ConvertImageToByteArray("2.png"), Advertisement = advertisement };
                //var photo3 = new Photo { Img = ConvertImageToByteArray("3.jpg"), Advertisement = advertisement };
                //var photo4 = new Photo { Img = ConvertImageToByteArray("4.png"), Advertisement = advertisement };

                //_context.Photos.Add(photo1);
                //_context.Photos.Add(photo2);
                //_context.Photos.Add(photo3);
                //_context.Photos.Add(photo4);

                //_context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            Console.ReadLine();
        }
    }
}