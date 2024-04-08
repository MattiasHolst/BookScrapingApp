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
			var booksDirectory = "catalogue/category/books/";
			if(!Directory.Exists("catalogue")) Directory.CreateDirectory("catalogue");
			if(!Directory.Exists("catalogue/category")) Directory.CreateDirectory("catalogue/category");
			if(!Directory.Exists("catalogue/category/books")) Directory.CreateDirectory("catalogue/category/books");
            // setting a global User-Agent header in HAP 
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

			var client = new WebClient();
			client.DownloadFile("https://books.toscrape.com/", "index.html");

			var startPageDocument = web.Load(startPage); 

			var booksSideNavUrl = HtmlEntity.DeEntitize(startPageDocument.DocumentNode.QuerySelector(".nav-list > li > a").Attributes["href"].Value);
			var booksIndex = booksSideNavUrl.Split("/");
			try{
				if (Directory.Exists("catalogue/category/"+booksIndex[booksIndex.Length-2]))
				{
					Console.WriteLine("That path exists already. catalogue/category/"+booksIndex[booksIndex.Length-2] + "/index.html");
				}else{
					Directory.CreateDirectory("catalogue/category/"+booksIndex[booksIndex.Length-2]);
					Console.WriteLine("URL is : " + startPage+"catalogue/category/"+booksIndex[booksIndex.Length-2]+"/index.html");
					client.DownloadFile(startPage+"catalogue/category/"+booksIndex[booksIndex.Length-2]+"/index.html","catalogue/category/"+booksIndex[booksIndex.Length-2]+"/index.html");
					Console.WriteLine("Directory created successfully");
				}
				
				
			}
			catch (Exception e) {
				Console.WriteLine("Unable to create directory : " + "catalogue/category/"+booksIndex[booksIndex.Length-2]);
				Console.WriteLine("Error is : " + e.ToString());
			}

 
			var sideNavElements = startPageDocument.DocumentNode.QuerySelectorAll(".nav-list > li > ul > li"); 

			foreach(var sideNavElement in sideNavElements){
				var sideNavUrl = HtmlEntity.DeEntitize(sideNavElement.QuerySelector("a").Attributes["href"].Value); 
				var pageSplit = sideNavUrl.Split('/');

				var category = pageSplit[pageSplit.Length-2];

				try{
					if (Directory.Exists(booksDirectory+category))
					{
						Console.WriteLine("That path exists already. " + booksDirectory+category+"/index.html");
						continue;
					}
					
					Directory.CreateDirectory(booksDirectory+category);
					Console.WriteLine("URL is : " + startPage+booksDirectory+category+"/index.html");
					client.DownloadFile(startPage+booksDirectory+category+"/index.html", booksDirectory+category+"/index.html");
					Console.WriteLine("Directory created successfully");
				}
				catch (Exception e) {
					Console.WriteLine("Unable to create directory : " + booksDirectory+category);
					Console.WriteLine("Error is : " + e.ToString());
				}
				Console.WriteLine("sideNavUrl is : " + sideNavUrl);
			}
           
		} 
	} 
}
