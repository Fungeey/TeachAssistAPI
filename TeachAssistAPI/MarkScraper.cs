using Newtonsoft.Json;
using TeachAssistAPI.Networking;
using TeachAssistAPI.ObjectModel;

namespace TeachAssistAPI {
	/// <summary>
	/// Scrapes data from TeachAssist and parses it into a collection of Course objects.
	/// </summary>
	public class MarkScraper {

		/// <summary>
		/// Scrapes course and assignment marks from TeachAssist.
		/// </summary>
		/// <param name="loginData">The data representing the student login.</param>
		/// <returns>A string representing the json information.</returns>
		public string ScrapeMarks(UserInfo loginData) {
			var login = new HtmlScraper(loginData);
			var htmlFiles = login.ScrapeHtml();

			ObjectParser scraper = new ObjectParser();
			var courses = scraper.ScrapeMarks(htmlFiles);

			string json = "";
			courses.ForEach(c => {
				json += JsonConvert.SerializeObject(c, new JsonSerializerSettings() {
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
					Formatting = Formatting.Indented
				});
			});

			//var converter = new JsonConverter();

			return json;
		}

		/// <summary>
		/// Scrapes course and assignment marks from TeachAssist.
		/// </summary>
		/// <param name="username">The username of the account</param>
		/// <param name="password">The password of the account</param>
		/// <returns>A string representing the json information.</returns>
		public string ScrapeMarks(string username, string password) {
			return ScrapeMarks(new UserInfo(username, password));
		}
	}
}
