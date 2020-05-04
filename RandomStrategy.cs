using System;
using System.Threading.Tasks;

namespace SCP
{
  public class RandomStrategy : IStrategy
  {
    private readonly ScpLibrary _library;

    public RandomStrategy(ScpLibrary lib) => this._library = lib;
    
    public async Task<string> GetText(string arg)
    {
      var (link, title) = await this._library.GetRandomPageAsync();
      if (link == "ERROR" || string.IsNullOrWhiteSpace(link) || string.IsNullOrWhiteSpace(title))
        return "Something went wrong with bot's logic, please tell author";
      
      return $"[{title.Escape(Config.ForbiddenCharacters)}]" + "(" + link + ")";
    }
  }
}