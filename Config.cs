using System.Collections.Generic;

namespace SCP
{
  public static class Config
  {
    public static readonly char[] ForbiddenCharacters = new char[]
      { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };

    public const string HomePage = "http://scpfoundation.net";
    public static readonly string[] PageLinks = new []
    {
      HomePage + "/scp-list",
      HomePage + "/scp-list-2",
      HomePage + "/scp-list-3",
      HomePage + "/scp-list-4",
      HomePage + "/scp-list-5",
      HomePage + "/scp-list-6",
      HomePage + "/scp-list-ru",
      HomePage + "/scp-list-fr",
      HomePage + "/scp-list-jp",
      HomePage + "/scp-list-es",
      HomePage + "/scp-list-pl",
      HomePage + "/scp-list-others",
      HomePage + "/scp-list-j",
      HomePage + "/explained-list",
      HomePage + "/archive"
    };

    public const string RandPage = "http://scpdb.org/wikidot_random_page";
    public const string SearchPage = HomePage + "/search:site/a/p/q/";
    public const string Token = "";
  }
}