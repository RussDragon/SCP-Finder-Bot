using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

//TODO Add HttpClient response codes & headers verification
namespace SCP
{
  public class ScpLibrary
  {
    private Dictionary<string, Tuple<string, string>> _objectLinks =
      new Dictionary<string, Tuple<string, string>>();

    public async Task LoadLibraryAsync()
    {
      var hrefRegex = new Regex("(scp)", RegexOptions.IgnoreCase);
      var objectRegex = new Regex("^SCP-(.*)", RegexOptions.IgnoreCase);
      Console.WriteLine("Parsing articles links...");

      using (var httpClient = new HttpClient())
      {
        var downloadTasks =
          Config.PageLinks.Select(t => httpClient.GetStringAsync(t)).ToList();
        var responses = await Task.WhenAll(downloadTasks);

        foreach (var html in responses)
        {
          var htmlDoc = new HtmlDocument();
          htmlDoc.LoadHtml(html);
          // TODO: Add error assertion

          var rootNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
          var nodeList =
            rootNode.SelectNodes(".//a")
              .Where(a => hrefRegex.IsMatch(a.Attributes["href"].Value))
              .ToList<HtmlNode>();
          
          foreach (var node in nodeList)
          {
            var id = objectRegex.Match(node.InnerHtml).Groups[1].Value;
            if (string.IsNullOrWhiteSpace(id))
              id = node.InnerHtml;
            
            var link = Config.HomePage + node.Attributes["href"].Value;

            // If we have some tags after link, span or #text for example
            var title = node.NextSibling != null ? node.NextSibling.InnerHtml : node.InnerHtml;
            if (string.IsNullOrWhiteSpace(title))
              title = node.InnerHtml;
            
            title = HttpUtility.HtmlDecode(title)
              .Replace("—", "")
              .Replace("- ", " ")
              .Trim();

            if (!_objectLinks.ContainsKey(id.ToLower()))
              _objectLinks[id.ToLower()] = new Tuple<string, string>(link, title);
            else
              Console.WriteLine($"[WARNING] Attempt to rewrite {id}, {title}");
          }
        }
      }

      Console.WriteLine("Articles links are loaded successfully");
    }

    public Tuple<string, string> GetObjectLink(string id)
    {
      id = id.ToLower();
      return !this._objectLinks.ContainsKey(id) ? new Tuple<string, string>("", "") : this._objectLinks[id];
    }

    public async Task<List<Tuple<string, string>>> SearchSite(string query, int amount = 5)
    {
      var resultsList = new List<Tuple<string, string>>();

      using (var httpClient = new HttpClient())
      {
        var encodedQuery = HttpUtility.UrlEncode(query);
        var response = await httpClient.GetAsync(Config.SearchPage + encodedQuery);

        var html = await response.Content.ReadAsStringAsync();
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        var searchRootNode =
          htmlDoc.DocumentNode.SelectSingleNode(xpath: "//div[@class='search-results']");

        // C'mon, remove all this ifs and make correct problem detection, you, fool! 
        if (searchRootNode != null)
        {
          var searchResultsNodes = searchRootNode.SelectNodes(".//div[@class='item']");

          if (searchResultsNodes != null)
          {
            amount = (amount > searchResultsNodes.Count) ? searchResultsNodes.Count : amount;
            for (var i = 0; i < amount; i++)
            {
              var searchItemNode = searchResultsNodes[i];
              var titleNode = searchItemNode.SelectSingleNode(".//div[@class='title']");
              var linkNode = titleNode.SelectSingleNode(".//a");

              var link = linkNode.Attributes["href"].Value;
              var linkName =
                HttpUtility.HtmlDecode(linkNode.InnerHtml)
                  .TrimHtmlTags()
                  .Trim();

              resultsList.Add(new Tuple<string, string>(link, linkName));
            }

            return resultsList;
          }
          
          return new List<Tuple<string, string>> 
              {new Tuple<string, string>("", "")};
        }
        
        return new List<Tuple<string, string>>
          {new Tuple<string, string>(Config.HomePage, "Error occured")};
      }
    }

    public async Task<Tuple<string, string>> GetRandomPageAsync()
    {
      try
      {
        using (var httpClient = new HttpClient())
        {
          var firstResponse = await httpClient.GetAsync(Config.RandPage);
          var redirectedUrl = firstResponse.Headers.Location.AbsoluteUri;

          var secondResponse = await httpClient.GetAsync(redirectedUrl);

          var html = await secondResponse.Content.ReadAsStringAsync();
          var htmlDoc = new HtmlDocument();
          htmlDoc.LoadHtml(html);

          var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-title']");

          return
            new Tuple<string, string>(
              redirectedUrl,
              HttpUtility.HtmlDecode(node.InnerHtml).TrimHtmlTags().Trim()
            );
        }
      }
      catch (Exception e)
      {
        return new Tuple<string, string>("ERROR", "");
      }
    }
  }
}