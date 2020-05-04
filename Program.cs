using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

//TODO: Add better error assertion to strategies
//TODO: Maybe encapsulate messages to objects?
//TODO: Add english foundation and other wikidots sites
//TODO: Fix spanish and chinese objects (regex)
//TODO: Parse pages for site search for titles fix
namespace SCP
{
  public static class StringExtension
  {
    public static string Escape(this string str, IEnumerable<char> forbiddenCharacters)
    {
      var isForbidden = new HashSet<char>(forbiddenCharacters);

      var escapedString = new StringBuilder(str.Length * 2);
      foreach (var character in str)
      {
        if (isForbidden.Contains(character))
          escapedString.Append("\\");

        escapedString.Append(character);
      }

      return escapedString.ToString();
    }

    public static string TrimHtmlTags(this string str)
    {
      return Regex.Replace(str, $"<[^>]*?>", "");
    }
  }
  
  class Program {
    private static ITelegramBotClient _botClient;
    private static readonly ScpLibrary Library = new ScpLibrary();

    private static readonly Dictionary<string, IStrategy> CommandsStrategies =
      new Dictionary<string, IStrategy>()
      {
        { "/start", new HelpStrategy() },
        { "/help", new HelpStrategy()},
        { "/o", new ObjectSearchStrategy(Library) },
        { "/random", new RandomStrategy(Library) },
        { "/s", new SiteSearchStrategy(Library) }
      };

    static async Task Main()
    {
      await Library.LoadLibraryAsync();

      _botClient = new TelegramBotClient(Config.Token);
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
      
      if (e.Message.Text[0] != '/') return;
      var args = e.Message.Text.Split(' ', 2);

      string textMessage;
      if (CommandsStrategies.ContainsKey(args[0]))
      {
        var arg = args.Length > 1 ? args[1].TrimStart() : "";
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