using System;
using System.Threading.Tasks;

namespace SCP
{
  public class SearchStrategy : IStrategy
  {
    private readonly ScpLibrary _library;
    
    public SearchStrategy(ScpLibrary lib) => this._library = lib;

    public Task<string> GetText(string arg)
    {
      var (link, title) = this._library.GetObjectLink(arg);
      if (string.IsNullOrWhiteSpace(link))
        return Task.FromResult("Sorry, I don't know such SCP");
      
      return Task.FromResult($"[{title.Escape(ConfigClass.ForbiddenCharacters)}]" + "(" + link + ")");
    }
  }
}