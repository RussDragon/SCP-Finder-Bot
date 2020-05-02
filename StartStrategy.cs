using System.Threading.Tasks;

namespace SCP
{
  public class StartStrategy : IStrategy
  {
    public Task<string> GetText(string arg)
    {
      return Task.FromResult("/random, /s \\*object number\\*");
    }
  }
}