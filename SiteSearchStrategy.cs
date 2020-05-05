using System;
using System.Text;
using System.Threading.Tasks;

namespace SCP
{
  public class SiteSearchStrategy : IStrategy
  {
    private readonly ScpLibrary _library;
    
    public SiteSearchStrategy(ScpLibrary lib) => this._library = lib;
    
    public async Task<string> GetText(string arg)
    {
      if (string.IsNullOrEmpty(arg))
        return "Please, specify your search query";

      if (arg.Length < 3)
        return "Please, make your query at least 3 characters long!";
      
      // TODO: Add amount of results limit parsing (probably last split of the string)
      var searchResults = await _library.SearchSite(arg);
      if (searchResults.Count == 1 && searchResults[0].Item1 == "" && searchResults[0].Item2 == "")
        return "Nothing found on your request";
      
      var textMessage = new StringBuilder("", searchResults.Count * 32);
      foreach (var (link, title) in searchResults)
      {
        if (string.IsNullOrEmpty(link) || string.IsNullOrEmpty(title))
          textMessage
            .Append(
              $"[Possibly error occured with searching, please, contact the bot author]({Config.HomePage})"
              );
        else
          textMessage
            .Append("— ")
            .Append($"[{title.Escape(Config.ForbiddenCharacters)}]")
            .Append("(")
            .Append(link)
            .Append(")")
            .Append("\n");
      }

      return textMessage.ToString();
    }
  }
}