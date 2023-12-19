using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("6748388504:AAGp_moHLOxoCjyyUFSIJx6xrgcP9TzUdhw");

using CancellationTokenSource cts = new();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() 
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    if (messageText == "Привет")
    {
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Привет",
            cancellationToken: cancellationToken);
    }
    if (messageText == "Картинка")
    {
        await botClient.SendPhotoAsync(
          chatId: chatId,
          photo: InputFile.FromUri("https://postium.ru/wp-content/uploads/2023/06/shedevrum.jpg"),
          parseMode: ParseMode.Html,
          cancellationToken: cancellationToken);
    }
    if (messageText == "Видео")
    {
        await botClient.SendVideoAsync(
          chatId: chatId,
          video: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4"),
          thumbnail: InputFile.FromUri("https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg"),
          supportsStreaming: true,
          cancellationToken: cancellationToken);


    }
    Message message1 = await botClient.SendStickerAsync(
    chatId: chatId,
    sticker: InputFile.FromUri("https://github.com/TelegramBots/book/raw/master/src/docs/sticker-fred.webp"),
    cancellationToken: cancellationToken);

    Message message2 = await botClient.SendStickerAsync(
        chatId: chatId,
        sticker: InputFile.FromFileId(message1.Sticker!.FileId),
        cancellationToken: cancellationToken);
}


Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
