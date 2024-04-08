using HtmlAgilityPack; 
using CsvHelper; 
using System.Globalization; 
using System.Collections.Concurrent; 
 
namespace BookScrapingApp 
{ 
	public class Program 
	{ 
		public static void Main() 
		{ 
			// initializing HAP 
			var web = new HtmlWeb(); 
            // setting a global User-Agent header in HAP 
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

		 
			// this can't be a List because it's not thread-safe 
			var books = new ConcurrentBag<Book>(); 
		 
			// the complete list of pages to scrape 
			var pagesToScrape = new ConcurrentBag<string> { 
				"https://books.toscrape.com/catalogue/category/books/travel_2/index.html",
                "https://books.toscrape.com/catalogue/category/books/mystery_3/index.html",
                /*"https://books.toscrape.com/catalogue/category/books/mystery_3/page-2.html",*/
			}; 

            var rootDirectory = "catalogue/category/";
 
			// performing parallel web scraping 
			Parallel.ForEach( 
				pagesToScrape, 
				new ParallelOptions { MaxDegreeOfParallelism = 4 }, 
				currentPage => 
				{ 
                    
                    
					var currentDocument = web.Load(currentPage); 

                    var category = HtmlEntity.DeEntitize(currentDocument.QuerySelector("h1").InnerText);

                    Console.WriteLine("Category is : " + category);

                    
                    try{
                            Directory.CreateDirectory(rootDirectory+category);
                            Console.WriteLine("Directory created successfully");
                        }
                        catch (Exception e) {
                            Console.WriteLine("Unable to create directory : " + category);
                            Console.WriteLine("Error is : " + e.ToString());
                        }
 
					var productHTMLElements = currentDocument.DocumentNode.QuerySelectorAll("article.product_pod"); 
					foreach (var productHTMLElement in productHTMLElements) 
					{ 
                        
						var url = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("a").Attributes["href"].Value); 
						var image = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("img").Attributes["src"].Value); 
						var name = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector("h3>a").Attributes["title"].Value); 
						var price = HtmlEntity.DeEntitize(productHTMLElement.QuerySelector(".price_color").InnerText); 
 
						var book = new Book() { Url = url, Image = image, Name = name, Price = price }; 
                        
                        try{
                            if (Directory.Exists(rootDirectory+category+"/"+book.Name))
                            {
                                Console.WriteLine("That path exists already. " + book.Name);
                                continue;
                            }
                            
                            Directory.CreateDirectory(rootDirectory+category+"/"+book.Name.Replace(":", ""));
							//TODO : Download the html file and book image
                            Console.WriteLine("Directory created successfully");
                        }
                        catch (Exception e) {
                            Console.WriteLine("Unable to create directory : " + book.Name);
                            Console.WriteLine("Error is : " + e.ToString());
                        }
					} 
				} 
			); 
           
		} 
	} 
}
