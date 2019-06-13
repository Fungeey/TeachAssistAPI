using System;
using TeachAssistAPI;

class Example {
	static void Main(string[] args) {
		MarkScraper scraper = new MarkScraper();

		var userInfo = new UserInfo("your_username", "your_password");
		var markData = scraper.ScrapeMarks(userInfo);

		Console.WriteLine(markData);
		Console.ReadKey();
	}
}