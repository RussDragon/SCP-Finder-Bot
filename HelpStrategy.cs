using System.Threading.Tasks;

namespace SCP
{
  public class HelpStrategy : IStrategy
  {
    public Task<string> GetText(string arg)
    {
      return Task.FromResult("/random, /o \\*object number\\*, /s \\*search query\\*, /help");
    }
  }
}