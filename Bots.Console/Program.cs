// See https://aka.ms/new-console-template for more information

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Telegram.Bot.Types.Message;
using Update = Telegram.Bot.Types.Update;

Console.WriteLine("Hello, World!");

var botClient = new TelegramBotClient("5371706610:AAGjFtmnWvTpuobGLGrJgOzb-IjDW2W6S9c");

using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { } // receive all update types
};
botClient.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Type != UpdateType.Message)
        return;
    // Only process text messages
    if (update.Message!.Type != MessageType.Text)
        return;

    var chatId = update.Message.Chat.Id;
    var messageText = update.Message.Text;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    // Echo received message text
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "You said:\n" + messageText,
        cancellationToken: cancellationToken);

    Message message = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Trying *all the parameters* of `sendMessage` method",
        parseMode: ParseMode.MarkdownV2,
        disableNotification: true,
        replyToMessageId: update.Message.MessageId,
        replyMarkup: new InlineKeyboardMarkup(
            InlineKeyboardButton.WithUrl(
                "Check sendMessage method",
                "https://core.telegram.org/bots/api#sendmessage")),
        cancellationToken: cancellationToken);

    //Message pollMessage = await botClient.SendPollAsync(
    //    chatId: "@group_or_channel_username",
    //    question: "Did you ever hear the tragedy of Darth Plagueis The Wise?",
    //    options: new[]
    //    {
    //        "Yes for the hundredth time!",
    //        "No, who`s that?"
    //    },
    //    cancellationToken: cancellationToken);

    Message message1 = await botClient.SendContactAsync(
        chatId: chatId,
        phoneNumber: "+1234567890",
        firstName: "Han",
        vCard: "BEGIN:VCARD\n" +
               "VERSION:3.0\n" +
               "N:Solo;Han\n" +
               "ORG:Scruffy-looking nerf herder\n" +
               "TEL;TYPE=voice,work,pref:+1234567890\n" +
               "EMAIL:hansolo@mfalcon.com\n" +
               "END:VCARD",
        cancellationToken: cancellationToken);

    Message message2 = await botClient.SendVenueAsync(
        chatId: chatId,
        latitude: 50.0840172f,
        longitude: 14.418288f,
        title: "Man Hanging out",
        address: "Husova, 110 00 Staré Město, Czechia",
        cancellationToken: cancellationToken);

    // using Telegram.Bot.Types.ReplyMarkups;

    InlineKeyboardMarkup inlineKeyboard = new(new[]
    {
        // first row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "1.1", callbackData: "11"),
            InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
        },
        // second row
        new []
        {
            InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
            InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22"),
        },
    });

    Message sentMessage3 = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "A message with an inline keyboard markup",
        replyMarkup: inlineKeyboard,
        cancellationToken: cancellationToken);

    // using Telegram.Bot.Types.ReplyMarkups;

    InlineKeyboardMarkup inlineKeyboard2 = new(new[]
        {
            InlineKeyboardButton.WithUrl(
                text: "Link to the Repository",
                url: "https://github.com/TelegramBots/Telegram.Bot"
            )
        }
    );

    Message sentMessage2 = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "A message with an inline keyboard markup",
        replyMarkup: inlineKeyboard2,
        cancellationToken: cancellationToken);

    // using Telegram.Bot.Types.ReplyMarkups;

    InlineKeyboardMarkup inlineKeyboard3 = new(new[]
        {
            InlineKeyboardButton.WithSwitchInlineQuery("switch_inline_query"),
            InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("switch_inline_query_current_chat"),
        }
    );

    Message sentMessage4 = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "A message with an inline keyboard markup",
        replyMarkup: inlineKeyboard3,
        cancellationToken: cancellationToken);

    using (FileStream stream = System.IO.File.OpenRead(@"C:\Users\HopeH\Downloads\276316963_913641749503212_1740395991607316817_n.jpg"))
    {
        InputOnlineFile inputOnlineFile = new InputOnlineFile(stream, "image.jpg");
        await botClient.SendDocumentAsync(chatId, inputOnlineFile);
    }

    Dictionary<string, string> fields = new Dictionary<string, string>();

  
}

Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}