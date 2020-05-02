using System.Threading.Tasks;

namespace SCP
{
  public interface IStrategy
  {
    public Task<string> GetText(string arg);
  }
}