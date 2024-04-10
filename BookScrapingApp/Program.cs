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
			Program bookScraping = new Program();
			// initializing HAP 
			var web = new HtmlWeb(); 
			var startPage = "https://books.toscrape.com/";

            // setting a global User-Agent header in HAP 
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

			var client = new WebClient();
			client.DownloadFile(startPage, "index.html");

			var startPageDocument = web.Load(startPage); 

			//CSS files
			bookScraping.downloadCssFiles(startPageDocument,startPage, client);


			// Create folder structure with index files 
			var sideNavElements = startPageDocument.DocumentNode.QuerySelectorAll("a");
			foreach(var sideNavElement in sideNavElements){
				var sideNavUrl = HtmlEntity.DeEntitize(sideNavElement.QuerySelector("a").Attributes["href"].Value);
				if(Path.GetDirectoryName(sideNavUrl) != ""){
					if(!Directory.Exists( Path.GetDirectoryName(sideNavUrl))) Directory.CreateDirectory(Path.GetDirectoryName(sideNavUrl));
				}
				client.DownloadFile(startPage+sideNavUrl, sideNavUrl);

				// Check if category folders got a Next button
				if(Path.GetDirectoryName(sideNavUrl).Contains("category")){
					Console.WriteLine("category : " + startPage+sideNavUrl);
					var categoryPageDocument = web.Load(startPage+sideNavUrl); 
					var nextButton = categoryPageDocument.DocumentNode.QuerySelector(".next > a");
					
					var path = Path.GetDirectoryName(sideNavUrl);
					//Small images for first page
					bookScraping.downloadSmallImages(categoryPageDocument, startPage, path, client);
					while(nextButton != null){
						var nextPageLink = nextButton.Attributes["href"].Value;
						client.DownloadFile(startPage+path+"/"+nextPageLink, path+"/"+nextPageLink);
						var nextButtonPageDocument = web.Load(startPage+path+"/"+nextPageLink);
						nextButton = nextButtonPageDocument.DocumentNode.QuerySelector(".next > a");
						// Small Images for paginagion pages
						bookScraping.downloadSmallImages(nextButtonPageDocument, startPage, path, client);
						// Articles
						var articleElements = nextButtonPageDocument.DocumentNode.QuerySelectorAll(".product_pod"); 
						foreach(var articleElement in articleElements){
							var detailPageUrl = HtmlEntity.DeEntitize(articleElement.QuerySelector("a").Attributes["href"].Value); 
							if(!Directory.Exists( Path.GetDirectoryName(detailPageUrl))) Directory.CreateDirectory(Path.GetDirectoryName(path+"/"+detailPageUrl));
							if(!File.Exists(detailPageUrl)) client.DownloadFile(startPage+path+"/"+detailPageUrl, path+"/"+detailPageUrl);
						}
					}

					
				}

			}
           
		}

		public void downloadSmallImages(HtmlDocument pageDocument, string startPage, string path, WebClient client){
			var imageElements = pageDocument.DocumentNode.QuerySelectorAll("img"); 
			foreach(var imageElement in imageElements){
				var imageUrl = HtmlEntity.DeEntitize(imageElement.Attributes["src"].Value); 
				if(!Directory.Exists( Path.GetDirectoryName(imageUrl))) Directory.CreateDirectory(Path.GetDirectoryName(path+"/"+imageUrl));
				if(!File.Exists(imageUrl)) client.DownloadFile(startPage+imageUrl, path+"/"+imageUrl);

			}
		}

		public void downloadCssFiles(HtmlDocument pageDocument, string startPage, WebClient client){
			var cssElements = pageDocument.DocumentNode.QuerySelectorAll("head > link"); 
			foreach(var cssElement in cssElements){
				var cssUrl = HtmlEntity.DeEntitize(cssElement.Attributes["href"].Value); 
				if(!Directory.Exists( Path.GetDirectoryName(cssUrl))) Directory.CreateDirectory(Path.GetDirectoryName(cssUrl));
				client.DownloadFile(startPage+cssUrl, cssUrl);

			}
		}
	
	} 
}