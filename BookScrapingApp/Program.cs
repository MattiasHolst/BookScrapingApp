using HtmlAgilityPack; 
using CsvHelper; 
using System.Globalization; 
using System.Collections.Concurrent;
using System.Net;

namespace BookScrapingApp 
{ 
	public class Program 
	{ 
 
		public static void Main() 
		{ 
			// initializing HAP 
			var web = new HtmlWeb(); 
			var startPage = "https://books.toscrape.com/";

            // setting a global User-Agent header in HAP 
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

			var client = new WebClient();
			client.DownloadFile("https://books.toscrape.com/", "index.html");

			var startPageDocument = web.Load(startPage); 

			//CSS files
			var cssElements = startPageDocument.DocumentNode.QuerySelectorAll("head > link"); 
			foreach(var cssElement in cssElements){
				var cssUrl = HtmlEntity.DeEntitize(cssElement.Attributes["href"].Value); 
				if(!Directory.Exists( Path.GetDirectoryName(cssUrl))) Directory.CreateDirectory(Path.GetDirectoryName(cssUrl));
				client.DownloadFile(startPage+cssUrl, cssUrl);

			}

			//Images
			var imageElements = startPageDocument.DocumentNode.QuerySelectorAll("img"); 
			foreach(var imageElement in imageElements){
				var cssUrl = HtmlEntity.DeEntitize(imageElement.Attributes["src"].Value); 
				if(!Directory.Exists( Path.GetDirectoryName(cssUrl))) Directory.CreateDirectory(Path.GetDirectoryName(cssUrl));
				client.DownloadFile(startPage+cssUrl, cssUrl);

			}


			/* Create folder structure to sidenav */
			var sideNavElements = startPageDocument.DocumentNode.QuerySelectorAll("a");
			foreach(var sideNavElement in sideNavElements){
				var sideNavUrl = HtmlEntity.DeEntitize(sideNavElement.QuerySelector("a").Attributes["href"].Value);
				Console.WriteLine("SideNavUrl is : " + sideNavUrl);
				if(Path.GetDirectoryName(sideNavUrl) != ""){
					if(!Directory.Exists( Path.GetDirectoryName(sideNavUrl))) Directory.CreateDirectory(Path.GetDirectoryName(sideNavUrl));
				}
				client.DownloadFile(startPage+sideNavUrl, sideNavUrl);


			}
           
		} 
	} 
}
