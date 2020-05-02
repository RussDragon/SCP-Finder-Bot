using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SCP
{
  public class ScpLibrary
  {
    private Dictionary<string, Tuple<string, string>> _objectLinks = 
      new Dictionary<string, Tuple<string, string>>();
    
    public async Task LoadLibraryAsync()
    {
      var regex = new Regex("(scp)", RegexOptions.IgnoreCase);
      Console.WriteLine("Parsing articles links...");
      
      using (var httpClient = new HttpClient())
      {
        var downloadTasks =
          ConfigClass.PageLinks.Select(t => httpClient.GetStringAsync(t)).ToList();
        var responses = await Task.WhenAll(downloadTasks);

        foreach (var html in responses)
        {
          var htmlDoc = new HtmlDocument();
          htmlDoc.LoadHtml(html);
          // TODO: Add error assertion

          var rootNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
          var nodeList =
            rootNode.SelectNodes(".//a")
              .Where(a => regex.IsMatch(a.Attributes["href"].Value))
              .ToList<HtmlNode>();

          foreach (var node in nodeList)
          {
            string id;
            if (Regex.IsMatch(node.InnerHtml, @"\d\d\d\d?\D*"))
              id = Regex.Match(node.InnerHtml, @"\d\d\d\d?\D*").Value;
            else
              id = node.InnerHtml;

            var link = "https://scpfoundation.net" + node.Attributes["href"].Value;

            var title = "";
            title = node.NextSibling != null ? node.NextSibling.InnerHtml : node.InnerHtml;

            title = HttpUtility.HtmlDecode(title)
              .Replace("—", "")
              .Replace("- ", " ")
              .Trim();
            this._objectLinks[id.ToLower()] = new Tuple<string, string>(link, title);
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

    public async Task<Tuple<string, string>> GetRandomPageAsync()
    {
      try
      {
        using (var httpClient = new HttpClient())
        {
          var firstResponse = await httpClient.GetAsync(ConfigClass.RandPage);
          var redirectedUrl = firstResponse.Headers.Location.AbsoluteUri;

          var response = await httpClient.GetAsync(redirectedUrl);

          var html = await response.Content.ReadAsStringAsync();
          var htmlDoc = new HtmlDocument();
          htmlDoc.LoadHtml(html);

          var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-title']");

          return new Tuple<string, string>(redirectedUrl, HttpUtility.HtmlDecode(node.InnerHtml).Trim());
        }
      }
      catch (Exception e)
      {
        return new Tuple<string, string>("ERROR", "");
      };
    }
  }
}