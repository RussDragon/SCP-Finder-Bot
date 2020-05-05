using System;
using System.Threading.Tasks;

namespace SCP
{
  public class ObjectSearchStrategy : IStrategy
  {
    private readonly ScpLibrary _library;
    
    public ObjectSearchStrategy(ScpLibrary lib) => this._library = lib;

    public Task<string> GetText(string arg)
    {
      if (string.IsNullOrEmpty(arg))
        return Task.FromResult("Please, specify object number");
      
      var (link, title) = this._library.GetObjectLink(arg);
      if (string.IsNullOrWhiteSpace(link))
        return Task.FromResult("Sorry, I don't know such SCP");
      
      return Task.FromResult($"[{title.Escape(Config.ForbiddenCharacters)}]" + "(" + link + ")");
    }
  }
}