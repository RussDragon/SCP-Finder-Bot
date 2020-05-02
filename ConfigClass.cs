using System.Collections.Generic;

namespace SCP
{
  public static class ConfigClass
  {
    public static readonly char[] ForbiddenCharacters = new char[]
      { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };

    public static readonly string[] PageLinks = new []
    {
      "http://scpfoundation.net/scp-list",
      "http://scpfoundation.net/scp-list-2",
      "http://scpfoundation.net/scp-list-3",
      "http://scpfoundation.net/scp-list-4",
      "http://scpfoundation.net/scp-list-5",
      "http://scpfoundation.net/scp-list-6",
      "http://scpfoundation.net/scp-list-ru",
      "http://scpfoundation.net/scp-list-fr",
      "http://scpfoundation.net/scp-list-jp",
      "http://scpfoundation.net/scp-list-es",
      "http://scpfoundation.net/scp-list-pl",
      "http://scpfoundation.net/scp-list-others",
      "http://scpfoundation.net/scp-list-j",
      "http://scpfoundation.net/explained-list",
      "http://scpfoundation.net/archive"
    };

    public const string RandPage = "http://scpdb.org/wikidot_random_page";
    public const string Token = "";
  }
}