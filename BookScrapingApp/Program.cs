using HtmlAgilityPack;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace BookScrapingApp 
{ 
	public class Program 
	{
		readonly string createdAndDownloadedObjectsText = "Created/Downloaded objects : ";

		int numberOfCreatedAndDownloadedObjects = 0;

		int sumCreatedAndDownloadedObjects = 6170;
 
		public static void Main() 
		{ 
			Program bookScraping = new();

			// initializing HAP 
			var web = new HtmlWeb(); 
			string startPage = "https://books.toscrape.com/";
			string mediaDirectory = "media/cache";
			
			
            // setting a global User-Agent header in HAP 
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

			var client = new WebClient();
			if(!File.Exists("index.html")) {
				client.DownloadFile(startPage, "index.html");
				bookScraping.numberOfCreatedAndDownloadedObjects++;
				Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
			}

			var startPageDocument = web.Load(startPage); 

			//CSS files
			DownloadCssFiles(startPageDocument,startPage, client, bookScraping);

			//JS files
			DownloadJSFiles(startPageDocument, startPage, client, bookScraping);
			// Create folder structure with index files 
			var sideNavElements = startPageDocument.DocumentNode.QuerySelectorAll("a");
			foreach(var sideNavElement in sideNavElements){
				string sideNavUrl = HtmlEntity.DeEntitize(sideNavElement.QuerySelector("a").Attributes["href"].Value);
				if(Path.GetDirectoryName(sideNavUrl) != ""){
					if(!Directory.Exists( Path.GetDirectoryName(sideNavUrl))){
						Directory.CreateDirectory(Path.GetDirectoryName(sideNavUrl)!);
						bookScraping.numberOfCreatedAndDownloadedObjects++;
						Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
					} 
				}
				if(!File.Exists(sideNavUrl)){
					client.DownloadFile(startPage+sideNavUrl, sideNavUrl);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
				}


				// Check if category folders got a Next button
				if(Path.GetDirectoryName(sideNavUrl)!.Contains("category")){
					var categoryPageDocument = web.Load(startPage+sideNavUrl); 
					var nextButton = categoryPageDocument.DocumentNode.QuerySelector(".next > a");
					
					string path = Path.GetDirectoryName(sideNavUrl)!;
					//Small images for first page
					if(path!=null) {
						DownloadSmallImages(categoryPageDocument, startPage, path, client, bookScraping);
						DownloadDetailPageImages(categoryPageDocument, startPage, path,mediaDirectory, client, web, bookScraping);
					}
					while(nextButton != null){
						string nextPageLink = nextButton.Attributes["href"].Value;
						if(!File.Exists(nextPageLink)) {
							client.DownloadFile(startPage+path+"/"+nextPageLink, path+"/"+nextPageLink);
							bookScraping.numberOfCreatedAndDownloadedObjects++;
							Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
						}
						var nextButtonPageDocument = web.Load(startPage+path+"/"+nextPageLink);
						nextButton = nextButtonPageDocument.DocumentNode.QuerySelector(".next > a");
						// Small Images for paginagion pages
						DownloadSmallImages(nextButtonPageDocument, startPage, path!, client, bookScraping);
						// Articles
						DownloadDetailPageImages(nextButtonPageDocument, startPage, path!,mediaDirectory, client, web, bookScraping);
					}

					
				}

			}
		
		}

		private static void  DownloadSmallImages(HtmlDocument pageDocument, string startPage, string path, WebClient client, Program bookScraping){
			var imageElements = pageDocument.DocumentNode.QuerySelectorAll("img"); 
			foreach(var imageElement in imageElements){
				var imageUrl = HtmlEntity.DeEntitize(imageElement.Attributes["src"].Value); 
				if(!Directory.Exists( Path.GetDirectoryName(path+"/"+imageUrl))) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path + "/" + imageUrl)!);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
				} 
				if(!File.Exists(path+"/"+imageUrl)) {
					
					client.DownloadFile(startPage+imageUrl, path+"/"+imageUrl);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);

				}
				
			}
		}

		private static void  DownloadDetailPageImages(HtmlDocument pageDocument, string startPage, string path,string mediaDirectory, WebClient client, HtmlWeb web, Program bookScraping){
			var articleElements = pageDocument.DocumentNode.QuerySelectorAll(".product_pod"); 
			foreach(var articleElement in articleElements){
				var detailPageUrl = HtmlEntity.DeEntitize(articleElement.QuerySelector("a").Attributes["href"].Value); 
				if(!Directory.Exists( Path.GetDirectoryName(path+"/"+detailPageUrl))) {
					Directory.CreateDirectory(Path.GetDirectoryName(path+"/"+detailPageUrl)!);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
				}
				if(!File.Exists(path+"/"+detailPageUrl)) {
					client.DownloadFile(startPage+path+"/"+detailPageUrl, path+"/"+detailPageUrl);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
				}

				// Todo : Download the detailpages images
				var detailPageDocument = web.Load(startPage+path+"/"+detailPageUrl); 
				string imageSrc = detailPageDocument.QuerySelector("img").Attributes["src"].Value;
				if(!Directory.Exists( Path.GetDirectoryName(mediaDirectory+"/"+imageSrc))) {
					Directory.CreateDirectory(Path.GetDirectoryName(mediaDirectory+"/"+imageSrc)!);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
				}
				if(!File.Exists(mediaDirectory+"/"+imageSrc)) {
					client.DownloadFile(startPage+mediaDirectory+"/"+imageSrc, mediaDirectory+"/"+imageSrc);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
				}
			}
		}

		private static void DownloadCssFiles(HtmlDocument pageDocument, string startPage, WebClient client, Program bookScraping){
			var cssElements = pageDocument.DocumentNode.QuerySelectorAll("head > link"); 
			foreach(var cssElement in cssElements){
				var cssUrl = HtmlEntity.DeEntitize(cssElement.Attributes["href"].Value); 
				if(!Directory.Exists( Path.GetDirectoryName(cssUrl))) {
					Directory.CreateDirectory(Path.GetDirectoryName(cssUrl)!);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
				}
				if(!File.Exists(cssUrl)){
					client.DownloadFile(startPage+cssUrl, cssUrl);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);

				}
				
			}
		}

		private static void DownloadJSFiles(HtmlDocument pageDocument, string startPage, WebClient client, Program bookScraping){
			var jsElements = pageDocument.DocumentNode.QuerySelectorAll("script"); 
			foreach(var jsElement in jsElements){
				if(jsElement.Attributes["src"] == null) continue;
				var jsUrl = HtmlEntity.DeEntitize(jsElement.Attributes["src"].Value); 
				if (jsUrl.Contains("http")) continue;
				if(!Directory.Exists( Path.GetDirectoryName(jsUrl))) {
					Directory.CreateDirectory(Path.GetDirectoryName(jsUrl)!);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);
				}
				if(!File.Exists(jsUrl)){
					client.DownloadFile(startPage+jsUrl, jsUrl);
					bookScraping.numberOfCreatedAndDownloadedObjects++;
					Console.WriteLine(bookScraping.createdAndDownloadedObjectsText + bookScraping.numberOfCreatedAndDownloadedObjects+"/"+bookScraping.sumCreatedAndDownloadedObjects);

				}
				
			}
		}
	
	} 
}