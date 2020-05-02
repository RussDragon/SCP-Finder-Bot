using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace SCP
{
  public static class StringExtension
  {
    public static string Escape(this string text, IEnumerable<char> forbiddenCharacters)
    {
      var isForbidden = new HashSet<char>(forbiddenCharacters);

      var escapedString = new StringBuilder(text.Length * 2);
      foreach (var character in text)
      {
        if (isForbidden.Contains(character))
          escapedString.Append("\\");

        escapedString.Append(character);
      }

      return escapedString.ToString();
    }
  }
  
  class Program {
    private static ITelegramBotClient _botClient;
    private static readonly ScpLibrary Library = new ScpLibrary();

    private static readonly Dictionary<string, IStrategy> CommandsStrategies =
      new Dictionary<string, IStrategy>()
      {
        { "/start", new StartStrategy() },
        { "/s", new SearchStrategy(Library) },
        { "/random", new RandomStrategy(Library) }
      };

    static async Task Main()
    {
      await Library.LoadLibraryAsync();
      
      _botClient = new TelegramBotClient(ConfigClass.Token);
      var me = await _botClient.GetMeAsync();
      Console.WriteLine(
        $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
      );
      
      _botClient.OnMessage += Bot_OnMessage;
      _botClient.StartReceiving();
      Thread.Sleep(int.MaxValue);
    }
    private static async void Bot_OnMessage(object sender, MessageEventArgs e)
    {
      if (e.Message.Text == null) return;
      Console.WriteLine($"Received a text message in chat {e.Message.Chat.Username}.");
      
      var args = e.Message.Text.Split(new char[] { ' ' }, 2);

      string textMessage;
      if (CommandsStrategies.ContainsKey(args[0]))
      {
        var arg = ((args.Length > 1) ? args[1].TrimStart() : "");
        textMessage = await CommandsStrategies[args[0]].GetText(arg);
      }
      else
         textMessage = "Sorry, I don't know what do you want from me";

      await _botClient.SendTextMessageAsync(
        chatId: e.Message.Chat,
        text: textMessage,
        ParseMode.MarkdownV2
      );
    }
  }
}